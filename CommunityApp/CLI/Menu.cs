namespace CommunityApp;

using Spectre.Console;

public class Menu
{
    private Resident? _currentUser;
    private readonly LoginView _loginView = new();
    private readonly AnnouncementView _announcementView = new();
    private readonly EventView _eventView = new();
    private readonly JsonDataService _dataService = new();
    private readonly string _announcementsPath = Path.Combine("Data", "announcements.json");

    public void Run()
    {
        while (true)
        {
            if (_currentUser == null && (_currentUser = _loginView.Show()) == null)
            {
                return;
            }

            AnsiConsole.Clear();
            ShowLatestAnnouncement();
            ShowUpcomingReminders();

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[yellow]{_currentUser.Name} ({_currentUser.Role})[/] — What would you like to do?")
                    .AddChoices("Create Announcement", "View Announcements", "Add Event", "View Events", "Logout", "Exit"));

            switch (choice)
            {
                case "Create Announcement":
                    _announcementView.CreateStandard(_currentUser);
                    break;

                case "View Announcements":
                    _announcementView.ViewAll();
                    break;

                case "Add Event":
                    _eventView.Add(_currentUser);
                    break;

                case "View Events":
                    _eventView.ViewAll();
                    break;

                case "Logout":
                    _currentUser = null;
                    break;

                case "Exit":
                    return;
            }
        }
    }

    private void ShowUpcomingReminders()
    {
        if (_currentUser == null) return;

        var residents = _dataService.LoadData<Resident>(Path.Combine("Data", "residents.json"));
        var fullUser = residents.FirstOrDefault(r => r.Id == _currentUser.Id);
        if (fullUser == null) return;

        var now = DateTime.Now;
        var upcoming = fullUser.Commitments
            .Where(c => c.Attending && c.Event.Start > now && c.Event.Start <= now.AddDays(7))
            .OrderBy(c => c.Event.Start)
            .ToList();

        if (!upcoming.Any()) return;

        var lines = string.Join("\n", upcoming.Select(
            c => $"• [bold]{c.Event.Title}[/] — {c.Event.Start:MMM dd, yyyy h:mm tt} at {c.Event.Location}"));

        AnsiConsole.Write(
            new Panel(lines)
                .Header("[yellow]Upcoming Events (Next 7 Days)[/]")
                .BorderColor(Color.Yellow)
                .Expand());
    }

    private void ShowLatestAnnouncement()
    {
        var announcements = _dataService.LoadData<Announcement>(_announcementsPath);
        var latest = announcements.OrderByDescending(a => a.CreatedAt).FirstOrDefault();

        if (latest != null)
        {
            var color = latest.Type switch
            {
                AnnouncementType.AdminAlert   => "red",
                AnnouncementType.LostAndFound => "orange3",
                AnnouncementType.ForSale      => "green",
                AnnouncementType.Maintenance  => "yellow",
                AnnouncementType.Community    => "purple",
                _                             => "blue",
            };
            AnsiConsole.Write(
                new Panel($"[bold {color}]{latest.Title}[/]\n{latest.Body}")
                    .Header("[grey]Latest Announcement[/]")
                    .BorderColor(Color.Grey)
                    .Expand());
        }
    }
}