using System;
using System.Collections.Generic;
using DisablerAi;
using DisablerAi.Interfaces;

namespace TestRobot
{
    class MockLocation : ILocation
    {
        private readonly float _x, _y, _z;
        private readonly MockLocation _randomLocation;

        public MockLocation(float x = 0.0f, float y = 0.0f, float z = 0.0f, MockLocation randomLocation = null)
        {
            _x = x;
            _y = y;
            _z = z;
            this._randomLocation = randomLocation;
        }


        public ILocation RandomLocation(float distanceFromPlayer, float distanceFromRobots)
        {
            return this._randomLocation ?? this;
        }

        protected bool Equals(MockLocation other)
        {
            return _x.Equals(other._x) && _y.Equals(other._y) && _z.Equals(other._z);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MockLocation) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _x.GetHashCode();
                hashCode = (hashCode * 397) ^ _y.GetHashCode();
                hashCode = (hashCode * 397) ^ _z.GetHashCode();
                return hashCode;
            }
        }

        public float DistanceFrom(ILocation location)
        {
            if (location.GetType() == typeof(MockLocation))
            {
                MockLocation l = (MockLocation) location;
                return (float) Math.Sqrt(
                    Math.Pow(_x - l._x, 2.0f) +
                    Math.Pow(_y - l._y, 2.0f) +
                    Math.Pow(_z - l._z, 2.0f)
                );
            }

            throw new NotImplementedException("MockLocation cannot calculate distance from ILocation");
        }
    }

    class MockDisabler : IDisabler
    {
        public ILocation Location { get; set; }

        public MockDisabler(ILocation location)
        {
            this.Location = location;
        }
    }

    class MockPlayer : IPlayer
    {
        public IDisabler Disabler { get; set; }
        public ILocation Location { get; set; }

        public MockPlayer(IDisabler disabler, ILocation location)
        {
            this.Disabler = disabler;
            this.Location = location;
        }
    }


    class MockRobotHead : IRobotHead
    {
        public ILocation Location { get; set; }
        public bool Shot { get; set; }

        public MockRobotHead(ILocation location, bool shot)
        {
            this.Location = location;
            this.Shot = shot;
        }
    }

    class MockRobot : IRobot
    {
        public ILocation Location { get; set; }
        public ILocation Target { get; set; }
        public ILocation PatrolStart { get; set; }
        public ILocation PatrolEnd { get; set; }
        public List<ILocation> PointsOfInterest { get; set; } = new List<ILocation>();
        public RobotAnimation PlayingAnimation { get; set; }
        public bool DetectionLineOfSight { get; set; }
        public bool DetectionAudio { get; set; }
        public bool Shot { get; set; }
        public bool HitWithItem { get; set; }
        public IRobotHead Head { get; }
        public float Health { get; set; }


        public bool CanSeePlayer { get; set; }
        public bool CanHearPlayer { get; set; }

        public MockRobot(
            ILocation location = null,
            bool hitWithItem = false,
            float health = 100.0f,
            bool shot = false,
            bool headShot = false
        )
        {
            this.Target = null;
            this.PlayingAnimation = RobotAnimation.None;
            this.CanSeePlayer = false;
            this.CanHearPlayer = false;
            this.DetectionLineOfSight = true;
            this.DetectionAudio = true;

            // Provided values
            this.PatrolStart = this.PatrolEnd = this.Location = location ?? new MockLocation();
            this.HitWithItem = hitWithItem;
            this.Health = health;
            this.Shot = shot;
            this.Head = new MockRobotHead(this.Location, headShot);
        }

        public bool CanSee(IPlayer player)
        {
            if (!this.DetectionLineOfSight)
                return false;

            return CanSeePlayer;
        }

        public bool CanHear(IPlayer player)
        {
            if (!this.DetectionAudio)
                return false;
            return CanHearPlayer;
        }

        public bool ReachedTarget(float distanceForgiveness = 0.5f)
        {
            return this.Location.DistanceFrom(this.Target) <= distanceForgiveness;
        }
    }

    class MockRobotAi : RobotAi
    {
        public MockRobotAi(IRobot robot = null, IPlayer player = null) :
            base(
                robot ?? new MockRobot(),
                player ?? new MockPlayer(new MockDisabler(new MockLocation()), new MockLocation())
            )
        {
        }
    }
}