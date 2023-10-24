using System.Diagnostics;
using System.Windows.Threading;
using _3D_visualization.Model;

namespace _3D_visualization;

public class GameLoop
{
    private Game _game;
    
    private Stopwatch _stopwatch;
    private float _fps;
    private bool _isRunning;

    private DispatcherTimer timer;
    public GameLoop(float fps)
    {
        _fps = fps;
        _stopwatch = new Stopwatch();
        _isRunning = false;
        
        timer = new DispatcherTimer();
        timer.Tick += Timer_Tick;
    }
    
    private void Timer_Tick(object sender, EventArgs e)
    {
        _stopwatch.Start();
        Update(1.0f / _fps);
        _stopwatch.Stop();
        _stopwatch.Reset();
    }
    
    public void LoadGame(Game game)
    {
        _game = game;
    }
    
    public void Start()
    {
        if (_isRunning)
            return;

        _isRunning = true;
        TimeSpan targetElapsedTime = TimeSpan.FromSeconds(1.0 / _fps);

        timer.Interval = targetElapsedTime;
        timer.Start();
    }

    public void Stop()
    {
        _isRunning = false;
    }

    private void Update(float deltaTime)
    {
        if (_isRunning)
        {
            _game.Update(deltaTime);
        }
    }
}