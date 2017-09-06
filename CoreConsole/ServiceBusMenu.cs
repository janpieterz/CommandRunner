using System;

namespace CommandRunner.CoreConsole
{
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