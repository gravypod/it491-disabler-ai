using DisablerAi;
using NUnit.Framework;

namespace TestRobot
{
    [TestFixture]
    public class ThinkPain
    {
        [Test]
        public void TestHurtHit()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;

            robot.Health = 3;
            robot.Shot = true;
            ai.State = RobotAiState.Patrol;
            ai.Think();

            Assert.AreEqual(RobotAiState.Hurt, ai.State);
            Assert.AreEqual(2, robot.Health);
            Assert.AreEqual(RobotAnimation.HurtStagger, robot.PlayingAnimation);
        }
        
        [Test]
        public void TestDisableHit()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;

            robot.Health = 1;
            robot.Shot = true;
            ai.State = RobotAiState.Patrol;
            ai.Think();

            Assert.AreEqual(RobotAiState.Disabled, ai.State);
            Assert.AreEqual(0, robot.Health);
            Assert.AreEqual(RobotAnimation.RagDoll, robot.PlayingAnimation);
        }
        
        [Test]
        public void TestDisableHeadShot()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;

            robot.Health = 3;
            robot.Head.Shot = true;
            ai.State = RobotAiState.Patrol;
            ai.Think();

            Assert.AreEqual(RobotAiState.Disabled, ai.State);
            Assert.AreEqual(0, robot.Health);
            Assert.AreEqual(RobotAnimation.RagDoll, robot.PlayingAnimation);
        }
    }
}