// QuizService.cs - Manages the cybersecurity quiz with 12 questions
using CyberGuardAssistant.Models;

namespace CyberGuardAssistant.Services
{
    // Manages the cybersecurity quiz game
    // Part 3 - Task 2
    public class QuizService
    {
        private List<QuizQuestion> _allQuestions;
        private List<QuizQuestion> _sessionQuestions;
        private int _currentIndex = 0;
        private int _score = 0;
        private bool _isActive = false;

        public bool IsActive => _isActive;
        public int CurrentIndex => _currentIndex;
        public int TotalQuestions => _sessionQuestions?.Count ?? 0;
        public int Score => _score;

        public QuizService()
        {
            _allQuestions = BuildQuestions();
        }

        // ----------------------------------------------------------------
        // Starts a new quiz session with 10 randomly picked questions
        // ----------------------------------------------------------------
        public string StartQuiz()
        {
            // Shuffle questions and pick 10
            var shuffled = _allQuestions.OrderBy(_ => Guid.NewGuid()).ToList();
            _sessionQuestions = shuffled.Take(10).ToList();
            _currentIndex = 0;
            _score = 0;
            _isActive = true;

            return "🎮 Welcome to the Cybersecurity Quiz!\n\n" +
                   "You will get 10 questions. Answer with A, B, C, D or True/False.\n" +
                   "Let us see how much you know!\n\n" +
                   GetCurrentQuestion();
        }

        // ----------------------------------------------------------------
        // Returns the current question formatted for display
        // ----------------------------------------------------------------
        public string GetCurrentQuestion()
        {
            if (_currentIndex >= _sessionQuestions.Count)
                return FinishQuiz();

            var q = _sessionQuestions[_currentIndex];
            string header = $"Question {_currentIndex + 1} of {_sessionQuestions.Count}\n\n";
            string question = q.Question + "\n\n";
            string options = string.Join("\n", q.Options);

            return header + question + options;
        }

        // ----------------------------------------------------------------
        // Processes the user's answer and returns feedback
        // ----------------------------------------------------------------
        public string SubmitAnswer(string userAnswer)
        {
            if (!_isActive || _currentIndex >= _sessionQuestions.Count)
                return "No quiz is currently active. Type 'quiz' to start one.";

            var q = _sessionQuestions[_currentIndex];
            string feedback;

            if (q.IsCorrect(userAnswer))
            {
                _score++;
                feedback = "✅ Correct! " + q.Explanation;
            }
            else
            {
                feedback = $"❌ Not quite. The correct answer is {q.CorrectAnswer}.\n{q.Explanation}";
            }

            _currentIndex++;

            // Check if quiz is finished
            if (_currentIndex >= _sessionQuestions.Count)
                return feedback + "\n\n" + FinishQuiz();

            return feedback + "\n\n" + GetCurrentQuestion();
        }

        // ----------------------------------------------------------------
        // Ends the quiz and returns the final score with feedback
        // ----------------------------------------------------------------
        private string FinishQuiz()
        {
            _isActive = false;
            int total = _sessionQuestions.Count;
            string grade;

            if (_score >= 9)
                grade = "🏆 Outstanding! You are a cybersecurity pro!";
            else if (_score >= 7)
                grade = "🌟 Great job! You know your cybersecurity well.";
            else if (_score >= 5)
                grade = "👍 Good effort! Keep learning to stay safe online.";
            else
                grade = "📚 Keep learning! Cybersecurity knowledge is very important.";

            return $"Quiz Complete!\n\n" +
                   $"Your score: {_score} out of {total}\n\n" +
                   grade + "\n\n" +
                   "Type 'quiz' to play again or ask me anything else.";
        }

        // ----------------------------------------------------------------
        // Checks if input looks like a quiz answer (A/B/C/D or True/False)
        // ----------------------------------------------------------------
        public bool LooksLikeAnswer(string input)
        {
            if (!_isActive) return false;

            string clean = input.Trim().ToLower();
            return clean == "a" || clean == "b" || clean == "c" || clean == "d"
                || clean == "true" || clean == "false"
                || clean == "t" || clean == "f";
        }

