using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace r4ns0m
{
    public partial class TauntWindow : Window
    {
        private readonly CancellationTokenSource _closeCts = new();
        private readonly CancellationTokenSource _shortTauntCts = new();
        private bool _closed;

        public TauntWindow()
        {
            InitializeComponent();

            Closed += TauntWindow_Closed;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Global.HideSystemMenu(this);

            Title = Global.tauntTitles[
                Random.Shared.Next(Global.tauntTitles.Count)
            ];

            Background = new ImageBrush(
                Global.tauntImages[
                    Random.Shared.Next(Global.tauntImages.Count)
                ])
            {
                Stretch = Stretch.Fill
            };

            Width = Random.Shared.Next(200, 400);
            Height = Random.Shared.Next(200, 400);

            Global.RandomPosWindow(this);

            // This must not contain a busy while-loop.
            new Thread(async () => Global.GlitchIdle(this)) { IsBackground = true }.Start();

            _ = Global.ShortTauntIdle(_shortTauntCts.Token);
            _ = PlaySpawnSoundAsync();
            _ = CloseAfterDelayAsync();
        }

        private static Task PlaySpawnSoundAsync()
        {
            return Task.Run(() =>
            {
                using var stream = Global.GetResourceSteam(
                    "Sounds/tauntSpawn.wav");

                SoundHelper.Create(stream).Play();
            });
        }

        private async Task CloseAfterDelayAsync()
        {
            try
            {
                int delay = Random.Shared.Next(4_000, 10_000);

                await Task.Delay(delay, _closeCts.Token);

                if (_closed || !IsVisible)
                    return;

                _ = Task.Run(() =>
                {
                    using var stream = Global.GetResourceSteam(
                        "Sounds/tauntLeave.wav");

                    SoundHelper.Create(stream).Play();
                });

                await Dispatcher.InvokeAsync(Close);
            }
            catch (OperationCanceledException)
            {
                // The window was closed before the delay completed.
            }
        }

        private void TauntWindow_Closed(
            object? sender,
            EventArgs e)
        {
            _closed = true;
            _shortTauntCts.Cancel();

            if (!_closeCts.IsCancellationRequested)
                _closeCts.Cancel();

            _closeCts.Dispose();
        }
    }
}
