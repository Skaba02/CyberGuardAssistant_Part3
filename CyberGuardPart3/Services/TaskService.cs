using CyberGuardAssistant.Data;
using CyberGuardAssistant.Models;

namespace CyberGuardAssistant.Services
{
    // Handles task assistant logic — adding, viewing, completing, deleting tasks
    // Connects to the MySQL database through DatabaseService
    // Part 3 - Task 1
    public class TaskService
    {
        // Tracks whether we are waiting for the user to answer
        // "Would you like a reminder?" after adding a task
        private bool _waitingForReminderResponse = false;
        private string _pendingTaskTitle = "";
        private string _pendingTaskDescription = "";

        // ----------------------------------------------------------------
        // Called when the user wants to add a new task
        // ----------------------------------------------------------------
        public string AddTask(string userInput)
        {
            // Extract the task title from natural language
            string title = NLPService.ExtractTaskTitle(userInput);

            // Build a helpful cybersecurity description automatically
            string description = BuildDescription(title);

            _pendingTaskTitle = title;
            _pendingTaskDescription = description;
            _waitingForReminderResponse = true;

            return $"Task added: '{title}'\n\n" +
                   $"Description: {description}\n\n" +
                   "Would you like to set a reminder for this task? " +
                   "Reply with a number of days (e.g. 'remind me in 3 days') or 'no'.";
        }

        // ----------------------------------------------------------------
        // Handles the user's response after being asked about a reminder
        // ----------------------------------------------------------------
        public bool IsWaitingForReminder => _waitingForReminderResponse;

        public string HandleReminderResponse(string input)
        {
            _waitingForReminderResponse = false;
            string lower = input.ToLower().Trim();

            DateTime? reminderDate = null;

            // Check if they want a reminder
            if (lower != "no" && lower != "n" && !lower.Contains("no reminder"))
            {
                int days = NLPService.ExtractDays(input);
                if (days > 0)
                {
                    reminderDate = DateTime.Now.AddDays(days);
                }
                else if (lower.Contains("tomorrow"))
                {
                    reminderDate = DateTime.Now.AddDays(1);
                }
                else if (lower.Contains("next week"))
                {
                    reminderDate = DateTime.Now.AddDays(7);
                }
            }

            // Save to database
            var task = new CyberTask(_pendingTaskTitle, _pendingTaskDescription, reminderDate);
            bool saved = DatabaseService.AddTask(task);

            if (!saved)
                return "I could not connect to the database right now, but I have noted your task for this session.";

            string reminder = reminderDate.HasValue
                ? $"Reminder set for {reminderDate.Value:dd MMM yyyy}."
                : "No reminder set.";

            return $"Got it! Task saved to your task list.\n{reminder}\n\n" +
                   "Type 'show tasks' to see all your tasks.";
        }

        // ----------------------------------------------------------------
        // Returns all tasks from the database
        // ----------------------------------------------------------------
        public string GetAllTasks()
        {
            var tasks = DatabaseService.GetAllTasks();

            if (tasks.Count == 0)
                return "You have no tasks yet. Try saying 'add task - enable two-factor authentication'.";

            string result = "📋 Your Cybersecurity Tasks:\n\n";
            foreach (var task in tasks)
                result += $"  [{task.Id}] {task.GetSummary()}\n";

            result += "\nTo complete a task: 'complete task 1'\nTo delete a task: 'delete task 1'";
            return result;
        }

        // ----------------------------------------------------------------
        // Marks a task as complete
        // ----------------------------------------------------------------
        public string CompleteTask(string input)
        {
            int id = NLPService.ExtractNumber(input);
            if (id < 0)
                return "Please tell me which task number to complete. Example: 'complete task 2'";

            bool success = DatabaseService.CompleteTask(id);
            return success
                ? $"✅ Task {id} marked as completed. Well done!"
                : $"I could not find task {id}. Type 'show tasks' to see your task list.";
        }

        // ----------------------------------------------------------------
        // Deletes a task
        // ----------------------------------------------------------------
        public string DeleteTask(string input)
        {
            int id = NLPService.ExtractNumber(input);
            if (id < 0)
                return "Please tell me which task number to delete. Example: 'delete task 2'";

            bool success = DatabaseService.DeleteTask(id);
            return success
                ? $"🗑️ Task {id} has been deleted."
                : $"I could not find task {id}. Type 'show tasks' to see your task list.";
        }

        // ----------------------------------------------------------------
        // Builds a helpful description for common cybersecurity tasks
        // ----------------------------------------------------------------
        private string BuildDescription(string title)
        {
            string lower = title.ToLower();

            if (lower.Contains("password"))
                return "Update your passwords to strong, unique ones for each account.";
            if (lower.Contains("2fa") || lower.Contains("two-factor") || lower.Contains("authentication"))
                return "Enable two-factor authentication to add an extra layer of security.";
            if (lower.Contains("privacy"))
                return "Review your privacy settings to control who can see your information.";
            if (lower.Contains("antivirus") || lower.Contains("virus"))
                return "Install or update your antivirus software to protect against threats.";
            if (lower.Contains("backup"))
                return "Back up your important data to prevent loss from ransomware or hardware failure.";
            if (lower.Contains("update") || lower.Contains("patch"))
                return "Keep your software and operating system updated to fix security vulnerabilities.";
            if (lower.Contains("firewall"))
                return "Ensure your firewall is enabled to block unauthorised network access.";

            // Generic description if no keyword matched
            return $"Complete this cybersecurity task: {title}.";
        }
    }
}
