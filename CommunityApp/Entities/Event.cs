namespace CommunityApp
{
    public class Event
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required DateTime Start { get; set; }
        public required DateTime End { get; set; }
        public required string Location { get; set; }

        public required Resident CreatedBy { get; set; }
        public required EventType EventType { get; set; }

        public List<TimeSlot> TimeSlots { get; set; } = new();
        public List<Commitment> Commitments { get; set; } = new();
    }
}