using System;
using DisablerAi;
using NUnit.Framework;

namespace TestRobot
{
    [TestFixture]
    public class CanEnterPatrolStates
    {
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
    }
}