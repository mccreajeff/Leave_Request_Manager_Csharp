using System;
using System.Drawing;
using System.Windows.Forms;
using LeaveRequestManager.Data;
using LeaveRequestManager.Models;
using LeaveRequestManager.Services;

namespace LeaveRequestManager.Forms
{
    public partial class LeaveRequestForm : Form
    {
        private DateTimePicker dtpStartDate;
        private DateTimePicker dtpEndDate;
        private ComboBox cmbLeaveType;
        private TextBox txtReason;
        private Button btnSubmit;
        private Button btnCancel;
        private Label lblStartDate;
        private Label lblEndDate;
        private Label lblLeaveType;
        private Label lblReason;
        private Label lblDays;
        private Label lblTitle;

        public LeaveRequestForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "New Leave Request";
            this.Size = new Size(500, 450);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            // Title
            lblTitle = new Label
            {
                Text = "Submit Leave Request",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 150, 243),
                Location = new Point(20, 20),
                Size = new Size(460, 35),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Start Date
            lblStartDate = new Label
            {
                Text = "Start Date:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(30, 70),
                Size = new Size(100, 25)
            };

            dtpStartDate = new DateTimePicker
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(30, 95),
                Size = new Size(200, 25),
                Format = DateTimePickerFormat.Short,
                MinDate = DateTime.Today
            };
            dtpStartDate.ValueChanged += DatePicker_ValueChanged;

            // End Date
            lblEndDate = new Label
            {
                Text = "End Date:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(250, 70),
                Size = new Size(100, 25)
            };

            dtpEndDate = new DateTimePicker
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(250, 95),
                Size = new Size(200, 25),
                Format = DateTimePickerFormat.Short,
                MinDate = DateTime.Today
            };
            dtpEndDate.ValueChanged += DatePicker_ValueChanged;

            // Days label
            lblDays = new Label
            {
                Text = "Total Days: 1",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 150, 243),
                Location = new Point(30, 130),
                Size = new Size(420, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Leave Type
            lblLeaveType = new Label
            {
                Text = "Leave Type:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(30, 165),
                Size = new Size(100, 25)
            };

            cmbLeaveType = new ComboBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(30, 190),
                Size = new Size(420, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbLeaveType.Items.AddRange(new string[] { "Annual Leave", "Sick Leave", "Personal Leave", "Maternity Leave", "Paternity Leave", "Other" });
            cmbLeaveType.SelectedIndex = 0;

            // Reason
            lblReason = new Label
            {
                Text = "Reason:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(30, 230),
                Size = new Size(100, 25)
            };

            txtReason = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(30, 255),
                Size = new Size(420, 80),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            // Buttons
            btnSubmit = new Button
            {
                Text = "Submit Request",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(140, 355),
                Size = new Size(130, 40),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSubmit.FlatAppearance.BorderSize = 0;
            btnSubmit.Click += BtnSubmit_Click;

            btnCancel = new Button
            {
                Text = "Cancel",
                Font = new Font("Segoe UI", 10),
                Location = new Point(280, 355),
                Size = new Size(100, 40),
                BackColor = Color.FromArgb(158, 158, 158),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            // Add controls
            this.Controls.AddRange(new Control[] {
                lblTitle, lblStartDate, dtpStartDate, lblEndDate, dtpEndDate,
                lblDays, lblLeaveType, cmbLeaveType, lblReason, txtReason,
                btnSubmit, btnCancel
            });
        }

        private void DatePicker_ValueChanged(object sender, EventArgs e)
        {
            if (dtpEndDate.Value < dtpStartDate.Value)
            {
                dtpEndDate.Value = dtpStartDate.Value;
            }

            int days = (dtpEndDate.Value - dtpStartDate.Value).Days + 1;
            lblDays.Text = $"Total Days: {days}";
        }

        private async void BtnSubmit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtReason.Text))
            {
                MessageBox.Show("Please provide a reason for your leave request.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtReason.Focus();
                return;
            }

            btnSubmit.Enabled = false;
            btnSubmit.Text = "Submitting...";

            try
            {
                using (var context = new AppDbContext())
                {
                    var leaveRequest = new LeaveRequest
                    {
                        UserId = AuthService.CurrentUser.Id,
                        EmployeeName = AuthService.CurrentUser.EmployeeName,
                        StartDate = dtpStartDate.Value.Date,
                        EndDate = dtpEndDate.Value.Date,
                        LeaveType = cmbLeaveType.Text,
                        Reason = txtReason.Text.Trim(),
                        Status = "Pending",
                        RequestedDate = DateTime.Now
                    };

                    // Validate the request
                    var validationResult = await ValidationService.ValidateLeaveRequest(leaveRequest, context);
                    if (!validationResult.isValid)
                    {
                        MessageBox.Show(validationResult.errorMessage, "Validation Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    context.LeaveRequests.Add(leaveRequest);
                    await context.SaveChangesAsync();

                    MessageBox.Show("Leave request submitted successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error submitting request: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSubmit.Enabled = true;
                btnSubmit.Text = "Submit Request";
            }
        }
    }
} 