using CyberGuardAssistant.Models;
using CyberGuardAssistant.Services;

namespace CyberGuardAssistant.Services
{
    // ResponseEngine is the brain of the chatbot.
    // Part 1: keyword map, topic catalog, response methods
    // Part 2: random responses, sentiment, memory, follow-up flow
    // Part 3: routes to task assistant, quiz, NLP, and activity log
    public class ResponseEngine
    {
        // --- From Part 1 ---
        private readonly Dictionary<string, Func<string>> _responseMap;
        private readonly List<SecurityTopic> _topicCatalog;

        // --- From Part 2 ---
        private readonly Random _rand = new Random();
        private string _lastTopic = "";
        private UserProfile? _currentUser;

        // --- New in Part 3 ---
        private readonly TaskService _taskService;
        private readonly QuizService _quizService;
        private readonly ActivityLogService _activityLog;

        public ResponseEngine()
        {
            _responseMap    = InitializeResponses();
            _topicCatalog   = BuildTopicCatalog();
            _taskService    = new TaskService();
            _quizService    = new QuizService();
            _activityLog    = new ActivityLogService();
        }

        public void SetUser(UserProfile user) => _currentUser = user;

        // ----------------------------------------------------------------
        // Part 1 — response map (kept, extended with Part 3 keywords)
        // ----------------------------------------------------------------
        private Dictionary<string, Func<string>> InitializeResponses()
        {
            return new Dictionary<string, Func<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["fundamentals"]          = () => GetCyberSecurityBasics(),
                ["basics"]                = () => GetCyberSecurityBasics(),
                ["what is cybersecurity"] = () => GetCyberSecurityBasics(),
                ["links"]                 = () => AnalyzeSuspiciousLinks(),
                ["suspicious links"]      = () => AnalyzeSuspiciousLinks(),
                ["bad links"]             = () => AnalyzeSuspiciousLinks(),
                ["phishing"]              = () => DetectPhishingAttempts(),
                ["emails"]                = () => DetectPhishingAttempts(),
                ["prevent"]               = () => PreventionStrategies(),
                ["prevention"]            = () => PreventionStrategies(),
                ["protect"]               = () => PreventionStrategies(),
                ["report"]                = () => ReportingProcedures(),
                ["reporting"]             = () => ReportingProcedures(),
                ["password"]              = () => GetRandomPasswordTip(),
                ["scam"]                  = () => GetRandomScamTip(),
                ["privacy"]               = () => GetRandomPrivacyTip(),
                ["malware"]               = () => GetRandomMalwareTip(),
                ["2fa"]                   = () => GetTwoFactorTip(),
                ["firewall"]              = () => GetFirewallTip(),
                ["help"]                  = () => ShowAvailableCommands(),
                ["commands"]              = () => ShowAvailableCommands(),
                ["menu"]                  = () => ShowAvailableCommands(),
                ["about"]                 = () => GetSystemInfo(),
                ["info"]                  = () => GetSystemInfo()
            };
        }

        // ----------------------------------------------------------------
        // Part 1 — topic catalog (kept exactly)
        // ----------------------------------------------------------------
        private List<SecurityTopic> BuildTopicCatalog()
        {
            return new List<SecurityTopic>
            {
                new SecurityTopic("security", SecurityCategory.Fundamentals,
                    new[] { "protection", "defense", "safety" }),
                new SecurityTopic("phishing", SecurityCategory.ThreatDetection,
                    new[] { "fraud", "scam", "deception", "email" }),
                new SecurityTopic("malware", SecurityCategory.ThreatDetection,
                    new[] { "virus", "trojan", "ransomware" }),
                new SecurityTopic("password", SecurityCategory.Prevention,
                    new[] { "passphrase", "credentials", "login" }),
                new SecurityTopic("privacy", SecurityCategory.Prevention,
                    new[] { "personal data", "information", "vpn" }),
                new SecurityTopic("firewall", SecurityCategory.Prevention,
                    new[] { "network", "block", "traffic" })
            };
        }

