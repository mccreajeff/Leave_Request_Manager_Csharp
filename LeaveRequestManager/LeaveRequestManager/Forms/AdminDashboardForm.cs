using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using LeaveRequestManager.Data;
using LeaveRequestManager.Models;
using LeaveRequestManager.Services;
using System.Text;
using System.IO;
using ClosedXML.Excel;

namespace LeaveRequestManager.Forms
{
    /// <summary>
    /// Administrative dashboard for managing employee leave requests.
    /// Provides functionality for viewing, approving, denying, and exporting leave request data.
    /// </summary>
    public partial class AdminDashboardForm : Form
    {
        #region Private Fields

        private Label lblWelcome;
        private DataGridView dgvAllRequests;
        private Button btnApprove;
        private Button btnDeny;
        private Button btnRefresh;
        private Button btnExportCSV;
        private Button btnExportExcel;
        private Button btnLogout;
        private ComboBox cmbFilter;
        private Label lblFilter;
        private Panel panelTop;
        private Panel panelMain;
        private Panel panelButtons;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the AdminDashboardForm.
        /// </summary>
        public AdminDashboardForm()
        {
            InitializeComponent();
            LoadAllRequests();
        }

        #endregion

        #region UI Initialization

        /// <summary>
        /// Initializes and configures all UI components for the admin dashboard.
        /// </summary>
        private void InitializeComponent()
        {
            this.Text = "Admin Dashboard - Leave Request Manager";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            InitializeTopPanel();
            InitializeMainPanel();
            InitializeDataGridView();

            this.Controls.Add(panelMain);
            this.Controls.Add(panelTop);
        }

        /// <summary>
        /// Creates and configures the top navigation panel.
        /// </summary>
        private void InitializeTopPanel()
        {
            panelTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(33, 150, 243),
                Padding = new Padding(20, 10, 20, 10)
            };

            // Welcome label showing current user
            lblWelcome = new Label
            {
                Text = $"Admin Dashboard - {AuthService.CurrentUser?.EmployeeName}",
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
        }

        /// <summary>
        /// Initializes the main content panel with buttons and filters.
        /// </summary>
        private void InitializeMainPanel()
        {
            panelMain = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            // Create button panel
            panelButtons = new Panel
            {
                Height = 60,
                Dock = DockStyle.Top
            };

            InitializeFilterControls();
            InitializeActionButtons();

            panelMain.Controls.Add(panelButtons);
        }

