using System;
using DisablerAi.Interfaces;

namespace DisablerAi
{
    public class PlayerLocation
    {
        public DateTime Time { get; }
        public ILocation Location { get; }

        public bool Seen { get; set; }

        public bool Heard { get; set; }

        public PlayerLocation(DateTime time, ILocation location, bool seen, bool heard)
        {
            this.Time = time;
            this.Location = location;
            this.Seen = seen;
            this.Heard = heard;
        }
    }
}