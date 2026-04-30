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

        var availableTypes = (currentUser.Role == Role.Admin || currentUser.Role == Role.Manager)
            ? Enum.GetValues<AnnouncementType>()
            : Enum.GetValues<AnnouncementType>().Where(t => t != AnnouncementType.AdminAlert).ToArray();

        var type = AnsiConsole.Prompt(
            new SelectionPrompt<AnnouncementType>()
                .Title("Select [yellow]announcement type[/]:")
                .HighlightStyle("green")
                .AddChoices(availableTypes));

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

    public void ViewAll()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Panel("[bold blue]Announcements[/]").BorderColor(Color.Blue));

        var filterOptions = new[] { "All", "Normal", "Admin Alert", "Lost & Found", "For Sale", "Maintenance", "Community", "← Back" };

        var filter = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Filter by [yellow]type[/]:")
                .AddChoices(filterOptions));

        if (filter == "← Back") return;

        var typeMap = new Dictionary<string, AnnouncementType>
        {
            ["Normal"]       = AnnouncementType.Normal,
            ["Admin Alert"]  = AnnouncementType.AdminAlert,
            ["Lost & Found"] = AnnouncementType.LostAndFound,
            ["For Sale"]     = AnnouncementType.ForSale,
            ["Maintenance"]  = AnnouncementType.Maintenance,
            ["Community"]    = AnnouncementType.Community,
        };

        var announcements = _dataService.LoadData<Announcement>(_filePath)
            .OrderByDescending(a => a.CreatedAt)
            .ToList();

        if (typeMap.TryGetValue(filter, out var selectedType))
            announcements = announcements.Where(a => a.Type == selectedType).ToList();

        if (!announcements.Any())
        {
            AnsiConsole.MarkupLine("[grey]No announcements found.[/]");
            UIHelpers.PressAnyKey();
            return;
        }

        foreach (var a in announcements)
        {
            var (textColor, borderColor) = a.Type switch
            {
                AnnouncementType.AdminAlert  => ("red",    Color.Red),
                AnnouncementType.LostAndFound => ("orange3", Color.Orange3),
                AnnouncementType.ForSale      => ("green",  Color.Green),
                AnnouncementType.Maintenance  => ("yellow", Color.Yellow),
                AnnouncementType.Community    => ("purple", Color.Purple),
                _                             => ("blue",   Color.Blue),
            };
            AnsiConsole.Write(
                new Panel($"[bold {textColor}]{a.Title}[/]\n{a.Body}\n[grey]{a.CreatedAt:MMM dd, yyyy HH:mm} UTC · {a.Type}[/]")
                    .BorderColor(borderColor)
                    .Expand());
        }

        UIHelpers.PressAnyKey();
    }
}