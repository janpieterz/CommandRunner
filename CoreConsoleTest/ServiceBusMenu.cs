using System;

namespace CommandRunner.CoreConsoleTest
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