        // ================================================================
        // MAIN QUERY PROCESSOR
        // Order of checks matters — Part 3 features checked first,
        // then Part 2 features, then Part 1 keyword matching
        // ================================================================
        public string ProcessUserQuery(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return "Please type something so I can help you.";

            string lower = query.Trim().ToLower();

            // --- Part 3: Activity log request ---
            if (ActivityLogService.IsLogRequest(lower))
            {
                _activityLog.Log("User viewed activity log");
                return _activityLog.GetActivitySummary();
            }

            // --- Part 3: Quiz is active — treat input as quiz answer ---
            if (_quizService.IsActive && _quizService.LooksLikeAnswer(lower))
            {
                string result = _quizService.SubmitAnswer(lower);
                _activityLog.Log($"Quiz answer submitted — Score: {_quizService.Score}/{_quizService.TotalQuestions}");
                return result;
            }

            // --- Part 3: Start quiz ---
            if (NLPService.IsQuizRequest(lower))
            {
                _activityLog.Log("Quiz started");
                return _quizService.StartQuiz();
            }

            // --- Part 3: Waiting for reminder response after adding task ---
            if (_taskService.IsWaitingForReminder)
            {
                string result = _taskService.HandleReminderResponse(query);
                _activityLog.Log("Reminder preference recorded for task");
                return result;
            }

            // --- Part 3: Add task (NLP detects many phrasings) ---
            if (NLPService.IsAddTaskRequest(lower))
            {
                string result = _taskService.AddTask(query);
                _activityLog.Log($"Task added: {NLPService.ExtractTaskTitle(query)}");
                return result;
            }

            // --- Part 3: View tasks ---
            if (NLPService.IsViewTaskRequest(lower))
            {
                _activityLog.Log("User viewed task list");
                return _taskService.GetAllTasks();
            }

            // --- Part 3: Complete a task ---
            if (NLPService.IsCompleteTaskRequest(lower))
            {
                string result = _taskService.CompleteTask(query);
                _activityLog.Log($"Task marked complete: input was '{query}'");
                return result;
            }

            // --- Part 3: Delete a task ---
            if (NLPService.IsDeleteTaskRequest(lower))
            {
                string result = _taskService.DeleteTask(query);
                _activityLog.Log($"Task deleted: input was '{query}'");
                return result;
            }

            // --- Part 2: Name memory ---
            if (lower.Contains("my name is"))
            {
                string name = ExtractAfterPhrase(lower, "my name is");
                if (!string.IsNullOrEmpty(name) && _currentUser != null)
                {
                    _currentUser.Username = CapFirst(name);
                    _activityLog.Log($"User introduced themselves as {_currentUser.Username}");
                    return $"Nice to meet you, {_currentUser.Username}! How can I help you stay safe online?";
                }
            }

            // --- Part 2: Favourite topic memory ---
            if (lower.Contains("interested in") || lower.Contains("i like"))
            {
                string phrase = lower.Contains("interested in") ? "interested in" : "i like";
                string topic  = ExtractAfterPhrase(lower, phrase);
                if (!string.IsNullOrEmpty(topic) && _currentUser != null)
                {
                    _currentUser.FavouriteTopic = topic;
                    _activityLog.Log($"User interest recorded: {topic}");
                    return $"Great! I will remember that you are interested in {topic}. " +
                           "It is a really important part of staying safe online.";
                }
            }

            // --- Part 2: Follow-up conversation ---
            if (IsFollowUp(lower))
            {
                if (!string.IsNullOrEmpty(_lastTopic))
                {
                    _activityLog.Log($"Follow-up on topic: {_lastTopic}");
                    return DetectSentimentPrefix(lower) + RouteByKeyword(_lastTopic);
                }
                return "Sure! What topic would you like more on? " +
                       "Try typing 'phishing', 'password', or 'scam'.";
            }

            // --- Part 1: Exact match in response map ---
            if (_responseMap.TryGetValue(query.Trim(), out var responseFunc))
            {
                _lastTopic = query.Trim();
                _activityLog.Log($"Topic discussed: {query.Trim()}");
                return DetectSentimentPrefix(lower) + responseFunc() + PersonalisedSuffix();
            }

            // --- Part 1: Partial keyword matching via topic catalog ---
            var cleaned = SanitizeInput(query);
            foreach (var topic in _topicCatalog)
            {
                if (cleaned.Contains(topic.Keyword) ||
                    topic.RelatedTerms.Any(term => cleaned.Contains(term)))
                {
                    _lastTopic = topic.Keyword;
                    _activityLog.Log($"Topic discussed: {topic.Keyword}");
                    return DetectSentimentPrefix(lower) +
                           RouteByCategory(topic.Category) +
                           PersonalisedSuffix();
                }
            }

            // --- Check response map keys as partial matches ---
            foreach (var key in _responseMap.Keys)
            {
                if (cleaned.Contains(key.ToLower()))
                {
                    _lastTopic = key;
                    _activityLog.Log($"Topic discussed: {key}");
                    return DetectSentimentPrefix(lower) +
                           _responseMap[key]() + PersonalisedSuffix();
                }
            }

            // --- Default fallback ---
            _activityLog.Log($"Unrecognised input: '{query}'");
            return DetectSentimentPrefix(lower) + GenerateContextualResponse(cleaned);
        }

