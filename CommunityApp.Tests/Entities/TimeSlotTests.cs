using CommunityApp;

public class TimeSlotTests
{
    private static Resident MakeResident(string name = "Alice") => new()
    {
        Name = name,
        Address = "123 Main St",
        Role = Role.Resident
    };

    [Fact]
    public void TimeSlot_Properties_AreAssignedCorrectly()
    {
        var start = new DateTime(2026, 6, 1, 10, 0, 0);
        var end = new DateTime(2026, 6, 1, 11, 0, 0);

        var slot = new TimeSlot { Start = start, End = end, MaxParticipants = 10 };

        Assert.Equal(start, slot.Start);
        Assert.Equal(end, slot.End);
        Assert.Equal(10, slot.MaxParticipants);
    }

    [Fact]
    public void TimeSlot_Participants_DefaultsToEmpty()
    {
        var slot = new TimeSlot
        {
            Start = DateTime.Now,
            End = DateTime.Now.AddMinutes(30),
            MaxParticipants = 5
        };

        Assert.NotNull(slot.Participants);
        Assert.Empty(slot.Participants);
    }

    [Fact]
    public void TimeSlot_CanAdd_Participant()
    {
        var slot = new TimeSlot
        {
            Start = DateTime.Now,
            End = DateTime.Now.AddMinutes(30),
            MaxParticipants = 5
        };

        slot.Participants.Add(MakeResident());

        Assert.Single(slot.Participants);
    }

    [Fact]
    public void TimeSlot_DoesNotExceedMaxParticipants_WhenEnforced()
    {
        var slot = new TimeSlot
        {
            Start = DateTime.Now,
            End = DateTime.Now.AddMinutes(30),
            MaxParticipants = 2
        };

        slot.Participants.Add(MakeResident("Alice"));
        slot.Participants.Add(MakeResident("Bob"));

        bool canAdd = slot.Participants.Count < slot.MaxParticipants;

        Assert.False(canAdd);
        Assert.Equal(2, slot.Participants.Count);
    }

    [Fact]
    public void TimeSlot_End_ShouldBeAfter_Start()
    {
        var slot = new TimeSlot
        {
            Start = new DateTime(2026, 6, 1, 10, 0, 0),
            End = new DateTime(2026, 6, 1, 11, 0, 0),
            MaxParticipants = 5
        };

        Assert.True(slot.End > slot.Start);
    }
}
