using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace GTAVICountdown;

public partial class MainWindow : Window
{
    private readonly DispatcherTimer _timer = new();
    private readonly DateTime _releaseDate = new(2026, 11, 19, 0, 0, 0);

    public MainWindow()
    {
        InitializeComponent();
        LoadPreferences();

        DataContext = this;

        Closing += (_, _) => SavePreferences();
        MouseDown += Window_MouseDown;

        _timer.Tick += (_, _) => UpdateCountdown();

        UpdateCountdown();

        // Only start our timer at the beginning of the next second
        TimeSpan delay = TimeSpan.FromTicks(TimeSpan.TicksPerSecond - DateTime.Now.Ticks % TimeSpan.TicksPerSecond);
        DispatcherTimer syncTimer = new() { Interval = delay };
        syncTimer.Tick += (_, _) =>
        {
            syncTimer.Stop();
            UpdateCountdown();
            _timer.Start();
        };

        syncTimer.Start();
	}

    private void UpdateCountdown()
    {
        TimeSpan remaining = _releaseDate - DateTime.Now;

        if (remaining <= TimeSpan.Zero)
        {
            DaysTextBlock.Text = "00";
            HoursTextBlock.Text = "00";
            MinutesTextBlock.Text = "00";
            SecondsTextBlock.Text = "00";
            _timer.Stop();
            return;
        }

        DaysTextBlock.Text = remaining.Days.ToString("00");
        HoursTextBlock.Text = remaining.Hours.ToString("00");
        MinutesTextBlock.Text = remaining.Minutes.ToString("00");
        SecondsTextBlock.Text = remaining.Seconds.ToString("00");
    }

    private void Window_MouseDown(object sender, MouseButtonEventArgs args)
    {
        if (args.ChangedButton == MouseButton.Left)
            DragMove();
    }

    private void MenuItemClose_Click(object sender, RoutedEventArgs args)
    {
        Close();
    }

    private void LoadPreferences()
    {
        Left = Preferences.Default.WindowLeft;
        Top = Preferences.Default.WindowTop;
        Topmost = Preferences.Default.WindowTopmost;
		ShowInTaskbar = Preferences.Default.WindowShowInTaskbar;
	}

    private void SavePreferences()
    {
		Preferences.Default.WindowLeft = Left;
		Preferences.Default.WindowTop = Top;
		Preferences.Default.WindowTopmost = Topmost;
		Preferences.Default.WindowShowInTaskbar = ShowInTaskbar;
		Preferences.Default.Save();
	}
}
