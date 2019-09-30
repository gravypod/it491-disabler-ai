using DisablerAi;
using NUnit.Framework;

namespace TestRobot
{
    [TestFixture]
    public class CanEnterHeldUpStates
    {
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

            disabler.Location = new MockLocation(1, 1, 1);
            robot.Head.Location = new MockLocation(1, 1, 1);

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
            robot.Head.Location = new MockLocation(1, 1, 1);

            Assert.False(ai.Can(RobotAiState.HeldUp));
        }

        [Test]
        public void TestHeldUpCanMarkItem()
        {
            RobotAi ai = new MockRobotAi();
            ai.State = RobotAiState.HeldUp;
            ai.MarkItemsRequested = true;
            Assert.True(ai.Can(RobotAiState.HeldUpDemandMarkAmmo));
        }


        [Test]
        public void TestHeldUpCanMarkEnemies()
        {
            RobotAi ai = new MockRobotAi();
            ai.State = RobotAiState.HeldUp;
            ai.MarkEnemiesRequested = true;
            Assert.True(ai.Can(RobotAiState.HeldUpDemandMarkEnemies));
        }

        [Test]
        public void TestHeldUpCannotMarkTwice()
        {
            RobotAi ai = new MockRobotAi();
            ai.State = RobotAiState.HeldUp;
            ai.HasHeldUpDemandBeenMade = true;
            Assert.False(ai.Can(RobotAiState.HeldUpDemandMarkEnemies));
            Assert.False(ai.Can(RobotAiState.HeldUpDemandMarkAmmo));
        }

        [Test]
        public void TestHeldUpRefuse()
        {
            RobotAi ai = new MockRobotAi();
            ai.State = RobotAiState.HeldUp;
            ai.HasHeldUpDemandBeenMade = true;
            Assert.True(ai.Can(RobotAiState.HeldUpRefuse));
        }

        [Test]
        public void TestHeldUpSubStatesCanMoveToHeldUp()
        {
            RobotAi ai = new MockRobotAi();
            ai.HasHeldUpDemandBeenMade = false;

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
            ai.HasHeldUpDemandBeenMade = false;
            ai.MarkEnemiesRequested = ai.MarkItemsRequested = ai.GetDownRequested = true;
            
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
            ai.HasHeldUpDemandBeenMade = false;

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
            ai.HasHeldUpDemandBeenMade = false;
            ai.State = RobotAiState.HeldUpGetDown;
            Assert.False(ai.Can(RobotAiState.HeldUp));
        }
    }
}