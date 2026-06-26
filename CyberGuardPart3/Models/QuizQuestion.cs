namespace CyberGuardAssistant.Models
{
    // Defines the type of quiz question
    public enum QuestionType
    {
        MultipleChoice,
        TrueFalse
    }

    // Represents a single quiz question
    // Used by the Cybersecurity Mini-Game (Part 3 - Task 2)
    public class QuizQuestion
    {
        public string Question { get; set; } = "";
        public List<string> Options { get; set; } = new List<string>();
        public string CorrectAnswer { get; set; } = "";
        public string Explanation { get; set; } = "";
        public QuestionType Type { get; set; }

        public QuizQuestion(string question, List<string> options,
            string correctAnswer, string explanation, QuestionType type)
        {
            Question = question;
            Options = options;
            CorrectAnswer = correctAnswer;
            Explanation = explanation;
            Type = type;
        }

        // Checks if the user's answer is correct (not case sensitive)
        public bool IsCorrect(string userAnswer)
        {
            return string.Equals(userAnswer.Trim(), CorrectAnswer.Trim(),
                StringComparison.OrdinalIgnoreCase);
        }
    }
}
