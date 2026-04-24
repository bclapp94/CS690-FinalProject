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

    public bool HasConflict(DateTime start, DateTime end)
    {
        var events = _dataService.LoadData<Event>(FilePath);
        return events.Any(e => start < e.End && end > e.Start);
    }
}
