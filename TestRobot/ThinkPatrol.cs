using System;
using DisablerAi;
using NUnit.Framework;

namespace TestRobot
{
    [TestFixture]
    public class ThinkPatrol
    {
        [Test]
        public void TestPatrolTargetStart()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;

            robot.Target = robot.Location = robot.PatrolEnd = new MockLocation(0, 0, 0);
            robot.PatrolStart = new MockLocation(100, 100, 100);
            ai.State = RobotAiState.Patrol;
            ai.Think();

            Assert.AreEqual(RobotAiState.PatrolMarchToStart, ai.State);
            Assert.AreEqual(new MockLocation(100, 100, 100), robot.Target);
            Assert.False(robot.ReachedTarget());
        }

        [Test]
        public void TestPatrolTargetStartReached()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;

            robot.PatrolEnd = new MockLocation(0, 0, 0);
            robot.Target = robot.Location = robot.PatrolStart = new MockLocation(100, 100, 100);
            ai.State = RobotAiState.PatrolMarchToStart;
            ai.Think();

            Assert.AreEqual(RobotAiState.PatrolLookAround, ai.State);
            Assert.AreEqual(new MockLocation(100, 100, 100), robot.Target);
            Assert.True(robot.ReachedTarget());
        }

        [Test]
        public void TestPatrolTargetEnd()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;

            robot.PatrolEnd = new MockLocation(0, 0, 0);
            robot.Target = robot.Location = robot.PatrolStart = new MockLocation(100, 100, 100);
            ai.State = RobotAiState.Patrol;
            ai.Think();

            Assert.AreEqual(RobotAiState.PatrolMarchToEnd, ai.State);
            Assert.AreEqual(new MockLocation(0, 0, 0), robot.Target);
            Assert.False(robot.ReachedTarget());
        }

        [Test]
        public void TestPatrolTargetEndReached()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;

            robot.Target = robot.Location = robot.PatrolEnd = new MockLocation(0, 0, 0);
            robot.PatrolStart = new MockLocation(100, 100, 100);
            ai.State = RobotAiState.PatrolMarchToEnd;
            ai.Think();

            Assert.AreEqual(RobotAiState.PatrolLookAround, ai.State);
            Assert.AreEqual(new MockLocation(0, 0, 0), robot.Target);
            Assert.True(robot.ReachedTarget());
        }

        [Test]
        public void TestPatrolLookAroundRestartPatrol()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;

            robot.PatrolEnd = new MockLocation(0, 0, 0);
            robot.Target = robot.Location = robot.PatrolStart = new MockLocation(100, 100, 100);
            ai.State = RobotAiState.PatrolLookAround;
            ai.TimeMarker = DateTime.Now - TimeSpan.FromMinutes(10);
            ai.Think();

            Assert.AreEqual(RobotAiState.Patrol, ai.State);
            Assert.True(robot.ReachedTarget());
        }
    }
}