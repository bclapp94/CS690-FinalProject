using CommunityApp;

[Collection("Sequential")]
public class EventServiceTests : IDisposable
{
    private readonly EventService _service;
    private readonly string _originalDir;
    private readonly List<string> _tempDirs = new();

    public EventServiceTests()
    {
        _originalDir = Directory.GetCurrentDirectory();
        _service = new EventService();
    }

    public void Dispose()
    {
        Directory.SetCurrentDirectory(_originalDir);
        Globals.CurrentUser = null;

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

    private static Resident MakeResident(string name = "Alice") => new()
    {
        Name = name,
        Address = "123 Main St",
        Role = Role.Resident
    };

    private static Event MakeEvent(string title, DateTime start, DateTime end, Resident? creator = null) => new()
    {
        Title = title,
        Description = "Test",
        Start = start,
        End = end,
        Location = "Test Location",
        CreatedBy = creator ?? MakeResident(),
        EventType = EventType.Standard
    };

    [Fact]
    public void GetAll_ReturnsEmpty_WhenNoEvents()
    {
        SetupTempWorkingDir();

        var result = _service.GetAll();

        Assert.Empty(result);
    }

    [Fact]
    public void Save_PersistsEvent()
    {
        SetupTempWorkingDir();

        var ev = MakeEvent("BBQ", DateTime.Today.AddHours(10), DateTime.Today.AddHours(12));
        _service.Save(ev);
        var result = _service.GetAll();

        Assert.Single(result);
        Assert.Equal("BBQ", result[0].Title);
    }

    [Fact]
    public void GetAll_ReturnEvents_SortedByStart()
    {
        SetupTempWorkingDir();

        var later   = MakeEvent("Later",   DateTime.Today.AddHours(14), DateTime.Today.AddHours(16));
        var earlier = MakeEvent("Earlier", DateTime.Today.AddHours(8),  DateTime.Today.AddHours(10));

        _service.Save(later);
        _service.Save(earlier);
        var result = _service.GetAll();

        Assert.Equal("Earlier", result[0].Title);
        Assert.Equal("Later",   result[1].Title);
    }

    [Fact]
    public void HasConflict_ReturnsFalse_WhenNoEvents()
    {
        SetupTempWorkingDir();

        var result = _service.HasConflict(DateTime.Today.AddHours(10), DateTime.Today.AddHours(12));

        Assert.False(result);
    }

    [Fact]
    public void HasConflict_ReturnsTrue_WhenEventOverlaps()
    {
        SetupTempWorkingDir();
        _service.Save(MakeEvent("Existing", DateTime.Today.AddHours(10), DateTime.Today.AddHours(12)));

        var result = _service.HasConflict(DateTime.Today.AddHours(11), DateTime.Today.AddHours(13));

        Assert.True(result);
    }

    [Fact]
    public void HasConflict_ReturnsFalse_WhenEventIsAdjacent()
    {
        SetupTempWorkingDir();
        _service.Save(MakeEvent("Existing", DateTime.Today.AddHours(10), DateTime.Today.AddHours(12)));

        var result = _service.HasConflict(DateTime.Today.AddHours(12), DateTime.Today.AddHours(14));

        Assert.False(result);
    }

    [Fact]
    public void HasConflict_ReturnsTrue_WhenNewEvent_FullyContainsExisting()
    {
        SetupTempWorkingDir();
        _service.Save(MakeEvent("Existing", DateTime.Today.AddHours(10), DateTime.Today.AddHours(11)));

        var result = _service.HasConflict(DateTime.Today.AddHours(9), DateTime.Today.AddHours(12));

        Assert.True(result);
    }

    [Fact]
    public void Update_ReplacesExistingEvent()
    {
        SetupTempWorkingDir();

        var ev = MakeEvent("BBQ", DateTime.Today.AddHours(10), DateTime.Today.AddHours(12));
        _service.Save(ev);
        ev.Description = "Updated description";
        _service.Update(ev);

        var result = _service.GetAll();
        Assert.Single(result);
        Assert.Equal("Updated description", result[0].Description);
    }

    [Fact]
    public void Update_DoesNothing_WhenEventNotFound()
    {
        SetupTempWorkingDir();

        var ev = MakeEvent("BBQ", DateTime.Today.AddHours(10), DateTime.Today.AddHours(12));
        _service.Save(ev);

        var unknown = MakeEvent("Unknown Event", DateTime.Today.AddHours(10), DateTime.Today.AddHours(12));
        _service.Update(unknown);

        var result = _service.GetAll();
        Assert.Single(result);
        Assert.Equal("BBQ", result[0].Title);
    }

    [Fact]
    public void CurrentUserAttending_ReturnsFalse_WhenNoCurrentUser()
    {
        SetupTempWorkingDir();
        Globals.CurrentUser = null;

        var ev = MakeEvent("BBQ", DateTime.Today.AddHours(10), DateTime.Today.AddHours(12));
        var result = _service.CurrentUserAttending(ev);

        Assert.False(result);
    }

    [Fact]
    public void CurrentUserAttending_ReturnsFalse_WhenUserHasNoCommitments()
    {
        SetupTempWorkingDir();
        var resident = MakeResident("Bob");
        Globals.CurrentUser = resident;

        new JsonDataService().SaveData("Data/residents.json", new List<Resident> { resident });

        var ev = MakeEvent("BBQ", DateTime.Today.AddHours(10), DateTime.Today.AddHours(12));
        var result = _service.CurrentUserAttending(ev);

        Assert.False(result);
    }

    [Fact]
    public void CurrentUserAttending_ReturnsTrue_WhenUserHasMatchingCommitment()
    {
        SetupTempWorkingDir();
        var resident = MakeResident("Bob");
        Globals.CurrentUser = resident;

        var ev = MakeEvent("BBQ", DateTime.Today.AddHours(10), DateTime.Today.AddHours(12));
        resident.Commitments.Add(new Commitment { Resident = resident, Event = ev, Attending = true });

        new JsonDataService().SaveData("Data/residents.json", new List<Resident> { resident });

        var result = _service.CurrentUserAttending(ev);

        Assert.True(result);
    }
}
