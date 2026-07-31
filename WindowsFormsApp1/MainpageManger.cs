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
    internal class MainpageManger
    {
        private readonly Form _mainForm;
        public Panel pnlMain, pnlHeader, pnlContent, pnlChart;
        private Label lblLivePrice, lblPriceChange;
        public Panel pnlKeyLevels, pnlSettings, pnlSession;
        private string selectedTF = "H1";
        private string profileType = "Session";
        private Button btnApply, btnReset;
        private ListView lvLevels;
        private readonly Random rng = new Random();
        private bool showPOC = true;
        private bool showVA = true;
        private bool splitMode = false; 
        private double livePrice = 1.08547;
        private double priceChange = 0.0012;
        private CheckBox chkPOC, chkVA, chkSplit;
        private readonly List<VPLevel> vpLevels = new List<VPLevel>
        {
            new VPLevel { Price = 1.08680, Volume = 28400, BuyPct = 62, Type = "VAH" },
            new VPLevel { Price = 1.08620, Volume = 41200, BuyPct = 58, Type = ""    },
            new VPLevel { Price = 1.08560, Volume = 52800, BuyPct = 54, Type = ""    },
            new VPLevel { Price = 1.08520, Volume = 68900, BuyPct = 51, Type = "POC" },
            new VPLevel { Price = 1.08470, Volume = 47300, BuyPct = 47, Type = ""    },
            new VPLevel { Price = 1.08400, Volume = 33100, BuyPct = 43, Type = ""    },
            new VPLevel { Price = 1.08340, Volume = 22600, BuyPct = 39, Type = "VAL" },
        };
        public class VPLevel
        {
            public double Price { get; set; }
            public int Volume { get; set; }
            public int BuyPct { get; set; }
            public string Type { get; set; }
        }
        public MainpageManger(Form mainForm)
        {
            _mainForm = mainForm;
        }

        public void BuildMain()
        {
            pnlMain = new Panel { BackColor = AppColor.BgDeep };
            BuildMainHeader();
            BuildContentArea();
            _mainForm.Controls.Add(pnlMain);
        }

        private void BuildMainHeader()
        {
            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = AppColor.BgPanel };
            pnlHeader.Paint += Header_Paint;

            string[] tfs = { "M1", "M5", "M15", "H1", "H4", "D1" };
            int tx = 310;
            foreach (var tfStr in tfs)
            {
                string tf = tfStr;
                var b = new Button
                {
                    Text = tf,
                    Location = new Point(tx, 17),
                    Size = new Size(34, 20),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = tf == selectedTF ? Color.FromArgb(20, 0, 212, 170) : Color.Transparent,
                    ForeColor = tf == selectedTF ? AppColor.Accent : AppColor.TextSecondary,
                    Font = new Font("Consolas", 8f, FontStyle.Bold),
                    Cursor = Cursors.Hand,
                };
                b.FlatAppearance.BorderColor = tf == selectedTF
                    ? Color.FromArgb(80, 0, 212, 170)
                    : Color.FromArgb(25, 255, 255, 255);
                b.FlatAppearance.BorderSize = 1;
                b.Click += (s, e) =>
                {
                    selectedTF = tf;
                    foreach (Control c in pnlHeader.Controls)
                    {
                        if (!(c is Button btn) || Array.IndexOf(tfs, btn.Text) < 0) continue;
                        bool sel = btn.Text == selectedTF;
                        btn.BackColor = sel ? Color.FromArgb(20, 0, 212, 170) : Color.Transparent;
                        btn.ForeColor = sel ? AppColor.Accent : AppColor.TextSecondary;
                        btn.FlatAppearance.BorderColor = sel
                            ? Color.FromArgb(80, 0, 212, 170)
                            : Color.FromArgb(25, 255, 255, 255);
                    }
                    pnlChart?.Invalidate();
                };
                pnlHeader.Controls.Add(b);
                tx += 38;
            }

            lblLivePrice = new Label
            {
                Text = "1.08547",
                Location = new Point(560, 15),
                Size = new Size(90, 22),
                ForeColor = AppColor.Accent,
                Font = new Font("Consolas", 13f, FontStyle.Bold),
                BackColor = Color.Transparent,
            };
            lblPriceChange = new Label
            {
                Text = "▲ +0.00012",
                Location = new Point(655, 20),
                Size = new Size(90, 16),
                ForeColor = AppColor.Accent,
                Font = new Font("Consolas", 9f),
                BackColor = Color.Transparent,
            };

            btnReset = MakeBtn("↺ Reset", 80, AppColor.BgPanel, AppColor.TextSecondary);
            btnApply = MakeBtn("▶ Apply", 80, AppColor.Accent, AppColor.BgDeep);
            btnApply.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            btnApply.Click += BtnApply_Click;
            btnReset.Click += (s, e) => pnlChart?.Invalidate();

            pnlHeader.Controls.AddRange(new Control[] { lblLivePrice, lblPriceChange, btnApply, btnReset });

            void Pos()
            {
                btnApply.Location = new Point(pnlHeader.Width - 98, 15);
                btnReset.Location = new Point(pnlHeader.Width - 184, 15);
            }
            Pos();
            pnlHeader.Resize += (s, e) => Pos();
            pnlMain.Controls.Add(pnlHeader);
        }
        private void BuildContentArea()
        {
            pnlContent = new Panel { BackColor = AppColor.BgDeep, AutoScroll = true };

            pnlChart = new Panel { BackColor = AppColor.BgDeep };
            pnlChart.Paint += Chart_Paint;

            pnlKeyLevels = BuildKeyLevelsPanel();
            pnlSettings = BuildSettingsPanel();
            pnlSession = BuildSessionPanel();
            lvLevels = BuildLevelsTable();

            pnlContent.Controls.AddRange(new Control[]
            {
                pnlChart, pnlKeyLevels, pnlSettings, pnlSession, lvLevels
            });
            pnlMain.Controls.Add(pnlContent);
            pnlContent.Resize += (s, e) => LayoutContent();
            LayoutContent();
        }
        private void Header_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var br = new SolidBrush(Color.FromArgb(30, 0, 212, 170)))
                g.FillRectangle(br, 14, 10, 36, 36);
            using (var pen = new Pen(Color.FromArgb(80, 0, 212, 170)))
                g.DrawRectangle(pen, 14, 10, 35, 35);
            using (var f = new Font("Segoe UI", 16f))
            using (var br = new SolidBrush(AppColor.Accent))
                g.DrawString("▦", f, br, 18, 12);
            using (var f = new Font("Segoe UI", 13f, FontStyle.Bold))
            using (var br = new SolidBrush(AppColor.TextPrimary))
                g.DrawString("Volume Profile", f, br, 58, 12);
            using (var f = new Font("Consolas", 8f))
            using (var br = new SolidBrush(AppColor.TextSecondary))
                g.DrawString("TradingView-style volume profile for MT5  ·  Price action & SMC", f, br, 60, 32);
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
            b.FlatAppearance.BorderColor = Color.FromArgb(60, 255, 255, 255);
            b.FlatAppearance.BorderSize = 1;
            b.MouseEnter += (s, e) => b.BackColor = ControlPaint.Dark(bg, 0.1f);
            b.MouseLeave += (s, e) => b.BackColor = bg;
            return b;
        }
        private async void BtnApply_Click(object sender, EventArgs e)
        {
            btnApply.Text = "◌ Applying...";
            btnApply.BackColor = AppColor.AccentDim;
            btnApply.Enabled = false;
            await System.Threading.Tasks.Task.Delay(1200);
            btnApply.Text = "✓ Applied";
            btnApply.BackColor = Color.FromArgb(0, 150, 120);
            //if (lblStatus != null) lblStatus.Text = "● Profile applied";
            await System.Threading.Tasks.Task.Delay(2000);
            btnApply.Text = "▶ Apply";
            btnApply.BackColor = AppColor.Accent;
            btnApply.Enabled = true;
            //if (lblStatus != null) lblStatus.Text = "● Ready";
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

            pnlKeyLevels.Location = new Point(12, row2Y);
            pnlKeyLevels.Size = new Size(colW, 165);
            pnlSettings.Location = new Point(12 + colW + pad, row2Y);
            pnlSettings.Size = new Size(colW, 165);
            pnlSession.Location = new Point(12 + (colW + pad) * 2, row2Y);
            pnlSession.Size = new Size(colW, 165);

            lvLevels.Location = new Point(12, pnlKeyLevels.Bottom + pad);
            lvLevels.Size = new Size(W, 165);
        }
        private void Chart_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int W = pnlChart.Width, H = pnlChart.Height;
            if (W < 10 || H < 10) return;

            g.Clear(Color.FromArgb(13, 15, 20));

            using (var pen = new Pen(Color.FromArgb(12, 255, 255, 255), 0.5f))
            {
                for (int i = 1; i < 8; i++) g.DrawLine(pen, i * W / 8, 0, i * W / 8, H);
                for (int i = 1; i < 6; i++) g.DrawLine(pen, 0, i * H / 6, W, i * H / 6);
            }

            double priceMin = 1.0816, priceMax = 1.0878;
            double priceRange = priceMax - priceMin;
            int chartLeft = (int)(W * 0.02);
            int chartRight = (int)(W * 0.84);
            int vpLeft = chartRight + 4;

            int nBars = 55;
            float barW = (float)(chartRight - chartLeft) / nBars;
            double p = 1.0852;

            for (int i = 0; i < nBars; i++)
            {
                double o = p;
                double c = p + (rng.NextDouble() - 0.485) * 0.0006;
                double h = Math.Max(o, c) + rng.NextDouble() * 0.0003;
                double l = Math.Min(o, c) - rng.NextDouble() * 0.0003;
                p = c;

                float cx = chartLeft + i * barW + barW / 2f;
                float yH = PriceToY(h, priceMin, priceRange, H);
                float yL = PriceToY(l, priceMin, priceRange, H);
                float yO = PriceToY(o, priceMin, priceRange, H);
                float yC = PriceToY(c, priceMin, priceRange, H);
                bool bull = c >= o;
                Color col = bull ? AppColor.Accent : AppColor.Red;

                using (var pen = new Pen(col, 0.8f)) g.DrawLine(pen, cx, yH, cx, yL);
                float top = Math.Min(yO, yC);
                float bh = Math.Max(Math.Abs(yO - yC), 1.5f);
                using (var br = new SolidBrush(Color.FromArgb(180, col.R, col.G, col.B)))
                    g.FillRectangle(br, cx - barW * 0.35f, top, barW * 0.7f, bh);
            }

            int[] vols = { 22600, 28400, 33100, 41200, 47300, 52800, 68900, 52800, 47300, 41200, 33100, 28400, 22600 };
            float vpBarH = (float)H / vols.Length;
            int vpW = W - vpLeft - 2;

            for (int i = 0; i < vols.Length; i++)
            {
                float bw = (float)vols[i] / 68900 * vpW;
                float by = i * vpBarH;
                bool isPOC = vols[i] == 68900;
                using (var br = new SolidBrush(isPOC
                    ? Color.FromArgb(130, 240, 165, 0)
                    : Color.FromArgb(50, 0, 212, 170)))
                    g.FillRectangle(br, vpLeft, by + 1, bw, vpBarH - 2);
                if (splitMode && !isPOC)
                    using (var br = new SolidBrush(Color.FromArgb(40, 255, 77, 106)))
                        g.FillRectangle(br, vpLeft + bw * 0.6f, by + 1, bw * 0.4f, vpBarH - 2);
            }

            if (showPOC)
            {
                float pocY = PriceToY(1.08520, priceMin, priceRange, H);
                using (var pen = new Pen(Color.FromArgb(180, 240, 165, 0), 1f) { DashStyle = DashStyle.Dash })
                    g.DrawLine(pen, chartLeft, pocY, W, pocY);
                using (var f = new Font("Consolas", 7.5f))
                using (var br = new SolidBrush(AppColor.Gold))
                    g.DrawString("POC 1.08520", f, br, chartLeft + 4, pocY - 12);
            }

            if (showVA)
            {
                float vahY = PriceToY(1.08680, priceMin, priceRange, H);
                float valY = PriceToY(1.08340, priceMin, priceRange, H);
                using (var pen = new Pen(Color.FromArgb(120, 0, 212, 170), 0.8f) { DashStyle = DashStyle.Dot })
                {
                    g.DrawLine(pen, chartLeft, vahY, W, vahY);
                    g.DrawLine(pen, chartLeft, valY, W, valY);
                }
                using (var f = new Font("Consolas", 7.5f))
                {
                    using (var br = new SolidBrush(Color.FromArgb(160, 0, 212, 170)))
                        g.DrawString("VAH 1.08680", f, br, chartLeft + 4, vahY - 12);
                    using (var br = new SolidBrush(Color.FromArgb(160, 255, 77, 106)))
                        g.DrawString("VAL 1.08340", f, br, chartLeft + 4, valY - 12);
                }
            }

            float curY = PriceToY(livePrice, priceMin, priceRange, H);
            using (var pen = new Pen(Color.FromArgb(60, 255, 255, 255), 0.6f) { DashStyle = DashStyle.Dot })
                g.DrawLine(pen, chartLeft, curY, chartRight, curY);

            using (var f = new Font("Consolas", 9f, FontStyle.Bold))
            using (var br = new SolidBrush(AppColor.TextPrimary))
                g.DrawString($"EURUSD · {selectedTF}", f, br, chartLeft + 4, 6);
        }
        private Panel BuildKeyLevelsPanel()
        {
            var p = new Panel { BackColor = AppColor.BgCard };
            p.Paint += (s, e) => PaintCard(e.Graphics, p, "Key Levels");
            var rows = new[]
            {
                ("POC (Point of Control)", "1.08520", AppColor.Gold),
                ("VAH (Value Area High)",  "1.08680", AppColor.Accent),
                ("VAL (Value Area Low)",   "1.08340", AppColor.Red),
                ("Value Area %",           "70%",     AppColor.TextPrimary),
                ("Total Volume",           "184,320", AppColor.TextPrimary),
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

        // ── Settings Panel ────────────────────────────────────────
        private Panel BuildSettingsPanel()
        {
            var p = new Panel { BackColor = AppColor.BgCard };
            p.Paint += (s, e) => PaintCard(e.Graphics, p, "Settings");

            p.Controls.Add(new Label { Text = "Profile Type", Location = new Point(10, 37), Size = new Size(90, 16), ForeColor = AppColor.TextSecondary, Font = new Font("Segoe UI", 8f), BackColor = Color.Transparent });
            var cmb = new ComboBox { Location = new Point(105, 34), Size = new Size(105, 20), DropDownStyle = ComboBoxStyle.DropDownList, BackColor = AppColor.BgHover, ForeColor = AppColor.TextPrimary, FlatStyle = FlatStyle.Flat, Font = new Font("Consolas", 8f) };
            cmb.Items.AddRange(new object[] { "Session", "Fixed Range", "Visible Range", "Daily" });
            cmb.SelectedIndex = 0;
            cmb.SelectedIndexChanged += (s, e) => { profileType = cmb.SelectedItem.ToString(); pnlChart?.Invalidate(); };
            p.Controls.Add(cmb);

            chkPOC = new CheckBox { Text = "Show POC", Checked = true, Location = new Point(10, 72), ForeColor = AppColor.Accent, Font = new Font("Segoe UI", 9f), BackColor = Color.Transparent };
            chkPOC.CheckedChanged += (s, e) => { showPOC = chkPOC.Checked; pnlChart?.Invalidate(); };

            chkVA = new CheckBox { Text = "Show Value Area", Checked = true, Location = new Point(10, 98), ForeColor = AppColor.TextSecondary, Font = new Font("Segoe UI", 9f), BackColor = Color.Transparent };
            chkVA.CheckedChanged += (s, e) => { showVA = chkVA.Checked; pnlChart?.Invalidate(); };

            chkSplit = new CheckBox { Text = "Buy / Sell Split", Checked = false, Location = new Point(10, 124), ForeColor = AppColor.TextSecondary, Font = new Font("Segoe UI", 9f), BackColor = Color.Transparent };
            chkSplit.CheckedChanged += (s, e) => { splitMode = chkSplit.Checked; pnlChart?.Invalidate(); };

            p.Controls.AddRange(new Control[] { chkPOC, chkVA, chkSplit });
            return p;
        }

        // ── Session Panel ─────────────────────────────────────────
        private Panel BuildSessionPanel()
        {
            var p = new Panel { BackColor = AppColor.BgCard };
            p.Paint += (s, e) => PaintCard(e.Graphics, p, "Session Stats");
            var rows = new[]
            {
                ("Session",       "London",  AppColor.Blue),
                ("High",          "1.08742", AppColor.Accent),
                ("Low",           "1.08201", AppColor.Red),
                ("Range",         "0.00541", AppColor.TextPrimary),
                ("Dominant Side", "Buy ▲",   AppColor.Accent),
            };
            int y = 34;
            foreach (var (lbl, val, col) in rows)
            {
                p.Controls.Add(new Label { Text = lbl, Location = new Point(10, y), Size = new Size(110, 16), ForeColor = AppColor.TextSecondary, Font = new Font("Segoe UI", 8f), BackColor = Color.Transparent });
                p.Controls.Add(new Label { Text = val, Location = new Point(125, y), Size = new Size(80, 16), ForeColor = col, Font = new Font("Consolas", 8.5f, FontStyle.Bold), TextAlign = ContentAlignment.MiddleRight, BackColor = Color.Transparent });
                y += 22;
            }
            return p;
        }
        private ListView BuildLevelsTable()
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
            lv.Columns.Add("Price", 80);
            lv.Columns.Add("Volume", 80);
            lv.Columns.Add("Buy %", 60);
            lv.Columns.Add("Sell %", 60);
            lv.Columns.Add("Type", 55);
            lv.Columns.Add("Distribution", 200);

            foreach (var lvl in vpLevels)
            {
                var item = new ListViewItem(lvl.Price.ToString("F5")) { Tag = lvl };
                item.SubItems.Add(lvl.Volume.ToString("N0"));
                item.SubItems.Add(lvl.BuyPct + "%");
                item.SubItems.Add((100 - lvl.BuyPct) + "%");
                item.SubItems.Add(lvl.Type);
                item.SubItems.Add("");
                lv.Items.Add(item);
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

            lv.DrawSubItem += (s, e) =>
            {
                if (!(e.Item.Tag is VPLevel lvl)) return;
                var g = e.Graphics;
                var rc = e.Bounds;

                Color fg = AppColor.TextPrimary;
                switch (e.ColumnIndex)
                {
                    case 0: fg = lvl.Type == "POC" ? AppColor.Gold : lvl.Type == "VAH" ? AppColor.Accent : lvl.Type == "VAL" ? AppColor.Red : AppColor.TextPrimary; break;
                    case 2: fg = AppColor.Accent; break;
                    case 3: fg = AppColor.Red; break;
                    case 4: fg = lvl.Type == "POC" ? AppColor.Gold : lvl.Type == "VAH" ? AppColor.Accent : lvl.Type == "VAL" ? AppColor.Red : AppColor.TextMuted; break;
                }

                if (e.ColumnIndex == 5)
                {
                    float pct = (float)lvl.Volume / 68900;
                    int bW = (int)(rc.Width * 0.85f);
                    int buyW = (int)(bW * pct * lvl.BuyPct / 100f);
                    int selW = (int)(bW * pct * (100 - lvl.BuyPct) / 100f);
                    int barY = rc.Y + rc.Height / 2 - 3;
                    g.FillRectangle(new SolidBrush(AppColor.BgHover), rc.X + 4, barY, bW, 6);
                    g.FillRectangle(new SolidBrush(Color.FromArgb(160, 0, 212, 170)), rc.X + 4, barY, buyW, 6);
                    g.FillRectangle(new SolidBrush(Color.FromArgb(160, 255, 77, 106)), rc.X + 4 + buyW, barY, selW, 6);
                }
                else
                {
                    using (var f = new Font("Consolas", 8.5f))
                    using (var br = new SolidBrush(fg))
                        g.DrawString(e.SubItem.Text, f, br, rc.X + 4, rc.Y + 4);
                }
                using (var pen = new Pen(Color.FromArgb(8, 255, 255, 255)))
                    g.DrawLine(pen, rc.Left, rc.Bottom - 1, rc.Right, rc.Bottom - 1);
            };

            return lv;
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
        private static float PriceToY(double price, double min, double range, int H)
            => (float)(H - ((price - min) / range) * H);
    }
}
