using System;

namespace CommandRunner.CoreConsoleTest
{
    public class AccountClientsMenu
    {
        public AccountMenu AccountMenu { get; set; }
        public Guid ClientId { get; set; }

        [NavigatableCommandInitialization]
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
}