namespace CommandRunner
{
    public class MenuCreator
    {
        // internal static IEnumerable<IMenuItem> CreateMenuItems(List<MethodInfo> commandMethods,
        //     IActivationConfiguration activationConfiguration)
        // {
        //     var menuItems = new List<IMenuItem>();
        //     commandMethods.ForEach(commandMethod =>
        //     {
        //         var commandAttribute = commandMethod.GetCustomAttribute<CommandAttribute>();
        //         var attributeOnClass = commandMethod.DeclaringType.GetTypeInfo().GetCustomAttribute<CommandAttribute>();
        //         var identifier = commandAttribute.Identifier;
        //         ContainerCommand container = null;
        //         if (attributeOnClass != null)
        //         {
        //             identifier = attributeOnClass.Identifier + " " + identifier;
        //             var containerItem = menuItems.SingleOrDefault(x => x.Title == attributeOnClass.Identifier);
        //             if (containerItem == null)
        //             {
        //                 container = new ContainerCommand(attributeOnClass.Identifier, attributeOnClass.Help);
        //                 menuItems.Add(container);
        //             }
        //             else
        //             {
        //                 container = (ContainerCommand) containerItem;
        //             }
        //         }
        //         IMenuItem menuItem;
        //         if (activationConfiguration.Activator != null)
        //         {
        //             menuItem = new CustomActivatorCommand(identifier, commandAttribute.Help, commandMethod,
        //                 activationConfiguration.Activator);
        //             //menuItems.Add(new CustomActivatorCommand(identifier, commandAttribute.Help, commandMethod, activationConfiguration.Activator));
        //         }
        //         else
        //         {
        //             menuItem = new ReflectedMethodCommand(identifier, commandAttribute.Help, commandMethod);
        //             //menuItems.Add(new ReflectedMethodCommand(identifier, commandAttribute.Help, commandMethod));
        //         }
        //         if (container == null)
        //         {
        //             menuItems.Add(menuItem);
        //         }
        //         else
        //         {
        //             container.Items.Add(menuItem);
        //         }
        //     });
        //     return menuItems;
        // }
    }
}