using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    internal class StatusBarManager
    {
        private readonly Form _mainForm;
        public Panel pnlStatusBar;
        private Label lblStatus, lblClock;
        public StatusBarManager(Form mainForm)
        {
            _mainForm = mainForm;
        }
        public void BuildStatusBar()
        {
            pnlStatusBar = new Panel { Dock = DockStyle.Bottom, Height = 28, BackColor = AppColor.BgPanel };
            pnlStatusBar.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(25, 255, 255, 255)))
                    e.Graphics.DrawLine(pen, 0, 0, pnlStatusBar.Width, 0);
                using (var f = new Font("Consolas", 8f))
                using (var br = new SolidBrush(AppColor.TextMuted))
                {
                    e.Graphics.DrawString("EURUSD · Spread: 0.8", f, br, 110, 7);
                    e.Graphics.DrawString("VP v2.1.0", f, br, pnlStatusBar.Width - 80, 7);
                }
            };

            lblStatus = new Label { Text = "● Ready", Location = new Point(14, 7), Size = new Size(90, 14), ForeColor = AppColor.Accent, Font = new Font("Consolas", 8f), BackColor = Color.Transparent };
            lblClock = new Label { Location = new Point(200, 7), Size = new Size(180, 14), ForeColor = AppColor.TextMuted, Font = new Font("Consolas", 8f), BackColor = Color.Transparent, Text = DateTime.Now.ToString("HH:mm:ss · dd.MM.yyyy") };
            pnlStatusBar.Controls.AddRange(new Control[] { lblStatus, lblClock });
            _mainForm.Controls.Add(pnlStatusBar);
        }
    }
}
