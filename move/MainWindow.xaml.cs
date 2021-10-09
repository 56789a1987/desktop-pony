using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Interop;

namespace move
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private NotifyIcon notifyIcon;
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Resource));
        private double wx, wy;
		readonly double ww = SystemParameters.PrimaryScreenWidth;
        readonly double wh = SystemParameters.PrimaryScreenHeight;
        private bool dragging = false, key = true, mouse = false;
        private bool hidden = false, top = true, drag = false;
        private double vy = 0;
        public double upSpeed = 10;
        public double xSpeed = 5;
        public double g = 0.5;
        public double mouseSpeed = 0.015;
        private string fn;
        private SettingsWindow settingsWin = null;
        public MainWindow()
        {
            InitializeComponent();
            scaleTag.CenterX = 53;
            wx = ww / 2;
            wy = wh / 2;

            Topmost = true;

            DispatcherTimer mainTimer = new DispatcherTimer();
            mainTimer.Tick += new EventHandler(timer_Tick);
            mainTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / 60);
            mainTimer.Start();

            Thread idleAnim = new Thread(new ThreadStart(idleAnimThread));
            idleAnim.Start();
            InitialTray();
        }

        int idleimg = 0;
        private void idleAnimThread()
        {
            while (!hidden)
            {
                Random rand = new Random();
                int m = rand.Next(0, 4);
                if (m == 0)
                {
                    idleimg = 1;
                    Thread.Sleep(80);
                    idleimg = 2;
                    Thread.Sleep(80);
                    idleimg = 3;
                    Thread.Sleep(80);
                    idleimg = 4;
                    Thread.Sleep(80);
                    idleimg = 5;
                    Thread.Sleep(1000);
                    idleimg = 6;
                    Thread.Sleep(80);
                    idleimg = 7;
                    Thread.Sleep(80);
                    idleimg = 0;
                }
                else if (m == 1)
                {
                    idleimg = 11;
                    Thread.Sleep(40);
                    idleimg = 12;
                    Thread.Sleep(80);
                    idleimg = 13;
                    Thread.Sleep(80);
                    idleimg = 14;
                    Thread.Sleep(80);
                    idleimg = 15;
                    Thread.Sleep(80);
                    idleimg = 16;
                    Thread.Sleep(3000);
                    idleimg = 17;
                    Thread.Sleep(80);
                    idleimg = 18;
                    Thread.Sleep(80);
                    idleimg = 0;
                }
                else
                {
                    idleimg = 21;
                    Thread.Sleep(100);
                    idleimg = 22;
                    Thread.Sleep(100);
                    idleimg = 0;
                }
                Thread.Sleep(rand.Next(1000, 5000));
            }
        }

        private bool GetKeyState(Keys keys)
        {
            return (Win32.GetKeyState((int)keys) & 0x8000) != 0;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                dragging = true;
                DragMove();
                wx = Left + Width / 2;
                wy = Top + Height / 2;
                dragging = false;
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (dragging || hidden)
                return;

            int animation;
            double vx = 0;

            if (!mouse)
			{
                bool drop = false;
                vy += g;

                if (key)
                {
                    if (GetKeyState(Keys.Left))
                    {
                        vx = -xSpeed;
                    }
                    if (GetKeyState(Keys.Right))
                    {
                        vx = xSpeed;
                    }
                    if (GetKeyState(Keys.Up))
                    {
                        vy = -upSpeed;
                    }
                    if (GetKeyState(Keys.Down))
                    {
                        drop = true;
                    }
                }

				Win32.POINT pos = new Win32.POINT
				{
					X = (int)(Left + Width / 2),
					Y = (int)(Top + Height + vy + 1)
				};

				IntPtr hwnd = Win32.WindowFromPoint(pos);
                Win32.RECT rekt = new Win32.RECT();
                Win32.GetWindowRect(hwnd, ref rekt);

                if (vx != 0)
                    animation = 1;
                else
                    animation = 0;

                if (Top + Height < rekt.Top && vy > 0 && !drop)
                {
                    wy = rekt.Top - Height / 2 - 1;
                    vy = 0;
                }
                else if (wy < wh - Height / 2)
                {
                    animation = 2;
                }

                if (vy > 20)
                    vy = 20;
            }
            else
			{
                animation = 2;

				Win32.GetCursorPos(out Win32.POINT mp);

				if (Left + Width / 2 - mp.X < 5 && Left + Width / 2 - mp.X > -5)
                    vx = 0;
                else
                    vx = -(Left + Width / 2 - mp.X) * mouseSpeed;

                if (Top + Height / 2 - mp.Y < 5 && Top + Height / 2 - mp.Y > -5)
                    vy = 0;
                else
                    vy = -(Top + Height / 2 - mp.Y) * mouseSpeed;
            }

            if (vx > 0)
                scaleTag.ScaleX = 1;
            else if (vx < 0)
                scaleTag.ScaleX = -1;

            wx += vx;
            wy += vy;

            if (wx < 0)
                wx = 0;

            if (wy < -Height / 2 + 5)
            {
                wy = -Height / 2 + 5;
                vy = 0;
            }

            if (wx > ww)
                wx = ww;

            if (wy > wh - Height / 2)
            {
                wy = wh - Height / 2;
                vy = 0;
            }

            if (animation == 2)
            {
                long aimg = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond / 30) % 15;
                fn = "fly_" + string.Format("{0:00}", aimg) + ".png";
            }
            else if (animation == 1)
            {
                long aimg = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond / 30) % 15;
                fn = "walk_" + string.Format("{0:00}", aimg) + ".png";
            }
            else
            {
                fn = "idle_" + string.Format("{0:00}", idleimg) + ".png";
            }

            string imagePath = "pack://application:,,,/Images/" + fn;
            MainImg.Source = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));

            Left = wx - Width / 2;
            Top = wy - Height / 2;
        }

        private void MainWin_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void InitialTray()
        {
			notifyIcon = new NotifyIcon
			{
				Text = "NotifyIcon",
				Icon = (System.Drawing.Icon)resources.GetObject("exe"),
				Visible = true
			};
			notifyIcon.Click += notify_Click;
            InitialMenu();
        }

        private void InitialMenu()
        {
            MenuItem separator = new MenuItem("-");

            MenuItem kbdMenuItem = new MenuItem("Enable &Keyboard Control");
            kbdMenuItem.Checked = key;
            kbdMenuItem.Click += new EventHandler(keyControl_Click);

            MenuItem mouseMenuItem = new MenuItem("Follow &Mouse Pointer");
            mouseMenuItem.Checked = mouse;
            mouseMenuItem.Click += new EventHandler(mouse_Click);

            MenuItem speedMenuItem = new MenuItem("&Speed settings");
            speedMenuItem.Click += new EventHandler(speed_Click);

            MenuItem dragMenuItem = new MenuItem("&Draggable");
            dragMenuItem.Checked = drag;
            dragMenuItem.Click += new EventHandler(drag_Click);

            MenuItem topMenuItem = new MenuItem("&Top Most");
            topMenuItem.Checked = top;
            topMenuItem.Click += new EventHandler(top_Click);

            MenuItem hideMenuItem = new MenuItem("&Hide");
            hideMenuItem.Checked = hidden;
            hideMenuItem.Click += new EventHandler(hide_Click);

            MenuItem exitMenuItem = new MenuItem("E&xit");
            exitMenuItem.Click += new EventHandler(exit_Click);

            MenuItem[] childen = new MenuItem[]
            {
                kbdMenuItem, mouseMenuItem, separator, speedMenuItem, dragMenuItem, topMenuItem, hideMenuItem, exitMenuItem
            };
            notifyIcon.ContextMenu = new ContextMenu(childen);
        }

        private void notify_Click(object sender, EventArgs e)
        {
            Activate();
        }

        private void top_Click(object sender, EventArgs e)
        {
            top = !top;
            Topmost = top;
            InitialMenu();
        }

        private void mouse_Click(object sender, EventArgs e)
        {
            mouse = !mouse;
            key = false;
            InitialMenu();
        }

        private void keyControl_Click(object sender, EventArgs e)
        {
            key = !key;
            mouse = false;
            InitialMenu();
        }

        private void drag_Click(object sender, EventArgs e)
        {
            drag = !drag;
            updateClickThrough();
            InitialMenu();
        }

		private void MainWin_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
            if (IsVisible)
			{
                updateClickThrough();
			}
		}

		private void updateClickThrough()
		{
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            uint extendedStyle = Win32.GetWindowLong(hwnd, Win32.GWL_EXSTYLE);
            if (drag)
            {
                Win32.SetWindowLong(hwnd, Win32.GWL_EXSTYLE, extendedStyle & ~Win32.WS_EX_TRANSPARENT);
            }
            else
            {
                Win32.SetWindowLong(hwnd, Win32.GWL_EXSTYLE, extendedStyle | Win32.WS_EX_TRANSPARENT);
            }
        }

        private void hide_Click(object sender, EventArgs e)
        {
            hidden = !hidden;
            if (hidden)
                Hide();
            else
                Show();
            InitialMenu();
        }

        private void speed_Click(object sender, EventArgs e)
		{
            if (settingsWin == null)
            {
                settingsWin = new SettingsWindow(this);
                settingsWin.Show();
                settingsWin.Closed += settings_Closed;
            }
            else
			{
                settingsWin.WindowState = WindowState.Normal;
                settingsWin.Activate();
			}
		}

        private void exit_Click(object sender, EventArgs e)
        {
            notifyIcon.Dispose();
            Close();
        }
        private void settings_Closed(object sender, EventArgs e)
		{
            settingsWin = null;
		}
    }
}
