using CyberGuardAssistant.Models;

namespace CyberGuardAssistant.Services
{
    // Records everything the chatbot has done during the session
    // Part 3 - Task 4
    public class ActivityLogService
    {
        // Stores up to 50 entries — we only show the last 10
        private readonly List<ActivityEntry> _log = new List<ActivityEntry>();
        private const int MaxDisplay = 10;

        // ----------------------------------------------------------------
        // Add a new entry to the activity log
        // ----------------------------------------------------------------
        public void Log(string description)
        {
            _log.Add(new ActivityEntry(description));

            // Keep the list from growing too large
            if (_log.Count > 50)
                _log.RemoveAt(0);
        }

        // ----------------------------------------------------------------
        // Returns the last 10 actions as a readable summary
        // ----------------------------------------------------------------
        public string GetActivitySummary()
        {
            if (_log.Count == 0)
                return "No activity recorded yet. Start chatting and I will log what we do!";

            // Take the most recent entries
            var recent = _log.TakeLast(MaxDisplay).ToList();

            string header = "📋 Here is a summary of recent actions:\n\n";
            string entries = "";

            for (int i = 0; i < recent.Count; i++)
                entries += $"  {i + 1}. {recent[i].GetDisplay()}\n";

            return header + entries;
        }

        // ----------------------------------------------------------------
        // Checks if the user is asking to see the activity log
        // ----------------------------------------------------------------
        public static bool IsLogRequest(string input)
        {
            string lower = input.ToLower();
            return lower.Contains("activity log")
                || lower.Contains("what have you done")
                || lower.Contains("show log")
                || lower.Contains("show history")
                || lower.Contains("what did you do")
                || lower.Contains("recent actions");
        }
    }
}
