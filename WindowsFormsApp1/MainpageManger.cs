using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    internal class MainpageManger
    {
        private readonly Form _mainForm;
        public Panel pnlMain, pnlContent, pnlHeader;
        private Panel pnlLine, pnlCategory, pnlChart;
        private Button btnApply, btnReset;
        private ListView lvrecentIssues;
        public MainpageManger(Form mainForm)
        {
            _mainForm = mainForm;
        }
        public void BuildMain()
        {
            pnlMain = new Panel { BackColor = AppColor.BgDeep };
            BuildHeader();
            BuildContent();
            _mainForm.Controls.Add(pnlMain);
        }
        private void BuildHeader()
        {
            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = AppColor.BgPanel };
            pnlHeader.Paint += Header_Paint;

            btnReset = MakeBtn("↺ Reset", 80, AppColor.BgPanel, AppColor.TextSecondary);
            btnApply = MakeBtn("▶ Apply", 80, AppColor.Accent, AppColor.BgDeep);
            btnApply.Font = new Font("Segoe UI", 9f, FontStyle.Bold);

            pnlHeader.Controls.AddRange(new Control[] { btnApply, btnReset });

            void Pos()
            {
                btnApply.Location = new Point(pnlHeader.Width - 98, 15);
                btnReset.Location = new Point(pnlHeader.Width - 184, 15);
            }
            Pos();
            pnlHeader.Resize += (s, e) => Pos();
            pnlMain.Controls.Add(pnlHeader);
        }
        private void BuildContent()
        {
            pnlContent = new Panel { BackColor = AppColor.BgDeep };
            pnlChart = new Panel { BackColor = AppColor.BgDeep };
            pnlChart.Paint += Chart_Paint;

            pnlLine = BuildLinePanel();
            pnlCategory = BuildCategoryPanel();
            lvrecentIssues = BuildRecentIssueTable();

            pnlContent.Controls.AddRange(new Control[] {pnlChart, pnlLine,pnlCategory });
            pnlMain.Controls.Add(pnlContent);
            pnlContent.Resize += (s, e) => LayoutContent();
            LayoutContent();
        }
        public void LayoutContent()
        {
            if (pnlContent == null) return;
            int W = Math.Max(pnlContent.ClientSize.Width - 24, 100);
            int pad = 10;

            pnlChart.Location = new Point(12, 12);
            pnlChart.Size = new Size(W, 230);

            int colW = (W - pad * 2) / 3;
            int row2Y = pnlChart.Bottom + pad;

            pnlLine.Location = new Point(12, row2Y);
            pnlLine.Size = new Size(colW, 165);
            pnlCategory.Location = new Point(12 + colW + pad, row2Y);
            pnlCategory.Size = new Size(colW * 2, 165);

            pnlChart.Invalidate();
            pnlCategory.Invalidate();
            pnlLine.Invalidate();
        }
        private void Header_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var f = new Font("Segoe UI", 13f, FontStyle.Bold))
            using (var br = new SolidBrush(AppColor.TextPrimary))
                g.DrawString("Report Manage", f, br, 48, 12);
            using (var f = new Font("Consolas", 8f))
            using (var br = new SolidBrush(AppColor.TextSecondary))
                g.DrawString("MES Report", f, br, 60, 32);
            using (var pen = new Pen(Color.FromArgb(25, 255, 255, 255)))
                g.DrawLine(pen, 0, pnlHeader.Height - 1, pnlHeader.Width, pnlHeader.Height - 1);
        }
        private Button MakeBtn(string text, int w, Color bg, Color fg)
        {
            var b = new Button
            {
                Text = text,
                Size = new Size(w, 26),
                FlatStyle = FlatStyle.Flat,
                BackColor = bg,
                ForeColor = fg,
                Font = new Font("Segoe UI", 9f),
                Cursor = Cursors.Hand,
            };
            b.MouseEnter += (s, e) => b.BackColor = ControlPaint.Dark(bg, 0.1f);
            b.MouseLeave += (s, e) => b.BackColor = bg;
            return b;
        }
        private void Chart_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int W = pnlChart.Width, H = pnlChart.Height;

            g.Clear(Color.FromArgb(13, 15, 20));

            using (var pen = new Pen(Color.FromArgb(20, 255, 255, 255), 0.5f))
            {
                for (int i = 1; i < 8; i++) g.DrawLine(pen, i * W / 8, 0, i * W / 8, H);
                for (int i = 1; i < 6; i++) g.DrawLine(pen, 0, i * H / 6, W, i * H / 6);
            }

            int nBars = 55;
            int chartLeft = (int)(W * 0.02);
            int chartRight = (int)(W * 0.98);
            float barW = (float)(chartRight - chartLeft) / nBars;
            int vpLeft = chartRight + 4;
            for (int i = 0; i < nBars-13; i++)
            {
                using (var br = new SolidBrush(AppColor.Accent))
                    g.FillRectangle(br, chartLeft+i*(barW+5),H-100,barW,100);
            }
        }
        private Panel BuildCategoryPanel()
        {
            var p = new Panel { BackColor = AppColor.BgPanel };
            p.Paint += (s,e)=> PaintCard(e.Graphics,p,"Issues by Category");

            return p;
        }

        private void P_Paint(object sender, PaintEventArgs e)
        {
            throw new NotImplementedException();
        }

        private Panel BuildLinePanel()
        {
            var p = new Panel { BackColor = AppColor.BgPanel };
            p.Paint += (sender, e) => PaintCard(e.Graphics, p, "Issues by Line");
            var rows = new[]
            {
                ("Line1","100",AppColor.Gold),
                ("Line2","100",AppColor.Gold),
                ("Line3","100",AppColor.Gold),
                ("Line4","100",AppColor.Gold),
            };
            int y = 34;
            foreach (var (lbl, val, col) in rows)
            {
                p.Controls.Add(new Label { Text = lbl, Location = new Point(10, y), Size = new Size(135, 16), ForeColor = AppColor.TextSecondary, Font = new Font("Segoe UI", 8f), BackColor = Color.Transparent });
                p.Controls.Add(new Label { Text = val, Location = new Point(148, y), Size = new Size(70, 16), ForeColor = col, Font = new Font("Consolas", 8.5f, FontStyle.Bold), TextAlign = ContentAlignment.MiddleRight, BackColor = Color.Transparent });
                y += 22;
            }

            return p;
        }
        private ListView BuildRecentIssueTable()
        {
            var p = new ListView { BackColor = AppColor.BgCard };

            return p;
        }

        private void PaintCard(Graphics g, Panel p, string title)
        {
            using (var pen = new Pen(Color.FromArgb(30, 255, 255, 255)))
                g.DrawRectangle(pen, 0, 0, p.Width - 1, p.Height - 1);
            using (var br = new SolidBrush(Color.FromArgb(20, 255, 255, 255)))
                g.FillRectangle(br, 0, 0, p.Width, 24);
            using (var pen = new Pen(Color.FromArgb(25, 255, 255, 255)))
                g.DrawLine(pen, 0, 24, p.Width, 24);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var br = new SolidBrush(AppColor.Accent))
                g.FillEllipse(br, 8, 10, 4, 4);
            using (var f = new Font("Consolas", 7.5f, FontStyle.Bold))
            using (var br = new SolidBrush(AppColor.TextMuted))
                g.DrawString(title.ToUpper(), f, br, 16, 7);
        }
    }
}
