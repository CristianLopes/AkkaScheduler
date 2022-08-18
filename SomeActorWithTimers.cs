using Akka.Actor;
using Akka.Event;

namespace AkkaScheduler
{
    internal class SomeActorWithTimers : ReceiveActor, IWithTimers
    {
        public ITimerScheduler Timers { get; set; }

        private readonly ILoggingAdapter _loggingAdapter = Context.GetLogger();

        private int countCancelableMessage = 0;
        private const string scheduleTellOnceKey = "ScheduleTellOnce";
        private const string scheduleTellRepeatedly = "ScheduleTellRepeatedly";
        private const string scheduleTellRepeatedlyCancel = "ScheduleTellRepeatedlyCancel";

        public SomeActorWithTimers()
        {
            Receive<SomeScheduleOnceMessage>(command =>
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"SomeActorWithTimers >> SomeScheduleOnceMessage Executed");
            });

            Receive<SomeScheduleRepeatedlyMessage>(command =>
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"SomeActorWithTimers >> SomeScheduleRepeatedlyMessage executed at {DateTime.Now}");
            });

            Receive<SomeScheduleRepeatedlyCancelableMessage>(command =>
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                countCancelableMessage++;
                Console.WriteLine($"SomeActorWithTimers >> SomeScheduleRepeatedlyCancelableMessage executed {countCancelableMessage} times, {DateTime.Now}");

                if (countCancelableMessage == 5)
                {
                    Console.WriteLine($"SomeActorWithTimers >> SomeScheduleRepeatedlyCancelableMessage was cancelled.");
                    
                    if (Timers!.IsTimerActive(scheduleTellRepeatedlyCancel))
                    {
                        Timers.Cancel(scheduleTellRepeatedlyCancel);
                    }
                }
            });
        }

        protected override void PreStart()
        {
            // both timers will be automatically disposed when actor is stopped
            //start a single timer that fires off 1 second in the future
            Timers.StartSingleTimer(
                scheduleTellOnceKey,
                new SomeScheduleOnceMessage(),
                TimeSpan.FromSeconds(1));

            //start a recurring timer that fires off first time 5 seconds in the future
            //and fires off every 3 seconds
            Timers.StartPeriodicTimer(
                scheduleTellRepeatedly,
                new SomeScheduleRepeatedlyMessage(),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(3));

            //start a recurring timer that fires off first time 2 seconds in the future
            //and fires off every 2 seconds
            Timers.StartPeriodicTimer(
                scheduleTellRepeatedlyCancel,
                new SomeScheduleRepeatedlyCancelableMessage(),
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(2));
        }
    }
}
