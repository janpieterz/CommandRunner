using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandRunner
{
    internal interface ICommandScanner
    {
        Tuple<List<ICommand>, List<Type>> ScanTypes(List<Type> types);
    }

    internal class CommandScanner : ICommandScanner
    {
        private List<ICommand> _menu;
        private List<Type> _navigatableTypes;
        private List<Type> _scannableTypes;
        public Tuple<List<ICommand>, List<Type>> ScanTypes(List<Type> types)
        {
            _menu = new List<ICommand>();
            _navigatableTypes = new List<Type>();
            _scannableTypes = types;

            ProcessNestedCommands();
            ProcessNavigatableCommands();
            ProcessSingleCommands();
            return new Tuple<List<ICommand>, List<Type>>(_menu, _navigatableTypes);
        }

        private void ProcessNavigatableCommands()
        {
            var navigatableCommands =
                _scannableTypes.Where(
                    x => x.GetTypeInfo().GetCustomAttribute<NavigatableCommandAttribute>() != null);
            foreach (Type navigatableCommand in navigatableCommands)
            {
                var navigatableAttribute = navigatableCommand.GetTypeInfo().GetCustomAttribute<NavigatableCommandAttribute>();
                var menuItem = ProcessNavigatableCommand(navigatableCommand, navigatableAttribute);
                _menu.Add(menuItem);
            }
        }

        private NavigatableCommand ProcessNavigatableCommand(Type navigatableCommand, NavigatableCommandAttribute navigatableAttribute)
        {
            _navigatableTypes.Add(navigatableCommand);

            var subItems = new List<ICommand>();

            //Add single commands on the navigatable item
            navigatableCommand.GetMethods()
                    .Where(x => x.GetCustomAttribute<CommandAttribute>() != null)
                    .ToList()
                    .ForEach(
                        commandMethod =>
                            subItems.Add(CreateCommand(navigatableCommand, commandMethod,
                                commandMethod.GetCustomAttribute<CommandAttribute>())));


            //Add properties with NavigatableCommandAttribute as subitems.
            navigatableCommand.GetProperties()
                    .Where(x => x.GetCustomAttribute<NavigatableCommandAttribute>() != null)
                    .ToList()
                    .ForEach(
                        subCommand =>
                            subItems.Add(ProcessNavigatableCommand(subCommand.PropertyType,
                                subCommand.GetCustomAttribute<NavigatableCommandAttribute>())));

            var typedNavigatableCommand = new NavigatableCommand()
            {
                Identifier = navigatableAttribute.Identifier.Trim().ToLowerInvariant(),
                Help = navigatableAttribute.Help,
                SubItems = subItems,
                Type = navigatableCommand,
                AnnounceMethod = GetAnnounceMethod(navigatableCommand)
            };

            AddInitializeMethod(navigatableCommand, typedNavigatableCommand);
            
            return typedNavigatableCommand;
        }

        private MethodInfo GetAnnounceMethod(Type navigatableCommand)
        {
            var navigatableCommandMethods = navigatableCommand.GetMethods();

            var announceMethods =
                navigatableCommandMethods.Where(
                    x => x.GetCustomAttribute<NavigatableCommandAnnouncementAttribute>() != null).ToList();


            if (announceMethods.Count > 1)
            {
                throw new Exception(
                    $"{navigatableCommand.Name} has multiple methods with the attribute {nameof(NavigatableCommandAnnouncementAttribute)}");
            }

            var announceMethod = announceMethods.SingleOrDefault();
            return announceMethod;
        }

        private void AddInitializeMethod(Type navigatableCommand, NavigatableCommand typedNavigatableCommand)
        {
            var initializeMethods =
                navigatableCommand.GetMethods()
                    .Where(x => x.GetCustomAttribute<NavigatableCommandInitialisationAttribute>() != null).ToList();
            if (initializeMethods.Count > 1)
            {
                throw new Exception(
                    $"{navigatableCommand.Name} has multiple methods with the attribute {nameof(NavigatableCommandInitialisationAttribute)}");
            }
            var initializeMethod = initializeMethods.SingleOrDefault();
            typedNavigatableCommand.Parameters = initializeMethod?.GetParameters().ToList();
            typedNavigatableCommand.MethodInfo = initializeMethod;
        }

        private void ProcessNestedCommands()
        {
            var nestedCommands = _scannableTypes.Where(x =>
            {
                var typeInfo = x.GetTypeInfo();
                return typeInfo.GetCustomAttribute<NestedCommandAttribute>() != null;
            });
            
            foreach (var nestedCommand in nestedCommands)
            {
                var nestedAttribute = nestedCommand.GetTypeInfo().GetCustomAttribute<NestedCommandAttribute>();
                var nestedMethods = nestedCommand.GetMethods().Where(x => x.GetCustomAttribute<CommandAttribute>() != null);
                foreach (MethodInfo methodInfo in nestedMethods)
                {
                    var methodAttribute = methodInfo.GetCustomAttribute<CommandAttribute>();
                    var identifier = $"{nestedAttribute.Identifier.Trim()} {methodAttribute.Identifier.Trim()}";
                    AddSingleCommand(nestedCommand, methodInfo, identifier, methodAttribute.Help);
                }
            }
        }
        private void ProcessSingleCommands()
        {
            var typesLeft = _scannableTypes.Where(
                x =>
                    _navigatableTypes.All(y => y != x) &&
                    x.GetTypeInfo().GetCustomAttribute<NestedCommandAttribute>() == null);

            foreach (Type type in typesLeft)
            {
                var methods = type.GetMethods().Where(x => x.GetCustomAttribute<CommandAttribute>() != null);
                foreach (MethodInfo methodInfo in methods)
                {
                    var methodAttribute = methodInfo.GetCustomAttribute<CommandAttribute>();
                    AddSingleCommand(type, methodAttribute, methodInfo);
                }
            }
        }
        private void AddSingleCommand(Type type, CommandAttribute methodAttribute, MethodInfo methodInfo)
        {
            AddSingleCommand(type, methodInfo, methodAttribute.Identifier.Trim(), methodAttribute.Help);
        }
        private void AddSingleCommand(Type type, MethodInfo methodInfo, string identifier, string help)
        {
            _menu.Add(CreateCommand(type, methodInfo, identifier, help));
        }

        private SingleCommand CreateCommand(Type type, MethodInfo methodInfo, string identifier, string help)
        {
            return new SingleCommand
            {
                Identifier = identifier.ToLowerInvariant(),
                Help = help,
                Parameters = methodInfo.GetParameters().ToList(),
                Type = type,
                MethodInfo = methodInfo
            };
        }

        private SingleCommand CreateCommand(Type type, MethodInfo methodInfo, CommandAttribute attribute)
        {
            return CreateCommand(type, methodInfo, attribute.Identifier.Trim(), attribute.Help);
        }
    }
}
