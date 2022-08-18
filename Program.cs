using Akka.Actor;

namespace AkkaScheduler
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Akka .Net - Scheduler events \n");

            var actorSystem = ActorSystem.Create("scheduler-system");
            actorSystem.ActorOf<SomeActor>("some-actor");
            actorSystem.ActorOf<SomeActorWithTimers>("some-actor-with-timers");

            Console.ReadKey();
        }
    }
}