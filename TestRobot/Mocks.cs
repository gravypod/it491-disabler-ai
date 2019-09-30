using System;
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
        public RobotAnimation PlayingAnimation { get; set; }
        public bool DetectionLineOfSight { get; set; }
        public bool DetectionAudio { get; set; }
        public bool Shot { get; set; }
        public bool HitWithItem { get; set; }
        public IRobotHead Head { get; }
        public float Health { get; set; }


        public float DistanceFromBeginningOfPatrol { get; set; }
        public float DistanceFromEndingOfPatrol { get; set; }
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
            this.DistanceFromBeginningOfPatrol = 0.0f;
            this.DistanceFromEndingOfPatrol = 0.0f;
            this.DetectionLineOfSight = true;
            this.DetectionAudio = true;

            // Provided values
            this.Location = location ?? new MockLocation();
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

        float IRobot.DistanceFromBeginningOfPatrol()
        {
            return DistanceFromBeginningOfPatrol;
        }

        float IRobot.DistanceFromEndingOfPatrol()
        {
            return DistanceFromEndingOfPatrol;
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