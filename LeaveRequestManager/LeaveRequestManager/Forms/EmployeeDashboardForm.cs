using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using LeaveRequestManager.Data;
using LeaveRequestManager.Models;
using LeaveRequestManager.Services;

namespace LeaveRequestManager.Forms
{
    public partial class EmployeeDashboardForm : Form
    {
        private Label lblWelcome;
        private DataGridView dgvMyRequests;
        private Button btnNewRequest;
        private Button btnRefresh;
        private Button btnLogout;
        private Panel panelTop;
        private Panel panelMain;

        public EmployeeDashboardForm()
        {
            InitializeComponent();
            LoadMyRequests();
        }

        private void InitializeComponent()
        {
            this.Text = "Employee Dashboard - Leave Request Manager";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            // Top panel
            panelTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(33, 150, 243),
                Padding = new Padding(20, 10, 20, 10)
            };

            // Welcome label
            lblWelcome = new Label
            {
                Text = $"Welcome, {AuthService.CurrentUser?.EmployeeName}!",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 25),
                AutoSize = true
            };

            // Logout button
            btnLogout = new Button
            {
                Text = "Logout",
                Font = new Font("Segoe UI", 10),
                Size = new Size(100, 35),
                Location = new Point(this.Width - 140, 22),
                BackColor = Color.FromArgb(244, 67, 54),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Click += BtnLogout_Click;

            panelTop.Controls.Add(lblWelcome);
            panelTop.Controls.Add(btnLogout);

            // Main panel
            panelMain = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            // Buttons panel
            var panelButtons = new Panel
            {
                Height = 50,
                Dock = DockStyle.Top
            };

            btnNewRequest = new Button
            {
                Text = "New Leave Request",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(160, 40),
                Location = new Point(0, 5),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnNewRequest.FlatAppearance.BorderSize = 0;
            btnNewRequest.Click += BtnNewRequest_Click;

            btnRefresh = new Button
            {
                Text = "Refresh",
                Font = new Font("Segoe UI", 10),
                Size = new Size(100, 40),
                Location = new Point(170, 5),
                BackColor = Color.FromArgb(158, 158, 158),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += (s, e) => LoadMyRequests();

            panelButtons.Controls.Add(btnNewRequest);
            panelButtons.Controls.Add(btnRefresh);

            // DataGridView
            dgvMyRequests = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9),
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(250, 250, 250) }
            };

            dgvMyRequests.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvMyRequests.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvMyRequests.ColumnHeadersHeight = 40;
            dgvMyRequests.EnableHeadersVisualStyles = false;

            panelMain.Controls.Add(dgvMyRequests);
            panelMain.Controls.Add(panelButtons);

            this.Controls.Add(panelMain);
            this.Controls.Add(panelTop);
        }

        private async void LoadMyRequests()
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var myRequests = await context.LeaveRequests
                        .Where(lr => lr.UserId == AuthService.CurrentUser.Id)
                        .OrderByDescending(lr => lr.RequestedDate)
                        .Select(lr => new
                        {
                            lr.Id,
                            StartDate = lr.StartDate.ToString("yyyy-MM-dd"),
                            EndDate = lr.EndDate.ToString("yyyy-MM-dd"),
                            lr.LeaveType,
                            lr.Reason,
                            lr.Status,
                            lr.AdminComments,
                            RequestedDate = lr.RequestedDate.ToString("yyyy-MM-dd HH:mm"),
                            Days = lr.TotalDays
                        })
                        .ToListAsync();

                    dgvMyRequests.DataSource = myRequests;

                    // Format columns
                    if (dgvMyRequests.Columns["Id"] != null)
                        dgvMyRequests.Columns["Id"].Visible = false;

                    // Apply status colors
                    dgvMyRequests.CellFormatting += (s, e) =>
                    {
                        if (e.ColumnIndex == dgvMyRequests.Columns["Status"].Index && e.Value != null)
                        {
                            switch (e.Value.ToString())
                            {
                                case "Approved":
                                    e.CellStyle.BackColor = Color.LightGreen;
                                    e.CellStyle.ForeColor = Color.DarkGreen;
                                    break;
                                case "Denied":
                                    e.CellStyle.BackColor = Color.LightPink;
                                    e.CellStyle.ForeColor = Color.DarkRed;
                                    break;
                                case "Pending":
                                    e.CellStyle.BackColor = Color.LightYellow;
                                    e.CellStyle.ForeColor = Color.DarkOrange;
                                    break;
                            }
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading requests: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnNewRequest_Click(object sender, EventArgs e)
        {
            var requestForm = new LeaveRequestForm();
            if (requestForm.ShowDialog() == DialogResult.OK)
            {
                LoadMyRequests();
            }
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to logout?", "Confirm Logout", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                AuthService.Logout();
                this.Close();
            }
        }
    }
} 