        // ================================================================
        // PART 1 HELPERS (kept exactly)
        // ================================================================
        private string SanitizeInput(string input)
        {
            return new string(input
                .Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))
                .ToArray()).ToLower();
        }

        private string RouteByCategory(SecurityCategory category)
        {
            return category switch
            {
                SecurityCategory.Fundamentals    => GetCyberSecurityBasics(),
                SecurityCategory.ThreatDetection => DetectPhishingAttempts(),
                SecurityCategory.Prevention      => PreventionStrategies(),
                SecurityCategory.IncidentResponse=> ReportingProcedures(),
                _                                => GenerateContextualResponse("general")
            };
        }

        private string RouteByKeyword(string keyword)
        {
            return keyword.ToLower() switch
            {
                "password" => GetRandomPasswordTip(),
                "phishing" => DetectPhishingAttempts(),
                "scam"     => GetRandomScamTip(),
                "privacy"  => GetRandomPrivacyTip(),
                "malware"  => GetRandomMalwareTip(),
                "2fa"      => GetTwoFactorTip(),
                "firewall" => GetFirewallTip(),
                "links"    => AnalyzeSuspiciousLinks(),
                "prevent"  => PreventionStrategies(),
                "report"   => ReportingProcedures(),
                _          => GetCyberSecurityBasics()
            };
        }

        // ================================================================
        // PART 2 FEATURES (kept exactly)
        // ================================================================
        private string DetectSentimentPrefix(string input)
        {
            string[] worriedWords    = { "worried", "scared", "nervous", "afraid", "anxious" };
            string[] frustratedWords = { "frustrated", "confused", "annoyed", "lost", "angry" };
            string[] curiousWords    = { "curious", "wonder", "interested", "want to know" };

            if (worriedWords.Any(w => input.Contains(w)))
                return "It is completely understandable to feel that way. " +
                       "Cyber threats can feel overwhelming, but you are already taking the right step by learning.\n\n";
            if (frustratedWords.Any(w => input.Contains(w)))
                return "I hear you — let me try to make this as clear as possible.\n\n";
            if (curiousWords.Any(w => input.Contains(w)))
                return "Great question! Staying curious is the first step to staying safe.\n\n";

            return "";
        }

        private bool IsFollowUp(string input)
        {
            string[] phrases = {
                "give me another tip", "tell me more", "explain more",
                "more info", "another tip", "go on", "continue",
                "what else", "keep going", "more please", "say more", "elaborate"
            };
            return phrases.Any(p => input.Contains(p));
        }

        private string PersonalisedSuffix()
        {
            if (_currentUser == null) return "";
            string suffix = "";
            if (!string.IsNullOrEmpty(_currentUser.FavouriteTopic))
                suffix += $"\n\nAs someone interested in {_currentUser.FavouriteTopic}, " +
                          "you may also want to review your account security settings regularly.";
            if (_currentUser.Username != "Guest")
                suffix += $"\n\nI hope that helps, {_currentUser.Username}!";
            return suffix;
        }

        // ================================================================
        // PART 2 RANDOM RESPONSES (kept exactly)
        // ================================================================
        private string GetRandomPasswordTip()
        {
            var tips = new List<string>
            {
                "🔑 Use at least 12 characters mixing uppercase, lowercase, numbers, and symbols.",
                "🔑 Never reuse the same password on different sites.",
                "🔑 Consider using a password manager like Bitwarden or KeePassXC.",
                "🔑 Avoid using your name, birthday, or pet's name as a password.",
                "🔑 Change your passwords every 3 to 6 months to stay safe."
            };
            return tips[_rand.Next(tips.Count)];
        }

        private string GetRandomScamTip()
        {
            var tips = new List<string>
            {
                "💰 If an offer sounds too good to be true, it almost certainly is.",
                "💰 Never send money or gift cards to someone you have only spoken to online.",
                "💰 Be suspicious of unexpected prizes or lottery wins that arrive out of nowhere.",
                "💰 Always verify by contacting the company directly using their official website."
            };
            return tips[_rand.Next(tips.Count)];
        }

        private string GetRandomPrivacyTip()
        {
            var tips = new List<string>
            {
                "🔒 Check your social media privacy settings and limit who can see your posts.",
                "🔒 Use a VPN on public Wi-Fi to protect your data from being intercepted.",
                "🔒 Think before you post. Once information is online it is hard to remove.",
                "🔒 Review app permissions — many apps request access they do not actually need."
            };
            return tips[_rand.Next(tips.Count)];
        }

        private string GetRandomMalwareTip()
        {
            var tips = new List<string>
            {
                "🦠 Keep your operating system and software updated to patch security holes.",
                "🦠 Only download software from official, trusted sources.",
                "🦠 Run reputable antivirus with real-time scanning enabled.",
                "🦠 Be careful with USB drives — unknown ones can automatically install malware."
            };
            return tips[_rand.Next(tips.Count)];
        }

        private string GetTwoFactorTip() =>
            "📱 Two-Factor Authentication adds a second step to logging in.\n\n" +
            "Even if someone steals your password, they cannot access your account without the second factor.\n\n" +
            "Enable it on email, banking, and social media first — those are highest-value targets.";

        private string GetFirewallTip() =>
            "🛡️ A firewall monitors traffic and blocks suspicious connections.\n\n" +
            "Make sure your operating system's built-in firewall is switched on.\n\n" +
            "Combine it with antivirus software and safe browsing habits for best protection.";

        // ================================================================
        // PART 1 RESPONSE CONTENT (kept exactly from Part 1)
        // ================================================================
        private string GetCyberSecurityBasics() => @"🔒 CYBERSECURITY FUNDAMENTALS

