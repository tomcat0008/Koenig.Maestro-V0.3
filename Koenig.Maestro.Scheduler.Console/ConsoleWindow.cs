using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Koenig.Maestro.Scheduler.Console
{
    public partial class ConsoleWindow : Form
    {

        MaestroTaskScheduler scheduler;

        public ConsoleWindow()
        {
            InitializeComponent();
        }

        private void ConsoleWindow_Load(object sender, EventArgs e)
        {
            scheduler = new MaestroTaskScheduler();
            scheduler.ConsoleLogEvent += Scheduler_ConsoleLogEvent;
            StartScheduler();


        }


        private void StartScheduler()
        {
            try
            {

                scheduler.OnStart(null);
            }
            catch (Exception ex)
            {
                Scheduler_ConsoleLogEvent(null, new ConsoleLogEventArgs(ex.Message, ex));
            }
        }


        private void Scheduler_ConsoleLogEvent(object sender, ConsoleLogEventArgs e)
        {
            if (this.txtConsole.InvokeRequired)
                txtConsole.Invoke((MethodInvoker)delegate (){DisplayLogMessage(e);});
            else
            {
                DisplayLogMessage(e);
            }


        }

        void DisplayLogMessage(ConsoleLogEventArgs e)
        {
            string dump = e.Exception == null ? string.Empty : e.Exception.ToString();

            if (txtConsole.TextLength >= 5000)
                txtConsole.Clear();
            txtConsole.AppendText("\r\n");
            txtConsole.AppendText(e.Message);
            
            if (!string.IsNullOrWhiteSpace(dump))
            {
                txtConsole.AppendText("\r\n");
                txtConsole.AppendText(dump);
            }


        }


        private void ConsoleWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (scheduler != null)
                scheduler.OnStop();
        }

        private void ConsoleWindow_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
        }
    }
}
