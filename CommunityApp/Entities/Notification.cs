namespace CommunityApp
{
    public class Notification
    {
        public required Announcement Announcement { get; set; }
        public required Resident Resident { get; set; }
        public bool Read { get; set; }
    }
}