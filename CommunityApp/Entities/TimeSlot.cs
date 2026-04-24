namespace CommunityApp
{
    public class TimeSlot
    {
        public required DateTime Start { get; set; }
        public required DateTime End { get; set; }
        public int MaxParticipants { get; set; }

        public List<Resident> Participants { get; set; } = new();
    }
}
