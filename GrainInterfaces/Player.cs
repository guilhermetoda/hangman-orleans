using Orleans.Concurrency;
using System;
using System.Collections.Generic;

namespace Fork
{
    [Immutable]
    public class Player
    {
        public Guid Key { get; set; }
        public string Name { get; set; }
        // array to register how many games the player had complete
        public List<int> GamesGuessed { get; set; }
        public int WordIndex { get; set; }
        public bool FoundTheWord { get; set; }
    }
}