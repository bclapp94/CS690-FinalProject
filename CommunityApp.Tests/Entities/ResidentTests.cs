using CommunityApp;

public class ResidentTests
{
    [Fact]
    public void Resident_Properties_AreAssignedCorrectly()
    {
        var resident = new Resident
        {
            Name = "Bob",
            Address = "456 Oak Ave",
            Role = Role.Admin
        };

        Assert.Equal("Bob", resident.Name);
        Assert.Equal("456 Oak Ave", resident.Address);
        Assert.Equal(Role.Admin, resident.Role);
    }

    [Fact]
    public void Resident_Id_IsGeneratedAutomatically()
    {
        var r1 = new Resident { Name = "Alice", Address = "123 Main St", Role = Role.Resident };
        var r2 = new Resident { Name = "Bob", Address = "456 Oak Ave", Role = Role.Resident };

        Assert.NotEqual(Guid.Empty, r1.Id);
        Assert.NotEqual(r1.Id, r2.Id);
    }

    [Fact]
    public void Resident_Collections_DefaultToEmpty()
    {
        var resident = new Resident { Name = "Alice", Address = "123 Main St", Role = Role.Resident };

        Assert.Empty(resident.CreatedEvents);
        Assert.Empty(resident.CreatedAnnouncements);
        Assert.Empty(resident.Commitments);
        Assert.Empty(resident.Notifications);
    }

    [Fact]
    public void Resident_CanAdd_Commitment()
    {
        var resident = new Resident { Name = "Alice", Address = "123 Main St", Role = Role.Resident };
        var ev = new Event
        {
            Title = "Test",
            Description = "Test",
            Start = DateTime.Now,
            End = DateTime.Now.AddHours(1),
            Location = "Test",
            CreatedBy = resident,
            EventType = EventType.Standard
        };

        var commitment = new Commitment { Resident = resident, Event = ev, Attending = true };
        resident.Commitments.Add(commitment);

        Assert.Single(resident.Commitments);
        Assert.True(resident.Commitments[0].Attending);
    }

    [Fact]
    public void Resident_CanAdd_CreatedEvent()
    {
        var resident = new Resident { Name = "Alice", Address = "123 Main St", Role = Role.Resident };
        var ev = new Event
        {
            Title = "Test",
            Description = "Test",
            Start = DateTime.Now,
            End = DateTime.Now.AddHours(1),
            Location = "Test",
            CreatedBy = resident,
            EventType = EventType.Standard
        };

        resident.CreatedEvents.Add(ev);

        Assert.Single(resident.CreatedEvents);
        Assert.Equal("Test", resident.CreatedEvents[0].Title);
    }
}
