using Orleans.Concurrency;
using System;

namespace Fork
{
    [Immutable]
    public class Player
    {
        public Guid Key { get; set; }
        public string Name { get; set; }
        public int Points { get; set; }
    }
}