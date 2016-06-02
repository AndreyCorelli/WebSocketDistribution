using System;
using System.Threading;

namespace WebSocketDistribution.Model
{
    public abstract class Scheduler
    {
        public class Schedule
        {
            public Action action;

            public Action<object> actionOnPassedObject;

            public int minMilsBetween;

            public ThreadSafeTimeStamp lastTimeChecked = new ThreadSafeTimeStamp();

            public object passedObject;

            public Schedule()
            {
            }

            public Schedule(Action action, int minMilsBetween)
            {
                this.action = action;
                this.minMilsBetween = minMilsBetween;
            }

            public Schedule(Action<object> action, int minMilsBetween, object passedObject)
            {
                actionOnPassedObject = action;
                this.minMilsBetween = minMilsBetween;
                this.passedObject = passedObject;
            }
        }

        public class DailySchedule : Schedule
        {
            private readonly ThreadSafeTimeStamp lastTimeDailyRoutineExecuted = new ThreadSafeTimeStamp();

            public DailySchedule(
                Action<object> actionOnObj,
                int minMilsBetweenCheck,
                object passedObject,
                int hourStart, int hourEnd, int minuteStart, int minuteEnd,
                string logMessageThenExecuted)
            {
                minMilsBetween = minMilsBetweenCheck;
                this.passedObject = passedObject;

                actionOnPassedObject = o =>
                {
                    if (DateTime.Now.Hour < hourStart || DateTime.Now.Hour > hourEnd ||
                        DateTime.Now.Minute < minuteStart || DateTime.Now.Minute > minuteEnd) return;

                    var lastTime = lastTimeDailyRoutineExecuted.GetLastHitIfHitted();
                    if (lastTime.HasValue && lastTime.Value.Date == DateTime.Now.Date)
                        return;
                    //if (!string.IsNullOrEmpty(logMessageThenExecuted))
                    //    Logger.Info(logMessageThenExecuted);

                    try
                    {
                        actionOnObj(o);
                    }
                    finally
                    {
                        lastTimeDailyRoutineExecuted.Touch();
                    }
                };
            }
        }

        protected Thread scheduleThread;

        protected volatile bool isStopped;

        protected int threadIntervalMils = 100;

        protected Schedule[] schedules;

        public virtual void Start()
        {
            isStopped = false;
            scheduleThread = new Thread(SchedulePlannerRoutine);
            scheduleThread.Start();
        }

        public virtual void Stop()
        {
            isStopped = true;
            scheduleThread.Join();
        }

        protected void SchedulePlannerRoutine()
        {
            if (schedules == null || schedules.Length == 0) return;

            if (threadIntervalMils <= 0)
                throw new Exception("Scheduler - интервал между опросом должен быть строго больше 0");

            while (!isStopped)
            {
                Thread.Sleep(threadIntervalMils);

                foreach (var sched in schedules)
                {
                    var timeCheck = sched.lastTimeChecked.GetLastHitIfHitted();
                    var milsSinceCheck = timeCheck.HasValue
                                             ? (DateTime.Now - timeCheck.Value).TotalMilliseconds
                                             : int.MaxValue;
                    if (milsSinceCheck < sched.minMilsBetween)
                        continue;
                    if (sched.action != null)
                        sched.action();
                    else
                        sched.actionOnPassedObject(sched.passedObject);
                    sched.lastTimeChecked.Touch();
                }
            }
        }
    }

}