Digital protection encompasses three core pillars:

1. CONFIDENTIALITY - Ensuring private data stays private
   • Encryption of sensitive information
   • Access control mechanisms

2. INTEGRITY - Maintaining data accuracy
   • Hash verification
   • Backup strategies

3. AVAILABILITY - Reliable access to resources
   • Redundant systems
   • DDoS protection

The threat landscape evolves daily. Staying informed is your best defense.";

        private string AnalyzeSuspiciousLinks() => @"🔗 LINK VERIFICATION PROTOCOL

Before clicking any URL:

✓ HOVER TEST: Float your mouse over the link first
✓ SPELLING AUDIT: Look for slight misspellings in the domain
✓ SSL CERTIFICATE: Look for the padlock icon
✓ SHORT URL CAUTION: Use checkshorturl.com to expand short links

RED FLAGS:
• Urgency tactics ('Account expires in 1 hour!')
• Grammatical errors in professional communications
• Requests for credentials via email";

        private string DetectPhishingAttempts() => @"📧 EMAIL THREAT ANALYSIS

SENDER ANOMALIES:
• Display name spoofing: 'Microsoft Support' <hacker@xyz.ru>
• Domain typos: 'support@micros0ft.com'

CONTENT WARNINGS:
• Generic greetings ('Dear Customer')
• Threatening language ('Legal action pending')
• Unexpected attachments (.zip, .exe, .scr)

