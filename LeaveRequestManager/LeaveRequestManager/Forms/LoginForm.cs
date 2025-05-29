using System;
using System.Drawing;
using System.Windows.Forms;
using LeaveRequestManager.Services;

namespace LeaveRequestManager.Forms
{
    public partial class LoginForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Label lblTitle;
        private Label lblUsername;
        private Label lblPassword;
        private Label lblStatus;
        private Panel panelMain;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Leave Request Manager - Login";
            this.Size = new Size(400, 350);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.WhiteSmoke;

            // Main panel
            panelMain = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(30),
                BackColor = Color.White
            };

            // Title
            lblTitle = new Label
            {
                Text = "Leave Request Manager",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 150, 243),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(30, 20),
                Size = new Size(300, 40),
                AutoSize = false
            };

            // Username label
            lblUsername = new Label
            {
                Text = "Username:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(30, 80),
                Size = new Size(80, 25)
            };

            // Username textbox
            txtUsername = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(30, 105),
                Size = new Size(300, 25),
                PlaceholderText = "Enter your username"
            };

            // Password label
            lblPassword = new Label
            {
                Text = "Password:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(30, 140),
                Size = new Size(80, 25)
            };

            // Password textbox
            txtPassword = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(30, 165),
                Size = new Size(300, 25),
                UseSystemPasswordChar = true,
                PlaceholderText = "Enter your password"
            };

            // Login button
            btnLogin = new Button
            {
                Text = "Login",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(30, 210),
                Size = new Size(300, 40),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += BtnLogin_Click;

            // Status label
            lblStatus = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Red,
                Location = new Point(30, 260),
                Size = new Size(300, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Add controls to panel
            panelMain.Controls.AddRange(new Control[] {
                lblTitle, lblUsername, txtUsername, 
                lblPassword, txtPassword, btnLogin, lblStatus
            });

            // Add panel to form
            this.Controls.Add(panelMain);

            // Set tab order
            txtUsername.TabIndex = 0;
            txtPassword.TabIndex = 1;
            btnLogin.TabIndex = 2;

            // Handle Enter key
            txtPassword.KeyDown += (s, e) => {
                if (e.KeyCode == Keys.Enter)
                {
                    BtnLogin_Click(s, e);
                }
            };
        }

        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                lblStatus.Text = "Please enter username and password.";
                lblStatus.ForeColor = Color.Red;
                return;
            }

            btnLogin.Enabled = false;
            btnLogin.Text = "Logging in...";
            lblStatus.Text = "";

            try
            {
                lblStatus.Text = "Logging in...";
                lblStatus.ForeColor = Color.Blue;
                btnLogin.Enabled = false;

                var user = await AuthService.LoginAsync(txtUsername.Text, txtPassword.Text);

                if (user != null)
                {
                    this.Hide();
                    
                    // Open appropriate dashboard based on role
                    if (user.Role == "Admin")
                    {
                        var adminForm = new AdminDashboardForm();
                        adminForm.FormClosed += (s, args) => Application.Exit();
                        adminForm.Show();
                    }
                    else if (user.Role == "Employee")
                    {
                        var employeeForm = new EmployeeDashboardForm();
                        employeeForm.FormClosed += (s, args) => Application.Exit();
                        employeeForm.Show();
                    }
                }
                else
                {
                    lblStatus.Text = "Invalid username or password";
                    lblStatus.ForeColor = Color.Red;
                    txtPassword.Clear();
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred. Please try again.";
                lblStatus.ForeColor = Color.Red;
                MessageBox.Show($"Error: {ex.Message}", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnLogin.Enabled = true;
                btnLogin.Text = "Login";
            }
        }
    }
} 