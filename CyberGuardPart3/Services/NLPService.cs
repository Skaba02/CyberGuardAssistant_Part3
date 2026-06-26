// NLPService.cs - Detects user intent even when worded differently
namespace CyberGuardAssistant.Services
{
    // Simulates Natural Language Processing using string matching
    // Recognises user intent even when worded differently
    // Part 3 - Task 3
    public class NLPService
    {
        // ----------------------------------------------------------------
        // Detects if the user wants to ADD a task (even if worded differently)
        // Examples: "add a task", "create task", "I need to", "remind me to"
        // ----------------------------------------------------------------
        public static bool IsAddTaskRequest(string input)
        {
            string lower = input.ToLower();
            return lower.Contains("add task")
                || lower.Contains("add a task")
                || lower.Contains("create task")
                || lower.Contains("new task")
                || lower.Contains("remind me to")
                || lower.Contains("can you remind me")
                || lower.Contains("set a reminder")
                || lower.Contains("set reminder")
                || lower.Contains("i need to")
                || lower.Contains("don't forget to")
                || lower.Contains("remember to");
        }

        // ----------------------------------------------------------------
        // Detects if the user wants to VIEW their tasks
        // ----------------------------------------------------------------
        public static bool IsViewTaskRequest(string input)
        {
            string lower = input.ToLower();
            return lower.Contains("show tasks")
                || lower.Contains("view tasks")
                || lower.Contains("my tasks")
                || lower.Contains("list tasks")
                || lower.Contains("what are my tasks")
                || lower.Contains("show my tasks")
                || lower.Contains("pending tasks");
        }

        // ----------------------------------------------------------------
        // Detects if the user wants to COMPLETE a task
        // ----------------------------------------------------------------
        public static bool IsCompleteTaskRequest(string input)
        {
            string lower = input.ToLower();
            return lower.Contains("complete task")
                || lower.Contains("done task")
                || lower.Contains("finish task")
                || lower.Contains("mark as done")
                || lower.Contains("mark complete")
                || lower.Contains("task done");
        }

        // ----------------------------------------------------------------
        // Detects if the user wants to DELETE a task
        // ----------------------------------------------------------------
        public static bool IsDeleteTaskRequest(string input)
        {
            string lower = input.ToLower();
            return lower.Contains("delete task")
                || lower.Contains("remove task")
                || lower.Contains("cancel task")
                || lower.Contains("get rid of task");
        }

        // ----------------------------------------------------------------
        // Detects if the user wants to start the quiz
        // ----------------------------------------------------------------
        public static bool IsQuizRequest(string input)
        {
            string lower = input.ToLower();
            return lower.Contains("quiz")
                || lower.Contains("test me")
                || lower.Contains("test my knowledge")
                || lower.Contains("play game")
                || lower.Contains("start game")
                || lower.Contains("cybersecurity game")
                || lower.Contains("question");
        }

        // ----------------------------------------------------------------
        // Tries to extract a task title from natural language input
        // Example: "remind me to update my password" → "update my password"
        // ----------------------------------------------------------------
        public static string ExtractTaskTitle(string input)
        {
            string lower = input.ToLower();

            // Try to extract what comes after common phrases
            string[] triggers = {
                "remind me to", "can you remind me to", "add task ",
                "add a task ", "create task ", "new task ",
                "i need to", "remember to", "don't forget to",
                "set a reminder for", "set reminder for"
            };

            foreach (string trigger in triggers)
            {
                if (lower.Contains(trigger))
                {
                    int idx = lower.IndexOf(trigger) + trigger.Length;
                    string extracted = input.Substring(idx).Trim().TrimEnd('.', '!', '?');
                    if (!string.IsNullOrEmpty(extracted))
                        return CapFirst(extracted);
                }
            }

            // If no trigger phrase found, use the whole input as title
            return CapFirst(input.Trim().TrimEnd('.', '!', '?'));
        }

        // ----------------------------------------------------------------
        // Tries to extract a number from the user's input
        // Example: "complete task 3" → 3
        // ----------------------------------------------------------------
        public static int ExtractNumber(string input)
        {
            var words = input.Split(' ');
            foreach (var word in words)
            {
                if (int.TryParse(word, out int number))
                    return number;
            }
            return -1; // No number found
        }

        // ----------------------------------------------------------------
        // Tries to extract a number of days from input like "in 3 days"
        // ----------------------------------------------------------------
        public static int ExtractDays(string input)
        {
            string lower = input.ToLower();
            string[] words = lower.Split(' ');

            for (int i = 0; i < words.Length - 1; i++)
            {
                if (int.TryParse(words[i], out int days))
                {
                    if (words[i + 1].Contains("day"))
                        return days;
                }
            }
            return -1;
        }

        private static string CapFirst(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            return char.ToUpper(text[0]) + text.Substring(1);
        }
    }
}
