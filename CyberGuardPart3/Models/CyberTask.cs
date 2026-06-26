namespace CyberGuardAssistant.Models
{
    // Represents a single cybersecurity task the user wants to manage
    // Used by the Task Assistant feature (Part 3 - Task 1)
    public class CyberTask
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public bool IsCompleted { get; set; } = false;

        // Optional reminder - null means no reminder set
        public DateTime? ReminderDate { get; set; }

        public CyberTask() { }

        public CyberTask(string title, string description, DateTime? reminderDate = null)
        {
            Title = title;
            Description = description;
            ReminderDate = reminderDate;
        }

        // Returns a readable summary of this task
        public string GetSummary()
        {
            string status = IsCompleted ? "✅ Done" : "⏳ Pending";
            string reminder = ReminderDate.HasValue
                ? $"Reminder: {ReminderDate.Value:dd MMM yyyy}"
                : "No reminder set";

            return $"{status} | {Title} — {reminder}";
        }
    }
}
