using System.Collections.Generic;

namespace DisablerAi.Interfaces
{
    public interface IRobot
    {
        ILocation Location { get; set; }
        ILocation Target { get; set; }

        ILocation PatrolStart { get; set; }
        ILocation PatrolEnd { get; set; }

        List<ILocation> PointsOfInterest { get; set; }

        // Acting
        RobotAnimation PlayingAnimation { get; set; }

        // Vision configuration
        bool DetectionLineOfSight { get; set; }
        bool DetectionAudio { get; set; }

        // Health Data
        bool Shot { get; set; }
        bool HitWithItem { get; set; }
        IRobotHead Head { get; }
        int Health { get; set; }

        // Visibility Checks
        bool CanSee(IPlayer player);
        bool CanHear(IPlayer player);

        /// <summary>
        /// Check to see if this robot has reached it's target
        /// </summary>
        /// <param name="distanceForgiveness">How far, in meters, from the target before this returns true</param>
        /// <returns>true when the robot is within distanceForgiveness from it's target</returns>
        bool ReachedTarget(float distanceForgiveness = 0.5f);
    }
}