using LeaveRequestManager.Forms;
using LeaveRequestManager.Services;

namespace LeaveRequestManager;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static async Task Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

        // Initialize database
        try
        {
            await DatabaseInitializerService.InitializeAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Database initialization error: {ex.Message}\n\nPlease ensure SQL Server is running and check your connection string.", 
                "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        Application.Run(new LoginForm());
    }
} 