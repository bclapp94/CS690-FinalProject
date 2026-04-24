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
            if (_currentUser == null)
            {
                _currentUser = _loginView.Show();
                continue;
            }

            AnsiConsole.Clear();
            ShowLatestAnnouncement();

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[yellow]{_currentUser.Name} ({_currentUser.Role})[/] — What would you like to do?")
                    .AddChoices("Create Announcement", "Add Event", "View Events", "Logout", "Exit"));

            switch (choice)
            {
                case "Create Announcement":
                    _announcementView.CreateStandard(_currentUser);
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

    private void ShowLatestAnnouncement()
    {
        var announcements = _dataService.LoadData<Announcement>(_announcementsPath);
        var latest = announcements.OrderByDescending(a => a.CreatedAt).FirstOrDefault();

        if (latest != null)
        {
            var color = latest.Type == AnnouncementType.AdminAlert ? "red" : "blue";
            AnsiConsole.Write(
                new Panel($"[bold {color}]{latest.Title}[/]\n{latest.Body}")
                    .Header("[grey]Latest Announcement[/]")
                    .BorderColor(Color.Grey)
                    .Expand());
        }
    }
}