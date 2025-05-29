using LeaveRequestManager.Models;
using LeaveRequestManager.Data;
using Microsoft.EntityFrameworkCore;

namespace LeaveRequestManager.Services
{
    public class ValidationService
    {
        public static async Task<(bool isValid, string errorMessage)> ValidateLeaveRequest(
            LeaveRequest request, AppDbContext context)
        {
            // Check if start date is not in the past
            if (request.StartDate.Date < DateTime.Now.Date)
            {
                return (false, "Start date cannot be in the past.");
            }

            // Check if end date is not before start date
            if (request.EndDate < request.StartDate)
            {
                return (false, "End date cannot be before start date.");
            }

            // Check for overlapping requests
            var overlappingRequest = await context.LeaveRequests
                .Where(lr => lr.UserId == request.UserId && 
                            lr.Id != request.Id && // Exclude current request if editing
                            lr.Status != "Denied" &&
                            ((lr.StartDate <= request.StartDate && lr.EndDate >= request.StartDate) ||
                             (lr.StartDate <= request.EndDate && lr.EndDate >= request.EndDate) ||
                             (lr.StartDate >= request.StartDate && lr.EndDate <= request.EndDate)))
                .FirstOrDefaultAsync();

            if (overlappingRequest != null)
            {
                return (false, $"You have an overlapping leave request from {overlappingRequest.StartDate:yyyy-MM-dd} to {overlappingRequest.EndDate:yyyy-MM-dd}");
            }

            // Check maximum days per request (example: 30 days)
            if (request.TotalDays > 30)
            {
                return (false, "Leave request cannot exceed 30 days.");
            }

            // Check if reason is not empty
            if (string.IsNullOrWhiteSpace(request.Reason))
            {
                return (false, "Please provide a reason for your leave request.");
            }

            return (true, string.Empty);
        }

        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsStrongPassword(string password)
        {
            if (password.Length < 8)
                return false;

            bool hasLetter = false;
            bool hasDigit = false;

            foreach (char c in password)
            {
                if (char.IsLetter(c)) hasLetter = true;
                if (char.IsDigit(c)) hasDigit = true;
            }

            return hasLetter && hasDigit;
        }
    }
} 