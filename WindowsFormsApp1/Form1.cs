using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private TitlebarManager titlebarManager;
        private SidebarManager sidebarManager;
        private StatusBarManager statusBarManager;
        private MainpageManger mainpageManger;
        public Form1()
        {
            //InitializeComponent();
            this.Text = "AutoScripts · Volume Profile MT5";
            this.Size = new Size(1100, 700);
            this.MinimumSize = new Size(900, 600);
            this.BackColor = AppColor.BgDeep;
            this.ForeColor = AppColor.TextPrimary;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9f);
            this.DoubleBuffered = true;
            this.FormBorderStyle = FormBorderStyle.None;

            titlebarManager = new TitlebarManager(this);
            sidebarManager = new SidebarManager(this);
            statusBarManager = new StatusBarManager(this);
            mainpageManger = new MainpageManger(this);

            BuildUI();
        }
        private void BuildUI()
        {
            titlebarManager.BuildTitlebar();
            sidebarManager.BuildSidebar();
            mainpageManger.BuildMain();
            statusBarManager.BuildStatusBar();
            this.Resize += (s, e) => RelayoutMain();
            RelayoutMain();

        }
        private void RelayoutMain()
        {
            int top = titlebarManager.pnlTitleBar?.Height ?? 46;
            int bot = statusBarManager.pnlStatusBar?.Height ?? 28;

            if (sidebarManager.pnlSidebar != null)
            {
                sidebarManager.pnlSidebar.Location = new Point(0, top);
                sidebarManager.pnlSidebar.Size = new Size(200, this.ClientSize.Height - top - bot);
            }
            if (mainpageManger.pnlMain != null)
            {
                mainpageManger.pnlMain.Location = new Point(200, top);
                mainpageManger.pnlMain.Size = new Size(this.ClientSize.Width - 200, this.ClientSize.Height - top - bot);
            }
            if (mainpageManger.pnlContent != null && mainpageManger.pnlHeader != null)
            {
                mainpageManger.pnlContent.Location = new Point(0, mainpageManger.pnlHeader.Height);
                mainpageManger.pnlContent.Size = new Size(mainpageManger.pnlMain.Width, mainpageManger.pnlMain.Height - mainpageManger.pnlHeader.Height);
            }
            mainpageManger.LayoutContent();
        }
    }
    internal static class NativeMethods
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
    }
}
