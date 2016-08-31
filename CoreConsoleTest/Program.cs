namespace CommandRunner.CoreConsoleTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new Runner(options =>
            {
                options.Title = "Command Runner";
                options.Scan.AllAssemblies();
                options.Activate.WithReflectionActivator();

            }).Run();
        }
    }
}
