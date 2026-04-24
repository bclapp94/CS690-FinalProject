namespace CommunityApp
{
    public class Announcement
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Title { get; set; }
        public required string Body { get; set; }
        public AnnouncementType Type { get; set; }
        public DateTime CreatedAt { get; set; }

        public Guid CreatedById { get; set; }

        public List<Notification> Notifications { get; set; } = new();
    }
}