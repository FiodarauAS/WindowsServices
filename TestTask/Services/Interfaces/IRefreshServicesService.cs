using System;
using System.Timers;

namespace TaskWpf
{
    public interface IRefreshServicesService
    {
        void InitTimer();
        void RefreshTimerElapsed(Object source, ElapsedEventArgs e);
    }
}
