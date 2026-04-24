namespace CommunityApp;

using Spectre.Console;

public class LoginView
{
    private readonly JsonDataService _dataService = new();

    private readonly string _residentsPath =
        Path.Combine("Data", "residents.json");

    public Resident? Show()
    {
        AnsiConsole.Clear();

        AnsiConsole.Write(
            new Panel("[bold green]Login[/]")
                .BorderColor(Color.Green));

        var residents = _dataService.LoadData<Resident>(_residentsPath);

        if (residents == null || !residents.Any())
        {
            AnsiConsole.MarkupLine("[red]No users found in system.[/]");
            AnsiConsole.MarkupLine("[grey]Press any key to exit...[/]");
            Console.ReadKey();
            return null;
        }

        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<Resident>()
                .Title("Select a [yellow]user[/] to login:")
                .PageSize(10)
                .UseConverter(r => $"{r.Name} ({r.Role})")
                .AddChoices(residents));

        AnsiConsole.MarkupLine($"[green]Logged in as {selected.Name}[/]");
        AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
        Console.ReadKey();

        return selected;
    }
}