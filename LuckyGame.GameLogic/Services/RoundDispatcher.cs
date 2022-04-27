namespace LuckyGame.GameLogic.Services;

public class RoundDispatcher
{
    private readonly TimeSpan _period;

    private bool _isRunning;

    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _task;

    public RoundDispatcher(TimeSpan period)
    {
        _period = period;
        _isRunning = false;
    }

    public event EventHandler? OnRound;

    public void Start()
    {
        if (_isRunning)
        {
            return;
        }

        _isRunning = true;
        _cancellationTokenSource = new CancellationTokenSource();

        _task = Task.Run(
            async () =>
            {
                while (true)
                {
                    if (_cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        return;
                    }

                    OnRound?.Invoke(this, EventArgs.Empty);

                    await Task.Delay(_period);
                }
            },
            _cancellationTokenSource.Token);
    }

    public void Stop()
    {
        _cancellationTokenSource?.Cancel();
        _task?.Wait();

        _task = null;
        _cancellationTokenSource = null;
        _isRunning = false;
    }
}
