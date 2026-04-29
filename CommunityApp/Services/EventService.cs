namespace CommunityApp;

public class EventService
{
    private readonly JsonDataService _dataService = new();
    private const string FilePath = "Data/events.json";

    public List<Event> GetAll()
    {
        return _dataService.LoadData<Event>(FilePath)
            .OrderBy(e => e.Start)
            .ToList();
    }

    public void Save(Event ev)
    {
        var events = _dataService.LoadData<Event>(FilePath);
        events.Add(ev);
        _dataService.SaveData(FilePath, events);
    }

    public void Update(Event ev)
    {
        var events = _dataService.LoadData<Event>(FilePath);
        var index = events.FindIndex(e => e.Title == ev.Title && e.Start == ev.Start);

        if (index >= 0)
        {
            events[index] = ev;
            _dataService.SaveData(FilePath, events);
        }
    }

    public bool HasConflict(DateTime start, DateTime end)
    {
        var events = _dataService.LoadData<Event>(FilePath);
        return events.Any(e => start < e.End && end > e.Start);
    }

    public bool CurrentUserAttending(Event ev)
    {
        var residents = new JsonDataService().LoadData<Resident>("Data/residents.json");
        var currentUser = Globals.CurrentUser != null
            ? residents.FirstOrDefault(r => r.Id == Globals.CurrentUser.Id)
            : null;

        return currentUser != null && currentUser.Commitments.Any(c => c.Event.Title == ev.Title && c.Event.Start == ev.Start);
    }
}
