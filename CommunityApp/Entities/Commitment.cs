namespace CommunityApp
{
    public class Commitment
    {
        public required Resident Resident { get; set; }
        public required Event Event { get; set; }
        public bool Attending { get; set; }
    }
}