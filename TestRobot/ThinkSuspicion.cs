using System;
using System.Configuration;
using System.Linq;
using DisablerAi;
using NUnit.Framework;

namespace TestRobot
{
    [TestFixture]
    public class ThinkSearching
    {
        [Test]
        public void TestHurtBeforeStaggerAnimationWaitsForCompletion()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;

            // Just Hurt
            ai.State = RobotAiState.Hurt;
            robot.PlayingAnimation = RobotAnimation.HurtStagger;
            ai.Think();
            Assert.AreEqual(RobotAiState.Hurt, ai.State);

            // Two seconds pass and animation is finished
            robot.PlayingAnimation = RobotAnimation.None;
            ai.TimeMarker = DateTime.Now - TimeSpan.FromSeconds(2.1);
            ai.Think();
            Assert.AreEqual(RobotAiState.Suspicion, ai.State);
        }

        [Test]
        public void TestPatrolRobotWasSeenWithin50M()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;

            robot.Head.Location = robot.Location = new MockLocation(0, 0, 0);
            player.Disabler.Location = player.Location = new MockLocation(49, 0, 0);

            robot.DetectionLineOfSight = robot.CanSeePlayer = true;
            robot.DetectionAudio = robot.CanHearPlayer = false;
            ai.State = RobotAiState.Patrol;
            ai.Think();

            Assert.AreEqual(RobotAiState.Suspicion, ai.State);
        }

        [Test]
        public void TestPatrolRobotWasHeardWithin50M()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;

            robot.Head.Location = robot.Location = new MockLocation(0, 0, 0);
            player.Disabler.Location = player.Location = new MockLocation(39, 0, 0);

            robot.DetectionLineOfSight = robot.CanSeePlayer = false;
            robot.DetectionAudio = robot.CanHearPlayer = true;
            ai.State = RobotAiState.Patrol;
            ai.Think();

            Assert.AreEqual(RobotAiState.Suspicion, ai.State);
        }

        [Test]
        public void TestSuspicionAfter3SightingsCallHq()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;

            robot.Head.Location = robot.Location = new MockLocation(0, 0, 0);
            player.Disabler.Location = player.Location = new MockLocation(39, 0, 0);

            for (int i = 0; i < 3; i++)
            {
                ai.PlayerLocations.Add(new PlayerLocation(DateTime.Now, player.Location, true, true));
            }

            robot.CanSeePlayer = robot.CanHearPlayer = true;
            ai.State = RobotAiState.Patrol;
            ai.Think();
            Assert.AreEqual(RobotAiState.Suspicion, ai.State);

            robot.CanSeePlayer = robot.CanHearPlayer = false;
            ai.Think();
            Assert.AreEqual(RobotAiState.SuspicionCallHeadQuarters, ai.State);
        }

// TODO: Move into searching unit tests when build
//        [Test]
//        public void TestSuspicionCallHqFinishedStartSearching()
//        {
//            RobotAi ai = new MockRobotAi();
//            MockRobot robot = (MockRobot) ai.Robot;
//            MockPlayer player = (MockPlayer) ai.Player;
//
//            robot.Head.Location = robot.Location = new MockLocation(0, 0, 0);
//            player.Disabler.Location = player.Location = new MockLocation(39, 0, 0);
//
//            for (int i = 0; i < 3; i++)
//            {
//                ai.PlayerLocations.Add(new PlayerLocation(DateTime.Now, player.Location, true, true));
//            }
//
//            robot.PlayingAnimation = RobotAnimation.AlertCallHeadQuarters;
//            ai.State = RobotAiState.SuspicionCallHeadQuarters;
//            ai.Think();
//            Assert.AreEqual(RobotAiState.SuspicionCallHeadQuarters, ai.State);
//
//            robot.PlayingAnimation = RobotAnimation.None;
//            ai.Think();
//            Assert.AreEqual(RobotAiState.Searching, ai.State);
//        }


        [Test]
        public void TestSuspicionSeenThreeTimesStartFollowUp()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;

            robot.Head.Location = robot.Location = new MockLocation(0, 0, 0);
            player.Disabler.Location = player.Location = new MockLocation(39, 0, 0);

            ai.State = RobotAiState.Patrol;
            for (int i = 0; i < 5; i++)
            {
                // true, false, true, false, true
                robot.CanSeePlayer = robot.CanHearPlayer = i % 2 == 0;
                ai.Think();
            }

            Assert.AreEqual(RobotAiState.SuspicionCallHeadQuarters, ai.State);
            Assert.AreEqual(RobotAnimation.AlertCallHeadQuarters, robot.PlayingAnimation);

            robot.PlayingAnimation = RobotAnimation.None;
            robot.CanSeePlayer = robot.CanHearPlayer = false;
            ai.Think();
            Assert.AreEqual(RobotAiState.SuspicionFollowUp, ai.State);
            Assert.AreEqual(ai.PlayerLocations.Last().Location, robot.Target);
        }

        [Test]
        public void TestSuspicionFollowUpReachedLocationLookAround()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;


            robot.Location = robot.Head.Location = robot.Target = new MockLocation(100, 100, 100);

            ai.State = RobotAiState.SuspicionFollowUp;
            ai.Think();

            Assert.AreEqual(RobotAiState.SuspicionLookAround, ai.State);
            Assert.AreEqual(RobotAnimation.LookAround, robot.PlayingAnimation);
        }


        [Test]
        public void TestSuspicionFollowUpToShrugOff()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;


            robot.Location = robot.Head.Location = robot.Target = new MockLocation(100, 100, 100);

            robot.PlayingAnimation = RobotAnimation.None;
            ai.State = RobotAiState.SuspicionLookAround;
            ai.Think();

            Assert.AreEqual(RobotAiState.SuspicionShrugOff, ai.State);
        }
        
        [Test]
        public void TestSuspicionShrugOffBackToPatrol()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;


            robot.Location = robot.Head.Location = robot.Target = new MockLocation(100, 100, 100);

            robot.PlayingAnimation = RobotAnimation.None;
            ai.State = RobotAiState.SuspicionShrugOff;
            ai.Think();

            Assert.AreEqual(RobotAiState.Patrol, ai.State);
        }
    }
}