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
            robot.DistanceFromEndingOfPatrol = 0;
            robot.DistanceFromBeginningOfPatrol = 100;

            Assert.True(ai.Can(RobotAiState.PatrolMarchToStart));
        }

        [Test]
        public void TestPatrolCanMoveToPatrolWalkToEnding()
        {
            RobotAi ai = new MockRobotAi();
            ai.State = RobotAiState.Patrol;

            MockRobot robot = (MockRobot) ai.Robot;
            robot.DistanceFromEndingOfPatrol = 100;
            robot.DistanceFromBeginningOfPatrol = 0;

            Assert.True(ai.Can(RobotAiState.PatrolMarchToEnd));
        }

        [Test]
        public void TestPatrolReachedWalkingDestinationCanMoveToLookAround()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;

            // Reached ending
            ai.State = RobotAiState.PatrolMarchToEnd;
            robot.DistanceFromEndingOfPatrol = 0;
            robot.DistanceFromBeginningOfPatrol = 100;
            Assert.True(ai.Can(RobotAiState.PatrolLookAround));

            // Reached beginning
            ai.State = RobotAiState.PatrolMarchToStart;
            robot.DistanceFromEndingOfPatrol = 100;
            robot.DistanceFromBeginningOfPatrol = 0;
            Assert.True(ai.Can(RobotAiState.PatrolLookAround));
        }

        [Test]
        public void TestPatrolNotReachedWalkingDestinationCannotMoveToLookAround()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;

            // Still need to continue walking
            ai.State = RobotAiState.PatrolMarchToStart;
            robot.DistanceFromEndingOfPatrol = 50;
            robot.DistanceFromBeginningOfPatrol = 50;
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

            disabler.Location = new MockLocation(0, 0, 0);
            robot.Head.Location = new MockLocation(0, 0, 0);

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
            robot.Head.Location = new MockLocation(0, 0, 0);

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
            player.Location = new MockLocation(0, 0, 0);
            robot.Location = new MockLocation(0, 0, 0);
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
            robot.Location = new MockLocation(0, 0, 0);
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
            MockPlayer player = (MockPlayer) ai.Player;
            MockRobot robot = (MockRobot) ai.Robot;

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
            MockPlayer player = (MockPlayer) ai.Player;
            MockRobot robot = (MockRobot) ai.Robot;


            robot.CanSeePlayer = false;
            robot.DetectionLineOfSight = true;
            ai.PlayerLocations.Add(new PlayerLocation(DateTime.Now, new MockLocation(10, 10, 10)));

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
            robot.Location = new MockLocation(0, 0, 0);
            robot.CanSeePlayer = true;
            robot.DetectionLineOfSight = true;
            ai.PlayerLocations.Add(new PlayerLocation(DateTime.Now, player.Location));

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

            player.Location = new MockLocation(0, 0, 0);
            robot.Location = new MockLocation(0, 0, 0);
            robot.CanSeePlayer = true;
            robot.DetectionLineOfSight = true;
            ai.PlayerLocations.Add(new PlayerLocation(DateTime.Now, player.Location));

            ai.State = RobotAiState.AlertAttack;
            Assert.False(ai.Can(RobotAiState.AlertFollowUp));

            ai.State = RobotAiState.AlertReposition;
            Assert.False(ai.Can(RobotAiState.AlertFollowUp));
        }


        [Test]
        public void TestAlertRepositionCanMoveToAttackWhenReachedTargetAndAfter2Seconds()
        {
            RobotAi ai = new MockRobotAi();
            MockPlayer player = (MockPlayer) ai.Player;
            MockRobot robot = (MockRobot) ai.Robot;

            robot.Target = robot.Location = new MockLocation(0, 0, 0);
            ai.TimeMarker = DateTime.Now - TimeSpan.FromSeconds(3);

            ai.State = RobotAiState.AlertReposition;
            Assert.True(ai.Can(RobotAiState.AlertAttack));
        }

        [Test]
        public void TestAlertRepositionCannotMoveToAttackWhenReachedTargetAndBefore2Seconds()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;

            robot.Target = robot.Location = new MockLocation(0, 0, 0);
            ai.TimeMarker = DateTime.Now - TimeSpan.FromSeconds(1);

            ai.State = RobotAiState.AlertReposition;
            Assert.False(ai.Can(RobotAiState.AlertAttack));
        }

        [Test]
        public void TestAlertRepositionCannotMoveToAttackBeforeReachedTargetAndAfter2Seconds()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;

            robot.Location = new MockLocation(0, 0, 0);
            robot.Target = new MockLocation(100, 100, 100);
            ai.TimeMarker = DateTime.Now - TimeSpan.FromSeconds(10);

            ai.State = RobotAiState.AlertReposition;
            Assert.False(ai.Can(RobotAiState.AlertAttack));
        }
    }
}