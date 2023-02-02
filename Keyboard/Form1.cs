using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Globalization;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;

namespace Keyboard
{
    public partial class Form1 : Form
    {

        System.Timers.Timer _timer;

        Appconfig _config;

        ILogger _logger;

        KeyboardManager keyboardManager;

        private readonly HttpClient _client;

        private bool _realyNeedClose= false;

        public System.Timers.Timer Timer
        {
            [System.Diagnostics.DebuggerNonUserCode]
            get
            {
                return _timer;
            }
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized), System.Diagnostics.DebuggerNonUserCode]
            set
            {
                if (_timer != null)
                {
                    _timer.Elapsed -= DoWork_Tick;
                }

                _timer = value;

                if (_timer != null)
                {
                    _timer.Elapsed += DoWork_Tick;
                }
            }
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetKeyboardLayout(uint idThread);
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr processId);

        int countTimer = 0;
        int countTimerMax = 0;

        public Form1()
        {
            InitializeComponent();

            StelSeriesEngineConfig ssConfig = new StelSeriesEngineConfig();

            using (StreamReader file = File.OpenText(@"C:\ProgramData\SteelSeries\SteelSeries Engine 3\coreProps.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                ssConfig = (StelSeriesEngineConfig)serializer.Deserialize(file, typeof(StelSeriesEngineConfig));
            }

            _client = new HttpClient();
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Connection", "keep-alive");

            _client.BaseAddress = new Uri("http://" + ssConfig.address);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            try
            {
                var procArray = System.Diagnostics.Process.GetProcessesByName(System.Diagnostics.Process.GetCurrentProcess().ProcessName);
                if (procArray.Length > 1)
                {
                    this.Close();
                }
                InitilizeNotifyIcon();
                Initilize();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Initilize()
        {
            ConfigManager config = new ConfigManager();
            var _config = config.ReadConfig();
            Timer = new System.Timers.Timer(_config.Timer);
            Timer.Start();
            TimerStatus();


            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _config.LogFileName);

            _logger = new TxTLogger(logPath);
            comboBox1.DataSource = Enum.GetValues(typeof(eZones));
            comboBox1.SelectedIndex = comboBox1.FindStringExact(eZones.one.ToString());

            keyboardManager = new KeyboardManager(_client, _logger, _config);

            this.textBox2.Text = "LANGUAGE";
            this.textBox3.Text = "CHANGE_" + ((int)comboBox1.SelectedItem).ToString();
            this.numericUpDown2.Value = 100;

            countTimerMax = 10000 / (int)_config.Timer;
           // WindowState = FormWindowState.Minimized;
            this.FormClosing += FormCLosing;
          
        }

        private void InitilizeNotifyIcon()
        {
            notifyIcon1.BalloonTipTitle = "KeyboardLanguageSwitcher";
            notifyIcon1.BalloonTipText = "Application hided";
            notifyIcon1.Text = "KeyboardLanguageSwitcher";
        }

        private void DoWork_Tick(object sender, System.EventArgs e)
        {
            try
            {
                Timer.Stop();

                countTimer++;
                if (countTimer >= countTimerMax)
                {
                    var t = keyboardManager.BeatEvent();
                    countTimer = 0;
                }

                HandleCurrentLanguage();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            finally
            {
                Timer.Start();
            }
            TimerStatus();
        }

        private void Log(string text)
        {
            // this.textBox1.BeginInvoke(() => this.textBox1.AppendText(Environment.NewLine + text));
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(Log), new object[] { text });
                return;
            }
            textBox1.AppendText(Environment.NewLine + text);
        }

        private void TimerStatus()
        {
            // this.textBox1.BeginInvoke(() => this.textBox1.AppendText(Environment.NewLine + text));
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action(TimerStatus), new object[] { });
                return;
            }
            if (Timer.Enabled)
                label8.Text = "Запущен";
            else
                label8.Text = "Отключен";
        }

        private static CultureInfo GetCurrentCulture()
        {
            var id = GetForegroundWindow();

            var l = GetKeyboardLayout(GetWindowThreadProcessId(GetForegroundWindow(), IntPtr.Zero));
            return new CultureInfo((short)l.ToInt64());
        }
        CultureInfo _currentLanaguge = null;
        private void HandleCurrentLanguage()
        {
            var currentCulture = GetCurrentCulture();
            if (_currentLanaguge == null || _currentLanaguge.LCID != currentCulture.LCID)
            {
                _currentLanaguge = currentCulture;

                Task t = Task.CompletedTask;
                //ru
                if (_currentLanaguge.LCID == 1049)
                {
                    t = keyboardManager.FireLanguageEvent(100);
                }
                //en
                else if (_currentLanaguge.LCID == 1033)
                {
                    t = keyboardManager.FireLanguageEvent(0);
                }
                t.Wait(60000);
            }
        }


        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            notifyIcon1.Visible = false;
            WindowState = FormWindowState.Normal;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                notifyIcon1.Visible = true;
                // notifyIcon1.ShowBalloonTip(1000);
            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                notifyIcon1.Visible = false;
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _realyNeedClose = true;
            this.Close();
        }

        private void FormCLosing(object sender, FormClosingEventArgs e)
        {
            if (_realyNeedClose)
            {

            }
            else
            {
                e.Cancel = true;
                WindowState = FormWindowState.Minimized;
            }
       
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            WindowState = FormWindowState.Normal;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            BindEventModel model = new BindEventModel();
            model.game = this.textBox2.Text;
            model._event = this.textBox3.Text;
            model.min_value = (int)this.numericUpDown1.Value;
            model.max_value = (int)this.numericUpDown2.Value;
            model.icon_id = 1;
            model.handlers = new Handler[1];
            model.handlers[0] = new Handler();
            model.handlers[0].devicetype = "rgb-zoned-device";
            model.handlers[0].zone = this.comboBox1.SelectedItem.ToString();
            model.handlers[0].color = new Color();
            model.handlers[0].color.gradient = new Gradient();
            model.handlers[0].color.gradient.zero = new Zero();
            model.handlers[0].color.gradient.zero.red = 65;
            model.handlers[0].color.gradient.zero.green = 242;
            model.handlers[0].color.gradient.zero.blue = 113;
            model.handlers[0].color.gradient.hundred = new Hundred();
            model.handlers[0].color.gradient.hundred.red = 35;
            model.handlers[0].color.gradient.hundred.green = 183;
            model.handlers[0].color.gradient.hundred.blue = 236;

            model.handlers[0].mode = "color";

            var result = await keyboardManager.BindEvent(model);

            Log(result);

        }

        private async void button2_Click(object sender, EventArgs e)
        {
            RemoveModel model = new RemoveModel();
            model.game = textBox4.Text;
            model._event = textBox5.Text;

            var result = await keyboardManager.RemoveEvent(model);

            Log(result);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Timer.Enabled = !Timer.Enabled;
            TimerStatus();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.textBox3.Text = "CHANGE_" + ((int)comboBox1.SelectedItem).ToString();
        }


    }
}