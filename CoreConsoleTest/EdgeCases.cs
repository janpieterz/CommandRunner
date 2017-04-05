using System;
using System.Threading.Tasks;

namespace CommandRunner.CoreConsoleTest
{
    public class EdgeCases
    {
        [Command("throw exception")]
        public void ThrowException()
        {
            throw new Exception("Exception thrown!");
        }
        [Command("count void async")]
        public async void CountAsync()
        {
            int t = await Task.Run(() => Allocate());
            Console.WriteLine("Compute: " + t);
        }

        [Command("count task async")]
        public async Task CountAsyncTask()
        {
            int t = await Task.Run(() => Allocate());
            Console.WriteLine("Compute: " + t);
        }

        [Command("count task")]
        public Task CountTask()
        {
            return Task.Run(() =>
            {
                Console.WriteLine(Allocate());
            });
        }

        static int Allocate()
        {
            // Compute total count of digits in strings.
            int size = 0;
            for (int z = 0; z < 100; z++)
            {
                for (int i = 0; i < 1000000; i++)
                {
                    string value = i.ToString();
                    if (value == null)
                    {
                        return 0;
                    }
                    size += value.Length;
                }
            }
            return size;
        }
    }
}
