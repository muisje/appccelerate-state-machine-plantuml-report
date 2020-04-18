using Appccelerate.StateMachine.Machine;
using Appccelerate.StateMachine;
using Appccelerate.StateMachine.Machine.Reports;
using System;
using System.IO;

namespace appccelerate_statemachine_plantuml_rapport
{
public class Elevator
    {
        private readonly PassiveStateMachine<States, Events> elevator;

        private enum States
        {
            Healthy,
            OnFloor,
            Moving,
            MovingUp,
            MovingDown,
            DoorOpen,
            DoorClosed,
            Error
        }

        private enum Events
        {
            GoUp,
            GoDown,
            OpenDoor,
            CloseDoor,
            Stop,
            Error,
            Reset
        }

        public Elevator()
        {
            var builder = new StateMachineDefinitionBuilder<States, Events>();

            builder.DefineHierarchyOn(States.Healthy)
                .WithHistoryType(HistoryType.Deep)
                .WithInitialSubState(States.OnFloor)
                .WithSubState(States.Moving);

            builder.DefineHierarchyOn(States.Moving)
                .WithHistoryType(HistoryType.Shallow)
                .WithInitialSubState(States.MovingUp)
                .WithSubState(States.MovingDown);

            builder.DefineHierarchyOn(States.OnFloor)
                .WithHistoryType(HistoryType.None)
                .WithInitialSubState(States.DoorClosed)
                .WithSubState(States.DoorOpen);

            builder.In(States.Healthy)
                .On(Events.Error).Goto(States.Error);

            builder.In(States.Error)
                .On(Events.Reset).Goto(States.Healthy)
                .On(Events.Error);

            builder.In(States.OnFloor)
                .ExecuteOnEntry(this.AnnounceFloor)
                .ExecuteOnExit(Beep)
                .ExecuteOnExit(Beep) // just beep a second time
                .On(Events.CloseDoor).Goto(States.DoorClosed)
                .On(Events.OpenDoor).Goto(States.DoorOpen)
                .On(Events.GoUp)
                    .If(CheckOverload).Goto(States.MovingUp)
                    .Otherwise().Execute(this.AnnounceOverload)
                .On(Events.GoDown)
                    .If(CheckOverload).Goto(States.MovingDown)
                    .Otherwise().Execute(this.AnnounceOverload);
            builder.In(States.Moving)
                .On(Events.Stop).Goto(States.OnFloor);

            builder.WithInitialState(States.OnFloor);

            var definition = builder
                .Build();


            elevator = definition
                .CreatePassiveStateMachine("Elevator");

            elevator.Start();
        }

        public void WritePlantumlRapport()
        {
            Console.WriteLine("Writing plantuml rapport...");
            
            var plant = new PlantumlStateMachineReport<States, Events>(File.CreateText(elevator.ToString() + ".plantuml"));
            elevator.Report(plant);
        }

        public void GoToUpperLevel()
        {
            this.elevator.Fire(Events.CloseDoor);
            this.elevator.Fire(Events.GoUp);
            this.elevator.Fire(Events.OpenDoor);
        }

        public void GoToLowerLevel()
        {
            this.elevator.Fire(Events.CloseDoor);
            this.elevator.Fire(Events.GoDown);
            this.elevator.Fire(Events.OpenDoor);
        }

        public void Error()
        {
            this.elevator.Fire(Events.Error);
        }

        public void Stop()
        {
            this.elevator.Fire(Events.Stop);
        }

        public void Reset()
        {
            this.elevator.Fire(Events.Reset);
        }

        private void AnnounceFloor()
        {
            /* announce floor number */
            Console.WriteLine("Announcement: floor number");
        }

        private void AnnounceOverload()
        {
            Console.WriteLine("Announcement: overload");
        }

        private void Beep()
        {
            Console.WriteLine("Beep!");
        }

        private bool CheckOverload()
        {
            return false;
        }
    }
}