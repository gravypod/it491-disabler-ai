using System;
using DisablerAi;
using NUnit.Framework;

namespace TestRobot
{
    [TestFixture]
    public class CanEnterAlertStates
    {
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

    }
}