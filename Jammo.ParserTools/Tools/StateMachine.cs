using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Jammo.ParserTools.Tools
{
    public class StateMachine<TStates> : IEnumerable<TStates> where TStates : Enum
    {
        private readonly List<TStates> previous = new();

        public TStates Current { get; private set; }

        public StateMachine() { }
        public StateMachine(TStates start)
        {
            Current = start;
        }

        public void MoveTo(TStates state)
        {
            previous.Add(Current);
            Current = state;
        }

        public void MoveLast()
        {
            Current = previous.Last();
            previous.Remove(Current);
        }
            
        public TStates PeekLast()
        {
            return previous.Last();
        }

        public IEnumerator<TStates> GetEnumerator()
        {
            return previous.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}