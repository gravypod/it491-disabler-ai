using System;
using DisablerAi.Interfaces;

namespace DisablerAi
{
    public class PlayerLocation
    {
        public DateTime TimeSeen { get; }
        public ILocation Location { get; }

        public PlayerLocation(DateTime seen, ILocation location)
        {
            this.TimeSeen = seen;
            this.Location = location;
        }
    }
}