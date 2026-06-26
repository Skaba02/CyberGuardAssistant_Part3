namespace CyberGuardAssistant.Models
{
    // Represents a single entry in the activity log
    // Used by the Activity Log feature (Part 3 - Task 4)
    public class ActivityEntry
    {
        public string Description { get; set; } = "";
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public ActivityEntry(string description)
        {
            Description = description;
            Timestamp = DateTime.Now;
        }

        // Returns a readable one-line summary of this entry
        public string GetDisplay()
        {
            return $"[{Timestamp:HH:mm}] {Description}";
        }
    }
}
