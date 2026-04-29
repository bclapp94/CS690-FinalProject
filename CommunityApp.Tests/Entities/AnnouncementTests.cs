using CommunityApp;

public class AnnouncementTests
{
    [Fact]
    public void Announcement_Properties_AreAssignedCorrectly()
    {
        var creatorId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;

        var announcement = new Announcement
        {
            Title = "Pool Closure",
            Body = "The pool will be closed for maintenance.",
            Type = AnnouncementType.Normal,
            CreatedAt = createdAt,
            CreatedById = creatorId
        };

        Assert.Equal("Pool Closure", announcement.Title);
        Assert.Equal("The pool will be closed for maintenance.", announcement.Body);
        Assert.Equal(AnnouncementType.Normal, announcement.Type);
        Assert.Equal(createdAt, announcement.CreatedAt);
        Assert.Equal(creatorId, announcement.CreatedById);
    }

    [Fact]
    public void Announcement_Id_IsGeneratedAutomatically()
    {
        var a1 = new Announcement { Title = "A", Body = "B", CreatedById = Guid.NewGuid() };
        var a2 = new Announcement { Title = "C", Body = "D", CreatedById = Guid.NewGuid() };

        Assert.NotEqual(Guid.Empty, a1.Id);
        Assert.NotEqual(a1.Id, a2.Id);
    }

    [Fact]
    public void Announcement_Notifications_DefaultToEmpty()
    {
        var announcement = new Announcement
        {
            Title = "Test",
            Body = "Test body",
            CreatedById = Guid.NewGuid()
        };

        Assert.NotNull(announcement.Notifications);
        Assert.Empty(announcement.Notifications);
    }

    [Fact]
    public void Announcement_DefaultType_IsNormal()
    {
        var announcement = new Announcement
        {
            Title = "Test",
            Body = "Test body",
            CreatedById = Guid.NewGuid()
        };

        Assert.Equal(AnnouncementType.Normal, announcement.Type);
    }

    [Fact]
    public void Announcement_CanBe_AdminAlert()
    {
        var announcement = new Announcement
        {
            Title = "Urgent",
            Body = "Emergency alert",
            Type = AnnouncementType.AdminAlert,
            CreatedById = Guid.NewGuid()
        };

        Assert.Equal(AnnouncementType.AdminAlert, announcement.Type);
    }
}
