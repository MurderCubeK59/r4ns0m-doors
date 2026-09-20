using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace r4ns0m
{
    public partial class ShortTauntWindow : Window
    {
        private readonly CancellationTokenSource _closeCts = new();
        private bool _closed;

        public ShortTauntWindow()
        {
            InitializeComponent();

            Closed += ShortTauntWindow_Closed;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Optional fallback timeout.
            // Global.ShortTauntIdle normally closes this window first.
            _ = CloseAfterDelayAsync();
        }

        private async Task CloseAfterDelayAsync()
        {
            try
            {
                // Prevents an individual popup from staying open forever
                await Task.Delay(1500, _closeCts.Token);

                if (!_closed && IsVisible)
                {
                    await Dispatcher.InvokeAsync(Close);
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when the window closes normally
            }
        }

        private void ShortTauntWindow_Closed(
            object? sender,
            EventArgs e)
        {
            _closed = true;

            if (!_closeCts.IsCancellationRequested)
            {
                _closeCts.Cancel();
            }

            _closeCts.Dispose();
        }
    }
}