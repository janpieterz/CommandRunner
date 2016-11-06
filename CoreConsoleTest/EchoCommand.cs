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
            
        }

        [Command("child")]
        public void Child(Guid test)
        {
            
        }

        [Command("more more")]
        public void More(List<string> items)
        {
            
        }

        [Command("optional", "Another example help text")]
        public void Optional(string test = null)
        {
            
        }

        [Command("multiple", "Multiple explanation")]
        public void Multiple(int count, string message = null)
        {
            
        }

        [Command("guids")]
        public void Loads(List<Guid> items)
        {
            
        }

        [Command("arrays")]
        public void ArraysFull(int[] items)
        {
            
        }

        [Command("nullable")]
        public void Nullable(Guid? nullable)
        {
            
        }

        [Command("numbers")]
        public void Numbers(int count = 0, int items = 5)
        {
            
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
        [NavigatableCommandInitialisation]
        public void Initialize()
        {
            
        }

        [Command("show users")]
        public void ShowUsers()
        {
            
        }

        [Command("set location")]
        public void SetLocation(string location)
        {
            
        }
    }
}
