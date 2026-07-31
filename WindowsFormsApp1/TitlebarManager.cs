using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    internal class TitlebarManager
    {
        private readonly Form _mainForm;
        public Panel pnlTitleBar;
        public TitlebarManager(Form mainForm)
        {
            _mainForm = mainForm;
        }
        public void BuildTitlebar()
        {
            pnlTitleBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 46,
                BackColor = AppColor.BgPanel,
            };
            pnlTitleBar.Paint += TitleBar_Paint;
            pnlTitleBar.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    NativeMethods.ReleaseCapture();
                    NativeMethods.SendMessage(_mainForm.Handle, 0xA1, (IntPtr)0x2, IntPtr.Zero);
                }
            };

            var btnClose = MakeWinBtn(Color.FromArgb(255, 95, 86));
            var btnMin = MakeWinBtn(Color.FromArgb(255, 189, 46));
            var btnMax = MakeWinBtn(Color.FromArgb(39, 201, 63));

            btnClose.Click += (s, e) => _mainForm.Close();
            btnMin.Click += (s, e) => _mainForm.WindowState = FormWindowState.Minimized;
            btnMax.Click += (s, e) => _mainForm.WindowState =
                _mainForm.WindowState == FormWindowState.Maximized
                    ? FormWindowState.Normal
                    : FormWindowState.Maximized;

            pnlTitleBar.Controls.AddRange(new Control[] { btnMin, btnMax, btnClose });

            void PositionWinBtns()
            {
                btnClose.Location = new Point(pnlTitleBar.Width - 28, 17);
                btnMax.Location = new Point(pnlTitleBar.Width - 48, 17);
                btnMin.Location = new Point(pnlTitleBar.Width - 68, 17);
            }
            PositionWinBtns();
            pnlTitleBar.Resize += (s, e) => PositionWinBtns();

            _mainForm.Controls.Add(pnlTitleBar);
        }

        private void TitleBar_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var br = new SolidBrush(AppColor.Accent))
                g.FillEllipse(br, 16, 19, 8, 8);
            using (var f = new Font("Segoe UI", 11f, FontStyle.Bold))
            using (var br = new SolidBrush(AppColor.TextPrimary))
                g.DrawString("MES Dashboard", f, br, 32, 14);
            using (var f = new Font("Consolas", 8f))
            using (var br = new SolidBrush(AppColor.TextMuted))
                g.DrawString("v1.0.0", f, br, 34, 30);

            int pw = pnlTitleBar.Width;
            var pill = new Rectangle(pw - 240, 13, 158, 20);
            using (var br = new SolidBrush(Color.FromArgb(30, 0, 212, 170)))
                g.FillRectangle(br, pill);
            using (var pen = new Pen(Color.FromArgb(80, 0, 212, 170)))
                g.DrawRectangle(pen, pill);
            using (var br = new SolidBrush(AppColor.Accent))
                g.FillEllipse(br, pill.X + 6, pill.Y + 7, 6, 6);
            using (var f = new Font("Consolas", 8f, FontStyle.Bold))
            using (var br = new SolidBrush(AppColor.Accent))
                g.DrawString("Server · Connected", f, br, pill.X + 16, pill.Y + 4);

            using (var pen = new Pen(Color.FromArgb(25, 255, 255, 255)))
                g.DrawLine(pen, 0, pnlTitleBar.Height - 1, pnlTitleBar.Width, pnlTitleBar.Height - 1);
        }

        private Panel MakeWinBtn(Color col)
        {
            var p = new Panel { Size = new Size(12, 12), BackColor = col, Cursor = Cursors.Hand };
            p.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var br = new SolidBrush(p.BackColor))
                    e.Graphics.FillEllipse(br, 0, 0, 11, 11);
            };
            p.MouseEnter += (s, e) => p.BackColor = ControlPaint.Light(col, 0.3f);
            p.MouseLeave += (s, e) => p.BackColor = col;
            return p;
        }
    }
}
