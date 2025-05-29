using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeaveRequestManager.Models
{
    public class LeaveRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string EmployeeName { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [StringLength(50)]
        public string LeaveType { get; set; } // Annual, Sick, Personal, etc.

        [Required]
        [StringLength(500)]
        public string Reason { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Denied

        [StringLength(500)]
        public string? AdminComments { get; set; }

        public DateTime RequestedDate { get; set; } = DateTime.Now;

        public DateTime? ProcessedDate { get; set; }

        public string? ProcessedBy { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        // Navigation property
        public virtual User? User { get; set; }

        // Calculated property
        [NotMapped]
        public int TotalDays => (EndDate - StartDate).Days + 1;
    }
} 