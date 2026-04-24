namespace CommunityApp
{
    public class AnnouncementService
{
    private readonly JsonDataService _dataService;
    private const string FilePath = "Data/announcements.json";

    public AnnouncementService(JsonDataService dataService)
    {
        _dataService = dataService;
    }

    public void CreateAnnouncement(Announcement announcement)
    {
        var announcements = _dataService.LoadData<Announcement>(FilePath);

        announcements.Add(announcement);

        _dataService.SaveData(FilePath, announcements);
    }

    public List<Announcement> GetAll()
    {
        return _dataService.LoadData<Announcement>(FilePath);
    }
}
}