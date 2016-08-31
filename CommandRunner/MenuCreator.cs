using System.Collections.Generic;
using System.Reflection;

namespace CommandRunner
{
    public class MenuCreator
    {
        internal static IEnumerable<IMenuItem> CreateMenuItems(List<MethodInfo> commandMethods,
            IActivationConfiguration activationConfiguration)
        {
            var menuItems = new List<IMenuItem>();
            commandMethods.ForEach(commandMethod =>
            {
                var commandAttribute = commandMethod.GetCustomAttribute<CommandAttribute>();
                var attributeOnClass = commandMethod.DeclaringType.GetTypeInfo().GetCustomAttribute<CommandAttribute>();
                var identifier = commandAttribute.Identifier;
                if (attributeOnClass != null)
                {
                    identifier = attributeOnClass.Identifier + " " + identifier;
                    menuItems.Add(new ContainerCommand(attributeOnClass.Identifier, attributeOnClass.Help));
                }
                if (activationConfiguration.Activator != null)
                {
                    menuItems.Add(new CustomActivatorCommand(identifier, commandAttribute.Help, commandMethod, activationConfiguration.Activator));
                }
                else
                {
                    menuItems.Add(new ReflectedMethodCommand(identifier, commandAttribute.Help, commandMethod));
                }
            });
            return menuItems;
        }
    }
}