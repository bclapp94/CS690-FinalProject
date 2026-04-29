using CommunityApp;

public class EventTests
{
    private static Resident MakeResident() => new()
    {
        Name = "Alice",
        Address = "123 Main St",
        Role = Role.Resident
    };

    [Fact]
    public void Event_Properties_AreAssignedCorrectly()
    {
        var creator = MakeResident();
        var start = new DateTime(2026, 6, 1, 10, 0, 0);
        var end = new DateTime(2026, 6, 1, 12, 0, 0);

        var ev = new Event
        {
            Title = "Community BBQ",
            Description = "Annual BBQ",
            Start = start,
            End = end,
            Location = "Park",
            CreatedBy = creator,
            EventType = EventType.Standard
        };

        Assert.Equal("Community BBQ", ev.Title);
        Assert.Equal("Annual BBQ", ev.Description);
        Assert.Equal(start, ev.Start);
        Assert.Equal(end, ev.End);
        Assert.Equal("Park", ev.Location);
        Assert.Equal(creator, ev.CreatedBy);
        Assert.Equal(EventType.Standard, ev.EventType);
    }

    [Fact]
    public void Event_TimeSlots_DefaultsToEmpty()
    {
        var ev = new Event
        {
            Title = "Test",
            Description = "Test",
            Start = DateTime.Now,
            End = DateTime.Now.AddHours(1),
            Location = "Test",
            CreatedBy = MakeResident(),
            EventType = EventType.Standard
        };

        Assert.NotNull(ev.TimeSlots);
        Assert.Empty(ev.TimeSlots);
    }

    [Fact]
    public void Event_Commitments_DefaultsToEmpty()
    {
        var ev = new Event
        {
            Title = "Test",
            Description = "Test",
            Start = DateTime.Now,
            End = DateTime.Now.AddHours(1),
            Location = "Test",
            CreatedBy = MakeResident(),
            EventType = EventType.Standard
        };

        Assert.NotNull(ev.Commitments);
        Assert.Empty(ev.Commitments);
    }

    [Fact]
    public void Event_CanAdd_TimeSlot()
    {
        var ev = new Event
        {
            Title = "Group Activity",
            Description = "Fun",
            Start = DateTime.Now,
            End = DateTime.Now.AddHours(2),
            Location = "Gym",
            CreatedBy = MakeResident(),
            EventType = EventType.GroupActivity
        };

        var slot = new TimeSlot
        {
            Start = ev.Start,
            End = ev.Start.AddHours(1),
            MaxParticipants = 5
        };

        ev.TimeSlots.Add(slot);

        Assert.Single(ev.TimeSlots);
        Assert.Equal(5, ev.TimeSlots[0].MaxParticipants);
    }

    [Fact]
    public void Event_CanAdd_Commitment()
    {
        var creator = MakeResident();
        var ev = new Event
        {
            Title = "Test",
            Description = "Test",
            Start = DateTime.Now,
            End = DateTime.Now.AddHours(1),
            Location = "Test",
            CreatedBy = creator,
            EventType = EventType.Standard
        };

        var commitment = new Commitment
        {
            Resident = creator,
            Event = ev,
            Attending = true
        };

        ev.Commitments.Add(commitment);

        Assert.Single(ev.Commitments);
        Assert.True(ev.Commitments[0].Attending);
    }

    [Fact]
    public void Event_EndTime_ShouldBeAfter_StartTime()
    {
        var ev = new Event
        {
            Title = "Test",
            Description = "Test",
            Start = new DateTime(2026, 6, 1, 10, 0, 0),
            End = new DateTime(2026, 6, 1, 12, 0, 0),
            Location = "Test",
            CreatedBy = MakeResident(),
            EventType = EventType.Standard
        };

        Assert.True(ev.End > ev.Start);
    }
}
