using System.Linq;
using DisablerAi;
using NUnit.Framework;

namespace TestRobot
{
    [TestFixture]
    public class ThinkHeldUp
    {
        [Test]
        public void TestHoldUp()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;

            robot.Head.Location = player.Disabler.Location = new MockLocation();
            ai.State = RobotAiState.Patrol;
            ai.Think();

            Assert.AreEqual(RobotAiState.HeldUp, ai.State);
            Assert.AreEqual(RobotAnimation.CoweringStanding, robot.PlayingAnimation);
            Assert.False(robot.DetectionLineOfSight);
            Assert.False(robot.DetectionAudio);
        }

        [Test]
        public void TestHoldUpGetDown()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;

            robot.Head.Location = player.Disabler.Location = new MockLocation();
            ai.State = RobotAiState.HeldUp;
            ai.GetDownRequested = true;
            ai.Think();

            Assert.AreEqual(RobotAiState.HeldUpGetDown, ai.State);
            Assert.AreEqual(RobotAnimation.CoweringOnGround, robot.PlayingAnimation);
            Assert.False(robot.DetectionLineOfSight);
            Assert.False(robot.DetectionAudio);
        }

        [Test]
        public void TestHoldUpMarkItem()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;
            MockItem item = new MockItem(robot.Location);

            robot.Head.Location = player.Disabler.Location = new MockLocation();
            ai.State = RobotAiState.HeldUp;
            ai.MarkItemsRequested = true;

            // Item within reach
            player.Items.Add(item);
            ai.Think();

            Assert.AreEqual(RobotAiState.HeldUpDemandMarkAmmo, ai.State);
            Assert.True(item.HasBeenMarkedForPlayer);
        }

        [Test]
        public void TestHoldUpMarkEnemy()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;
            MockRobot otherRobot = new MockRobot();

            robot.Head.Location = player.Disabler.Location = new MockLocation();
            ai.State = RobotAiState.HeldUp;
            ai.MarkEnemiesRequested = true;

            player.Robots.Add(robot);
            player.Robots.Add(otherRobot);
            ai.Think();

            Assert.AreEqual(RobotAiState.HeldUpDemandMarkEnemies, ai.State);
            Assert.True(otherRobot.HasBeenMarkedForPlayer);
            Assert.False(robot.HasBeenMarkedForPlayer);
        }

        [Test]
        public void TestHoldUpMarkEnemyReturnToHeldUp()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;
            MockRobot otherRobot = new MockRobot();

            robot.Head.Location = player.Disabler.Location = new MockLocation();
            ai.State = RobotAiState.HeldUp;
            ai.MarkEnemiesRequested = true;

            player.Robots.Add(robot);
            player.Robots.Add(otherRobot);
            ai.Think();

            Assert.AreEqual(RobotAiState.HeldUpDemandMarkEnemies, ai.State);
            Assert.True(otherRobot.HasBeenMarkedForPlayer);
            Assert.False(robot.HasBeenMarkedForPlayer);

            ai.Think();
            Assert.AreEqual(RobotAiState.HeldUp, ai.State);
        }

        [Test]
        public void TestHoldUpMarkItemReturnToHeldUp()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;
            MockItem item = new MockItem(robot.Location);

            robot.Head.Location = player.Disabler.Location = new MockLocation();
            ai.State = RobotAiState.HeldUp;
            ai.MarkItemsRequested = true;

            // Item within reach
            player.Items.Add(item);
            ai.Think();

            Assert.AreEqual(RobotAiState.HeldUpDemandMarkAmmo, ai.State);
            Assert.True(item.HasBeenMarkedForPlayer);

            ai.Think();
            Assert.AreEqual(RobotAiState.HeldUp, ai.State);
        }

        [Test]
        public void TestGetDownMarkItemReturnToGetDown()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;
            MockItem item = new MockItem(robot.Location);

            // Hold up
            robot.Head.Location = player.Disabler.Location = new MockLocation();
            ai.State = RobotAiState.HeldUp;
            ai.Think();
            Assert.AreEqual(RobotAiState.HeldUp, ai.State);

            // Get down
            ai.GetDownRequested = true;
            ai.Think();
            Assert.AreEqual(RobotAiState.HeldUpGetDown, ai.State);
            Assert.AreEqual(RobotAnimation.CoweringOnGround, robot.PlayingAnimation);

            // Mark items 
            ai.MarkItemsRequested = true;
            player.Items.Add(item);
            ai.Think();
            Assert.AreEqual(RobotAiState.HeldUpDemandMarkAmmo, ai.State);
            Assert.True(item.HasBeenMarkedForPlayer);

            // Return to get down
            ai.Think();
            Assert.AreEqual(RobotAiState.HeldUpGetDown, ai.State);
            Assert.AreEqual(RobotAnimation.CoweringOnGround, robot.PlayingAnimation);
        }

        [Test]
        public void TestHoldUpRefuse()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;

            robot.Head.Location = player.Disabler.Location = new MockLocation();
            ai.State = RobotAiState.HeldUp;
            ai.MarkEnemiesRequested = true;
            ai.HasHeldUpDemandBeenMade = true;
            ai.Think();
            Assert.AreEqual(RobotAiState.HeldUpRefuse, ai.State);
            Assert.AreEqual(RobotAnimation.CoweringRefuse, robot.PlayingAnimation);
        }

        [Test]
        public void TestHoldUpRefuseReturnToHoldUpAfterAnimationFinishes()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;

            robot.Head.Location = player.Disabler.Location = new MockLocation();

            // Demand something after we've already made a demands
            ai.State = RobotAiState.HeldUp;
            ai.MarkEnemiesRequested = true;
            ai.HasHeldUpDemandBeenMade = true;
            ai.Think();
            Assert.AreEqual(RobotAiState.HeldUpRefuse, ai.State);
            Assert.AreEqual(RobotAnimation.CoweringRefuse, robot.PlayingAnimation);

            // Animation is still playing
            ai.Think();
            Assert.AreEqual(RobotAiState.HeldUpRefuse, ai.State);
            Assert.AreEqual(RobotAnimation.CoweringRefuse, robot.PlayingAnimation);

            // Animation has finished
            robot.PlayingAnimation = RobotAnimation.None;
            ai.Think();
            Assert.AreEqual(RobotAiState.HeldUp, ai.State);
            Assert.AreEqual(RobotAnimation.CoweringStanding, robot.PlayingAnimation);
        }
    }
}