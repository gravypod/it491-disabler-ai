using DisablerAi;
using NUnit.Framework;

namespace TestRobot
{
    [TestFixture]
    public class CanEnterStartStates
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
    }
}