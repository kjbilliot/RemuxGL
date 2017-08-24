using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Emux.GameBoy.NativeTimer;

namespace Emux.GameBoy
{
    public class WinApiEmulator
    {
        public static class MultimediaTimer
        {
            private static List<Thread> SchedulerCatalog = new List<Thread>();
            private enum TimerEventType { TIME_ONESHOT, TIME_PERIODIC };

            private static void ScheduleThread(int fireTime, MmTimerProc callback, TimerEventType schedulerType)
            {
                try
                {
                    Stopwatch sw = new Stopwatch();
                    repeatSchedule:
                    sw.Start();
                    while (sw.ElapsedMilliseconds < fireTime)
                        Thread.Sleep(1);
                    sw.Reset();
                    callback((uint)SchedulerCatalog.IndexOf(Thread.CurrentThread), 0, IntPtr.Zero, 0, 0);
                    if (schedulerType == TimerEventType.TIME_PERIODIC)
                        goto repeatSchedule;
                }
                catch (ThreadInterruptedException e)
                {
                    Console.WriteLine($"Thread interrupted (callback={callback.Method.Name}())");
                }
            }

            public static uint SetTimer(int delay, int resolution, MmTimerProc callback, int user, int eventType)
            {
                Thread schedulerThread = new Thread(() => ScheduleThread(delay, callback, (TimerEventType)eventType));
                SchedulerCatalog.Add(schedulerThread);
                schedulerThread.Start();
                return (uint)SchedulerCatalog.IndexOf(schedulerThread);
            }

            public static void CancelTimer(uint id)
            {
                Thread t = SchedulerCatalog[(int)id];
                t.Interrupt();
                SchedulerCatalog.Remove(t);
            }
        }
    }
}
