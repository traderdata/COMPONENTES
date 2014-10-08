using System;
using System.Collections.Generic;
using System.Windows.Threading;

namespace ModulusFE.PaintObjects
{
    internal partial class ChartTimer
    {
        private readonly DispatcherTimer _timer;
        private bool _enabledAction;

        public ChartTimer(Action action, int interval)
        {
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(interval) };
            _timer.Stop();

            _timer.Tick += (sender, args) =>
            {
                if (_enabledAction) action();
            };
        }

        public bool Enabled
        {
            get { return _enabledAction; }
            set
            {
                if (_enabledAction == value) return;
                _enabledAction = value;
                if (_enabledAction)
                    _timer.Start();
                else
                    _timer.Stop();
            }
        }
    }

    internal class ChartTimers
    {
        private readonly Dictionary<string, ChartTimer> _timers = new Dictionary<string, ChartTimer>();

        public void RegisterTimer(string timerName, Action action, int interval)
        {
            if (_timers.ContainsKey(timerName))
                throw new ArgumentException("timerName");
            _timers[timerName] = new ChartTimer(action, interval);
        }

        public void StopTimerWork(string timerName)
        {
            if (!_timers.ContainsKey(timerName))
                return;
            //throw new ArgumentOutOfRangeException("timerName");
            _timers[timerName].Enabled = false;
        }

        public void StartTimerWork(string timerName)
        {
            if (!_timers.ContainsKey(timerName))
                return;
            //throw new ArgumentOutOfRangeException("timerName");
            _timers[timerName].Enabled = true;
        }
    }
}

