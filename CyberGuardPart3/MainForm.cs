using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;
using CyberGuardAssistant.Data;
using CyberGuardAssistant.Models;
using CyberGuardAssistant.Services;

namespace CyberGuardAssistant
{
    // MainForm is the main chat window
    // Carries over from Part 2 with no structural changes
    // Part 3 features are handled inside ResponseEngine and the service classes
    public partial class MainForm : Form
    {
        private ResponseEngine _engine;
        private UserProfile    _user;

        public MainForm(string userName)
        {
            InitializeComponent();

            // Initialise database on startup (Part 3 - Task 1)
            DatabaseService.Initialise();

            // Same objects as Part 2
            _user   = new UserProfile(userName);
            _engine = new ResponseEngine();
            _engine.SetUser(_user);

            // Play voice greeting WAV (carried over from Part 1)
            PlayVoiceGreeting();

            // Show welcome message with ASCII banner
            ShowWelcome();
        }

        // ----------------------------------------------------------------
        // Plays the voice greeting WAV file on startup
        // ----------------------------------------------------------------
        private void PlayVoiceGreeting()
        {
            try
            {
                string wavPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "VoiceGreeting.wav");

                if (File.Exists(wavPath))
                    new SoundPlayer(wavPath).Play();
            }
            catch
            {
                // Silent fail — app continues without audio if file is missing
            }
        }

        // ----------------------------------------------------------------
        // Welcome message shown on startup
        // ----------------------------------------------------------------
        private void ShowWelcome()
        {
            AppendRaw(
                "╔══════════════════════════════════════════════════════════╗\r\n" +
                "║        CYBERGUARD SECURITY AWARENESS ASSISTANT           ║\r\n" +
                "║           Part 3 — Full Feature Edition                  ║\r\n" +
                "╚══════════════════════════════════════════════════════════╝",
                Color.Cyan);

            AppendMessage("Bot",
                $"Welcome back, {_user.Username}!\n\n" +
                "Here is what I can help you with:\n\n" +
                "📚  Cybersecurity topics — password, phishing, scam, privacy...\n" +
                "📋  Task assistant     — 'add task - enable 2FA'\n" +
                "🎮  Quiz game          — type 'quiz' to test your knowledge\n" +
                "📊  Activity log       — 'show activity log'\n\n" +
                "Type 'help' to see all commands.");
        }

        // ----------------------------------------------------------------
        // Send button
        // ----------------------------------------------------------------
        private void btnSend_Click(object sender, EventArgs e) => SendMessage();

        // ----------------------------------------------------------------
        // Enter key
        // ----------------------------------------------------------------
        private void txtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                SendMessage();
            }
        }

        // ----------------------------------------------------------------
        // Core send logic — same as Part 2
        // ----------------------------------------------------------------
        private void SendMessage()
        {
            string userText = txtInput.Text.Trim();
            if (string.IsNullOrEmpty(userText)) return;

            AppendMessage("You", userText);
            string reply = _engine.ProcessUserQuery(userText);
            AppendMessage("Bot", reply);

            txtInput.Clear();
            txtInput.Focus();
        }

        // ----------------------------------------------------------------
        // Clear chat
        // ----------------------------------------------------------------
        private void btnClear_Click(object sender, EventArgs e)
        {
            rtbChat.Clear();
            ShowWelcome();
        }

        // ----------------------------------------------------------------
        // Display a message with colour coding — same as Part 2
        // ----------------------------------------------------------------
        private void AppendMessage(string sender, string message)
        {
            if (rtbChat.TextLength > 0)
                rtbChat.AppendText(Environment.NewLine);

            rtbChat.SelectionColor = sender == "You" ? Color.DeepSkyBlue : Color.LimeGreen;
            rtbChat.SelectionFont  = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            rtbChat.AppendText(sender + ":  ");

            rtbChat.SelectionColor = Color.WhiteSmoke;
            rtbChat.SelectionFont  = new Font("Segoe UI", 9.5f, FontStyle.Regular);
            rtbChat.AppendText(message + Environment.NewLine);

            rtbChat.ScrollToCaret();
        }

        // Used for the ASCII banner at the top
        private void AppendRaw(string text, Color color)
        {
            rtbChat.SelectionColor = color;
            rtbChat.SelectionFont  = new Font("Consolas", 8f, FontStyle.Bold);
            rtbChat.AppendText(text + Environment.NewLine);
        }
    }
}
