using System;
using System.Windows.Forms;

namespace CyberGuardAssistant
{
    // Entry point — same as Part 2
    // Shows a name dialog first, then opens the main chat window
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string userName = PromptForName();
            Application.Run(new MainForm(userName));
        }

        private static string PromptForName()
        {
            using Form dialog = new Form
            {
                Text            = "CyberGuard Assistant — Welcome",
                Size            = new Size(380, 160),
                StartPosition   = FormStartPosition.CenterScreen,
                BackColor       = System.Drawing.Color.FromArgb(22, 22, 42),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox     = false,
                MinimizeBox     = false
            };

            var lbl = new Label
            {
                Text      = "Enter your name to begin:",
                ForeColor = System.Drawing.Color.White,
                Font      = new System.Drawing.Font("Segoe UI", 10f),
                Location  = new System.Drawing.Point(20, 22),
                AutoSize  = true
            };

            var txt = new TextBox
            {
                Location    = new System.Drawing.Point(20, 50),
                Width       = 320,
                BackColor   = System.Drawing.Color.FromArgb(35, 35, 58),
                ForeColor   = System.Drawing.Color.White,
                Font        = new System.Drawing.Font("Segoe UI", 10f),
                BorderStyle = BorderStyle.FixedSingle
            };

            var btn = new Button
            {
                Text      = "Start",
                Location  = new System.Drawing.Point(240, 82),
                Width     = 100,
                Height    = 30,
                BackColor = System.Drawing.Color.FromArgb(0, 180, 100),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new System.Drawing.Font("Segoe UI", 9.5f, System.Drawing.FontStyle.Bold)
            };
            btn.FlatAppearance.BorderSize = 0;

            txt.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    dialog.DialogResult = DialogResult.OK;
                }
            };
            btn.Click += (s, e) => dialog.DialogResult = DialogResult.OK;

            dialog.Controls.AddRange(new System.Windows.Forms.Control[] { lbl, txt, btn });
            dialog.AcceptButton = btn;
            dialog.ShowDialog();

            string name = txt.Text.Trim();
            return string.IsNullOrEmpty(name) ? "User" : name;
        }
    }
}
