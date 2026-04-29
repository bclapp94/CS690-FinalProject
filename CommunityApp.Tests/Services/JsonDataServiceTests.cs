using CommunityApp;

public class JsonDataServiceTests : IDisposable
{
    private readonly string _tempDir;
    private readonly JsonDataService _service;

    public JsonDataServiceTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(_tempDir);
        _service = new JsonDataService();
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }

    private string TempFile(string name) => Path.Combine(_tempDir, name);

    [Fact]
    public void LoadData_ReturnsEmpty_WhenFileDoesNotExist()
    {
        var path = TempFile("nonexistent.json");

        var result = _service.LoadData<Announcement>(path);

        Assert.Empty(result);
    }

    [Fact]
    public void SaveData_CreatesFile_AndCanBeLoadedBack()
    {
        var path = TempFile("announcements.json");
        var items = new List<Announcement>
        {
            new() { Title = "Test", Body = "Body", CreatedById = Guid.NewGuid() }
        };

        _service.SaveData(path, items);
        var loaded = _service.LoadData<Announcement>(path);

        Assert.Single(loaded);
        Assert.Equal("Test", loaded[0].Title);
        Assert.Equal("Body", loaded[0].Body);
    }

    [Fact]
    public void SaveData_CreatesDirectory_IfItDoesNotExist()
    {
        var nestedPath = Path.Combine(_tempDir, "nested", "dir", "data.json");
        var items = new List<Announcement>
        {
            new() { Title = "Test", Body = "Body", CreatedById = Guid.NewGuid() }
        };

        _service.SaveData(nestedPath, items);

        Assert.True(File.Exists(nestedPath));
    }

    [Fact]
    public void SaveData_ThenLoadData_PreservesMultipleItems()
    {
        var path = TempFile("multi.json");
        var creatorId = Guid.NewGuid();
        var items = new List<Announcement>
        {
            new() { Title = "First",  Body = "Body 1", Type = AnnouncementType.AdminAlert, CreatedById = creatorId },
            new() { Title = "Second", Body = "Body 2", Type = AnnouncementType.Normal,     CreatedById = creatorId }
        };

        _service.SaveData(path, items);
        var loaded = _service.LoadData<Announcement>(path);

        Assert.Equal(2, loaded.Count);
        Assert.Equal("First", loaded[0].Title);
        Assert.Equal(AnnouncementType.Normal, loaded[1].Type);
    }

    [Fact]
    public void LoadData_ReturnsEmpty_WhenFileIsEmpty()
    {
        var path = TempFile("empty.json");
        File.WriteAllText(path, "");

        var result = _service.LoadData<Announcement>(path);

        Assert.Empty(result);
    }

    [Fact]
    public void SaveData_OverwritesExistingFile()
    {
        var path = TempFile("overwrite.json");
        var first = new List<Announcement> { new() { Title = "Old", Body = "Old", CreatedById = Guid.NewGuid() } };
        var second = new List<Announcement> { new() { Title = "New", Body = "New", CreatedById = Guid.NewGuid() } };

        _service.SaveData(path, first);
        _service.SaveData(path, second);
        var loaded = _service.LoadData<Announcement>(path);

        Assert.Single(loaded);
        Assert.Equal("New", loaded[0].Title);
    }

    [Fact]
    public void SaveData_PreservesEnum_AsString()
    {
        var path = TempFile("enum.json");
        var items = new List<Announcement>
        {
            new() { Title = "T", Body = "B", Type = AnnouncementType.AdminAlert, CreatedById = Guid.NewGuid() }
        };

        _service.SaveData(path, items);
        var json = File.ReadAllText(path);

        Assert.Contains("AdminAlert", json);
    }
}
