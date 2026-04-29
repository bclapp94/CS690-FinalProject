using CommunityApp;

public class CommitmentTests
{
    private static Resident MakeResident() => new()
    {
        Name = "Alice",
        Address = "123 Main St",
        Role = Role.Resident
    };

    private static Event MakeEvent(Resident creator) => new()
    {
        Title = "Test Event",
        Description = "Test",
        Start = DateTime.Now,
        End = DateTime.Now.AddHours(1),
        Location = "Test Location",
        CreatedBy = creator,
        EventType = EventType.Standard
    };

    [Fact]
    public void Commitment_Properties_AreAssignedCorrectly()
    {
        var resident = MakeResident();
        var ev = MakeEvent(resident);

        var commitment = new Commitment
        {
            Resident = resident,
            Event = ev,
            Attending = true
        };

        Assert.Equal(resident, commitment.Resident);
        Assert.Equal(ev, commitment.Event);
        Assert.True(commitment.Attending);
    }

    [Fact]
    public void Commitment_Attending_DefaultsToFalse()
    {
        var resident = MakeResident();
        var ev = MakeEvent(resident);

        var commitment = new Commitment { Resident = resident, Event = ev };

        Assert.False(commitment.Attending);
    }

    [Fact]
    public void Commitment_CanSet_AttendingToFalse()
    {
        var resident = MakeResident();
        var ev = MakeEvent(resident);

        var commitment = new Commitment { Resident = resident, Event = ev, Attending = true };
        commitment.Attending = false;

        Assert.False(commitment.Attending);
    }
}
