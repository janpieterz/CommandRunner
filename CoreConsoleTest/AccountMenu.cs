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

        [Command("show contacts", "Shows the contacts for the account and is a really really really, like extremely super long menu to show word wrapping and indentation.")]
        public void ShowContacts()
        {
            Console.WriteLine($"Showing contacts for account {AccountId}");
        }

        [NavigatableCommand("clients")]
        public AccountClientsMenu AccountClientsMenu { get; set; }
    }
}