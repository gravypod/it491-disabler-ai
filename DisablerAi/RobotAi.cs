using System;
using System.Collections.Generic;
using System.Linq;
using DisablerAi.Interfaces;

namespace DisablerAi
{
    public class RobotAi
    {
        public bool HasHeldUpDemandBeenMade { get; set; } = false;
        public bool HasHeldUpSentToGround { get; set; } = false;
        public IRobot Robot { get; set; }
        public IPlayer Player { get; set; }
        public RobotAiState State { get; set; }

        public DateTime TimeMarker = DateTime.Now;
        public int BurstsFired = 0;


        public bool GetDownRequested = false, MarkItemsRequested = false, MarkEnemiesRequested = false;

        /// <summary>
        /// A list of times we think we've seen or heard the player.
        /// </summary>
        public List<PlayerLocation> PlayerLocations = new List<PlayerLocation>();

        public RobotAi(IRobot robot, IPlayer player)
        {
            this.Robot = robot;
            this.Player = player;
            this.State = RobotAiState.Start;
        }

        public bool Can(RobotAiState state)
        {
            switch (state)
            {
                case RobotAiState.Start:
                    // No way to get back to start. Cannot collect $200.
                    return false;
                case RobotAiState.Inactive:
                    return this.State == RobotAiState.Start;

                // Searching State Machine
                case RobotAiState.Searching:

                    if (State == RobotAiState.AlertFollowUp)
                    {
                        if (!Robot.CanSee(Player) && !Robot.CanHear(Player))
                        {
                            if ((DateTime.Now - TimeMarker).TotalMinutes >= 1)
                                return true;
                        }
                    }

                    if (State == RobotAiState.SuspicionCallHeadQuarters)
                        return true;

                    return false;

                case RobotAiState.SearchingFollowUpPointOfInterest:
                    if (State == RobotAiState.Searching)
                    {
                        return true;
                    }


                    if (State == RobotAiState.SearchingLookAroundPlayerLastSeen)
                    {
                        if ((DateTime.Now - TimeMarker).TotalSeconds > 2)
                        {
                            return true;
                        }
                    }


                    if (State == RobotAiState.SearchingLookAroundPointOfInterest && PlayerLocations.Count() == 0)
                    {
                        if ((DateTime.Now - TimeMarker).TotalSeconds > 5)
                        {
                            return true;
                        }
                    }

                    return false;

                case RobotAiState.SearchingLookAroundPlayerLastSeen:
                    if (State == RobotAiState.SearchingFollowUpPlayerLastSeen)
                    {
                        if (Robot.ReachedTarget())
                            return true;
                    }

                    return false;
                case RobotAiState.SearchingLookAroundPointOfInterest:

                    if (State == RobotAiState.SearchingFollowUpPointOfInterest)
                    {
                        if (Robot.ReachedTarget())
                            return true;
                    }

                    return false;

                case RobotAiState.SearchingFollowUpPlayerLastSeen:
                    if (State == RobotAiState.SearchingLookAroundPointOfInterest && PlayerLocations.Any())
                    {
                        if ((DateTime.Now - TimeMarker).TotalSeconds > 5)
                        {
                            return true;
                        }
                    }

                    return false;

                // Alert State Machine
                case RobotAiState.Alert:

                    bool hasBeenSearching = State == RobotAiState.Searching ||
                                            State == RobotAiState.SearchingFollowUpPlayerLastSeen ||
                                            State == RobotAiState.SearchingFollowUpPointOfInterest ||
                                            State == RobotAiState.SearchingLookAroundPlayerLastSeen ||
                                            State == RobotAiState.SearchingLookAroundPointOfInterest;
                    if (hasBeenSearching && PlayerLocations.Any())
                    {
                        if ((DateTime.Now - PlayerLocations.Last().Time).TotalMinutes > 1)
                        {
                            return true;
                        }
                    }

                    if (State == RobotAiState.Patrol)
                    {
                        float distance = Robot.Location.DistanceFrom(Player.Location);
                        if (Robot.CanSee(Player) && distance < 15.0f)
                            return true;
                    }

                    return false;

                case RobotAiState.AlertCallHeadQuarters:
                    if (State == RobotAiState.Alert)
                    {
                        if ((DateTime.Now - TimeMarker).TotalSeconds >= 2)
                        {
                            return true;
                        }
                    }

                    return false;

                case RobotAiState.AlertAttack:
                    if (State == RobotAiState.AlertCallHeadQuarters)
                    {
                        // Once we've finished alerting everyone, start attacking
                        if (Robot.PlayingAnimation != RobotAnimation.AlertCallHeadQuarters)
                            return true;
                    }

                    if (State == RobotAiState.AlertReposition)
                    {
                        // Start attacking once we've gotten nice and cozy
                        if (Robot.ReachedTarget() && (DateTime.Now - TimeMarker).TotalSeconds >= 2)
                        {
                            return true;
                        }
                    }

                    return false;

                case RobotAiState.AlertReposition:
                    if (State == RobotAiState.AlertAttack)
                    {
                        if (BurstsFired >= 1 && BurstsFired <= 3)
                            return true;
                    }

                    return false;

                case RobotAiState.AlertFollowUp:

                    if (State == RobotAiState.AlertAttack || State == RobotAiState.AlertReposition)
                    {
                        // Cannot see the player or the player is not within 60m
                        if (!Robot.CanSee(Player))
                            return true;

                        if (Robot.Location.DistanceFrom(Player.Location) > 60)
                            return true;
                    }

                    return false;
                // Suspicion State Machine
                case RobotAiState.Suspicion:
                    if (this.State == RobotAiState.Patrol)
                    {
                        float distance = Robot.Location.DistanceFrom(Player.Location);
                        if (Robot.CanSee(Player) && distance > 14 && distance < 50)
                            return true;

                        if (Robot.CanHear(Player) && distance < 40)
                            return true;
                    }

                    if (this.State == RobotAiState.Hurt)
                    {
                        if (this.Robot.PlayingAnimation == RobotAnimation.HurtStagger)
                            return false;

                        if ((DateTime.Now - this.TimeMarker).TotalSeconds < 2)
                            return false;

                        return true;
                    }

                    return false;

                case RobotAiState.SuspicionFollowUp:
                    if (State == RobotAiState.Suspicion && this.PlayerLocations.Count() > 0)
                    {
                        return true;
                    }

                    return false;

                case RobotAiState.SuspicionLookAround:
                    if (State == RobotAiState.SuspicionFollowUp)
                    {
                        float distance = Robot.Location.DistanceFrom(PlayerLocations.Last().Location);
                        // Look around rapidly after we've moved the player's last known location
                        if (distance <= 1)
                        {
                            return true;
                        }
                    }

                    return false;

                case RobotAiState.SuspicionShrugOff:
                    if (State == RobotAiState.SuspicionLookAround)
                    {
                        if (Robot.CanSee(Player))
                        {
                            return false;
                        }

                        // If we can't see the player for 10 seconds after walking to the last location and looking 
                        // around, give up and go back to patrol
                        if ((DateTime.Now - TimeMarker).TotalSeconds > 10)
                        {
                            return true;
                        }
                    }

                    return false;

                case RobotAiState.SuspicionCallHeadQuarters:
                    if (State == RobotAiState.Suspicion)
                    {
                        // Call HQ if we can see a player within 50m
                        if (Robot.CanSee(Player) && Robot.Location.DistanceFrom(Player.Location) <= 50)
                            return true;

                        // Call HQ if we've heard a player 4 or 5 times within the last few minutes
                        var now = DateTime.Now;
                        int timesHeard = PlayerLocations
                            .Where(sighting => sighting.Heard)
                            .Count(sighting => (sighting.Time - now).TotalMinutes <= 3);
                        if (timesHeard >= 3 && timesHeard <= 4)
                            return true;
                    }

                    return false;


                // Patrol State Machine
                case RobotAiState.Patrol:
                    // TODO: Check conditions for starting via Suspicion
                    // todo: "Shrug off find and return back to set patrol path at jogging speed"

                    if (State == RobotAiState.PatrolLookAround)
                    {
                        if (Robot.PlayingAnimation == RobotAnimation.LookAround)
                        {
                            // Still playing animation
                            return false;
                        }

                        if ((DateTime.Now - TimeMarker).TotalSeconds <= 5)
                        {
                            // Still standing around
                            return false;
                        }

                        return true;
                    }


                    if (State == RobotAiState.SuspicionShrugOff)
                    {
                        // Wait for robot to return to beginning of patrol before starting patrol
                        if (Robot.Location.DistanceFrom(PlayerLocations.Last().Location) >= 1)
                        {
                            return false;
                        }

                        return true;
                    }

                    return this.State == RobotAiState.Start || this.State == RobotAiState.Suspicion ||
                           this.State == RobotAiState.Inactive;

                case RobotAiState.PatrolMarchToStart:
                    if (State == RobotAiState.Patrol)
                    {
                        // Walk to the beginning if we're far away from it
                        return Robot.Location.DistanceFrom(Robot.PatrolStart) >
                               Robot.Location.DistanceFrom(Robot.PatrolEnd);
                    }

                    return false;

                case RobotAiState.PatrolMarchToEnd:
                    if (State == RobotAiState.Patrol)
                    {
                        // Walk to the ending if we're far away from it
                        return Robot.Location.DistanceFrom(Robot.PatrolStart) <
                               Robot.Location.DistanceFrom(Robot.PatrolEnd);
                    }

                    return false;

                case RobotAiState.PatrolLookAround:
                    if (State == RobotAiState.PatrolMarchToEnd)
                        return Robot.Location.DistanceFrom(Robot.PatrolEnd) <= 0;

                    if (State == RobotAiState.PatrolMarchToStart)
                        return Robot.Location.DistanceFrom(Robot.PatrolStart) <= 0;

                    return false;


                // Hold Up State Machine
                case RobotAiState.HeldUp:
                    if (State == RobotAiState.Alert)
                        return false;

                    if (State == RobotAiState.Inactive)
                        return false;

                    if (State == RobotAiState.Disabled)
                        return false;

                    // Cannot go back to HeldUp after we are put on the ground 
                    if (State == RobotAiState.HeldUpGetDown)
                        return false;

                    // Cannot be held up when being held up
                    if (State == RobotAiState.HeldUp)
                        return false;

                    // While we are refusing, don't switch back to held up
                    if (Robot.PlayingAnimation == RobotAnimation.CoweringRefuse)
                    {
                        if (State == RobotAiState.HeldUpRefuse)
                            return false;
                    }

                    if (Robot.PlayingAnimation == RobotAnimation.CoweringOnGround)
                    {
                        if (State == RobotAiState.HeldUpDemandMarkAmmo)
                            return false;

                        if (State == RobotAiState.HeldUpDemandMarkEnemies)
                            return false;
                    }


                    // Can only hold up Player if it's disabler is <7m from the Head of this Robot
                    return Player.Disabler.Location.DistanceFrom(Robot.Head.Location) <= 7;
                case RobotAiState.HeldUpRefuse:
                    if (State == RobotAiState.HeldUp || State == RobotAiState.HeldUpGetDown)
                    {
                        if (HasHeldUpDemandBeenMade)
                            return true;
                    }

                    return false;
                case RobotAiState.HeldUpDemandMarkAmmo:
                    // Player never asked us to get down. Don't get down
                    if (!MarkItemsRequested)
                        return false;

                    if (State != RobotAiState.HeldUp && State != RobotAiState.HeldUpGetDown)
                        return false;

                    if (HasHeldUpDemandBeenMade)
                        return false;

                    return true;

                case RobotAiState.HeldUpDemandMarkEnemies:
                    // Player never asked us to get down. Don't get down
                    if (!MarkEnemiesRequested)
                        return false;

                    if (State != RobotAiState.HeldUp && State != RobotAiState.HeldUpGetDown)
                        return false;

                    if (HasHeldUpDemandBeenMade)
                        return false;

                    return true;
                case RobotAiState.HeldUpGetDown:
                    if (HasHeldUpSentToGround)
                    {
                        // Allow users to request marking even when sent to ground
                        if (MarkEnemiesRequested || MarkItemsRequested)
                            return false;
                        
                        // When sent to ground, stay on ground
                        return true;   
                    }
                        
                    
                    // Player never asked us to get down. Don't get down
                    if (!GetDownRequested)
                        return false;

                    if (State == RobotAiState.HeldUp)
                        return true;

                    // After we've been put on the ground, we will stay on the ground forever.
                    if (Robot.PlayingAnimation == RobotAnimation.CoweringOnGround)
                    {
                        if (State == RobotAiState.HeldUpDemandMarkAmmo)
                            return true;

                        if (State == RobotAiState.HeldUpDemandMarkEnemies)
                            return true;

                        if (State == RobotAiState.HeldUpRefuse)
                            return true;
                    }

                    return false;

                // Pain State Machine
                case RobotAiState.Hurt:
                    return (this.Robot.Shot || this.Robot.HitWithItem) && this.Robot.Health > 0;
                case RobotAiState.Disabled:
                    if (this.Robot.Head.Shot)
                        return true;
                    if (this.Robot.Health <= 0)
                        return true;
                    if ((this.Robot.Shot || this.Robot.HitWithItem) && this.Robot.Health == 1)
                        return true;
                    return this.Robot.Head.Shot || this.Robot.Health <= 0;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Target PatrolStart when we enter PatrolMarchToStart state
        /// </summary>
        private void ThinkPatrolMarchToStart()
        {
            Robot.Target = Robot.PatrolStart;
        }

        /// <summary>
        /// Target PatrolEnd when we enter PatrolMarchToEnd state
        /// </summary>
        private void ThinkPatrolMarchToEnd()
        {
            Robot.Target = Robot.PatrolEnd;
        }

        /// <summary>
        /// Track Look Around Starting Time
        /// </summary>
        private void ThinkPatrolLookAround()
        {
            TimeMarker = DateTime.Now;
        }

        private void ThinkHurt()
        {
            Robot.PlayingAnimation = RobotAnimation.HurtStagger;
            Robot.Health -= 1;
        }

        private void ThinkDisable()
        {
            Robot.PlayingAnimation = RobotAnimation.RagDoll;
            Robot.Health = 0;
            Robot.DetectionLineOfSight = false;
            Robot.DetectionAudio = false;
        }

        private void ThinkHeldUp()
        {
            Robot.PlayingAnimation = RobotAnimation.CoweringStanding;
            Robot.Target = null;
            Robot.DetectionLineOfSight = false;
            Robot.DetectionAudio = false;
        }

        private void ThinkHeldUpGetDown()
        {
            Robot.PlayingAnimation = RobotAnimation.CoweringOnGround;
            HasHeldUpSentToGround = true;
            GetDownRequested = false;
            Robot.DetectionLineOfSight = false;
            Robot.DetectionAudio = false;
        }

        private void ThinkHeldUpRefuse()
        {
            Robot.PlayingAnimation = RobotAnimation.CoweringRefuse;
            MarkEnemiesRequested = MarkItemsRequested = false;
        }

        private void ThinkHeldUpMarkAmmo()
        {
            MarkItemsRequested = false;
            HasHeldUpDemandBeenMade = true;
            int remainingToMark = 3;
            foreach (var item in Player.NearestItems())
            {
                if (remainingToMark <= 0)
                    return;

                item.MarkForPlayer();
                remainingToMark--;
            }
        }

        private void ThinkHeldUpMarkEnemies()
        {
            MarkEnemiesRequested = false;
            HasHeldUpDemandBeenMade = true;
            int remainingToMark = 3;
            foreach (var robot in Player.NearestRobots())
            {
                if (Robot == robot)
                    continue;

                if (remainingToMark <= 0)
                    return;

                robot.MarkForPlayer();
                remainingToMark--;
            }
        }

        public void Think()
        {
            var handlers = new Tuple<RobotAiState, Action>[]
            {
                new Tuple<RobotAiState, Action>(RobotAiState.Disabled, ThinkDisable),
                new Tuple<RobotAiState, Action>(RobotAiState.Hurt, ThinkHurt),
                new Tuple<RobotAiState, Action>(RobotAiState.Inactive, null),
                new Tuple<RobotAiState, Action>(RobotAiState.Patrol, null),
                new Tuple<RobotAiState, Action>(RobotAiState.PatrolMarchToStart, ThinkPatrolMarchToStart),
                new Tuple<RobotAiState, Action>(RobotAiState.PatrolMarchToEnd, ThinkPatrolMarchToEnd),
                new Tuple<RobotAiState, Action>(RobotAiState.PatrolLookAround, ThinkPatrolLookAround),
                new Tuple<RobotAiState, Action>(RobotAiState.HeldUp, ThinkHeldUp),
                new Tuple<RobotAiState, Action>(RobotAiState.HeldUpGetDown, ThinkHeldUpGetDown),
                new Tuple<RobotAiState, Action>(RobotAiState.HeldUpRefuse, ThinkHeldUpRefuse),
                new Tuple<RobotAiState, Action>(RobotAiState.HeldUpDemandMarkAmmo, ThinkHeldUpMarkAmmo),
                new Tuple<RobotAiState, Action>(RobotAiState.HeldUpDemandMarkEnemies, ThinkHeldUpMarkEnemies),
            };

            foreach (var handler in handlers)
            {
                if (!Can(handler.Item1))
                {
                    continue;
                }

                State = handler.Item1;
                handler.Item2?.Invoke();
                return;
            }
        }
    }
}