When uncertain: Contact the organisation directly through
their official website — NOT via email reply.";

        private string PreventionStrategies() => @"🛡️ PROACTIVE DEFENSE MEASURES

AUTHENTICATION:
• Enable Multi-Factor Authentication (MFA) everywhere
• Use password managers (Bitwarden, KeePassXC)
• Unique passwords per service

SYSTEM HYGIENE:
• Automatic security updates enabled
• Antimalware with real-time scanning
• Firewall enabled on all networks

BACKUP STRATEGY (3-2-1 Rule):
• 3 copies of data, 2 media types, 1 offsite";

        private string ReportingProcedures() => @"📢 INCIDENT REPORTING

IMMEDIATE ACTIONS:
1. Screenshot the evidence
2. Do NOT interact further
3. Disconnect if malware is suspected

REPORTING:
• Phishing: reportphishing@apwg.org
• Fraud: FBI IC3 at ic3.gov
• Identity Theft: FTC IdentityTheft.gov";

        private string ShowAvailableCommands() =>
            "📋 AVAILABLE COMMANDS\n\n" +
            "CYBERSECURITY TOPICS:\n" +
            "  password | phishing | scam | privacy | malware | 2fa | firewall\n\n" +
            "TASK ASSISTANT:\n" +
            "  'add task - enable 2FA'\n" +
            "  'remind me to update my password'\n" +
            "  'show tasks' | 'complete task 1' | 'delete task 2'\n\n" +
            "QUIZ GAME:\n" +
            "  'quiz' or 'test me'\n\n" +
            "ACTIVITY LOG:\n" +
            "  'show activity log' or 'what have you done for me?'\n\n" +
            "MEMORY:\n" +
            "  'My name is Alex'\n" +
            "  'I am interested in privacy'\n" +
            "  'Give me another tip'\n\n" +
            "SYSTEM:\n" +
            "  'help' | 'about'";

        private string GetSystemInfo() =>
            $"ℹ️ SYSTEM INFORMATION\n\n" +
            $"CyberGuard Assistant v3.0 — Part 3/POE Edition\n" +
            $"Session started: {DateTime.Now:yyyy-MM-dd HH:mm}\n\n" +
            "Features:\n" +
            "  Part 1: Keyword recognition + cybersecurity topics\n" +
            "  Part 2: GUI, sentiment, memory, random responses\n" +
            "  Part 3: Task assistant, quiz game, NLP, activity log\n\n" +
            "Type 'help' to see all commands.";

        private string GenerateContextualResponse(string context)
        {
            var fallbacks = new List<string>
            {
                $"I am not quite sure about '{context}'. Type 'help' to see what I can assist with.",
                "I did not recognise that. Try asking about 'password', 'phishing', 'quiz', or 'add task'.",
                "I'm not sure I understand. Could you rephrase? Type 'help' for available commands."
            };
            return fallbacks[_rand.Next(fallbacks.Count)];
        }

        // ================================================================
        // HELPERS
        // ================================================================
        private string ExtractAfterPhrase(string input, string phrase)
        {
            int idx = input.IndexOf(phrase);
            if (idx < 0) return "";
            return input.Substring(idx + phrase.Length).Trim().TrimEnd('.', '!', '?', ',');
        }

        private string CapFirst(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            return char.ToUpper(text[0]) + text.Substring(1);
        }
    }
}
