using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FakeCaYaOS
{
    public partial class Form1 : Form
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left, top, right, bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct APPBARDATA
        {
            public int cbSize;
            public IntPtr hWnd;
            public uint uCallbackMessage;
            public uint uEdge;
            public RECT rc;
            public int lParam;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern uint SHAppBarMessage(uint dwMessage, ref APPBARDATA pData);

        const uint ABM_GETTASKBARPOS = 5;

        [DllImport("user32.dll")]
        private static extern int FindWindow(string className, string windowText);

        [DllImport("user32.dll")]
        private static extern int ShowWindow(int hwnd, int command);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
        const int SW_RESTORE = 9;

        private List<Process> runningProcesses = new List<Process>();
        private FlowLayoutPanel taskIconsPanel;
        private Dictionary<int, PictureBox> taskIcons = new Dictionary<int, PictureBox>();



        public Form1()
        {
            InitializeComponent();


        }

        private void Form1_Load(object sender, EventArgs e)
        {
            HideTaskbar();


            int taskbarHeight = GetTaskbarHeight();
            AdjustFormSizeAndPosition(taskbarHeight);
            CreateFakeTaskbar(taskbarHeight);

            //LoadCurrentProcesses(); // Uygulama yüklendiðinde mevcut süreçleri al

            // Görev simgelerini güncelle
            UpdateRunningTasks();


            //LoadCurrentProcesses(); // Uygulama yüklendiðinde mevcut süreçleri al

            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer
            {
                Interval = 100
            };
            timer.Tick += (s, e) =>
            {
                UpdateRunningTasks();
            };
            timer.Start();

            //this.MouseEnter += (s, e) => this.TopMost = true; //this.Focus(); // Fare formdan çýkýnca odaklayabilirsin, istersen.
            //this.MouseLeave += (s, e) => this.TopMost = false;    //{ /* Hiçbir þey yapma */ };

            TopMost = true;
            //TopMost = true;
            TopMost = false;
        }

        private int GetTaskbarHeight()
        {
            APPBARDATA abd = new APPBARDATA();
            abd.cbSize = Marshal.SizeOf(abd);

            uint result = SHAppBarMessage(ABM_GETTASKBARPOS, ref abd);

            if (result != 0)
            {
                return abd.rc.bottom - abd.rc.top;
            }
            return 40; // Varsayýlan bir yükseklik
        }

        private List<string> excludedWindowTitles = new List<string>
        {
            "Windows Giriþ Deneyimi",
            "NVIDIA GeForce Overlay",
            "",
            // Daha fazla özel pencere adý ekleyebilirsiniz
        };

        private void LoadCurrentProcesses()
        {


            runningProcesses.Clear();
            //var currentProcesses = Process.GetProcesses()
            //    .Where(p => p.MainWindowHandle != IntPtr.Zero && p.Responding && !string.IsNullOrEmpty(p.MainWindowTitle))
            //    .ToList();

            //List<Process> currentProcesses = new List<Process>(Process.GetProcesses());
            List<Process> newProcesses = new List<Process>();
            List<int> endedProcessIds = new List<int>();

            List<Process> currentProcesses = Process.GetProcesses()
           .Where(p => p.MainWindowHandle != IntPtr.Zero && p.Responding && !string.IsNullOrEmpty(p.MainWindowTitle))
           .ToList();

            foreach (var process in currentProcesses)
            {
                // Pencereyi kontrol et ve özel pencere adlarýný hariç tut
                if (IsWindowVisible(process.MainWindowHandle) &&
                    !excludedWindowTitles.Contains(process.MainWindowTitle) &&
                    !runningProcesses.Any(p => p.Id == process.Id)) // runningProcesses'ta mevcut mu kontrol et
                {
                    runningProcesses.Add(process);
                    PictureBox taskIcon = CreateTaskIcon(process);
                    taskIcons[process.Id] = taskIcon;
                    taskIconsPanel.Controls.Add(taskIcon);
                }
            }
        }







        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowVisible(IntPtr hWnd);





        private void CreateFakeTaskbar(int height)
        {
            // Görev çubuðu paneli
            Panel taskBar = new Panel
            {
                Height = height,
                Dock = DockStyle.Bottom,
                BackColor = Color.FromArgb(45, 45, 45)  // Koyu gri-siyah
            };
            this.Controls.Add(taskBar);

            Icon windowsIcon = Properties.Resources.Windows2;
            Icon searchIcon = Properties.Resources.Search2;

            // Windows düðmesi
            PictureBox pictureBox1 = new PictureBox
            {
                Image = Properties.Resources.Windows, //windowsIcon.ToBitmap(),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Width = 40,
                Height = 40,
                Left = 5,
                BackColor = Color.Transparent
            };
            pictureBox1.MouseHover += (s, e) => ShowToolTip(pictureBox1, "Baþlat Menüsü");
            pictureBox1.Click += (s, e) => SimulateWindowsKey();
            taskBar.Controls.Add(pictureBox1);

            // Arama düðmesi
            PictureBox pictureBox2 = new PictureBox
            {
                Image = Properties.Resources.Search, //searchIcon.ToBitmap(),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Width = 40,
                Height = 40,
                Left = pictureBox1.Right + 5,
                BackColor = Color.Transparent
            };
            pictureBox2.MouseHover += (s, e) => ShowToolTip(pictureBox2, "Arama");
            pictureBox2.Click += (s, e) => SimulateSearch();
            taskBar.Controls.Add(pictureBox2);

            // Ayýrýcý çizgi
            Panel separator = new Panel
            {
                Width = 2, // Çizgi kalýnlýðý
                Height = height - 10, // Görev çubuðu yüksekliði ile uyumlu
                BackColor = Color.White,
                Left = pictureBox2.Right + 5,
                Top = 5 // Üstten biraz boþluk býrak
            };
            taskBar.Controls.Add(separator);



            // Saat göstergesi
            Label clock = new Label
            {
                Text = DateTime.Now.ToString("HH:mm:ss"),
                Font = new Font("CaYa Font", (int)(height * 0.3), FontStyle.Bold),
                Dock = DockStyle.Right,
                AutoSize = true, // Otomatik boyutlandýrma
                Padding = new Padding(0, 10, 10, 10),
                ForeColor = Color.White, // Saatin rengi beyaz
                BackColor = Color.Transparent // Þeffaf arka plan
            };

            // Saat üzerine gelindiðinde tooltip göster
            //clock.MouseHover += (s, e) => ShowToolTip(clock, "Tarih ve Bildirimler (Windows + N)");

            // Saat týklanýrsa Windows + N simülasyonu yap
            clock.Click += (s, e) => SimulateWindowsN();

            // Görev çubuðuna saati ekle
            taskBar.Controls.Add(clock);




            // Windows + A simüle eden düðme (Saatin soluna ekleniyor)
            PictureBox windowsANotificationBox = new PictureBox
            {
                Image = Properties.Resources.Menu, // Bu picturebox için bir simge seçin
                SizeMode = PictureBoxSizeMode.StretchImage,
                Width = 40,
                Height = 40,
                Left = clock.Left - 65, // Saatin soluna ekliyoruz
                BackColor = Color.Transparent
            };
            //windowsANotificationBox.MouseHover += (s, e) => ShowToolTip(windowsANotificationBox, "Kontrol Paneli (Windows + A)");
            windowsANotificationBox.Click += (s, e) => SimulateWindowsA();
            taskBar.Controls.Add(windowsANotificationBox);


            // Saatin her saniye güncellenmesi
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer
            {
                Interval = 1000
            };
            timer.Tick += (s, e) =>
            {
                clock.Text = DateTime.Now.ToString("HH:mm:ss");

                //UpdateRunningTasks();
            };
            timer.Start();

            // Çalýþan görev simgeleri
            taskIconsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.None, // Paneli docklamadan ayarlýyoruz
                Left = separator.Right + 10, // Ayýrýcý çizginin saðýnda baþlasýn
                Width = taskBar.Width - (separator.Right + 15), // Geniþliði ayarlayýn
                AutoScroll = true,
                BackColor = Color.FromArgb(45, 45, 45) // Arka plan rengi
            };
            taskBar.Controls.Add(taskIconsPanel);

            // Panelin sýnýr rengini belirleme
            taskBar.BorderStyle = BorderStyle.FixedSingle;

            // En sað tarafa çizgi ekleme
            Panel rightMostSeparator = new Panel
            {
                Width = 10, // Çizgi kalýnlýðý (Mouse týklamasý için geniþlik veriyoruz)
                Height = height - 10, // Görev çubuðu yüksekliði ile uyumlu
                BackColor = Color.White,
                Dock = DockStyle.Right, // En sað tarafa dockla
                Cursor = Cursors.Hand // Týklanabilir olduðunu belirtmek için el ikonu
            };
            rightMostSeparator.Click += (s, e) => SimulateWinD(); // Týklanýldýðýnda Win + D simülasyonu
            taskBar.Controls.Add(rightMostSeparator);
        }











        private void UpdateRunningTasks()
        {
            //List<Process> currentProcesses = new List<Process>(Process.GetProcesses());
            List<Process> newProcesses = new List<Process>();
            List<int> endedProcessIds = new List<int>();

            List<Process> currentProcesses = Process.GetProcesses()
           .Where(p => p.MainWindowHandle != IntPtr.Zero && p.Responding && !string.IsNullOrEmpty(p.MainWindowTitle))
           .ToList();


            // Yeni süreçleri ve kapatýlan süreçleri belirleme
            //foreach (var process in currentProcesses)
            //{
            //    if (process.MainWindowHandle != IntPtr.Zero && process.Responding &&
            //        !string.IsNullOrEmpty(process.MainWindowTitle))
            //    {
            //        if (!runningProcesses.Exists(p => p.Id == process.Id))
            //        {
            //            newProcesses.Add(process); // Yeni bir süreç
            //        }
            //    }
            //}
            //LoadCurrentProcesses();
            //foreach (var process in currentProcesses)
            //{
            //    if (!runningProcesses.Exists(p => p.Id == process.Id))
            //    {
            //        if (IsWindowVisible(process.MainWindowHandle) && !excludedWindowTitles.Contains(process.MainWindowTitle))
            //        {
            //            newProcesses.Add(process); // Yeni bir süreç
            //        }

            //    }

            //    if (process.MainWindowHandle != IntPtr.Zero && process.Responding &&
            //            !string.IsNullOrEmpty(process.MainWindowTitle))
            //    {

            //    }
            //    // Pencereyi kontrol et ve özel pencere adlarýný hariç tut
            //    //if (IsWindowVisible(process.MainWindowHandle) && !excludedWindowTitles.Contains(process.MainWindowTitle))
            //    //{
            //    //    if (process.MainWindowHandle != IntPtr.Zero && process.Responding &&
            //    //        !string.IsNullOrEmpty(process.MainWindowTitle))
            //    //    {
            //    //        if (!runningProcesses.Exists(p => p.Id == process.Id))
            //    //        {
            //    //            newProcesses.Add(process); // Yeni bir süreç
            //    //        }
            //    //    }

            //    //    //runningProcesses.Add(process);
            //    //    //PictureBox taskIcon = CreateTaskIcon(process);
            //    //    //taskIcons[process.Id] = taskIcon;
            //    //    //taskIconsPanel.Controls.Add(taskIcon);
            //    //}

            //}


            // Yeni süreçleri ekle
            foreach (var process in currentProcesses)
            {
                if (IsWindowVisible(process.MainWindowHandle) && !excludedWindowTitles.Contains(process.MainWindowTitle))
                {
                    if (!runningProcesses.Any(p => p.Id == process.Id))
                    {
                        runningProcesses.Add(process);
                        PictureBox taskIcon = CreateTaskIcon(process);
                        taskIcons[process.Id] = taskIcon;
                        taskIconsPanel.Controls.Add(taskIcon);
                    }

                    //taskIcons[process.Id].MouseHover -= (s, e) => ShowToolTip(taskIcons[process.Id], process.MainWindowTitle);
                    //taskIcons[process.Id].MouseHover += (s, e) => ShowToolTip(taskIcons[process.Id], process.MainWindowTitle);
                }
            }

            // Sonlanan süreçleri kaldýr
            foreach (var process in runningProcesses.ToList())
            {
                if (!currentProcesses.Any(p => p.Id == process.Id))
                {
                    runningProcesses.Remove(process);
                    if (taskIcons.TryGetValue(process.Id, out PictureBox taskIcon))
                    {
                        taskIconsPanel.Controls.Remove(taskIcon);
                        taskIcons.Remove(process.Id);
                    }
                }
            }



            foreach (var process in runningProcesses)
            {
                if (!currentProcesses.Exists(p => p.Id == process.Id))
                {
                    endedProcessIds.Add(process.Id); // Sonlanan bir süreç
                }
            }

            // Yeni süreçleri ekleme
            foreach (var process in newProcesses)
            {
                runningProcesses.Add(process);
                PictureBox taskIcon = CreateTaskIcon(process);
                taskIcons[process.Id] = taskIcon;
                taskIconsPanel.Controls.Add(taskIcon);
            }

            // Sonlanan süreçleri kaldýrma
            foreach (var processId in endedProcessIds)
            {
                var process = runningProcesses.FirstOrDefault(p => p.Id == processId);
                if (process != null)
                {
                    runningProcesses.Remove(process);

                    if (taskIcons.TryGetValue(process.Id, out PictureBox taskIcon))
                    {
                        taskIconsPanel.Controls.Remove(taskIcon);
                        taskIcons.Remove(process.Id);
                    }
                }
            }






        }

        private ToolTip toolTip = new ToolTip();

        private PictureBox CreateTaskIcon(Process process)
        {
            PictureBox taskIcon = new PictureBox
            {
                Width = 40,
                Height = 40
            };

            try
            {
                Icon appIcon = Icon.ExtractAssociatedIcon(process.MainModule.FileName);
                taskIcon.Image = appIcon.ToBitmap();
            }
            catch
            {
                taskIcon.Image = Properties.Resources.Discord_Logo_New; // Varsayýlan ikon
            }

            taskIcon.Click += (sender, e) => HandleTaskIconClick(process, e as MouseEventArgs);
            taskIcon.MouseDown += (sender, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    ShowTaskContextMenu(process);
                }
            };

            // Güncel pencere adýný gösterme
            taskIcon.MouseHover += (s, e) =>
            {
                // Önceki tooltip'i kapat
                toolTip.Hide(taskIcon);

                // Güncel baþlýðý al
                string windowTitle = Process.GetProcessById(process.Id).MainWindowTitle;
                toolTip.Show(windowTitle, taskIcon, taskIcon.Width / 2, taskIcon.Height, 2000); // 2000 ms göster
            };

            return taskIcon;
        }


        private void HandleTaskIconClick(Process process, MouseEventArgs e)
        {
            IntPtr hWnd = process.MainWindowHandle;

            if (e.Button == MouseButtons.Left)
            {
                if (hWnd == GetForegroundWindow())
                {
                    BringTaskToFront(hWnd);
                }
                else
                {
                    BringTaskToFront(hWnd);
                    SetForegroundWindow(hWnd);
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                ShowTaskContextMenu(process);
            }
        }

        private void ShowTaskContextMenu(Process process)
        {
            ContextMenuStrip contextMenu = new ContextMenuStrip();

            ToolStripMenuItem closeMenuItem = new ToolStripMenuItem("Görevi Sonlandýr");
            closeMenuItem.Click += (s, e) => process.Kill();
            contextMenu.Items.Add(closeMenuItem);

            contextMenu.Show(this, Cursor.Position);
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        private void BringTaskToFront(IntPtr hWnd)
        {
            if (IsIconic(hWnd))
            {
                ShowWindow(hWnd, SW_RESTORE);
            }
            SetForegroundWindow(hWnd);
        }

        private const int KEYEVENTF_KEYUP = 0x0002;

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        private void SimulateWindowsKey()
        {
            const byte VK_LWIN = 0x5B; // Windows tuþu


            // Windows tuþuna bas
            keybd_event(VK_LWIN, 0, 0, UIntPtr.Zero);

            // Windows tuþunu býrak
            keybd_event(VK_LWIN, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);

        }


        private void SimulateSearch()
        {
            const byte VK_LWIN = 0x5B; // Windows tuþu
            const byte VK_S = 0x53; // S tuþu

            // Windows tuþuna bas
            keybd_event(VK_LWIN, 0, 0, UIntPtr.Zero);
            //System.Threading.Thread.Sleep(100); // Küçük bir gecikme ekleyin
            keybd_event(VK_S, 0, 0, UIntPtr.Zero);
            keybd_event(VK_S, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            keybd_event(VK_LWIN, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        }

        // Win + D iþlemini simüle eden metot
        private void SimulateWinD()
        {
            // Win + D tuþlarýný simüle et
            //SendKeys.SendWait("^%{ESC}"); // Bu yöntem bazen deðiþiklik gösterebilir
            //MessageBox.Show("A");

            const byte VK_LWIN = 0x5B; // Sol Windows tuþu
            const byte VK_D = 0x44;    // D tuþu
            const uint KEYEVENTF_KEYUP = 0x0002; // Tuþ býrakma olayýný temsil eder

            // Windows tuþuna bas
            keybd_event(VK_LWIN, 0, 0, UIntPtr.Zero);

            // D tuþuna bas
            keybd_event(VK_D, 0, 0, UIntPtr.Zero);

            // D tuþunu býrak
            keybd_event(VK_D, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);

            // Windows tuþunu býrak
            keybd_event(VK_LWIN, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);

        }

        private void SimulateWindowsA()
        {
            const byte VK_LWIN = 0x5B; // Sol Windows tuþu
            const byte VK_A = 0x41; // A tuþu
            // Windows + A tuþ kombinasyonu simülasyonu
            keybd_event(VK_LWIN, 0, 0, UIntPtr.Zero); // Windows tuþuna bas
            keybd_event(VK_A, 0, 0, UIntPtr.Zero);    // A tuþuna bas
            keybd_event(VK_A, 0, KEYEVENTF_KEYUP, UIntPtr.Zero); // A tuþunu býrak
            keybd_event(VK_LWIN, 0, KEYEVENTF_KEYUP, UIntPtr.Zero); // Windows tuþunu býrak
        }

        private void SimulateWindowsN()
        {
            const byte VK_LWIN = 0x5B; // Sol Windows tuþu
            const byte VK_N = 0x4E; // N tuþu
                                    // Windows + N tuþ kombinasyonu simülasyonu
            keybd_event(VK_LWIN, 0, 0, UIntPtr.Zero); // Windows tuþuna bas
            keybd_event(VK_N, 0, 0, UIntPtr.Zero);    // N tuþuna bas
            keybd_event(VK_N, 0, KEYEVENTF_KEYUP, UIntPtr.Zero); // N tuþunu býrak
            keybd_event(VK_LWIN, 0, KEYEVENTF_KEYUP, UIntPtr.Zero); // Windows tuþunu býrak
        }


        private void ShowToolTip(Control control, string text)
        {
            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(control, text);
        }

        private void HideTaskbar()
        {
            int taskbarHandle = FindWindow("Shell_TrayWnd", "");
            ShowWindow(taskbarHandle, SW_HIDE);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Görev çubuðu görünümünü geri getir
            int taskbarHandle = FindWindow("Shell_TrayWnd", "");
            ShowWindow(taskbarHandle, SW_SHOW);
        }

        private void AdjustFormSizeAndPosition(int taskbarHeight)
        {
            var screenBounds = Screen.PrimaryScreen.Bounds;
            this.Width = screenBounds.Width;
            this.Height = taskbarHeight;
            this.Location = new Point(0, screenBounds.Height - taskbarHeight);
        }

        private void Form1_MouseEnter(object sender, EventArgs e)
        {
            this.Focus();
            this.TopMost = true;
            MessageBox.Show("LOGGED");
        }

        private void Form1_MouseLeave(object sender, EventArgs e)
        {
            this.TopMost = false;
            MessageBox.Show("LEAVED");
        }
    }
}
