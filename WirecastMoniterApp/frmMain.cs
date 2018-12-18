using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tulpep.NotificationWindow;

namespace WirecastMoniterApp
{
    public partial class frmMain : Form
    {
        private WirecastBinding _Wirecast;
        private bool _IsStarted = false;
        private string _Logfile = @"wirecast_start.log";

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            _Wirecast = new WirecastBinding();
            _Wirecast.Initialize();
        }

        private void btnStartMonitoring_Click(object sender, EventArgs e)
        {
            if (!_IsStarted)
            {
                int interval = int.Parse(cbInterval.Text);

                timer1.Interval = interval * 1000 * 60;
                timer1.Start();
                cbInterval.Enabled = false;
                _IsStarted = true;
                btnStartMonitoring.Text = @"Stop monitoring.";

                LogWrite("Start monitoring...");
            }
            else
            {
                timer1.Stop();
                cbInterval.Enabled = true;
                btnStartMonitoring.Text = @"Start monitoring.";
                _IsStarted = false;

                LogWrite("Stop monitoring");
            }
        }

        public void LogWrite(string logMessage)
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            try
            {
                using (StreamWriter w = File.AppendText(path + "\\" + _Logfile))
                {
                    Log(logMessage, w);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void Log(string logMessage, TextWriter txtWriter)
        {
            try
            {
                txtWriter.WriteLine("-------------------------------");
                txtWriter.WriteLine(DateTime.Now.ToString() + " : " + logMessage);
                txtWriter.WriteLine("-------------------------------");
            }
            catch (Exception ex)
            {
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!_Wirecast.IsBroadcasting())
            {
                LogWrite("Wirecast is stopped. Start now...");
                var notifier = new PopupNotifier()
                {
                    TitleText = @"Wirecast broadcast is stopped!",
                    TitleFont = new Font("Times New Roman", 20.0f),
                    ContentText = @"Start broadcast again!",
                    ContentFont = new Font("Times New Roman", 17.0f),
                };
                notifier.Popup();


                _Wirecast.StartBroadcast();
            }
        }
    }
}
