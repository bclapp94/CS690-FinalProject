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
            residents = new List<Resident>
            {
            new Resident { Name = "Admin", Role = Role.Admin, Address = "N/A" },
            new Resident { Name = "Resident", Role = Role.Resident, Address = "N/A" }
            };
        }
        
        AnsiConsole.MarkupLine("[yellow]No users found. Creating default users...[/]");
        UIHelpers.PressAnyKey();

        _dataService.SaveData(_residentsPath, residents);

        AnsiConsole.MarkupLine("[green]Default users created![/]");
        UIHelpers.PressAnyKey();

        residents = _dataService.LoadData<Resident>(_residentsPath);

        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<Resident>()
                .Title("Select a [yellow]user[/] to login:")
                .PageSize(10)
                .UseConverter(r => $"{r.Name} ({r.Role})")
                .AddChoices(residents));

        AnsiConsole.MarkupLine($"[green]Logged in as {selected.Name}[/]");
        UIHelpers.PressAnyKey();

        return selected;
    }
}