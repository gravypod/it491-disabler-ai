using System;
using DisablerAi;
using NUnit.Framework;

namespace TestRobot
{
    [TestFixture]
    public class ThinkAlert
    {
        [Test]
        public void TestAlertFromPatrol()
        {
            RobotAi ai = new MockRobotAi();
            MockPlayer player = (MockPlayer) ai.Player;
            MockRobot robot = (MockRobot) ai.Robot;

            // The Robot can see the player within 15m via Line of Sight
            robot.Head.Location = robot.Location = new MockLocation(0, 0, 0);
            player.Disabler.Location = player.Location = new MockLocation(14, 0, 0);
            robot.DetectionLineOfSight = robot.CanSeePlayer = true;
            ai.State = RobotAiState.Patrol;
            ai.Think();

            Assert.AreEqual(RobotAiState.Alert, ai.State);
        }

        [Test]
        public void TestAlertCallHeadQuartersAfterTwoSeconds()
        {
            RobotAi ai = new MockRobotAi();
            MockPlayer player = (MockPlayer) ai.Player;
            MockRobot robot = (MockRobot) ai.Robot;

            robot.DetectionLineOfSight = robot.CanSeePlayer = false;
            ai.TimeMarker = (DateTime.Now - TimeSpan.FromSeconds(2.1));
            ai.State = RobotAiState.Alert;
            ai.Think();

            Assert.AreEqual(RobotAiState.AlertCallHeadQuarters, ai.State);
            Assert.AreEqual(RobotAnimation.AlertCallHeadQuarters, robot.PlayingAnimation);
        }

        [Test]
        public void TestCallingHeadquartersStaysUntilAnimationIsFinished()
        {
            RobotAi ai = new MockRobotAi();
            MockPlayer player = (MockPlayer) ai.Player;
            MockRobot robot = (MockRobot) ai.Robot;

            robot.DetectionLineOfSight = robot.CanSeePlayer = false;
            robot.PlayingAnimation = RobotAnimation.AlertCallHeadQuarters;
            ai.State = RobotAiState.AlertCallHeadQuarters;
            ai.Think();

            Assert.AreEqual(RobotAiState.AlertCallHeadQuarters, ai.State);
        }

        [Test]
        public void TestFinishedCallingHeadquartersStartsAttacking()
        {
            RobotAi ai = new MockRobotAi();
            MockPlayer player = (MockPlayer) ai.Player;
            MockRobot robot = (MockRobot) ai.Robot;

            robot.DetectionLineOfSight = robot.CanSeePlayer = false;
            robot.PlayingAnimation = RobotAnimation.None;
            ai.State = RobotAiState.AlertCallHeadQuarters;
            ai.Think();

            Assert.AreEqual(RobotAiState.AlertAttack, ai.State);
        }

        [Test]
        public void TestAttackingFired4BurstsShouldReposition()
        {
            RobotAi ai = new MockRobotAi();
            MockPlayer player = (MockPlayer) ai.Player;
            MockRobot robot = (MockRobot) ai.Robot;

            MockLocation repositionTarget = new MockLocation(-999, -999, -999);
            robot.Location = new MockLocation(0, 0, 0, repositionTarget);
            ai.BurstsFired = 4;
            ai.State = RobotAiState.AlertAttack;
            ai.Think();


            Assert.AreEqual(RobotAiState.AlertReposition, ai.State);
            Assert.True(robot.Target.Equals(repositionTarget));
        }

        [Test]
        public void TestAttackingThreeSecondsPassedShouldReposition()
        {
            RobotAi ai = new MockRobotAi();
            MockPlayer player = (MockPlayer) ai.Player;
            MockRobot robot = (MockRobot) ai.Robot;

            MockLocation repositionTarget = new MockLocation(-999, -999, -999);
            robot.Location = new MockLocation(0, 0, 0, repositionTarget);
            ai.BurstsFired = 0;
            ai.State = RobotAiState.AlertAttack;
            ai.TimeMarker = DateTime.Now - TimeSpan.FromSeconds(3);
            ai.Think();


            Assert.AreEqual(RobotAiState.AlertReposition, ai.State);
            Assert.True(robot.Target.Equals(repositionTarget));
        }

        [Test]
        public void TestRepositionBackToAttacking()
        {
            RobotAi ai = new MockRobotAi();
            MockPlayer player = (MockPlayer) ai.Player;
            MockRobot robot = (MockRobot) ai.Robot;

            MockLocation repositionTarget = new MockLocation(-999, -999, -999);
            robot.Location = new MockLocation(0, 0, 0, repositionTarget);
            ai.BurstsFired = 0;
            ai.State = RobotAiState.AlertAttack;
            ai.TimeMarker = DateTime.Now - TimeSpan.FromSeconds(3);
            ai.Think();
            Assert.AreEqual(RobotAiState.AlertReposition, ai.State);
            Assert.True(robot.Target.Equals(repositionTarget));


            robot.Location = repositionTarget;
            ai.Think();
            Assert.AreEqual(RobotAiState.AlertAttack, ai.State);
        }

        [Test]
        public void TestAttackingPlayerMoves60MetersOutOfLineOfSightFollowUp()
        {
            RobotAi ai = new MockRobotAi();
            MockPlayer player = (MockPlayer) ai.Player;
            MockRobot robot = (MockRobot) ai.Robot;

            robot.Location = new MockLocation(0, 0, 0);
            player.Location = new MockLocation(61, 0, 0);
            robot.DetectionLineOfSight = robot.CanSeePlayer = true;
            ai.State = RobotAiState.AlertAttack;
            ai.Think();
            Assert.AreEqual(RobotAiState.AlertFollowUp, ai.State);
        }

        [Test]
        public void TestAttackingPlayerMoves60MetersOutOfLineOfSightFollowUpTargetsLastLocation()
        {
            RobotAi ai = new MockRobotAi();
            MockPlayer player = (MockPlayer) ai.Player;
            MockRobot robot = (MockRobot) ai.Robot;

            robot.Location = new MockLocation(0, 0, 0);
            player.Location = new MockLocation(61, 0, 0);

            var lastSeenPlayerLocation = new MockLocation(60, 0, 0);
            var lastHeardPlayerLocation = new MockLocation(40, 0, 0);
            ai.PlayerLocations.Add(new PlayerLocation(DateTime.Now, lastHeardPlayerLocation, false, true));
            ai.PlayerLocations.Add(new PlayerLocation(DateTime.Now, lastSeenPlayerLocation, true, false));

            robot.DetectionLineOfSight = robot.CanSeePlayer = true;
            ai.State = RobotAiState.AlertAttack;
            ai.Think();
            Assert.AreEqual(RobotAiState.AlertFollowUp, ai.State);
            Assert.True(robot.Target.Equals(lastSeenPlayerLocation));
        }
    }
}