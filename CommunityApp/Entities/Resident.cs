namespace CommunityApp
{
    public class Resident
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Name { get; set; }
        public required string Address { get; set; }
        public Role Role { get; set; }

        public List<Event> CreatedEvents { get; set; } = new();
        public List<Announcement> CreatedAnnouncements { get; set; } = new();
        public List<Commitment> Commitments { get; set; } = new();
        public List<Notification> Notifications { get; set; } = new();
    }
}
