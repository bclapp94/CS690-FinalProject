using CommunityApp;

public class NotificationTests
{
    private static Resident MakeResident() => new()
    {
        Name = "Alice",
        Address = "123 Main St",
        Role = Role.Resident
    };

    private static Announcement MakeAnnouncement() => new()
    {
        Title = "Test",
        Body = "Test body",
        CreatedById = Guid.NewGuid()
    };

    [Fact]
    public void Notification_Properties_AreAssignedCorrectly()
    {
        var resident = MakeResident();
        var announcement = MakeAnnouncement();

        var notification = new Notification
        {
            Announcement = announcement,
            Resident = resident,
            Read = false
        };

        Assert.Equal(announcement, notification.Announcement);
        Assert.Equal(resident, notification.Resident);
        Assert.False(notification.Read);
    }

    [Fact]
    public void Notification_CanBeMarkedAsRead()
    {
        var notification = new Notification
        {
            Announcement = MakeAnnouncement(),
            Resident = MakeResident(),
            Read = false
        };

        notification.Read = true;

        Assert.True(notification.Read);
    }

    [Fact]
    public void Notification_Read_DefaultsToFalse()
    {
        var notification = new Notification
        {
            Announcement = MakeAnnouncement(),
            Resident = MakeResident()
        };

        Assert.False(notification.Read);
    }
}
