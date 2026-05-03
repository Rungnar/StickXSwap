using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using SharpDX.XInput;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets.Xbox360;

class Program : ApplicationContext
{
    [DllImport("user32.dll")]
    static extern short GetAsyncKeyState(int vKey);

    const int TOGGLE_KEY = 0x76; // F7

    bool swap = true;

    NotifyIcon trayIcon;
    ToolStripMenuItem toggleItem;

    Form overlay;
    Label label;
    System.Windows.Forms.Timer fadeTimer;

    double opacityStep = 0.05;

    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        new Program();
        Application.Run();
    }

    public Program()
    {
        SetupTray();
        CreateOverlay();
        SetupFadeTimer();

        new Thread(InputLoop) { IsBackground = true }.Start();
        new Thread(KeyLoop) { IsBackground = true }.Start();
    }

    void SetupTray()
    {
        trayIcon = new NotifyIcon()
        {
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath),
            Visible = true,
            Text = "StickXSwap"
        };

        var menu = new ContextMenuStrip();

        toggleItem = new ToolStripMenuItem();
        toggleItem.Click += (s, e) =>
        {
            swap = !swap;
            UpdateToggleText();
            ShowOverlay();
        };

        menu.Items.Add(toggleItem);

        menu.Items.Add("Exit", null, (s, e) =>
        {
            trayIcon.Visible = false;
            Environment.Exit(0);
        });

        trayIcon.ContextMenuStrip = menu;

        UpdateToggleText(); // initialize text
    }

    void UpdateToggleText()
    {
        if (trayIcon != null && trayIcon.ContextMenuStrip != null)
        {
            var control = trayIcon.ContextMenuStrip;

            if (control.InvokeRequired)
            {
                control.Invoke(new Action(UpdateToggleText));
                return;
            }

            toggleItem.Text = swap ? "Swap: ON" : "Swap: OFF";
        }
    }

    void CreateOverlay()
    {
        overlay = new Form()
        {
            Size = new Size(140, 40),
            FormBorderStyle = FormBorderStyle.None,
            StartPosition = FormStartPosition.Manual,
            Location = new Point(10, 10),
            TopMost = true,
            BackColor = Color.Black,
            Opacity = 0,
            ShowInTaskbar = false
        };

        label = new Label()
        {
            Dock = DockStyle.Fill,
            ForeColor = Color.Lime,
            TextAlign = ContentAlignment.MiddleCenter
        };

        overlay.Controls.Add(label);
        overlay.Show();
    }

    void SetupFadeTimer()
    {
        fadeTimer = new System.Windows.Forms.Timer();
        fadeTimer.Interval = 50;

        fadeTimer.Tick += (s, e) =>
        {
            if (overlay.Opacity > 0)
                overlay.Opacity -= opacityStep;
            else
                fadeTimer.Stop();
        };
    }

    void UI(Action action)
    {
        if (overlay.InvokeRequired)
            overlay.Invoke(action);
        else
            action();
    }

    void ShowOverlay()
    {
        UI(() =>
        {
            label.Text = swap ? "SWAP: ON" : "SWAP: OFF";
            overlay.Opacity = 1;
            fadeTimer.Stop();
            fadeTimer.Start();
        });
    }

    void KeyLoop()
    {
        const int VK_ALT = 0x12;

        while (true)
        {
            if ((GetAsyncKeyState(TOGGLE_KEY) & 1) != 0 &&
                (GetAsyncKeyState(VK_ALT) & 0x8000) != 0)
            {
                swap = !swap;
                UpdateToggleText();
                ShowOverlay();
            }

            Thread.Sleep(10);
        }
    }

    void InputLoop()
    {
        Controller input = null;

        for (int i = 0; i < 4; i++)
        {
            var c = new Controller((UserIndex)i);
            if (c.IsConnected)
            {
                input = c;
                break;
            }
        }

        var client = new ViGEmClient();
        var pad = client.CreateXbox360Controller();
        pad.Connect();

        while (true)
        {
            if (input == null || !input.IsConnected)
                continue;

            var g = input.GetState().Gamepad;

            short lx = g.LeftThumbX;
            short rx = g.RightThumbX;

            // SWAP X AXES
            pad.SetAxisValue(Xbox360Axis.LeftThumbX, swap ? rx : lx);
            pad.SetAxisValue(Xbox360Axis.RightThumbX, swap ? lx : rx);

            pad.SetAxisValue(Xbox360Axis.LeftThumbY, g.LeftThumbY);
            pad.SetAxisValue(Xbox360Axis.RightThumbY, g.RightThumbY);

            // TRIGGERS
            pad.SetSliderValue(Xbox360Slider.LeftTrigger, g.LeftTrigger);
            pad.SetSliderValue(Xbox360Slider.RightTrigger, g.RightTrigger);

            // BUTTONS
            pad.SetButtonState(Xbox360Button.A, (g.Buttons & GamepadButtonFlags.A) != 0);
            pad.SetButtonState(Xbox360Button.B, (g.Buttons & GamepadButtonFlags.B) != 0);
            pad.SetButtonState(Xbox360Button.X, (g.Buttons & GamepadButtonFlags.X) != 0);
            pad.SetButtonState(Xbox360Button.Y, (g.Buttons & GamepadButtonFlags.Y) != 0);

            pad.SetButtonState(Xbox360Button.LeftThumb, (g.Buttons & GamepadButtonFlags.LeftThumb) != 0);
            pad.SetButtonState(Xbox360Button.RightThumb, (g.Buttons & GamepadButtonFlags.RightThumb) != 0);

            pad.SetButtonState(Xbox360Button.LeftShoulder, (g.Buttons & GamepadButtonFlags.LeftShoulder) != 0);
            pad.SetButtonState(Xbox360Button.RightShoulder, (g.Buttons & GamepadButtonFlags.RightShoulder) != 0);

            pad.SetButtonState(Xbox360Button.Start, (g.Buttons & GamepadButtonFlags.Start) != 0);
            pad.SetButtonState(Xbox360Button.Back, (g.Buttons & GamepadButtonFlags.Back) != 0);

            // D-PAD
            pad.SetButtonState(Xbox360Button.Up, (g.Buttons & GamepadButtonFlags.DPadUp) != 0);
            pad.SetButtonState(Xbox360Button.Down, (g.Buttons & GamepadButtonFlags.DPadDown) != 0);
            pad.SetButtonState(Xbox360Button.Left, (g.Buttons & GamepadButtonFlags.DPadLeft) != 0);
            pad.SetButtonState(Xbox360Button.Right, (g.Buttons & GamepadButtonFlags.DPadRight) != 0);

            pad.SubmitReport();

            Thread.Sleep(1);
        }
    }
}