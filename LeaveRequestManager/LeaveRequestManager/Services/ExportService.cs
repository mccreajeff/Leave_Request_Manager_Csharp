using System.Text;
using ClosedXML.Excel;
using LeaveRequestManager.Models;

namespace LeaveRequestManager.Services
{
    /// <summary>
    /// Provides data export functionality for leave requests.
    /// Supports CSV and Excel formats with proper formatting and error handling.
    /// </summary>
    public static class ExportService
    {
        #region CSV Export

        /// <summary>
        /// Exports leave request data to CSV format.
        /// </summary>
        /// <param name="requests">Collection of leave requests to export</param>
        /// <param name="filePath">Target file path for the CSV file</param>
        public static void ExportToCSVSimple(List<LeaveRequest> requests, string filePath)
        {
            var csv = new StringBuilder();
            
            // Add CSV headers
            csv.AppendLine("ID,Employee Name,Start Date,End Date,Leave Type,Reason,Status,Admin Comments,Requested Date,Total Days");
            
            // Process each request
            foreach (var request in requests)
            {
                // Escape special characters in text fields
                var reason = EscapeCSVField(request.Reason);
                var comments = EscapeCSVField(request.AdminComments);
                
                // Build CSV row
                csv.AppendLine($"{request.Id}," +
                    $"\"{request.EmployeeName}\"," +
                    $"{request.StartDate:yyyy-MM-dd}," +
                    $"{request.EndDate:yyyy-MM-dd}," +
                    $"\"{request.LeaveType}\"," +
                    $"\"{reason}\"," +
                    $"{request.Status}," +
                    $"\"{comments}\"," +
                    $"{request.RequestedDate:yyyy-MM-dd HH:mm}," +
                    $"{request.TotalDays}");
            }
            
            // Write to file
            File.WriteAllText(filePath, csv.ToString());
        }

        /// <summary>
        /// Escapes special characters in CSV fields to prevent formatting issues.
        /// </summary>
        /// <param name="field">The field value to escape</param>
        /// <returns>Escaped field value safe for CSV</returns>
        private static string EscapeCSVField(string? field)
        {
            if (string.IsNullOrEmpty(field))
                return string.Empty;
            
            // Escape double quotes by doubling them
            return field.Replace("\"", "\"\"");
        }

        #endregion

        #region Excel Export

        /// <summary>
        /// Exports leave request data to Excel format with formatting.
        /// </summary>
        /// <param name="requests">Collection of leave requests to export</param>
        /// <param name="filePath">Target file path for the Excel file</param>
        public static void ExportToExcelSimple(List<LeaveRequest> requests, string filePath)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Leave Requests");
                
                // Define column headers
                var headers = new[] 
                { 
                    "ID", "Employee Name", "Start Date", "End Date", "Leave Type", 
                    "Reason", "Status", "Admin Comments", "Requested Date", "Total Days" 
                };
                
                // Add headers to worksheet
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cell(1, i + 1).Value = headers[i];
                }
                
                // Apply header styling
                ApplyHeaderStyle(worksheet, headers.Length);
                
                // Add data rows
                int currentRow = 2;
                foreach (var request in requests)
                {
                    worksheet.Cell(currentRow, 1).Value = request.Id;
                    worksheet.Cell(currentRow, 2).Value = request.EmployeeName ?? "";
                    worksheet.Cell(currentRow, 3).Value = request.StartDate.ToString("yyyy-MM-dd");
                    worksheet.Cell(currentRow, 4).Value = request.EndDate.ToString("yyyy-MM-dd");
                    worksheet.Cell(currentRow, 5).Value = request.LeaveType ?? "";
                    worksheet.Cell(currentRow, 6).Value = request.Reason ?? "";
                    worksheet.Cell(currentRow, 7).Value = request.Status ?? "Pending";
                    worksheet.Cell(currentRow, 8).Value = request.AdminComments ?? "";
                    worksheet.Cell(currentRow, 9).Value = request.RequestedDate.ToString("yyyy-MM-dd HH:mm");
                    worksheet.Cell(currentRow, 10).Value = request.TotalDays;
                    
                    // Apply status-based formatting
                    ApplyStatusFormatting(worksheet.Cell(currentRow, 7), request.Status);
                    
                    currentRow++;
                }
                
                // Auto-fit columns for better readability
                worksheet.Columns().AdjustToContents();
                
                // Save the workbook
                workbook.SaveAs(filePath);
            }
        }

        /// <summary>
        /// Applies professional styling to the header row.
        /// </summary>
        /// <param name="worksheet">The worksheet to style</param>
        /// <param name="columnCount">Number of columns in the header</param>
        private static void ApplyHeaderStyle(IXLWorksheet worksheet, int columnCount)
        {
            var headerRange = worksheet.Range(1, 1, 1, columnCount);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
            headerRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }

        /// <summary>
        /// Applies color formatting based on leave request status.
        /// </summary>
        /// <param name="cell">The cell to format</param>
        /// <param name="status">The status value</param>
        private static void ApplyStatusFormatting(IXLCell cell, string? status)
        {
            switch (status)
            {
                case "Approved":
                    cell.Style.Fill.BackgroundColor = XLColor.LightGreen;
                    cell.Style.Font.FontColor = XLColor.DarkGreen;
                    break;
                case "Denied":
                    cell.Style.Fill.BackgroundColor = XLColor.LightPink;
                    cell.Style.Font.FontColor = XLColor.DarkRed;
                    break;
                case "Pending":
                    cell.Style.Fill.BackgroundColor = XLColor.LightYellow;
                    cell.Style.Font.FontColor = XLColor.DarkOrange;
                    break;
            }
        }

        #endregion

        #region Async Export Methods (Legacy)

        /// <summary>
        /// Asynchronously exports leave requests to CSV format with progress reporting.
        /// </summary>
        /// <param name="requests">Collection of leave requests</param>
        /// <param name="filePath">Target file path</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns>True if export successful, false otherwise</returns>
        public static async Task<bool> ExportToCSVAsync(List<LeaveRequest> requests, string filePath, IProgress<string>? progress = null)
        {
            return await Task.Run(() =>
            {
                try
                {
                    progress?.Report("Preparing CSV export...");
                    ExportToCSVSimple(requests, filePath);
                    progress?.Report("CSV export completed.");
                    return true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"CSV Export Error: {ex.Message}");
                    return false;
                }
            });
        }

        /// <summary>
        /// Asynchronously exports leave requests to Excel format with progress reporting.
        /// </summary>
        /// <param name="requests">Collection of leave requests</param>
        /// <param name="filePath">Target file path</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns>True if export successful, false otherwise</returns>
        public static async Task<bool> ExportToExcelAsync(List<LeaveRequest> requests, string filePath, IProgress<string>? progress = null)
        {
            return await Task.Run(() =>
            {
                try
                {
                    progress?.Report("Creating Excel workbook...");
                    ExportToExcelSimple(requests, filePath);
                    progress?.Report("Excel export completed.");
                    return true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Excel Export Error: {ex.Message}");
                    return false;
                }
            });
        }

        #endregion
    }
} 