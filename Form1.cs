using System;
using System.Windows.Forms;
using System.Management;

namespace TempAdmin
{
    public partial class Form1 : Form
    {
        System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo(@"C:\Windows\System32\cmd.exe");
        string username;
        private Timer tm;
        int countdownTime = 30; // minutes for the timer
        private const int WM_CLOSE = 0x0010; // to override the sudden close event

        public Form1()
        {

            InitializeComponent();
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit); // for normal exits

            // countdown to close timer
            tm = new Timer();
            tm.Interval = 1000 * 60; // 1000 = one second; 60 seconds = one minute;
            tm.Tick += new EventHandler(tm_Tick);
            tm.Start();

            // gets the username based on the running 'explorer.exe' owner
            System.Management.ManagementObjectSearcher Processes = new System.Management.ManagementObjectSearcher("SELECT * FROM Win32_Process");
            foreach (System.Management.ManagementObject Process in Processes.Get())
            {
                if (Process["ExecutablePath"] != null &&
                    System.IO.Path.GetFileName(Process["ExecutablePath"].ToString()).ToLower() == "explorer.exe")
                {
                    string[] OwnerInfo = new string[2];
                    Process.InvokeMethod("GetOwner", (object[])OwnerInfo);
                    username = OwnerInfo[0];

                    break;
                }
            }

            // setup the command to add to local admin
            info.UseShellExecute = true;
            info.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            info.Arguments = "/c net localgroup administrators /add " + username;
            System.Diagnostics.Process.Start(info);
        }

        private void removeUser() // removes the user from local admin
        {
            info.Arguments = "/c net localgroup administrators /delete " + username;
            System.Diagnostics.Process.Start(info);
            info.Arguments = "/c net localgroup administrators /delete \"domain users\""; // remove domain users in case
            System.Diagnostics.Process.Start(info);
        }

        private void OnProcessExit(object sender, EventArgs e) // on exit
        {
            removeUser();
        }

        private void tm_Tick(object sender, EventArgs e) // timer events
        {
            countdownTime--;
            if(countdownTime > 1)
            {
            this.Text = "Temporary Administrator - " + countdownTime + " minutes remaining"; // normal
            }
            else if (countdownTime == 1)
            {
                this.Text = "Temporary Administrator - " + countdownTime + " minute remaining"; // singular minute
            }
            else if(countdownTime < 1)
            {
                this.Text = "Temporary Administrator - <1 minute remaining"; // less than a minute
            }
            
            if (countdownTime < 0)
            {
                this.Close();
            }
        }

        protected override void WndProc(ref Message m) // on forced exits, overrides WM_CLOSE
        {
            if (m.Msg == WM_CLOSE)
            {
                removeUser();
            }
            base.WndProc(ref m);
        }
    }
}
