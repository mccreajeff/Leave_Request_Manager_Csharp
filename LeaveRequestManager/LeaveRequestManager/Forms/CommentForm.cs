using System;
using System.Drawing;
using System.Windows.Forms;

namespace LeaveRequestManager.Forms
{
    public partial class CommentForm : Form
    {
        private TextBox txtComments;
        private Button btnOK;
        private Button btnCancel;
        private Label lblTitle;
        private Label lblComments;

        private string _comments = "";
        public string Comments => _comments;

        public CommentForm(string action)
        {
            InitializeComponent(action);
        }

        private void InitializeComponent(string action)
        {
            this.Text = $"{action} Request";
            this.Size = new Size(450, 300);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            // Title
            lblTitle = new Label
            {
                Text = $"{action} Leave Request",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = action == "Approved" ? Color.FromArgb(76, 175, 80) : Color.FromArgb(244, 67, 54),
                Location = new Point(20, 20),
                Size = new Size(410, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Comments label
            lblComments = new Label
            {
                Text = "Comments (Optional):",
                Font = new Font("Segoe UI", 10),
                Location = new Point(30, 70),
                Size = new Size(150, 25)
            };

            // Comments textbox
            txtComments = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(30, 95),
                Size = new Size(380, 100),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                PlaceholderText = "Add any comments for the employee..."
            };

            // OK button
            btnOK = new Button
            {
                Text = "OK",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(120, 215),
                Size = new Size(100, 35),
                BackColor = action == "Approved" ? Color.FromArgb(76, 175, 80) : Color.FromArgb(244, 67, 54),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                DialogResult = DialogResult.OK
            };
            btnOK.FlatAppearance.BorderSize = 0;
            btnOK.Click += (s, e) => {
                _comments = txtComments.Text.Trim();
                this.Close();
            };

            // Cancel button
            btnCancel = new Button
            {
                Text = "Cancel",
                Font = new Font("Segoe UI", 10),
                Location = new Point(230, 215),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(158, 158, 158),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                DialogResult = DialogResult.Cancel
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            // Add controls
            this.Controls.AddRange(new Control[] {
                lblTitle, lblComments, txtComments, btnOK, btnCancel
            });

            // Set tab order
            txtComments.TabIndex = 0;
            btnOK.TabIndex = 1;
            btnCancel.TabIndex = 2;
        }
    }
} 