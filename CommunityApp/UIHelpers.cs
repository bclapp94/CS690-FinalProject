using Spectre.Console;

namespace CommunityApp
{
    public static class UIHelpers
    {
        public static void PressAnyKey(string markup = "[grey]Press any key to continue...[/]")
        {
            AnsiConsole.MarkupLine(markup);
            Console.ReadKey();
        }
    }
}