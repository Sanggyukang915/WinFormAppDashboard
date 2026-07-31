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
    internal class SidebarManager
    {
        private readonly Form _mainForm;
        public Panel pnlSidebar;
        private string activeNav = "Report Manage";
        public SidebarManager(Form mainForm)
        {
            _mainForm = mainForm;
        }
        public void BuildSidebar()
        {
            pnlSidebar = new Panel 
            { 
                BackColor = AppColor.BgPanel,
                Width = 200,
            };
            pnlSidebar.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(25, 255, 255, 255)))
                    e.Graphics.DrawLine(pen, pnlSidebar.Width - 1, 0,
                                             pnlSidebar.Width - 1, pnlSidebar.Height);
            };

            var items = new[]
            {
                new[]{ "MENU","" },
                new[]{ "ℹ","Report Manage" },
                new[]{ "▦","Line Manage" },
                new[]{ "▤","DB Data Search" },
                new[]{ "◎", "VNC MultiViwer" },
            };

            int y = 12;
            foreach (var item in items)
            {
                string icon = item[0];
                string label = item[1];

                if (label == "")
                {
                    pnlSidebar.Controls.Add(new Label
                    {
                        Text = icon,
                        Location = new Point(14, y),
                        Size = new Size(172, 18),
                        ForeColor = AppColor.TextMuted,
                        Font = new Font("Consolas", 7.5f, FontStyle.Bold),
                        BackColor = Color.Transparent,
                    });
                    y += 22;
                }
                else
                {
                    string navName = label;
                    bool isActive = navName == activeNav;

                    var nav = new Panel
                    {
                        Location = new Point(0, y),
                        Size = new Size(200, 32),
                        BackColor = isActive ? Color.FromArgb(18, 0, 212, 170) : Color.Transparent,
                        Cursor = Cursors.Hand,
                        Tag = navName,
                    };
                    nav.Paint += (s, e) =>
                    {
                        if ((string)nav.Tag == activeNav)
                            using (var pen = new Pen(AppColor.Accent, 2))
                                e.Graphics.DrawLine(pen, 0, 0, 0, nav.Height);
                    };
                    nav.MouseEnter += (s, e) =>
                    {
                        if ((string)nav.Tag != activeNav)
                            nav.BackColor = AppColor.BgHover;
                    };
                    nav.MouseLeave += (s, e) =>
                    {
                        nav.BackColor = (string)nav.Tag == activeNav
                            ? Color.FromArgb(18, 0, 212, 170)
                            : Color.Transparent;
                    };
                    nav.Click += (s, e) => SetActiveNav((string)nav.Tag);

                    var icLbl = new Label
                    {
                        Text = icon,
                        Location = new Point(14, 8),
                        Size = new Size(16, 16),
                        ForeColor = isActive ? AppColor.Accent : AppColor.TextSecondary,
                        Font = new Font("Segoe UI", 9f),
                        BackColor = Color.Transparent,
                        Tag = navName + "_ic",
                    };
                    var txtLbl = new Label
                    {
                        Text = navName,
                        Location = new Point(34, 8),
                        Size = new Size(130, 16),
                        ForeColor = isActive ? AppColor.Accent : AppColor.TextSecondary,
                        Font = new Font("Segoe UI", 9f),
                        BackColor = Color.Transparent,
                        Tag = navName + "_txt",
                    };
                    icLbl.Click += (s, e) => SetActiveNav(navName);
                    txtLbl.Click += (s, e) => SetActiveNav(navName);
                    nav.Controls.AddRange(new Control[] { icLbl, txtLbl });

                    pnlSidebar.Controls.Add(nav);
                    y += 34;
                }
            }

            // footer
            var footer = new Panel { Size = new Size(200, 52), BackColor = Color.Transparent };
            footer.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (var pen = new Pen(Color.FromArgb(25, 255, 255, 255)))
                    g.DrawLine(pen, 0, 0, 200, 0);
                using (var br = new SolidBrush(AppColor.Accent))
                    g.FillEllipse(br, 14, 18, 6, 6);
                using (var f = new Font("Consolas", 8f, FontStyle.Bold))
                using (var br = new SolidBrush(AppColor.Accent))
                    g.DrawString("CONNECTED", f, br, 26, 14);
                using (var f = new Font("Consolas", 7.5f))
                using (var br = new SolidBrush(AppColor.TextMuted))
                    g.DrawString("Done", f, br, 26, 28);
            };
            pnlSidebar.Controls.Add(footer);
            pnlSidebar.Resize += (s, e) =>
                footer.Location = new Point(0, pnlSidebar.Height - 52);

            _mainForm.Controls.Add(pnlSidebar);
        }

        private void SetActiveNav(string navName)
        {
            activeNav = navName;
            foreach (Control c in pnlSidebar.Controls)
            {
                if (!(c is Panel nav) || !(nav.Tag is string tag)) continue;
                bool active = tag == navName;
                nav.BackColor = active ? Color.FromArgb(18, 0, 212, 170) : Color.Transparent;
                nav.Invalidate();
                foreach (Control child in nav.Controls)
                {
                    if (!(child is Label lbl)) continue;
                    bool isThis = lbl.Tag is string lt &&
                                  (lt == navName + "_ic" || lt == navName + "_txt");
                    lbl.ForeColor = isThis ? AppColor.Accent : AppColor.TextSecondary;
                }
            }
        }
    }
}
