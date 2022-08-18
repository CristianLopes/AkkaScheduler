using Akka.Actor;

namespace AkkaScheduler
{
    internal class SomeActor : ReceiveActor
    {
        private ICancelable _cancelable;
        private int countCancelableMessage = 0;

        public SomeActor()
        {
            Receive<SomeScheduleOnceMessage>(command =>
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"SomeActor >> SomeScheduleOnceMessage Executed");
            });

            Receive<SomeScheduleRepeatedlyMessage>(command =>
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"SomeActor >> SomeScheduleRepeatedlyMessage executed at {DateTime.Now}");
            });

            Receive<SomeScheduleRepeatedlyCancelableMessage>(command =>
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                countCancelableMessage++;
                Console.WriteLine($"SomeActor >> SomeScheduleRepeatedlyCancelableMessage executed {countCancelableMessage} times, {DateTime.Now}");

                if (countCancelableMessage == 5)
                {
                    Console.WriteLine($"SomeActor >> SomeScheduleRepeatedlyCancelableMessage was cancelled.");
                    _cancelable?.Cancel();
                }
            });

            Context.System.Scheduler
                .ScheduleTellOnce(
                    TimeSpan.FromSeconds(1), 
                    Self, 
                    new SomeScheduleOnceMessage(), 
                    ActorRefs.Nobody);

            Context.System.Scheduler
                .ScheduleTellRepeatedly(
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(3),
                    Self,
                    new SomeScheduleRepeatedlyMessage(),
                    ActorRefs.Nobody);

            _cancelable = Context.System.Scheduler
                .ScheduleTellRepeatedlyCancelable(
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(2),
                    Self,
                    new SomeScheduleRepeatedlyCancelableMessage(),
                    ActorRefs.Nobody);
        }
    }
}
