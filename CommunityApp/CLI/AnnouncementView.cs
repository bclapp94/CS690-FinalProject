namespace CommunityApp;

using Spectre.Console;

public class AnnouncementView
{
    private readonly JsonDataService _dataService = new();
    private readonly string _filePath = Path.Combine("Data", "announcements.json");
    public void CreateStandard(Resident currentUser)
    {
        AnsiConsole.Clear();

        AnsiConsole.Write(
            new Panel("[bold blue]Post Announcement[/]")
                .BorderColor(Color.Blue));

        var title = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter [green]title[/]:")
                .Validate(input =>
                    string.IsNullOrWhiteSpace(input)
                        ? ValidationResult.Error("[red]Title is required[/]")
                        : ValidationResult.Success()));

        var body = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter [green]body[/]:")
                .Validate(input =>
                    string.IsNullOrWhiteSpace(input)
                        ? ValidationResult.Error("[red]Body is required[/]")
                        : ValidationResult.Success()));

        AnnouncementType type = AnnouncementType.Normal;

        if (currentUser.Role == Role.Admin || currentUser.Role == Role.Manager)
        {
            type = AnsiConsole.Prompt(
                new SelectionPrompt<AnnouncementType>()
                    .Title("Select [yellow]announcement type[/]:")
                    .HighlightStyle("green")
                    .AddChoices(Enum.GetValues<AnnouncementType>()));
        }
        else
        {
            AnsiConsole.MarkupLine("[grey]Type: Normal (default)[/]");
        }

        var confirm = AnsiConsole.Confirm("Submit announcement?");

        if (!confirm)
        {
            AnsiConsole.MarkupLine("[red]Cancelled.[/]");
            return;
        }

        var announcement = new Announcement
        {
            Title = title,
            Body = body,
            Type = type,
            CreatedAt = DateTime.UtcNow,
            CreatedById = currentUser.Id
        };

        var announcements = _dataService.LoadData<Announcement>(_filePath);
        announcements.Add(announcement);
        _dataService.SaveData(_filePath, announcements);

        AnsiConsole.MarkupLine("[green]✔ Announcement created successfully![/]");

        AnsiConsole.MarkupLine("[grey]Press any key to return...[/]");
        Console.ReadKey();
    }
}