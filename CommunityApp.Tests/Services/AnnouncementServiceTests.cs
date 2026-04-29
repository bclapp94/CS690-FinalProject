using CommunityApp;

[Collection("Sequential")]
public class AnnouncementServiceTests : IDisposable
{
    private readonly JsonDataService _dataService;
    private readonly AnnouncementService _service;
    private readonly string _originalDir;
    private readonly List<string> _tempDirs = new();

    public AnnouncementServiceTests()
    {
        _originalDir = Directory.GetCurrentDirectory();
        _dataService = new JsonDataService();
        _service = new AnnouncementService(_dataService);
    }

    public void Dispose()
    {
        Directory.SetCurrentDirectory(_originalDir);

        foreach (var dir in _tempDirs)
        {
            if (Directory.Exists(dir))
                Directory.Delete(dir, recursive: true);
        }
    }

    private void SetupTempWorkingDir()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(Path.Combine(tempDir, "Data"));
        _tempDirs.Add(tempDir);
        Directory.SetCurrentDirectory(tempDir);
    }

    [Fact]
    public void GetAll_ReturnsEmpty_WhenNoAnnouncements()
    {
        SetupTempWorkingDir();

        var result = _service.GetAll();

        Assert.Empty(result);
    }

    [Fact]
    public void CreateAnnouncement_AddsAnnouncement()
    {
        SetupTempWorkingDir();

        var announcement = new Announcement
        {
            Title = "Welcome",
            Body = "Welcome to the community!",
            Type = AnnouncementType.Normal,
            CreatedById = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };

        _service.CreateAnnouncement(announcement);
        var result = _service.GetAll();

        Assert.Single(result);
        Assert.Equal("Welcome", result[0].Title);
    }

    [Fact]
    public void CreateAnnouncement_Persists_MultipleAnnouncements()
    {
        SetupTempWorkingDir();

        _service.CreateAnnouncement(new Announcement { Title = "First",  Body = "B1", CreatedById = Guid.NewGuid() });
        _service.CreateAnnouncement(new Announcement { Title = "Second", Body = "B2", CreatedById = Guid.NewGuid() });

        var result = _service.GetAll();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void CreateAnnouncement_PreservesType()
    {
        SetupTempWorkingDir();

        _service.CreateAnnouncement(new Announcement
        {
            Title = "Alert",
            Body = "Emergency!",
            Type = AnnouncementType.AdminAlert,
            CreatedById = Guid.NewGuid()
        });

        var result = _service.GetAll();

        Assert.Equal(AnnouncementType.AdminAlert, result[0].Type);
    }

    [Fact]
    public void GetAll_ReturnsAll_SavedAnnouncements()
    {
        SetupTempWorkingDir();

        for (int i = 1; i <= 3; i++)
        {
            _service.CreateAnnouncement(new Announcement
            {
                Title = $"Announcement {i}",
                Body = $"Body {i}",
                CreatedById = Guid.NewGuid()
            });
        }

        var result = _service.GetAll();

        Assert.Equal(3, result.Count);
    }
}
