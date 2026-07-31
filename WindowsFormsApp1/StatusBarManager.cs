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
        public Label lblStatus, lblClock;
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
            };

            lblStatus = new Label { Text = "● Ready", Location = new Point(14, 7), Size = new Size(90, 14), ForeColor = AppColor.Accent, Font = new Font("Consolas", 8f), BackColor = Color.Transparent };
            lblClock = new Label { Location = new Point(200, 7), Size = new Size(180, 14), ForeColor = AppColor.TextMuted, Font = new Font("Consolas", 8f), BackColor = Color.Transparent, Text = DateTime.Now.ToString("yyyy.MM.dd · HH:mm:ss") };
            pnlStatusBar.Controls.AddRange(new Control[] { lblStatus, lblClock });
            _mainForm.Controls.Add(pnlStatusBar);
        }
    }
}
