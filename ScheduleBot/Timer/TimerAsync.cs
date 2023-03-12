using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Timer;

public class TimerAsync
{
    Task task;
    CancellationTokenSource cts;

    public TimerAsync(Action action, TimeSpan period, CancellationTokenSource cts)
    {
        this.cts = cts;
        this.task = new(() => Action(action, period), cts.Token);
    }
    public void Action(Action action, TimeSpan period)
    {
        while (true)
        {
            if (cts.IsCancellationRequested)
                break;

            action();

            int TotalMilliseconds = Convert.ToInt32(period.TotalMilliseconds);
            Task.Delay(TotalMilliseconds).Wait();
        }
    }

    public void Start()
        => task.Start();
    public void Stop()
        => cts.Cancel();
}
