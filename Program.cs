using System;

namespace appccelerate_statemachine_plantuml_rapport
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello Elevator!");

            Elevator elevator = new Elevator();
            elevator.WritePlantumlRapport();
        }
    }
}
