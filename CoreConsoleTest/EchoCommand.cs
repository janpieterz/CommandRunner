using System;
using System.Collections.Generic;

namespace CommandRunner.CoreConsoleTest
{
    public class EchoCommand
    {
        public Injectable Injectable { get; set; }

        [Command("echo", "Echo back anything following the command.")]
        public void Execute(List<string> args)
        {
            foreach (var arg in args) Console.WriteLine(arg);
        }

        [Command("techo", "Echo back with parsed param")]
        public void Execute(string whatever, bool say, int count, Guid itemId)
        {
            for (int i = 0; i < count; i++)
            {
                if (say)
                {
                    Console.WriteLine(whatever);
                }
            }
        }
    }
    [NestedCommand("nesting")]
    public class Nester
    {
        [Command("inside", "This is an example help.")]
        public void Test()
        {
            Console.WriteLine("Just a normal inside method.");
        }

        [Command("child")]
        public void Child(Guid test)
        {
            Console.WriteLine("A method with a nice Guid: {0}", test);
        }

        [Command("more more")]
        public void More(List<string> items)
        {
            Console.WriteLine("More more more stuff:");
            items.ForEach(Console.WriteLine);
        }

        [Command("optional", "Another example help text")]
        public void Optional(string test = null)
        {
            Console.WriteLine("Method with something optional: '{0}'", test);
        }

        [Command("multiple", "Multiple explanation")]
        public void Multiple(int count, string message = null)
        {
            Console.WriteLine("A count '{0}' and an optional message: '{1}'", count, message);
        }

        [Command("guids")]
        public void Loads(List<Guid> items)
        {
            Console.WriteLine("Loads ({0}) of guids as a list argument!", items.Count);
            items.ForEach(item => Console.WriteLine(item.ToString()));
        }

        [Command("arrays")]
        public void ArraysFull(int[] items)
        {
            Console.WriteLine("Loads ({0}) of int items as an int array", items.Length);
            foreach (int item in items)
            {
                Console.WriteLine(item);
            }
        }

        [Command("nullable")]
        public void Nullable(Guid? nullable)
        {
            Console.WriteLine("A nullable Guid: '{0}'", nullable.HasValue ? nullable.Value.ToString() : "null");
        }

        [Command("numbers")]
        public void Numbers(int count = 0, int items = 5)
        {
            Console.WriteLine("A count of '{0}' and '{1}' items", count, items);
        }
    }

    [NavigatableCommand("account")]
    public class AccountMenu
    {
        public Guid AccountId { get; set; }
        [NavigatableCommandInitialisation]
        public void Initialize(Guid accountId)
        {
            AccountId = accountId;
        }

        [NavigatableCommandAnnouncement]
        public void Announce()
        {
            Console.WriteLine("Account menu for account {0}", AccountId);
        }

        [Command("show contacts")]
        public void ShowContacts()
        {
            Console.WriteLine($"Showing contacts for account {AccountId}");
        }

        [NavigatableCommand("clients")]
        public AccountClientsMenu AccountClientsMenu { get; set; }
    }
    
    public class AccountClientsMenu
    {
        public AccountMenu AccountMenu { get; set; }
        public Guid ClientId { get; set; }

        [NavigatableCommandInitialisation]
        public void Initialize(Guid clientId)
        {
            ClientId = clientId;
        }

        [NavigatableCommandAnnouncement]
        public void Announce()
        {
            Console.WriteLine("Client menu for account {0}", ClientId);
        }

        [Command("show users")]
        public void ShowUsers()
        {
            Console.WriteLine("Showing users for client {0} and account {1}!", ClientId, AccountMenu.AccountId);
        }

        [Command("set location")]
        public void SetLocation(string location)
        {
            Console.WriteLine("Setting location: {0}", location);
        }
    }
    [NavigatableCommand("servicebus")]
    public class ServiceBusMenu
    {
        [Command("show queues")]
        public void ShowQueues()
        {
            Console.WriteLine("Showing queues.");
        }
    }
}
