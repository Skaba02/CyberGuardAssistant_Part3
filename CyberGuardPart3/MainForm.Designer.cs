using System.Drawing;
using System.Windows.Forms;

namespace CyberGuardAssistant
{
    // GUI layout — carried over from Part 2 with minor label update
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private RichTextBox rtbChat;
        private TextBox     txtInput;
        private Button      btnSend;
        private Button      btnClear;
        private Panel       pnlBottom;
        private Label       lblStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            Text          = "CyberGuard Assistant — Part 3 POE";
            Size          = new Size(780, 650);
            MinimumSize   = new Size(600, 500);
            BackColor     = Color.FromArgb(15, 15, 28);
            Font          = new Font("Segoe UI", 9.5f);
            StartPosition = FormStartPosition.CenterScreen;

            // Status bar
            lblStatus = new Label
            {
                Dock      = DockStyle.Bottom,
                Height    = 22,
                BackColor = Color.FromArgb(20, 20, 40),
                ForeColor = Color.Gray,
                Font      = new Font("Segoe UI", 8f),
                Text      = "  CyberGuard v3.0  |  Commands: help | quiz | add task | show tasks | show activity log",
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Padding   = new Padding(4, 0, 0, 0)
            };

            // Bottom panel
            pnlBottom = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 52,
                BackColor = Color.FromArgb(22, 22, 42),
                Padding   = new Padding(8, 8, 8, 8)
            };

            txtInput = new TextBox
            {
                Dock            = DockStyle.Fill,
                BackColor       = Color.FromArgb(35, 35, 58),
                ForeColor       = Color.White,
                Font            = new Font("Segoe UI", 10f),
                BorderStyle     = BorderStyle.FixedSingle,
                PlaceholderText = "Type your message and press Enter…"
            };

            btnSend = new Button
            {
                Text      = "Send ▶",
                Dock      = DockStyle.Right,
                Width     = 85,
                BackColor = Color.FromArgb(0, 180, 100),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btnSend.FlatAppearance.BorderSize = 0;

            btnClear = new Button
            {
                Text      = "Clear",
                Dock      = DockStyle.Right,
                Width     = 68,
                BackColor = Color.FromArgb(180, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btnClear.FlatAppearance.BorderSize = 0;

            pnlBottom.Controls.Add(txtInput);
            pnlBottom.Controls.Add(btnSend);
            pnlBottom.Controls.Add(btnClear);

            // Chat display
            rtbChat = new RichTextBox
            {
                Dock        = DockStyle.Fill,
                BackColor   = Color.FromArgb(18, 18, 35),
                ForeColor   = Color.WhiteSmoke,
                Font        = new Font("Segoe UI", 9.5f),
                ReadOnly    = true,
                BorderStyle = BorderStyle.None,
                ScrollBars  = RichTextBoxScrollBars.Vertical,
                Padding     = new Padding(10)
            };

            // Wire events
            btnSend.Click    += new System.EventHandler(btnSend_Click);
            btnClear.Click   += new System.EventHandler(btnClear_Click);
            txtInput.KeyDown += new KeyEventHandler(txtInput_KeyDown);

            Controls.Add(rtbChat);
            Controls.Add(pnlBottom);
            Controls.Add(lblStatus);
        }
    }
}
