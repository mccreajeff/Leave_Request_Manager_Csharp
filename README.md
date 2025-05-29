# Leave Request Management System

A comprehensive Windows Forms application for managing employee leave requests with role-based access control, built using C# and .NET 9.

## Overview

This Leave Request Management System streamlines the process of submitting, tracking, and approving employee leave requests. The application features a modern UI design with role-based access control, ensuring appropriate permissions for different user types.

## Features

### Core Functionality
- **Role-Based Access Control**: Separate interfaces for Employees and Administrators
- **Leave Request Management**: Submit, view, approve, and deny leave requests
- **Data Validation**: Comprehensive validation including date overlap detection
- **Export Capabilities**: Export leave data to CSV and Excel formats
- **Secure Authentication**: Password hashing using BCrypt for secure user authentication

### Employee Features
- Submit new leave requests with multiple leave types (Annual, Sick, Personal, etc.)
- View personal leave request history with status tracking
- Real-time validation of leave dates
- Visual feedback on request status (Pending, Approved, Denied)

### Administrator Features
- Dashboard view of all employee leave requests
- Approve or deny pending requests with comments
- Filter requests by status
- Export functionality for reporting
- Comprehensive overview of leave patterns

## Technical Stack

- **Framework**: .NET 9.0 with Windows Forms
- **Database**: SQL Server LocalDB with Entity Framework Core
- **Architecture**: Repository pattern with separation of concerns
- **Security**: BCrypt.Net for password hashing
- **Export**: ClosedXML for Excel generation

## Project Structure

```
LeaveRequestManager/
├── Data/
│   └── AppDbContext.cs          # Entity Framework database context
├── Models/
│   ├── User.cs                  # User entity model
│   └── LeaveRequest.cs          # Leave request entity model
├── Forms/
│   ├── LoginForm.cs             # Authentication interface
│   ├── EmployeeDashboardForm.cs # Employee portal
│   ├── AdminDashboardForm.cs    # Administrator portal
│   ├── LeaveRequestForm.cs      # Leave submission form
│   └── CommentForm.cs           # Admin comment dialog
├── Services/
│   ├── AuthService.cs           # Authentication logic
│   ├── ExportService.cs         # Data export functionality
│   ├── ValidationService.cs     # Business rule validation
│   └── DatabaseInitializerService.cs # Database setup
└── Program.cs                   # Application entry point
```

## Installation and Setup

### Prerequisites
- .NET 9.0 SDK or later
- SQL Server LocalDB (included with Visual Studio)
- Visual Studio 2022 or compatible IDE

### Database Setup

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/leave-request-manager.git
   cd leave-request-manager/LeaveRequestManager
   ```

2. Install Entity Framework Core tools (if not already installed):
   ```bash
   dotnet tool install --global dotnet-ef
   ```

3. Create the database:
   ```bash
   dotnet ef database update
   ```

4. Build and run the application:
   ```bash
   dotnet build
   dotnet run
   ```

### Default Credentials

The application seeds two default users on first run:

- **Administrator**
  - Username: `admin`
  - Password: `admin123`

- **Employee**
  - Username: `john.doe`
  - Password: `password123`

## Usage Guide

### For Employees
1. Log in with employee credentials
2. Click "New Request" to submit a leave request
3. Select leave type, dates, and provide a reason
4. View your request history in the dashboard
5. Track status updates on your requests

### For Administrators
1. Log in with admin credentials
2. View all pending requests in the dashboard
3. Select a request and click "Approve" or "Deny"
4. Add comments when processing requests
5. Use filters to view requests by status
6. Export data for reporting purposes

## Design Decisions

### Architecture
- **Separation of Concerns**: Clear separation between data access, business logic, and presentation layers
- **Service Layer**: Dedicated services for authentication, validation, and export functionality
- **Entity Framework Core**: Code-first approach with migrations for database management

### Security
- **Password Hashing**: BCrypt with appropriate work factor for secure password storage
- **Role-Based Access**: Database-driven role management for scalability
- **Input Validation**: Comprehensive validation at both UI and service layers

### User Experience
- **Modern UI**: Clean, intuitive interface with color-coded status indicators
- **Responsive Feedback**: Loading indicators and progress updates for long operations
- **Error Handling**: Graceful error handling with user-friendly messages

## Future Enhancements

- Email notifications for request status changes
- Leave balance tracking and automatic calculation
- Reporting dashboard with analytics
- Calendar integration for team leave visibility
- Mobile companion application
- Multi-level approval workflows

## Contributing

This project was developed as a portfolio demonstration. However, suggestions and feedback are welcome. Please feel free to open issues or submit pull requests.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Entity Framework Core team for the excellent ORM
- ClosedXML contributors for the Excel export functionality
- BCrypt.Net maintainers for the secure hashing implementation

---

**Developer**: Jeff McCrea
**Contact**: mccreajeff@gmail.com  
**Portfolio**: 