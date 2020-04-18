using Appccelerate.StateMachine;
using Appccelerate.StateMachine.Machine;
using Appccelerate.StateMachine.Machine.Reports;

using System;
using System.IO;
using System.Collections.Generic;
using Appccelerate.StateMachine.Machine.States;
// using Appccelerate.StateMachine.Machine.ActionHolders;
// using Appccelerate.StateMachine.Extensions;
// using Appccelerate.StateMachine.Infrastructure;
// using Appccelerate.StateMachine.Syntax;

namespace appccelerate_statemachine_plantuml_rapport
{
    public class PlantumlStateMachineReport<TState, TEvent> : IStateMachineReport<TState, TEvent>
        where TState : IComparable
        where TEvent : IComparable
    {
        private PlantumlStateMachineReport()
        {

        }
        public PlantumlStateMachineReport(TextWriter textWriter)
        {
            this.textWriter = textWriter;
        }
        TextWriter textWriter;
        public void Report(string name, IEnumerable<IStateDefinition<TState, TEvent>> states, TState initialStateId)
        {
            textWriter.WriteLine("@startuml " + name);

            textWriter.WriteLine("[*] --> " + initialStateId);


            foreach (var state in states)
            {

                if (state.Level != 1)
                {
                    continue;
                }
                WriteState(state);

            }

            textWriter.WriteLine("@enduml");
            textWriter.Close();
        }

        void WriteState(IStateDefinition<TState, TEvent> state)
        {
            textWriter.WriteLine("state " + state.Id + " {");
            if (state.HistoryType == HistoryType.Shallow)
            {
                textWriter.WriteLine("[H] -> " +  state.Id + " : shallow");
            }
            if (state.HistoryType == HistoryType.Deep)
            {
                textWriter.WriteLine("[H] -> " + state.Id + " : deep");
            }

            if (state.InitialState != null)
            {
                textWriter.WriteLine("[*] -> " + state.InitialState.Id);
            }
            foreach (var subState in state.SubStates)
            {
                WriteState(subState);
            }
            textWriter.WriteLine("}");

            foreach (var action in state.EntryActions)
            {
                textWriter.WriteLine(state.Id + " : entry / " + action.Describe());
            }
            foreach (var action in state.ExitActions)
            {
                textWriter.WriteLine(state.Id + " : exit / " + action.Describe());
            }

            foreach (var transitionInfo in state.TransitionInfos)
            {
                String text = transitionInfo.Source.Id.ToString();

                if (transitionInfo.Target != null)
                {
                    text += " --> " + transitionInfo.Target.Id;
                }
                if (transitionInfo.EventId != null)
                {
                    text += " : " + transitionInfo.EventId;
                    if (transitionInfo.Guard != null)
                    {
                        text += " [" + transitionInfo.Guard.Describe() + "]";
                    }

                    foreach (var action in transitionInfo.Actions)
                    {
                        text += " / " + action.Describe();
                    }
                }
                textWriter.WriteLine(text);
            }
        }
    }
}