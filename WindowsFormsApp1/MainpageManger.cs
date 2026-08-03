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
        private Panel pnlLine, pnlCategory,pnlRecent, pnlChart;
        private Button btnApply, btnReset;
        private readonly Random rng = new Random();
        private List<(CheckBox checkBox, bool isChecked)> checkLists = new List<(CheckBox, bool)>();
        private class Reports
        {
            public string ProdTime, Shift, Line, Station, ActualTime, StartTime, EndTime, Category1, Category2, problem, Action, Worker;
        }
        private readonly List<Reports> history = new List<Reports>
        {
            new Reports { ProdTime=DateTime.Now.ToString(), Shift="1",  Line="Line1", Station="Station1", ActualTime=DateTime.Now.ToString(), StartTime=DateTime.Now.ToString("HH:mm:ss"), EndTime=DateTime.Now.ToString("HH:mm:ss"), Category1="c1",  Category2="c2",problem="connection error",Action="...",Worker="Worker1"},
            new Reports { ProdTime=DateTime.Now.ToString(), Shift="1",  Line="Line1", Station="Station1", ActualTime=DateTime.Now.ToString(), StartTime=DateTime.Now.ToString("HH:mm:ss"), EndTime=DateTime.Now.ToString("HH:mm:ss"), Category1="c1",  Category2="c2",problem="connection error",Action="...",Worker="Worker1"},
            new Reports { ProdTime=DateTime.Now.ToString(), Shift="1",  Line="Line1", Station="Station1", ActualTime=DateTime.Now.ToString(), StartTime=DateTime.Now.ToString("HH:mm:ss"), EndTime=DateTime.Now.ToString("HH:mm:ss"), Category1="c1",  Category2="c2",problem="connection error",Action="...",Worker="Worker1"},
            new Reports { ProdTime=DateTime.Now.ToString(), Shift="1",  Line="Line1", Station="Station1", ActualTime=DateTime.Now.ToString(), StartTime=DateTime.Now.ToString("HH:mm:ss"), EndTime=DateTime.Now.ToString("HH:mm:ss"), Category1="c1",  Category2="c2",problem="connection error",Action="...",Worker="Worker1"},
            new Reports { ProdTime=DateTime.Now.ToString(), Shift="1",  Line="Line1", Station="Station1", ActualTime=DateTime.Now.ToString(), StartTime=DateTime.Now.ToString("HH:mm:ss"), EndTime=DateTime.Now.ToString("HH:mm:ss"), Category1="c1",  Category2="c2",problem="connection error",Action="...",Worker="Worker1"},
            new Reports { ProdTime=DateTime.Now.ToString(), Shift="1",  Line="Line1", Station="Station1", ActualTime=DateTime.Now.ToString(), StartTime=DateTime.Now.ToString("HH:mm:ss"), EndTime=DateTime.Now.ToString("HH:mm:ss"), Category1="c1",  Category2="c2",problem="connection error",Action="...",Worker="Worker1"},
            new Reports { ProdTime=DateTime.Now.ToString(), Shift="1",  Line="Line1", Station="Station1", ActualTime=DateTime.Now.ToString(), StartTime=DateTime.Now.ToString("HH:mm:ss"), EndTime=DateTime.Now.ToString("HH:mm:ss"), Category1="c1",  Category2="c2",problem="connection error",Action="...",Worker="Worker1"},
        };
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

            btnReset = MakeBtn("??Reset", 80, AppColor.BgPanel, AppColor.TextSecondary);
            btnApply = MakeBtn("??Apply", 80, AppColor.Accent, AppColor.BgDeep);
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
            pnlRecent = BuildRecentPanel();

            pnlContent.Controls.AddRange(new Control[] {pnlChart, pnlLine,pnlCategory, pnlRecent });
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
            pnlRecent.Location = new Point(12, pnlLine.Bottom + pad);
            pnlRecent.Size = new Size(W-pad, pnlContent.ClientSize.Height - pnlLine.Bottom - pad*2);

            pnlChart.Invalidate();
            pnlCategory.Invalidate();
            pnlLine.Invalidate();
            pnlRecent.Invalidate();
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
                for (int i = 1; i < 8; i++)
                    g.DrawLine(pen, i * (W / 8.0f), 0, i * (W / 8.0f), H);

                for (int i = 1; i < 6; i++)
                    g.DrawLine(pen, 0, i * (H / 6.0f), W, i * (H / 6.0f));
            }

            int totalCount = 23; 
            var data = new (string date, int value)[totalCount];
            for (int i = totalCount-1; i >=0; i--)
            {
                string dateStr = DateTime.Now.AddMonths(-i).ToString("yyyy/MM");

                int val = rng.Next(0,H-25);

                data[i] = (dateStr, val);
            }

            int nBars = data.Length * 2;
            float barW = (float)W / nBars;
            int index = 1;

            using (var textBrush = new SolidBrush(AppColor.TextMuted))
            using (var font = new Font("Segoe UI", 9f))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center })
            using (var br = new SolidBrush(AppColor.Accent))
            {
                foreach (var d in data)
                {
                    float centerX = index * (W / 24.0f);
                    float barX = centerX - (barW / 2.0f);
                    float barY = H - d.Item2 - 25; 

                    g.FillRectangle(br, barX, barY, barW, d.Item2);

                    if (index % 3 == 0)
                    {
                        g.DrawString(d.Item1, font, textBrush, centerX, H - 20, sf);
                    }

                    index++;
                }
            }
        }
        private Panel BuildCategoryPanel()
        {
            var p = new Panel { BackColor = AppColor.BgPanel };
            p.Paint += (s, e) => PaintCard(e.Graphics,p,"Issues by Category");

            p.Controls.Add(new Label { Text = "Profile Type", Location = new Point(10, 37), Size = new Size(90, 16), ForeColor = AppColor.TextSecondary, Font = new Font("Segoe UI", 8f), BackColor = Color.Transparent });
            var cmb = new ComboBox { Location = new Point(105, 34), Size = new Size(105, 20), DropDownStyle = ComboBoxStyle.DropDownList, BackColor = AppColor.BgHover, ForeColor = AppColor.TextPrimary, FlatStyle = FlatStyle.Flat, Font = new Font("Consolas", 8f) };
            cmb.Items.AddRange(new object[] { "Category1", "Category2" });
            cmb.SelectedIndex = 0;
            cmb.SelectedIndexChanged += (s, e) => pnlChart?.Invalidate(); ;
            p.Controls.Add(cmb);

            checkLists.Add((new CheckBox { Text = "Device1", Checked = true }, true));
            checkLists.Add((new CheckBox { Text = "Device2", Checked = true }, true));
            checkLists.Add((new CheckBox { Text = "Device3", Checked = true }, true));
            checkLists.Add((new CheckBox { Text = "Device4", Checked = true }, true));
            checkLists.Add((new CheckBox { Text = "Device5", Checked = true }, true));
            checkLists.Add((new CheckBox { Text = "Device6", Checked = true }, true));
            checkLists.Add((new CheckBox { Text = "Device7", Checked = true }, true));
            checkLists.Add((new CheckBox { Text = "Device8", Checked = true }, true));

            int startX = 10;
            int startY = 65; 
            int columnWidth = 110;
            int rowHeight = 24;

            for (int i = 0; i < checkLists.Count; i++)
            {
                var cb = checkLists[i].checkBox;

                int col = i % 2;
                int row = i / 2;

                cb.Location = new Point(startX + (col * columnWidth), startY + (row * rowHeight));
                cb.Size = new Size(105, 20);
                cb.ForeColor = AppColor.TextPrimary;
                cb.Font = new Font("Segoe UI", 9f);
                cb.BackColor = Color.Transparent;
                cb.CheckedChanged += (s, e) => { pnlChart?.Invalidate(); };

                p.Controls.Add(cb);
            }

            return p;
        }

        private Panel BuildLinePanel()
        {
            var p = new Panel { BackColor = AppColor.BgPanel };
            p.Paint += LineCar_Paint;

            return p;
        }
        private Panel BuildRecentPanel()
        {
            var p = new Panel { BackColor = AppColor.BgPanel };
            p.Paint += (s, e) => PaintCard(e.Graphics, p, "recent Issues");

            ListView lvTable = BuildRecentIssueTable();
            p.Controls.Add(lvTable);
            p.Resize += (s, e) =>
            {
                lvTable.Location = new Point(0, 24);
                lvTable.Size = new Size(p.Width, p.Height - 26);
            };

            return p;
        }
        private ListView BuildRecentIssueTable()
        {
            var lv = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = false,
                BackColor = AppColor.BgCard,
                ForeColor = AppColor.TextPrimary,
                BorderStyle = BorderStyle.None,
                Font = new Font("Consolas", 8.5f),
                OwnerDraw = true,
            };

            lv.Columns.Add("ProdTime", 80);
            lv.Columns.Add("Shift", 60);
            lv.Columns.Add("Line", 60);
            lv.Columns.Add("Station", 60);
            lv.Columns.Add("ActualTime", 80);
            lv.Columns.Add("StartTime", 60);
            lv.Columns.Add("EndTime", 60);
            lv.Columns.Add("Category1", 60);
            lv.Columns.Add("Category2", 80);
            lv.Columns.Add("problem", 100);
            lv.Columns.Add("Action", 100);
            lv.Columns.Add("Worker", 60);

            foreach (var h in history)
            {
                var subItem = new ListViewItem(h.ProdTime) { Tag = h };
                subItem.SubItems.Add(h.Shift);
                subItem.SubItems.Add(h.Line);
                subItem.SubItems.Add(h.Station);
                subItem.SubItems.Add(h.ActualTime);
                subItem.SubItems.Add(h.StartTime);
                subItem.SubItems.Add(h.EndTime);
                subItem.SubItems.Add(h.Category1);
                subItem.SubItems.Add(h.Category2);
                subItem.SubItems.Add(h.problem);
                subItem.SubItems.Add(h.Action);
                subItem.SubItems.Add(h.Worker);
                lv.Items.Insert(0, subItem);
            }

            lv.DrawColumnHeader += (s, e) =>
            {
                e.Graphics.FillRectangle(new SolidBrush(AppColor.BgPanel), e.Bounds);
                using (var pen = new Pen(Color.FromArgb(25, 255, 255, 255)))
                    e.Graphics.DrawLine(pen, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);
                using (var f = new Font("Consolas", 7.5f, FontStyle.Bold))
                using (var br = new SolidBrush(AppColor.TextMuted))
                    e.Graphics.DrawString(e.Header.Text.ToUpper(), f, br, e.Bounds.Left + 6, e.Bounds.Top + 5);
            };
            lv.DrawItem += (s, e) => e.DrawBackground();
            lv.DrawSubItem += RecentItem_DrawSubItem;

            return lv;
        }
        private void RecentItem_DrawSubItem(object s, DrawListViewSubItemEventArgs e) 
        {
            var g = e.Graphics; var rc = e.Bounds;
            Color fg = AppColor.TextPrimary;
            if (e.ColumnIndex == 0) fg = AppColor.Blue;

            if (e.Item.Index % 2 == 0)
                using (var br = new SolidBrush(Color.FromArgb(8, 255, 255, 255)))
                    g.FillRectangle(br, rc);

            using (var f = new Font("Consolas", 8.5f))
            using (var br = new SolidBrush(fg))
                g.DrawString(e.SubItem.Text, f, br, rc.X + 5, rc.Y + 4);
            using (var pen = new Pen(Color.FromArgb(7, 255, 255, 255)))
                g.DrawLine(pen, rc.Left, rc.Bottom - 1, rc.Right, rc.Bottom - 1);
        }
        private void LineCar_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            int W = pnlLine.Width, H = pnlLine.Height;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int pad = 16;
            PaintCard(g, pnlLine, "Issues by Category");

            int gX = pad, gY = 24+pad, gR = (H-24)-pad*2;
            var gRect = new Rectangle(gX,gY,gR,gR);

            var rows = new[]
            {
                ("Line1",10f, AppColor.Red),
                ("Line2",30f, AppColor.Accent),
                ("Line3",20f, AppColor.Gold),
                ("Line4",40f, AppColor.Blue),
                ("Line4",0f, AppColor.White),
            };
            int x = pnlLine.Width / 2;
            int y = 34;
            float total = rows.Sum(r => r.Item2);

            float currentAngle = -90f;
            foreach (var (lbl, val, col) in rows)
            {
                float sweepAngle = (val / total) * 360f;
                using (var pen = new Pen(col, 25f))
                {
                    g.DrawArc(pen, gRect, currentAngle, sweepAngle+1.5f);
                }
                using (var f = new Font("Segoe UI", 11f))
                using (var br = new SolidBrush(AppColor.TextSecondary))
                    g.DrawString(lbl, f, br, x, y);
                using (var f = new Font("Segoe UI", 11f))
                using (var br = new SolidBrush(col))
                    g.DrawString((val/total*100)+"%", f, br, x+50, y);
                  y += 22;
                currentAngle += sweepAngle;
            }
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
