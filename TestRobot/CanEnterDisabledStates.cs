using DisablerAi;
using NUnit.Framework;

namespace TestRobot
{
    [TestFixture]
    public class CanEnterDisabledStates
    {
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
    }
}