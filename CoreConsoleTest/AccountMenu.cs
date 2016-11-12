using System;

namespace CommandRunner.CoreConsoleTest
{
    [NavigatableCommand("account")]
    public class AccountMenu
    {
        public Guid AccountId { get; set; }
        [NavigatableCommandInitialization]
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
}