        /// <summary>
        /// Creates filter controls for request status filtering.
        /// </summary>
        private void InitializeFilterControls()
        {
            lblFilter = new Label
            {
                Text = "Filter:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(0, 20),
                Size = new Size(50, 25)
            };

            cmbFilter = new ComboBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(55, 17),
                Size = new Size(120, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbFilter.Items.AddRange(new string[] { "All", "Pending", "Approved", "Denied" });
            cmbFilter.SelectedIndex = 0;
            cmbFilter.SelectedIndexChanged += (s, e) => LoadAllRequests();

            panelButtons.Controls.Add(lblFilter);
            panelButtons.Controls.Add(cmbFilter);
        }

        /// <summary>
        /// Creates action buttons for request management and export functionality.
        /// </summary>
        private void InitializeActionButtons()
        {
            // Approve button
            btnApprove = new Button
            {
                Text = "Approve",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(100, 40),
                Location = new Point(200, 10),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnApprove.FlatAppearance.BorderSize = 0;
            btnApprove.Click += BtnApprove_Click;

            // Deny button
            btnDeny = new Button
            {
                Text = "Deny",
                Font = new Font("Segoe UI", 10),
                Size = new Size(100, 40),
                Location = new Point(310, 10),
                BackColor = Color.FromArgb(244, 67, 54),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnDeny.FlatAppearance.BorderSize = 0;
            btnDeny.Click += BtnDeny_Click;

            // Refresh button
            btnRefresh = new Button
            {
                Text = "Refresh",
                Font = new Font("Segoe UI", 10),
                Size = new Size(100, 40),
                Location = new Point(420, 10),
                BackColor = Color.FromArgb(158, 158, 158),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += (s, e) => LoadAllRequests();

            // Export CSV button
            btnExportCSV = new Button
            {
                Text = "Export CSV",
                Font = new Font("Segoe UI", 10),
                Size = new Size(110, 40),
                Location = new Point(540, 10),
                BackColor = Color.FromArgb(63, 81, 181),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnExportCSV.FlatAppearance.BorderSize = 0;
            btnExportCSV.Click += BtnExportCSV_Click;

            // Export Excel button
            btnExportExcel = new Button
            {
                Text = "Export Excel",
                Font = new Font("Segoe UI", 10),
                Size = new Size(110, 40),
                Location = new Point(660, 10),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnExportExcel.FlatAppearance.BorderSize = 0;
            btnExportExcel.Click += BtnExportExcel_Click;

            panelButtons.Controls.AddRange(new Control[] {
                btnApprove, btnDeny, btnRefresh, btnExportCSV, btnExportExcel
            });
        }

        /// <summary>
        /// Configures the DataGridView for displaying leave requests.
        /// </summary>
        private void InitializeDataGridView()
        {
            dgvAllRequests = new DataGridView
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

            // Configure header style
            dgvAllRequests.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvAllRequests.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvAllRequests.ColumnHeadersHeight = 40;
            dgvAllRequests.EnableHeadersVisualStyles = false;

            panelMain.Controls.Add(dgvAllRequests);
        }

        #endregion

        #region Data Loading

        /// <summary>
        /// Loads and displays all leave requests based on current filter settings.
        /// </summary>
        private async void LoadAllRequests()
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var query = context.LeaveRequests.Include(lr => lr.User).AsQueryable();

                    // Apply status filter if not "All"
                    if (cmbFilter.SelectedItem?.ToString() != "All")
                    {
                        query = query.Where(lr => lr.Status == cmbFilter.SelectedItem.ToString());
                    }

                    // Project to anonymous type for display
                    var requests = await query
                        .OrderByDescending(lr => lr.RequestedDate)
                        .Select(lr => new
                        {
                            lr.Id,
                            lr.EmployeeName,
                            StartDate = lr.StartDate.ToString("yyyy-MM-dd"),
                            EndDate = lr.EndDate.ToString("yyyy-MM-dd"),
                            lr.LeaveType,
                            lr.Reason,
                            lr.Status,
                            lr.AdminComments,
                            RequestedDate = lr.RequestedDate.ToString("yyyy-MM-dd HH:mm"),
                            Days = lr.TotalDays,
                            lr.UserId
                        })
                        .ToListAsync();

                    dgvAllRequests.DataSource = requests;
                    ConfigureGridColumns();
                    ApplyStatusFormatting();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading requests: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Configures grid column visibility and formatting.
        /// </summary>
        private void ConfigureGridColumns()
        {
            if (dgvAllRequests.Columns["Id"] != null)
                dgvAllRequests.Columns["Id"].Visible = false;
            if (dgvAllRequests.Columns["UserId"] != null)
                dgvAllRequests.Columns["UserId"].Visible = false;
        }

        /// <summary>
        /// Applies color formatting to status cells based on their values.
        /// </summary>
        private void ApplyStatusFormatting()
        {
            dgvAllRequests.CellFormatting += (s, e) =>
            {
                if (e.ColumnIndex == dgvAllRequests.Columns["Status"]?.Index && e.Value != null)
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

        #endregion

        #region Request Processing

        /// <summary>
        /// Handles the approval of selected leave request.
        /// </summary>
        private async void BtnApprove_Click(object sender, EventArgs e)
        {
            await ProcessRequest("Approved");
        }

        /// <summary>
        /// Handles the denial of selected leave request.
        /// </summary>
        private async void BtnDeny_Click(object sender, EventArgs e)
        {
            await ProcessRequest("Denied");
        }

        /// <summary>
        /// Processes a leave request with the specified status.
        /// </summary>
        /// <param name="status">The new status to apply (Approved or Denied)</param>
        private async Task ProcessRequest(string status)
        {
            if (dgvAllRequests.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a request to process.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = dgvAllRequests.SelectedRows[0];
            var requestId = (int)selectedRow.Cells["Id"].Value;
            var currentStatus = selectedRow.Cells["Status"].Value.ToString();

            // Validate that request is pending
            if (currentStatus != "Pending")
            {
                MessageBox.Show("Only pending requests can be processed.", "Invalid Status", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get admin comments
            string comments = "";
            using (var commentForm = new CommentForm(status))
            {
                if (commentForm.ShowDialog() == DialogResult.OK)
                {
                    comments = commentForm.Comments;
                }
            }

            try
            {
                using (var context = new AppDbContext())
                {
                    var request = await context.LeaveRequests.FindAsync(requestId);
                    if (request != null)
                    {
                        // Update request details
                        request.Status = status;
                        request.AdminComments = comments;
                        request.ProcessedDate = DateTime.Now;
                        request.ProcessedBy = AuthService.CurrentUser.EmployeeName;

                        await context.SaveChangesAsync();

                        MessageBox.Show($"Request has been {status.ToLower()} successfully!", "Success", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadAllRequests();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing request: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Export Functionality

        /// <summary>
        /// Handles CSV export button click.
        /// </summary>
        private void BtnExportCSV_Click(object sender, EventArgs e)
        {
            ShowExportOptions("CSV");
        }

        /// <summary>
        /// Handles Excel export button click.
        /// </summary>
        private void BtnExportExcel_Click(object sender, EventArgs e)
        {
            ShowExportOptions("Excel");
        }

        /// <summary>
        /// Shows export location options to the user.
        /// </summary>
        /// <param name="format">The export format (CSV or Excel)</param>
        private void ShowExportOptions(string format)
        {
            var result = MessageBox.Show(
                "Choose export location:\n\n" +
                "• Desktop - Quick export to desktop\n" +
                "• Choose Folder - Select where to save\n\n" +
                "Would you like to export to Desktop?", 
                "Export Location", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                ExportDirectToDesktop(format);
            }
            else
            {
                ExportWithFolderBrowser(format);
            }
        }

        /// <summary>
        /// Exports data directly to the user's desktop.
        /// </summary>
        /// <param name="format">The export format (CSV or Excel)</param>
        private void ExportDirectToDesktop(string format)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                btnExportCSV.Enabled = false;
                btnExportExcel.Enabled = false;

                // Retrieve data
                List<LeaveRequest> requests;
                using (var context = new AppDbContext())
                {
                    requests = context.LeaveRequests.ToList();
                }

                // Generate filename and path
                var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var fileName = $"LeaveRequests_{DateTime.Now:yyyyMMdd_HHmmss}";
                var filePath = Path.Combine(desktopPath, format == "CSV" ? $"{fileName}.csv" : $"{fileName}.xlsx");

                // Perform export
                if (format == "CSV")
                {
                    ExportService.ExportToCSVSimple(requests, filePath);
                }
                else
                {
                    ExportService.ExportToExcelSimple(requests, filePath);
                }

                MessageBox.Show($"Export completed successfully!\nFile saved to: {filePath}", 
                    "Export Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Export failed: {ex.Message}", "Export Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
                btnExportCSV.Enabled = true;
                btnExportExcel.Enabled = true;
            }
        }

        /// <summary>
        /// Exports data to a user-selected folder.
        /// </summary>
        /// <param name="format">The export format (CSV or Excel)</param>
        private void ExportWithFolderBrowser(string format)
        {
            try
            {
                using (var folderDialog = new FolderBrowserDialog())
                {
                    folderDialog.Description = "Select folder to save the export file";
                    folderDialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    
                    if (folderDialog.ShowDialog() == DialogResult.OK)
                    {
                        var fileName = $"LeaveRequests_{DateTime.Now:yyyyMMdd_HHmmss}";
                        var extension = format == "CSV" ? ".csv" : ".xlsx";
                        var filePath = Path.Combine(folderDialog.SelectedPath, fileName + extension);
                        
                        // Check for existing file
                        if (File.Exists(filePath))
                        {
                            var result = MessageBox.Show($"File {fileName + extension} already exists. Overwrite?", 
                                "File Exists", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result != DialogResult.Yes)
                                return;
                        }
                        
                        PerformExport(format, filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Performs the actual export operation.
        /// </summary>
        /// <param name="format">The export format</param>
        /// <param name="filePath">The target file path</param>
        private void PerformExport(string format, string filePath)
        {
            this.Cursor = Cursors.WaitCursor;
            btnExportCSV.Enabled = false;
            btnExportExcel.Enabled = false;

            try
            {
                List<LeaveRequest> requests;
                using (var context = new AppDbContext())
                {
                    requests = context.LeaveRequests.ToList();
                }

                if (format == "CSV")
                {
                    ExportService.ExportToCSVSimple(requests, filePath);
                }
                else
                {
                    ExportService.ExportToExcelSimple(requests, filePath);
                }

                MessageBox.Show($"Export completed successfully!\nFile saved to: {filePath}", 
                    "Export Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // Open folder in explorer
                System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{filePath}\"");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Export failed: {ex.Message}", "Export Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
                btnExportCSV.Enabled = true;
                btnExportExcel.Enabled = true;
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles the logout button click event.
        /// </summary>
        private void BtnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to logout?", "Confirm Logout", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                AuthService.Logout();
                this.Close();
            }
        }

        #endregion
    }
} 