        // ----------------------------------------------------------------
        // All 12 quiz questions — mix of multiple choice and true/false
        // ----------------------------------------------------------------
        private List<QuizQuestion> BuildQuestions()
        {
            return new List<QuizQuestion>
            {
                // --- Multiple Choice Questions ---
                new QuizQuestion(
                    "What should you do if you receive an email asking for your password?",
                    new List<string> { "A) Reply with your password", "B) Delete the email",
                        "C) Report the email as phishing", "D) Ignore it" },
                    "C",
                    "Reporting phishing emails helps protect others and stops scammers.",
                    QuestionType.MultipleChoice),

                new QuizQuestion(
                    "How long should a strong password be at minimum?",
                    new List<string> { "A) 4 characters", "B) 6 characters",
                        "C) 8 characters", "D) 12 characters" },
                    "D",
                    "A minimum of 12 characters makes passwords much harder to crack.",
                    QuestionType.MultipleChoice),

                new QuizQuestion(
                    "What does 2FA stand for?",
                    new List<string> { "A) Two File Access", "B) Two-Factor Authentication",
                        "C) Two Firewall Apps", "D) Transfer File Authentication" },
                    "B",
                    "Two-Factor Authentication adds an extra step to verify your identity.",
                    QuestionType.MultipleChoice),

                new QuizQuestion(
                    "Which of these is the safest type of Wi-Fi to use for banking?",
                    new List<string> { "A) Public café Wi-Fi", "B) Airport Wi-Fi",
                        "C) Your home private Wi-Fi", "D) A hotel Wi-Fi" },
                    "C",
                    "Your private home network is safest. Public Wi-Fi can be monitored by others.",
                    QuestionType.MultipleChoice),

                new QuizQuestion(
                    "What is phishing?",
                    new List<string> {
                        "A) A type of antivirus software",
                        "B) A trick to steal personal info through fake messages",
                        "C) A way to speed up your internet",
                        "D) A firewall setting" },
                    "B",
                    "Phishing tricks users into giving away passwords or personal details.",
                    QuestionType.MultipleChoice),

                new QuizQuestion(
                    "What is the safest way to store your passwords?",
                    new List<string> {
                        "A) Write them in a notebook",
                        "B) Save them in a text file on your desktop",
                        "C) Use a password manager",
                        "D) Use the same password for everything" },
                    "C",
                    "Password managers store passwords securely and generate strong ones for you.",
                    QuestionType.MultipleChoice),

                new QuizQuestion(
                    "Which file type is most commonly used to spread malware via email?",
                    new List<string> { "A) .jpg", "B) .pdf", "C) .exe", "D) .txt" },
                    "C",
                    "Executable files (.exe) can run code on your computer and install malware.",
                    QuestionType.MultipleChoice),

                new QuizQuestion(
                    "What is social engineering in cybersecurity?",
                    new List<string> {
                        "A) Building social media apps",
                        "B) Manipulating people into revealing confidential information",
                        "C) Engineering a new social network",
                        "D) A type of encryption" },
                    "B",
                    "Social engineering exploits human trust rather than technical weaknesses.",
                    QuestionType.MultipleChoice),

                // --- True/False Questions ---
                new QuizQuestion(
                    "True or False: HTTPS in a website URL means it is completely safe to use.",
                    new List<string> { "True", "False" },
                    "False",
                    "HTTPS means the connection is encrypted, but the site could still be fake or malicious.",
                    QuestionType.TrueFalse),

                new QuizQuestion(
                    "True or False: You should use the same password on multiple websites to remember it easily.",
                    new List<string> { "True", "False" },
                    "False",
                    "Reusing passwords is dangerous. If one site is breached, all your accounts are at risk.",
                    QuestionType.TrueFalse),

                new QuizQuestion(
                    "True or False: A VPN helps protect your privacy on public Wi-Fi.",
                    new List<string> { "True", "False" },
                    "True",
                    "A VPN encrypts your internet traffic, making it harder for others to intercept.",
                    QuestionType.TrueFalse),

                new QuizQuestion(
                    "True or False: Antivirus software alone is enough to keep your computer fully secure.",
                    new List<string> { "True", "False" },
                    "False",
                    "Antivirus is important but should be combined with safe habits, updates, and a firewall.",
                    QuestionType.TrueFalse)
            };
        }
    }
}
