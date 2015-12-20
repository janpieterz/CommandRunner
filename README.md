# CommandRunner
A simple command runner to enable quick command line (developer) tools
The runner is still in development mode so there will be breaking changes.

# Usage
```c#
static void Main(string[] args)
{
    var program = new Program();
    program.Run();
}

private void Run()
{
    Runner.Start(
        new GoogleSearchCommand(),
        new XyZCommand()
        );
}

public class GoogleSearchCommand : ICommand
  {
      public IEnumerable<string> Help => new List<string>()
      {
          "text <searchcontent>",
          "image <searchcontent>"
      };

      public string Command => "googlesearch";
      public void Execute(IEnumerable<string> arguments)
      {
          var args = arguments.ToList();
          if (!args.Any())
          {
              ConsoleMessages.Error("No arguments provided.");
              return;
          }

          if (args[0] == "text")
          {
              TextSearch(args.Skip(1).ToList());
          }
          else if (args[0] == "image")
          {
              ImageSearch(args.Skip(1).ToList());
          }
          else
          {
              ConsoleMessages.Error("No valid arguments provided.");
          }
      }

      private void ImageSearch(List<string> arguments)
      {
          throw new System.NotImplementedException();
      }

      private void TextSearch(List<string> arguments)
      {
          throw new System.NotImplementedException();
      }
  }
```
