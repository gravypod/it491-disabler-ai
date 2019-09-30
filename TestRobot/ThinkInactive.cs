using DisablerAi;
using NUnit.Framework;

namespace TestRobot
{
    [TestFixture]
    public class ThinkInactive
    {
        [Test]
        public void TestStartMoveToInactive()
        {
            RobotAi ai = new MockRobotAi();
            ai.State = RobotAiState.Start;
            ai.Think();
            Assert.AreEqual(RobotAiState.Inactive, ai.State);
        }
        /*
        TODO: No specified way of going from Inactive -> Patrol
        [Test]
        public void TestInactiveMoveToPatrol()
        {
            RobotAi ai = new MockRobotAi();
            ai.State = RobotAiState.Inactive;
            ai.Think();
            Assert.AreEqual(RobotAiState.Patrol, ai.State);
        }*/
    }
}