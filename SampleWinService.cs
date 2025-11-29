using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
//using System.Threading;
using System.Timers;

namespace SampleWindowsService
{
    public partial class SampleWinService : ServiceBase
    {
        private Timer _timer;
        private readonly string _logPath = @"C:\Temp\SampleWinServiceLog.txt";
        public SampleWinService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // ensure folder exists
            Directory.CreateDirectory(Path.GetDirectoryName(_logPath) ?? @"C:\Temp");

            // write start line
            File.AppendAllText(_logPath, $"Service Starting with latest changes at {DateTime.Now:yyyy-MM-dd HH:mm:ss}\r\n");

            // set up timer to fire every 10 seconds (10000 ms)
            _timer = new Timer(100000); // interval in ms
            _timer.Elapsed += Timer_Elapsed;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        protected override void OnStop()
        {
            _timer?.Stop();
            _timer?.Dispose();
            File.AppendAllText(_logPath, $"Service Stopped at {DateTime.Now:yyyy-MM-dd HH:mm:ss}\r\n");
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                File.AppendAllText(_logPath, $"Tick at {DateTime.Now:yyyy-MM-dd HH:mm:ss}\r\n");
            }
            catch (Exception ex)
            {
                // optionally write to EventLog if file access fails
                if (!EventLog.SourceExists("MyServiceSource"))
                {
                    EventLog.CreateEventSource("MyServiceSource", "Application");
                }
                EventLog.WriteEntry("MyServiceSource", $"Error writing log: {ex.Message}", EventLogEntryType.Error);
            }
        }
    }
}
