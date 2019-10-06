using System;
using System.Linq;
using DisablerAi;
using NUnit.Framework;

namespace TestRobot
{
    [TestFixture]
    public class CanEnterSuspicionStates
    {
        [Test]
        public void TestPatrolCanMoveToSuspicionWhenSeenAndGreaterThan15MetersAwayAndLessThan50MetersAway()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;

            robot.CanHearPlayer = false;
            robot.CanSeePlayer = true;
            robot.DetectionLineOfSight = true;
            robot.DetectionAudio = true;

            robot.Location = new MockLocation(21, 1, 1);
            player.Location = new MockLocation(1, 1, 1);

            ai.State = RobotAiState.Patrol;
            Assert.True(ai.Can(RobotAiState.Suspicion));
        }

        [Test]
        public void TestPatrolCannotMoveToSuspicionWhenSeenAndLessThan15Meters()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;

            robot.CanHearPlayer = false;
            robot.CanSeePlayer = true;
            robot.DetectionLineOfSight = true;
            robot.DetectionAudio = true;

            robot.Location = new MockLocation(11, 1, 1);
            player.Location = new MockLocation(1, 1, 1);

            ai.State = RobotAiState.Patrol;
            Assert.False(ai.Can(RobotAiState.Suspicion));
        }

        [Test]
        public void TestPatrolCanMoveToSuspicionWhenHeardAndUnder40MetersAway()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;

            robot.CanHearPlayer = true;
            robot.CanSeePlayer = false;
            robot.DetectionLineOfSight = true;
            robot.DetectionAudio = true;

            robot.Location = new MockLocation(21, 1, 1);
            player.Location = new MockLocation(1, 1, 1);

            ai.State = RobotAiState.Patrol;
            Assert.True(ai.Can(RobotAiState.Suspicion));
        }

        [Test]
        public void TestSuspicionCanMoveToCallHeadQuartersWhenSeenUnder50MetersAway()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;

            robot.CanHearPlayer = false;
            robot.CanSeePlayer = true;
            robot.DetectionLineOfSight = true;
            robot.DetectionAudio = true;

            robot.Location = new MockLocation(21, 1, 1);
            player.Location = new MockLocation(1, 1, 1);

            ai.State = RobotAiState.Suspicion;
            Assert.True(ai.Can(RobotAiState.SuspicionCallHeadQuarters));
        }


        [Test]
        public void TestSuspicionCanMoveToCallHeadQuartersWhenHeard3Times()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;

            robot.CanHearPlayer = false;
            robot.CanSeePlayer = false;
            robot.DetectionLineOfSight = true;
            robot.DetectionAudio = true;

            robot.Location = new MockLocation(999, 999, 999);
            player.Location = new MockLocation(1, 1, 1);

            for (int i = 0; i < 3; i++)
                ai.PlayerLocations.Add(new PlayerLocation(DateTime.Now, new MockLocation(), false, true));

            ai.State = RobotAiState.Suspicion;
            Assert.True(ai.Can(RobotAiState.SuspicionCallHeadQuarters));
        }


        [Test]
        public void TestSuspicionCannotMoveToCallHeadQuartersWhenHeard2Times()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;

            robot.CanHearPlayer = false;
            robot.CanSeePlayer = false;
            robot.DetectionLineOfSight = true;
            robot.DetectionAudio = true;

            robot.Location = new MockLocation(999, 999, 999);
            player.Location = new MockLocation(1, 1, 1);

            for (int i = 0; i < 2; i++)
                ai.PlayerLocations.Add(new PlayerLocation(DateTime.Now, new MockLocation(), false, true));

            ai.State = RobotAiState.Suspicion;
            Assert.False(ai.Can(RobotAiState.SuspicionCallHeadQuarters));
        }

        [Test]
        public void TestSuspicionCannotMoveToCallHeadQuartersWhenNeverSeenOrHeard()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;

            robot.CanHearPlayer = false;
            robot.CanSeePlayer = false;
            robot.DetectionLineOfSight = true;
            robot.DetectionAudio = true;

            robot.Location = new MockLocation(999, 999, 999);
            player.Location = new MockLocation(1, 1, 1);

            ai.State = RobotAiState.Suspicion;
            Assert.False(ai.Can(RobotAiState.SuspicionCallHeadQuarters));
        }


        [Test]
        public void TestSuspicionCanMoveToFollowUp()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;

            robot.CanHearPlayer = true;
            robot.CanSeePlayer = false;
            robot.DetectionLineOfSight = true;
            robot.DetectionAudio = true;

            robot.Location = new MockLocation(999, 999, 999);
            player.Location = new MockLocation(1, 1, 1);

            ai.PlayerLocations.Add(new PlayerLocation(DateTime.Now, new MockLocation(), false, true));

            ai.State = RobotAiState.SuspicionCallHeadQuarters;
            Assert.True(ai.Can(RobotAiState.SuspicionFollowUp));
        }

        [Test]
        public void TestSuspicionFollowUpCanMoveToLookAround()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;

            robot.CanHearPlayer = true;
            robot.CanSeePlayer = false;
            robot.DetectionLineOfSight = true;
            robot.DetectionAudio = true;

            robot.Target = robot.Location = new MockLocation(999, 999, 999);
            player.Location = new MockLocation(1, 1, 1);

            ai.PlayerLocations.Add(new PlayerLocation(DateTime.Now, robot.Location, false, true));

            ai.State = RobotAiState.SuspicionFollowUp;
            Assert.True(ai.Can(RobotAiState.SuspicionLookAround));
        }


        [Test]
        public void TestSuspicionFollowUpCannotMoveToLookAroundIfWeHaveNotReachedTarget()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;

            robot.CanHearPlayer = true;
            robot.CanSeePlayer = false;
            robot.DetectionLineOfSight = true;
            robot.DetectionAudio = true;

            robot.Location = new MockLocation(999, 999, 999);
            player.Location = new MockLocation(1, 1, 1);

            ai.PlayerLocations.Add(new PlayerLocation(DateTime.Now, new MockLocation(10, 10, 10), false, true));
            robot.Target = ai.PlayerLocations.Last().Location;

            ai.State = RobotAiState.SuspicionFollowUp;
            Assert.False(ai.Can(RobotAiState.SuspicionLookAround));
        }


        [Test]
        public void TestSuspicionLookAroundCanShrugOffIfCannotSeePlayerAtLastLocation()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;

            robot.CanHearPlayer = false;
            robot.CanSeePlayer = false;
            robot.DetectionLineOfSight = true;
            robot.DetectionAudio = true;

            robot.Location = new MockLocation(999, 999, 999);
            player.Location = new MockLocation(1, 1, 1);

            ai.PlayerLocations.Add(new PlayerLocation(DateTime.Now, robot.Location, false, true));
            robot.PlayingAnimation = RobotAnimation.None;

            ai.State = RobotAiState.SuspicionLookAround;
            Assert.True(ai.Can(RobotAiState.SuspicionShrugOff));
        }

        [Test]
        public void TestSuspicionLookAroundCannotShrugOffIfCanSeePlayerAtLastLocation()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;

            robot.CanHearPlayer = false;
            robot.CanSeePlayer = true;
            robot.DetectionLineOfSight = true;
            robot.DetectionAudio = true;

            robot.Location = new MockLocation(999, 999, 999);
            player.Location = new MockLocation(1, 1, 1);

            ai.PlayerLocations.Add(new PlayerLocation(DateTime.Now, robot.Location, false, true));
            ai.TimeMarker = DateTime.Now - TimeSpan.FromMinutes(1);

            ai.State = RobotAiState.SuspicionLookAround;
            Assert.False(ai.Can(RobotAiState.SuspicionShrugOff));
        }


        [Test]
        public void TestSuspicionLookAroundCannotShrugOffIfLookAroundAnimationPlaying()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;

            robot.CanHearPlayer = false;
            robot.CanSeePlayer = false;
            robot.DetectionLineOfSight = true;
            robot.DetectionAudio = true;

            robot.Location = new MockLocation(999, 999, 999);
            player.Location = new MockLocation(1, 1, 1);

            ai.PlayerLocations.Add(new PlayerLocation(DateTime.Now, robot.Location, false, true));

            robot.PlayingAnimation = RobotAnimation.LookAround;
            ai.State = RobotAiState.SuspicionLookAround;
            Assert.False(ai.Can(RobotAiState.SuspicionShrugOff));
        }

        [Test]
        public void TestSuspicionShrugOffCanMoveToPatrol()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;
            MockPlayer player = (MockPlayer) ai.Player;

            robot.CanHearPlayer = false;
            robot.CanSeePlayer = false;
            robot.DetectionLineOfSight = true;
            robot.DetectionAudio = true;

            robot.Target = robot.Location = new MockLocation(999, 999, 999);
            player.Location = new MockLocation(1, 1, 1);

            ai.PlayerLocations.Add(new PlayerLocation(DateTime.Now, robot.Location, false, true));
            ai.TimeMarker = DateTime.Now - TimeSpan.FromMinutes(1);

            ai.State = RobotAiState.SuspicionShrugOff;
            Assert.True(ai.Can(RobotAiState.Patrol));
        }
    }
}