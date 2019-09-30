using System;
using DisablerAi;
using NUnit.Framework;

namespace TestRobot
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void TestStartingStateCanMoveTo()
        {
            RobotAi ai = new MockRobotAi();
            ai.State = RobotAiState.Start;

            // Cannot go back to start
            Assert.False(ai.Can(RobotAiState.Start));
            Assert.True(ai.Can(RobotAiState.Inactive));
            Assert.True(ai.Can(RobotAiState.Patrol));
        }

        // Patrol Unit Tests

        [Test]
        public void TestInactiveCanMoveToPatrol()
        {
            RobotAi ai = new MockRobotAi();
            ai.State = RobotAiState.Inactive;
            Assert.True(ai.Can(RobotAiState.Patrol));
        }

        [Test]
        public void TestPatrolCanMoveToPatrolWalkToBeginning()
        {
            RobotAi ai = new MockRobotAi();
            ai.State = RobotAiState.Patrol;

            MockRobot robot = (MockRobot) ai.Robot;
            robot.Location = new MockLocation(1, 1, 1);
            robot.PatrolStart = new MockLocation(100, 100, 100);
            robot.PatrolEnd = new MockLocation(1, 1, 1);

            Assert.True(ai.Can(RobotAiState.PatrolMarchToStart));
        }

        [Test]
        public void TestPatrolCanMoveToPatrolWalkToEnding()
        {
            RobotAi ai = new MockRobotAi();
            ai.State = RobotAiState.Patrol;

            MockRobot robot = (MockRobot) ai.Robot;
            robot.Location = new MockLocation(1, 1, 1);
            robot.PatrolEnd = new MockLocation(100, 100, 100);
            robot.PatrolStart = new MockLocation(1, 1, 1);

            Assert.True(ai.Can(RobotAiState.PatrolMarchToEnd));
        }

        [Test]
        public void TestPatrolReachedWalkingDestinationCanMoveToLookAround()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            robot.Location = new MockLocation(1, 1, 1);

            // Reached ending
            ai.State = RobotAiState.PatrolMarchToEnd;
            robot.PatrolStart = new MockLocation(100, 100, 100);
            robot.PatrolEnd = new MockLocation(1, 1, 1);
            Assert.True(ai.Can(RobotAiState.PatrolLookAround));

            // Reached beginning
            ai.State = RobotAiState.PatrolMarchToStart;
            robot.PatrolEnd = new MockLocation(100, 100, 100);
            robot.PatrolStart = new MockLocation(1, 1, 1);
            Assert.True(ai.Can(RobotAiState.PatrolLookAround));
        }

        [Test]
        public void TestPatrolNotReachedWalkingDestinationCannotMoveToLookAround()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;

            // Still need to continue walking
            ai.State = RobotAiState.PatrolMarchToStart;
            robot.Location = new MockLocation(50, 50, 50);
            robot.PatrolEnd = new MockLocation(100, 100, 100);
            robot.PatrolStart = new MockLocation(1, 1, 1);
            Assert.False(ai.Can(RobotAiState.PatrolLookAround));
        }

        [Test]
        public void TestLookAroundJustStartedAndCannotMoveToPatrol()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;

            // Still need to continue walking
            ai.State = RobotAiState.PatrolLookAround;
            ai.TimeMarker = DateTime.Now;
            robot.PlayingAnimation = RobotAnimation.LookAround;
            Assert.False(ai.Can(RobotAiState.Patrol));
        }

        [Test]
        public void TestLookAroundAnimationStillPlayingCannotMoveToPatrol()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;

            // Still need to continue walking
            ai.State = RobotAiState.PatrolLookAround;
            robot.PlayingAnimation = RobotAnimation.LookAround;
            ai.TimeMarker = DateTime.Now - TimeSpan.FromSeconds(10);
            Assert.False(ai.Can(RobotAiState.Patrol));
        }

        [Test]
        public void TestLookAroundAnimationFinishedButTimeoutHasNotElapsedCannotMoveToPatrol()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;

            // Still need to continue walking
            ai.State = RobotAiState.PatrolLookAround;
            robot.PlayingAnimation = RobotAnimation.None;
            ai.TimeMarker = DateTime.Now;
            Assert.False(ai.Can(RobotAiState.Patrol));
        }

        [Test]
        public void TestLookAroundAndTimeoutFinishedCanMoveToPatrol()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;

            // Still need to continue walking
            ai.State = RobotAiState.PatrolLookAround;
            robot.PlayingAnimation = RobotAnimation.None;
            ai.TimeMarker = DateTime.Now - TimeSpan.FromDays(10);
            Assert.True(ai.Can(RobotAiState.Patrol));
        }

        // Class Hold Up Unit Tests
        [Test]
        public void TestDisabledCannotMoveToHeldUp()
        {
            RobotAi ai = new MockRobotAi();
            ai.State = RobotAiState.Disabled;
            Assert.False(ai.Can(RobotAiState.HeldUp));
        }

        [Test]
        public void TestAlertCannotMoveToHeldUp()
        {
            RobotAi ai = new MockRobotAi();
            ai.State = RobotAiState.Alert;
            Assert.False(ai.Can(RobotAiState.HeldUp));
        }

        [Test]
        public void TestInactiveCannotMoveToHeldUp()
        {
            RobotAi ai = new MockRobotAi();
            ai.State = RobotAiState.Inactive;
            Assert.False(ai.Can(RobotAiState.HeldUp));
        }

        [Test]
        public void TestCanHoldUpOnPatrolWithin7Meters()
        {
            RobotAi ai = new MockRobotAi();
            ai.State = RobotAiState.Patrol;
            MockDisabler disabler = (MockDisabler) ai.Player.Disabler;
            MockRobot robot = (MockRobot) ai.Robot;

            disabler.Location = new MockLocation(1, 1, 1);
            robot.Head.Location = new MockLocation(1, 1, 1);

            Assert.True(ai.Can(RobotAiState.HeldUp));
        }

        [Test]
        public void TestCannotHoldUpOnPatrolOver7Meters()
        {
            RobotAi ai = new MockRobotAi();
            ai.State = RobotAiState.Patrol;
            MockDisabler disabler = (MockDisabler) ai.Player.Disabler;
            MockRobot robot = (MockRobot) ai.Robot;

            disabler.Location = new MockLocation(10, 10, 10);
            robot.Head.Location = new MockLocation(1, 1, 1);

            Assert.False(ai.Can(RobotAiState.HeldUp));
        }

        [Test]
        public void TestHeldUpCanMarkItem()
        {
            RobotAi ai = new MockRobotAi();
            ai.State = RobotAiState.HeldUp;
            Assert.True(ai.Can(RobotAiState.HeldUpDemandMarkAmmo));
        }


        [Test]
        public void TestHeldUpCanMarkEnemies()
        {
            RobotAi ai = new MockRobotAi();
            ai.State = RobotAiState.HeldUp;
            Assert.True(ai.Can(RobotAiState.HeldUpDemandMarkEnemies));
        }

        [Test]
        public void TestHeldUpCannotMarkTwice()
        {
            RobotAi ai = new MockRobotAi();
            ai.State = RobotAiState.HeldUp;
            ai.HoldUpDemandMade = true;
            Assert.False(ai.Can(RobotAiState.HeldUpDemandMarkEnemies));
            Assert.False(ai.Can(RobotAiState.HeldUpDemandMarkAmmo));
        }

        [Test]
        public void TestHeldUpRefuse()
        {
            RobotAi ai = new MockRobotAi();
            ai.State = RobotAiState.HeldUp;
            ai.HoldUpDemandMade = true;
            Assert.True(ai.Can(RobotAiState.HeldUpRefuse));
        }

        [Test]
        public void TestHeldUpSubStatesCanMoveToHeldUp()
        {
            RobotAi ai = new MockRobotAi();
            ai.HoldUpDemandMade = false;

            ai.State = RobotAiState.HeldUpRefuse;
            Assert.True(ai.Can(RobotAiState.HeldUp));

            ai.State = RobotAiState.HeldUpDemandMarkAmmo;
            Assert.True(ai.Can(RobotAiState.HeldUp));

            ai.State = RobotAiState.HeldUpDemandMarkEnemies;
            Assert.True(ai.Can(RobotAiState.HeldUp));
        }

        [Test]
        public void TestHeldUpSubStatesCanMoveToGetDownIfPlayingAnimation()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;

            robot.PlayingAnimation = RobotAnimation.CoweringOnGround;
            ai.HoldUpDemandMade = false;

            ai.State = RobotAiState.HeldUpRefuse;
            Assert.True(ai.Can(RobotAiState.HeldUpGetDown));

            ai.State = RobotAiState.HeldUpDemandMarkAmmo;
            Assert.True(ai.Can(RobotAiState.HeldUpGetDown));

            ai.State = RobotAiState.HeldUpDemandMarkEnemies;
            Assert.True(ai.Can(RobotAiState.HeldUpGetDown));
        }

        [Test]
        public void TestHeldUpSubStatesCannotMoveToGetDownIfNotPlayingAnimation()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;

            robot.PlayingAnimation = RobotAnimation.None;
            ai.HoldUpDemandMade = false;

            ai.State = RobotAiState.HeldUpRefuse;
            Assert.False(ai.Can(RobotAiState.HeldUpGetDown));

            ai.State = RobotAiState.HeldUpDemandMarkAmmo;
            Assert.False(ai.Can(RobotAiState.HeldUpGetDown));

            ai.State = RobotAiState.HeldUpDemandMarkEnemies;
            Assert.False(ai.Can(RobotAiState.HeldUpGetDown));
        }

        [Test]
        public void TestHeldUpGetDownCannotMoveToHeldUp()
        {
            RobotAi ai = new MockRobotAi();
            ai.HoldUpDemandMade = false;
            ai.State = RobotAiState.HeldUpGetDown;
            Assert.False(ai.Can(RobotAiState.HeldUp));
        }

        // Alert Unit Tests
        [Test]
        public void TestPatrolMoveToAlert()
        {
            RobotAi ai = new MockRobotAi();
            MockPlayer player = (MockPlayer) ai.Player;
            MockRobot robot = (MockRobot) ai.Robot;

            ai.State = RobotAiState.Patrol;
            player.Location = new MockLocation(1, 1, 1);
            robot.Location = new MockLocation(1, 1, 1);
            robot.DetectionLineOfSight = true;
            robot.CanSeePlayer = true;

            Assert.True(ai.Can(RobotAiState.Alert));
        }

        [Test]
        public void TestPatrolCannotMoveToAlertWhenTooFarAway()
        {
            RobotAi ai = new MockRobotAi();
            MockPlayer player = (MockPlayer) ai.Player;
            MockRobot robot = (MockRobot) ai.Robot;

            ai.State = RobotAiState.Patrol;
            player.Location = new MockLocation(100, 100, 100);
            robot.Location = new MockLocation(1, 1, 1);
            robot.DetectionLineOfSight = true;
            robot.CanSeePlayer = true;

            Assert.False(ai.Can(RobotAiState.Alert));
        }

        [Test]
        public void TestAlertCanMoveToCallHeadQuartersAfter2Seconds()
        {
            RobotAi ai = new MockRobotAi();
            ai.State = RobotAiState.Alert;
            ai.TimeMarker = DateTime.Now - TimeSpan.FromSeconds(3);
            Assert.True(ai.Can(RobotAiState.AlertCallHeadQuarters));
        }


        [Test]
        public void TestAlertCannotMoveToCallHeadQuartersBefore2Seconds()
        {
            RobotAi ai = new MockRobotAi();
            ai.State = RobotAiState.Alert;
            ai.TimeMarker = DateTime.Now - TimeSpan.FromSeconds(1);
            Assert.False(ai.Can(RobotAiState.AlertCallHeadQuarters));
        }

        [Test]
        public void TestAlertCallHeadQuartersCanMoveToAttack()
        {
            RobotAi ai = new MockRobotAi();
            ai.State = RobotAiState.AlertCallHeadQuarters;
            Assert.True(ai.Can(RobotAiState.AlertAttack));
        }

        [Test]
        public void TestAlertAttackCanMoveToAlertRepositionAfter1To3Bursts()
        {
            RobotAi ai = new MockRobotAi();

            ai.State = RobotAiState.AlertAttack;

            ai.BurstsFired = 1;
            Assert.True(ai.Can(RobotAiState.AlertReposition));

            ai.BurstsFired = 2;
            Assert.True(ai.Can(RobotAiState.AlertReposition));

            ai.BurstsFired = 3;
            Assert.True(ai.Can(RobotAiState.AlertReposition));
        }

        [Test]
        public void TestAlertAttackCannotMoveToAlertRepositionBeforeBursting()
        {
            RobotAi ai = new MockRobotAi();

            ai.State = RobotAiState.AlertAttack;
            ai.BurstsFired = 0;

            Assert.False(ai.Can(RobotAiState.AlertReposition));
        }


        [Test]
        public void TestAlertAttackCannotMoveToAlertRepositionAfterBursting4Times()
        {
            RobotAi ai = new MockRobotAi();
            ai.State = RobotAiState.AlertAttack;
            ai.BurstsFired = 4;
            Assert.False(ai.Can(RobotAiState.AlertReposition));
        }

        [Test]
        public void TestAlertAttackAndAlertRepositionLostSightOfPlayerCanMoveToFollowUp()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;


            robot.CanSeePlayer = false;
            robot.DetectionLineOfSight = true;
            ai.PlayerLocations.Add(new PlayerLocation(DateTime.Now, new MockLocation(10, 10, 10), true, true));

            ai.State = RobotAiState.AlertAttack;
            Assert.True(ai.Can(RobotAiState.AlertFollowUp));

            ai.State = RobotAiState.AlertReposition;
            Assert.True(ai.Can(RobotAiState.AlertFollowUp));
        }

        [Test]
        public void TestAlertAttackAndAlertRepositionTooFarOfPlayerCanMoveToFollowUp()
        {
            RobotAi ai = new MockRobotAi();
            MockPlayer player = (MockPlayer) ai.Player;
            MockRobot robot = (MockRobot) ai.Robot;

            player.Location = new MockLocation(100, 100, 100);
            robot.Location = new MockLocation(1, 1, 1);
            robot.CanSeePlayer = true;
            robot.DetectionLineOfSight = true;
            ai.PlayerLocations.Add(new PlayerLocation(DateTime.Now, player.Location, true, true));

            ai.State = RobotAiState.AlertAttack;
            Assert.True(ai.Can(RobotAiState.AlertFollowUp));

            ai.State = RobotAiState.AlertReposition;
            Assert.True(ai.Can(RobotAiState.AlertFollowUp));
        }

        [Test]
        public void TestAlertAttackAndAlertCannotRepositionWhenInSightAndInRange()
        {
            RobotAi ai = new MockRobotAi();
            MockPlayer player = (MockPlayer) ai.Player;
            MockRobot robot = (MockRobot) ai.Robot;

            player.Location = new MockLocation(1, 1, 1);
            robot.Location = new MockLocation(1, 1, 1);
            robot.CanSeePlayer = true;
            robot.DetectionLineOfSight = true;
            ai.PlayerLocations.Add(new PlayerLocation(DateTime.Now, player.Location, true, true));

            ai.State = RobotAiState.AlertAttack;
            Assert.False(ai.Can(RobotAiState.AlertFollowUp));

            ai.State = RobotAiState.AlertReposition;
            Assert.False(ai.Can(RobotAiState.AlertFollowUp));
        }


        [Test]
        public void TestAlertRepositionCanMoveToAttackWhenReachedTargetAndAfter2Seconds()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;

            robot.Target = robot.Location = new MockLocation(1, 1, 1);
            ai.TimeMarker = DateTime.Now - TimeSpan.FromSeconds(3);

            ai.State = RobotAiState.AlertReposition;
            Assert.True(ai.Can(RobotAiState.AlertAttack));
        }

        [Test]
        public void TestAlertRepositionCannotMoveToAttackWhenReachedTargetAndBefore2Seconds()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;

            robot.Target = robot.Location = new MockLocation(1, 1, 1);
            ai.TimeMarker = DateTime.Now - TimeSpan.FromSeconds(1);

            ai.State = RobotAiState.AlertReposition;
            Assert.False(ai.Can(RobotAiState.AlertAttack));
        }

        [Test]
        public void TestAlertRepositionCannotMoveToAttackBeforeReachedTargetAndAfter2Seconds()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;

            robot.Location = new MockLocation(1, 1, 1);
            robot.Target = new MockLocation(100, 100, 100);
            ai.TimeMarker = DateTime.Now - TimeSpan.FromSeconds(10);

            ai.State = RobotAiState.AlertReposition;
            Assert.False(ai.Can(RobotAiState.AlertAttack));
        }


        // Disabled Unit Tests
        [Test]
        public void TestNoHealthMoveToDisabled()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;

            robot.Health = 0;
            ai.State = RobotAiState.Patrol;
            Assert.True(ai.Can(RobotAiState.Disabled));
        }

        [Test]
        public void TestNegativeHealthMoveToDisabled()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;

            robot.Health = -1;
            ai.State = RobotAiState.Patrol;
            Assert.True(ai.Can(RobotAiState.Disabled));
        }

        [Test]
        public void TestHeadShotMoveToDisabled()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;

            robot.Head.Shot = true;
            ai.State = RobotAiState.Patrol;
            Assert.True(ai.Can(RobotAiState.Disabled));
        }


        [Test]
        public void TestHealthyAndNoHeadShotShouldNotMoveToDisabled()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;

            robot.Head.Shot = false;
            robot.Health = 100;
            ai.State = RobotAiState.Patrol;
            Assert.False(ai.Can(RobotAiState.Disabled));
        }

        // Hurt Unit Test


        [Test]
        public void TestHitToHurt()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;

            robot.HitWithItem = true;
            robot.Health = 100;
            ai.State = RobotAiState.Patrol;
            Assert.True(ai.Can(RobotAiState.Hurt));
        }

        [Test]
        public void TestShotToHurt()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;

            robot.Shot = true;
            robot.Health = 100;
            ai.State = RobotAiState.Patrol;
            Assert.True(ai.Can(RobotAiState.Hurt));
        }

        [Test]
        public void TestNotShotAndNotHitShouldNotHurt()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;

            robot.Shot = false;
            robot.HitWithItem = false;
            robot.Health = 100;
            ai.State = RobotAiState.Patrol;
            Assert.False(ai.Can(RobotAiState.Hurt));
        }

        // Suspicion Unit Tests
        [Test]
        public void TestPatrolCanMoveToSuspicionWhenSeenAndGreaterThan15MetersAwayAndLessThan50MetersAway()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;

            robot.CanHearPlayer = false;
            robot.CanSeePlayer = true;
            robot.DetectionLineOfSight = true;
            robot.DetectionAudio = true;

            robot.Location = new MockLocation(21, 1, 1);
            player.Location = new MockLocation(1, 1, 1);

            ai.State = RobotAiState.Patrol;
            Assert.True(ai.Can(RobotAiState.Suspicion));
        }

        [Test]
        public void TestPatrolCannotMoveToSuspicionWhenSeenAndLessThan15Meters()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;

            robot.CanHearPlayer = false;
            robot.CanSeePlayer = true;
            robot.DetectionLineOfSight = true;
            robot.DetectionAudio = true;

            robot.Location = new MockLocation(11, 1, 1);
            player.Location = new MockLocation(1, 1, 1);

            ai.State = RobotAiState.Patrol;
            Assert.False(ai.Can(RobotAiState.Suspicion));
        }

        [Test]
        public void TestPatrolCanMoveToSuspicionWhenHeardAndUnder40MetersAway()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;

            robot.CanHearPlayer = true;
            robot.CanSeePlayer = false;
            robot.DetectionLineOfSight = true;
            robot.DetectionAudio = true;

            robot.Location = new MockLocation(21, 1, 1);
            player.Location = new MockLocation(1, 1, 1);

            ai.State = RobotAiState.Patrol;
            Assert.True(ai.Can(RobotAiState.Suspicion));
        }

        [Test]
        public void TestSuspicionCanMoveToCallHeadQuartersWhenSeenUnder50MetersAway()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;

            robot.CanHearPlayer = false;
            robot.CanSeePlayer = true;
            robot.DetectionLineOfSight = true;
            robot.DetectionAudio = true;

            robot.Location = new MockLocation(21, 1, 1);
            player.Location = new MockLocation(1, 1, 1);

            ai.State = RobotAiState.Suspicion;
            Assert.True(ai.Can(RobotAiState.SuspicionCallHeadQuarters));
        }


        [Test]
        public void TestSuspicionCanMoveToCallHeadQuartersWhenHeard3Times()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;

            robot.CanHearPlayer = false;
            robot.CanSeePlayer = false;
            robot.DetectionLineOfSight = true;
            robot.DetectionAudio = true;

            robot.Location = new MockLocation(999, 999, 999);
            player.Location = new MockLocation(1, 1, 1);

            for (int i = 0; i < 3; i++)
                ai.PlayerLocations.Add(new PlayerLocation(DateTime.Now, new MockLocation(), false, true));

            ai.State = RobotAiState.Suspicion;
            Assert.True(ai.Can(RobotAiState.SuspicionCallHeadQuarters));
        }


        [Test]
        public void TestSuspicionCannotMoveToCallHeadQuartersWhenHeard2Times()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;

            robot.CanHearPlayer = false;
            robot.CanSeePlayer = false;
            robot.DetectionLineOfSight = true;
            robot.DetectionAudio = true;

            robot.Location = new MockLocation(999, 999, 999);
            player.Location = new MockLocation(1, 1, 1);

            for (int i = 0; i < 2; i++)
                ai.PlayerLocations.Add(new PlayerLocation(DateTime.Now, new MockLocation(), false, true));

            ai.State = RobotAiState.Suspicion;
            Assert.False(ai.Can(RobotAiState.SuspicionCallHeadQuarters));
        }

        [Test]
        public void TestSuspicionCannotMoveToCallHeadQuartersWhenNeverSeenOrHeard()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;

            robot.CanHearPlayer = false;
            robot.CanSeePlayer = false;
            robot.DetectionLineOfSight = true;
            robot.DetectionAudio = true;

            robot.Location = new MockLocation(999, 999, 999);
            player.Location = new MockLocation(1, 1, 1);

            ai.State = RobotAiState.Suspicion;
            Assert.False(ai.Can(RobotAiState.SuspicionCallHeadQuarters));
        }


        [Test]
        public void TestSuspicionCanMoveToFollowUp()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;

            robot.CanHearPlayer = true;
            robot.CanSeePlayer = false;
            robot.DetectionLineOfSight = true;
            robot.DetectionAudio = true;

            robot.Location = new MockLocation(999, 999, 999);
            player.Location = new MockLocation(1, 1, 1);

            ai.PlayerLocations.Add(new PlayerLocation(DateTime.Now, new MockLocation(), false, true));

            ai.State = RobotAiState.Suspicion;
            Assert.True(ai.Can(RobotAiState.SuspicionFollowUp));
        }

        [Test]
        public void TestSuspicionFollowUpCanMoveToLookAround()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;

            robot.CanHearPlayer = true;
            robot.CanSeePlayer = false;
            robot.DetectionLineOfSight = true;
            robot.DetectionAudio = true;

            robot.Location = new MockLocation(999, 999, 999);
            player.Location = new MockLocation(1, 1, 1);

            ai.PlayerLocations.Add(new PlayerLocation(DateTime.Now, robot.Location, false, true));

            ai.State = RobotAiState.SuspicionFollowUp;
            Assert.True(ai.Can(RobotAiState.SuspicionLookAround));
        }


        [Test]
        public void TestSuspicionFollowUpCannotMoveToLookAroundIfWeHaveNotReachedTarget()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;

            robot.CanHearPlayer = true;
            robot.CanSeePlayer = false;
            robot.DetectionLineOfSight = true;
            robot.DetectionAudio = true;

            robot.Location = new MockLocation(999, 999, 999);
            player.Location = new MockLocation(1, 1, 1);

            ai.PlayerLocations.Add(new PlayerLocation(DateTime.Now, new MockLocation(10, 10, 10), false, true));

            ai.State = RobotAiState.SuspicionFollowUp;
            Assert.False(ai.Can(RobotAiState.SuspicionLookAround));
        }


        [Test]
        public void TestSuspicionLookAroundCanShrugOffIfCannotSeePlayerAtLastLocation()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;

            robot.CanHearPlayer = false;
            robot.CanSeePlayer = false;
            robot.DetectionLineOfSight = true;
            robot.DetectionAudio = true;

            robot.Location = new MockLocation(999, 999, 999);
            player.Location = new MockLocation(1, 1, 1);

            ai.PlayerLocations.Add(new PlayerLocation(DateTime.Now, robot.Location, false, true));
            ai.TimeMarker = DateTime.Now - TimeSpan.FromMinutes(1);

            ai.State = RobotAiState.SuspicionLookAround;
            Assert.True(ai.Can(RobotAiState.SuspicionShrugOff));
        }

        [Test]
        public void TestSuspicionLookAroundCannotShrugOffIfCanSeePlayerAtLastLocation()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;

            robot.CanHearPlayer = false;
            robot.CanSeePlayer = true;
            robot.DetectionLineOfSight = true;
            robot.DetectionAudio = true;

            robot.Location = new MockLocation(999, 999, 999);
            player.Location = new MockLocation(1, 1, 1);

            ai.PlayerLocations.Add(new PlayerLocation(DateTime.Now, robot.Location, false, true));
            ai.TimeMarker = DateTime.Now - TimeSpan.FromMinutes(1);

            ai.State = RobotAiState.SuspicionLookAround;
            Assert.False(ai.Can(RobotAiState.SuspicionShrugOff));
        }


        [Test]
        public void TestSuspicionLookAroundCannotShrugOffIfLookAroundTimeoutNotExpired()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;

            robot.CanHearPlayer = false;
            robot.CanSeePlayer = false;
            robot.DetectionLineOfSight = true;
            robot.DetectionAudio = true;

            robot.Location = new MockLocation(999, 999, 999);
            player.Location = new MockLocation(1, 1, 1);

            ai.PlayerLocations.Add(new PlayerLocation(DateTime.Now, robot.Location, false, true));
            ai.TimeMarker = DateTime.Now;

            ai.State = RobotAiState.SuspicionLookAround;
            Assert.False(ai.Can(RobotAiState.SuspicionShrugOff));
        }

        [Test]
        public void TestSuspicionShrugOffCanMoveToPatrol()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;

            robot.CanHearPlayer = false;
            robot.CanSeePlayer = false;
            robot.DetectionLineOfSight = true;
            robot.DetectionAudio = true;

            robot.Location = new MockLocation(999, 999, 999);
            player.Location = new MockLocation(1, 1, 1);

            ai.PlayerLocations.Add(new PlayerLocation(DateTime.Now, robot.Location, false, true));
            ai.TimeMarker = DateTime.Now - TimeSpan.FromMinutes(1);

            ai.State = RobotAiState.SuspicionShrugOff;
            Assert.True(ai.Can(RobotAiState.Patrol));
        }
        
        // Searching Unit Tests
        
        

        [Test]
        public void TestSuspicionCallHeadQuartersCanMoveToSearching()
        {
            RobotAi ai = new MockRobotAi();

            ai.State = RobotAiState.SuspicionCallHeadQuarters;
            Assert.True(ai.Can(RobotAiState.Searching));
        }
        

        [Test]
        public void TestAlertFollowUpCannotSeeCannotHearAfter1MinuteCanMoveToSearching()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;

            robot.CanHearPlayer = false;
            robot.CanSeePlayer = false;

            ai.TimeMarker = DateTime.Now - TimeSpan.FromMinutes(10);

            ai.State = RobotAiState.AlertFollowUp;
            Assert.True(ai.Can(RobotAiState.Searching));
        }
    }
}