-- Create Database
CREATE DATABASE LeaveRequestDB;
GO

USE LeaveRequestDB;
GO

-- Create Users Table
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(100) NOT NULL,
    EmployeeName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    Role NVARCHAR(20) NOT NULL,
    CreatedAt DATETIME2(7) NOT NULL DEFAULT GETDATE(),
    IsActive BIT NOT NULL DEFAULT 1
);
GO

-- Create unique indexes for Users
CREATE UNIQUE INDEX IX_Users_Username ON Users(Username);
CREATE UNIQUE INDEX IX_Users_Email ON Users(Email);
GO

-- Create LeaveRequests Table
CREATE TABLE LeaveRequests (
    Id INT PRIMARY KEY IDENTITY(1,1),
    EmployeeName NVARCHAR(100) NOT NULL,
    StartDate DATETIME2(7) NOT NULL,
    EndDate DATETIME2(7) NOT NULL,
    LeaveType NVARCHAR(50) NOT NULL,
    Reason NVARCHAR(500) NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    AdminComments NVARCHAR(500) NULL,
    RequestedDate DATETIME2(7) NOT NULL DEFAULT GETDATE(),
    ProcessedDate DATETIME2(7) NULL,
    ProcessedBy NVARCHAR(MAX) NULL,
    UserId INT NOT NULL,
    CONSTRAINT FK_LeaveRequests_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE NO ACTION
);
GO

-- Create index for better query performance
CREATE INDEX IX_LeaveRequests_UserId ON LeaveRequests(UserId);
CREATE INDEX IX_LeaveRequests_Status ON LeaveRequests(Status);
CREATE INDEX IX_LeaveRequests_RequestedDate ON LeaveRequests(RequestedDate DESC);
GO

-- Insert default admin user
-- Password: admin123 (BCrypt hash)
INSERT INTO Users (Username, EmployeeName, Email, PasswordHash, Role, CreatedAt, IsActive)
VALUES ('admin', 'Administrator', 'admin@company.com', 
        '$2a$11$rBaP0p.t5fXPmLN4LfoQVuyNWGzqeJYGSQQmZR6dxTjLWY8bQoJPO', 
        'Admin', GETDATE(), 1);

-- Insert default employee user
-- Password: password123 (BCrypt hash)
INSERT INTO Users (Username, EmployeeName, Email, PasswordHash, Role, CreatedAt, IsActive)
VALUES ('john.doe', 'John Doe', 'john.doe@company.com', 
        '$2a$11$LVKyXuXvJP4mHFQhHGYKcOKVxXvJHVKPoxONJhBKhTjTGxvUUxEYa', 
        'Employee', GETDATE(), 1);
GO

-- Sample leave requests (optional)
INSERT INTO LeaveRequests (EmployeeName, StartDate, EndDate, LeaveType, Reason, Status, RequestedDate, UserId)
VALUES 
    ('John Doe', DATEADD(day, 7, GETDATE()), DATEADD(day, 9, GETDATE()), 'Annual Leave', 'Family vacation', 'Pending', GETDATE(), 2),
    ('John Doe', DATEADD(day, -30, GETDATE()), DATEADD(day, -28, GETDATE()), 'Sick Leave', 'Flu', 'Approved', DATEADD(day, -31, GETDATE()), 2);
GO

-- View to get leave request summary
CREATE VIEW vw_LeaveRequestSummary AS
SELECT 
    lr.Id,
    lr.EmployeeName,
    u.Username,
    u.Email,
    lr.StartDate,
    lr.EndDate,
    DATEDIFF(day, lr.StartDate, lr.EndDate) + 1 AS TotalDays,
    lr.LeaveType,
    lr.Reason,
    lr.Status,
    lr.AdminComments,
    lr.RequestedDate,
    lr.ProcessedDate,
    lr.ProcessedBy
FROM LeaveRequests lr
INNER JOIN Users u ON lr.UserId = u.Id;
GO

-- Stored procedure for leave request statistics
CREATE PROCEDURE sp_GetLeaveStatistics
    @UserId INT = NULL
AS
BEGIN
    SELECT 
        COUNT(*) AS TotalRequests,
        SUM(CASE WHEN Status = 'Approved' THEN 1 ELSE 0 END) AS ApprovedRequests,
        SUM(CASE WHEN Status = 'Denied' THEN 1 ELSE 0 END) AS DeniedRequests,
        SUM(CASE WHEN Status = 'Pending' THEN 1 ELSE 0 END) AS PendingRequests,
        SUM(CASE WHEN Status = 'Approved' THEN DATEDIFF(day, StartDate, EndDate) + 1 ELSE 0 END) AS TotalApprovedDays
    FROM LeaveRequests
    WHERE (@UserId IS NULL OR UserId = @UserId);
END;
GO

PRINT 'Database setup completed successfully!';
PRINT 'Default users created:';
PRINT '  Admin - Username: admin, Password: admin123';
PRINT '  Employee - Username: john.doe, Password: password123'; 