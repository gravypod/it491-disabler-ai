using DisablerAi;
using NUnit.Framework;

namespace TestRobot
{
    [TestFixture]
    public class CanEnterHurtStates
    {
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

    }
}