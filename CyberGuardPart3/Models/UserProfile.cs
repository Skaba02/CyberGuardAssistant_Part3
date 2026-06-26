namespace CyberGuardAssistant.Models
{
    // Stores information about the current user session
    // Carried over from Part 1 and Part 2, no changes needed here
    public class UserProfile
    {
        public string Username { get; set; } = "Guest";
        public bool WantsToExit { get; set; }
        public DateTime SessionStart { get; set; } = DateTime.Now;

        // Added in Part 2 - remembers favourite topic
        public string FavouriteTopic { get; set; } = "";

        public UserProfile(string name)
        {
            Username = string.IsNullOrWhiteSpace(name) ? "Guest" : name.Trim();
        }
    }
}
