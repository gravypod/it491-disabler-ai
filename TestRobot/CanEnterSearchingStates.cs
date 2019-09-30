using System;
using DisablerAi;
using NUnit.Framework;

namespace TestRobot
{
    [TestFixture]
    public class CanEnterSearchingStates
    {
        [Test]
        public void TestSuspicionCallHeadQuartersCanMoveToSearching()
        {
            RobotAi ai = new MockRobotAi();

            ai.State = RobotAiState.SuspicionCallHeadQuarters;
            Assert.True(ai.Can(RobotAiState.Searching));
        }


        [Test]
        public void TestAlertFollowUpCannotSeeCannotHearAfter1MinuteCanMoveToSearching()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;

            robot.CanHearPlayer = false;
            robot.CanSeePlayer = false;

            ai.TimeMarker = DateTime.Now - TimeSpan.FromMinutes(10);

            ai.State = RobotAiState.AlertFollowUp;
            Assert.True(ai.Can(RobotAiState.Searching));
        }

        [Test]
        public void TestSearchingCanMoveToFollowUpPointOfInterest()
        {
            RobotAi ai = new MockRobotAi();
            ai.State = RobotAiState.Searching;
            Assert.True(ai.Can(RobotAiState.SearchingFollowUpPointOfInterest));
        }

        [Test]
        public void TestSearchingFollowUpCanMoveToLookAround()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;

            robot.Target = new MockLocation(10, 10, 10);
            robot.Location = robot.Target;

            ai.State = RobotAiState.SearchingFollowUpPointOfInterest;
            Assert.True(ai.Can(RobotAiState.SearchingLookAroundPointOfInterest));
        }

        [Test]
        public void TestSearchingFollowUpCannotMoveToLookAroundIfHasNotReachedTarget()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;

            robot.Target = new MockLocation(10, 10, 10);
            robot.Location = new MockLocation(1, 1, 1);

            ai.State = RobotAiState.SearchingFollowUpPointOfInterest;
            Assert.False(ai.Can(RobotAiState.SearchingLookAroundPointOfInterest));
        }

        [Test]
        public void TestSearchingLookAroundCannotFollowUpPlayerLastSeenIfPlayerNeverSeen()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;

            ai.State = RobotAiState.SearchingLookAroundPointOfInterest;
            ai.TimeMarker = DateTime.Now - TimeSpan.FromMinutes(10);
            Assert.False(ai.Can(RobotAiState.SearchingFollowUpPlayerLastSeen));
        }

        [Test]
        public void TestSearchingLookAroundCanFollowUpPointOfInterestIfPlayerNeverSeen()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;

            ai.State = RobotAiState.SearchingLookAroundPointOfInterest;
            ai.TimeMarker = DateTime.Now - TimeSpan.FromMinutes(10);
            Assert.True(ai.Can(RobotAiState.SearchingFollowUpPointOfInterest));
        }


        [Test]
        public void TestSearchingLookAroundShouldFollowUpPlayerPositionIfSeenPlayer()
        {
            RobotAi ai = new MockRobotAi();
            MockRobot robot = (MockRobot) ai.Robot;

            ai.PlayerLocations.Add(new PlayerLocation(DateTime.Now, new MockLocation(), true, true));

            ai.State = RobotAiState.SearchingLookAroundPointOfInterest;
            ai.TimeMarker = DateTime.Now - TimeSpan.FromMinutes(10);
            Assert.True(ai.Can(RobotAiState.SearchingFollowUpPlayerLastSeen));
            Assert.False(ai.Can(RobotAiState.SearchingFollowUpPointOfInterest));
        }

        [Test]
        public void TestSearchingLookAroundPointOfInterestShouldFlipFlopIfPlayerHasBeenSeen()
        {
            RobotAi ai = new MockRobotAi();

            ai.PlayerLocations.Add(new PlayerLocation(DateTime.Now, new MockLocation(), true, true));

            ai.TimeMarker = DateTime.Now - TimeSpan.FromMinutes(10);

            ai.State = RobotAiState.SearchingLookAroundPlayerLastSeen;
            Assert.False(ai.Can(RobotAiState.SearchingFollowUpPlayerLastSeen));
            Assert.True(ai.Can(RobotAiState.SearchingFollowUpPointOfInterest));

            ai.State = RobotAiState.SearchingLookAroundPointOfInterest;
            Assert.False(ai.Can(RobotAiState.SearchingFollowUpPointOfInterest));
            Assert.True(ai.Can(RobotAiState.SearchingFollowUpPlayerLastSeen));
        }


        [Test]
        public void TestSearchingLookAroundPlayerLastKnownPositionChangeFollowUpAfter2Seconds()
        {
            RobotAi ai = new MockRobotAi();

            ai.PlayerLocations.Add(new PlayerLocation(DateTime.Now, new MockLocation(), true, true));

            ai.TimeMarker = DateTime.Now - TimeSpan.FromSeconds(3);

            ai.State = RobotAiState.SearchingLookAroundPlayerLastSeen;
            Assert.True(ai.Can(RobotAiState.SearchingFollowUpPointOfInterest));
        }

        [Test]
        public void TestSearchingLookAroundPlayerLastKnownPositionDontChangeFollowUpBefore2Seconds()
        {
            RobotAi ai = new MockRobotAi();

            ai.PlayerLocations.Add(new PlayerLocation(DateTime.Now, new MockLocation(), true, true));

            ai.TimeMarker = DateTime.Now - TimeSpan.FromSeconds(1);

            ai.State = RobotAiState.SearchingLookAroundPlayerLastSeen;
            Assert.False(ai.Can(RobotAiState.SearchingFollowUpPointOfInterest));
        }

        [Test]
        public void TestSearchingChangeToAlertIfPlayerNotSeenOrHeardWithin1Minute()
        {
            RobotAi ai = new MockRobotAi();

            ai.PlayerLocations.Add(new PlayerLocation(DateTime.Now - TimeSpan.FromMinutes(2), new MockLocation(), true,
                true));

            var alertStates = new RobotAiState[]
            {
                RobotAiState.Searching,
                RobotAiState.SearchingFollowUpPlayerLastSeen,
                RobotAiState.SearchingFollowUpPointOfInterest,
                RobotAiState.SearchingLookAroundPlayerLastSeen,
                RobotAiState.SearchingLookAroundPointOfInterest,
            };

            foreach (var alertState in alertStates)
            {
                ai.State = alertState;
                Assert.True(ai.Can(RobotAiState.Alert));
            }
        }


        [Test]
        public void TestSearchingDoNotChangeToAlertIfPlayerSeenOrHeardWithin1Minute()
        {
            RobotAi ai = new MockRobotAi();

            ai.PlayerLocations.Add(new PlayerLocation(DateTime.Now, new MockLocation(), true, true));

            var alertStates = new RobotAiState[]
            {
                RobotAiState.Searching,
                RobotAiState.SearchingFollowUpPlayerLastSeen,
                RobotAiState.SearchingFollowUpPointOfInterest,
                RobotAiState.SearchingLookAroundPlayerLastSeen,
                RobotAiState.SearchingLookAroundPointOfInterest,
            };

            foreach (var alertState in alertStates)
            {
                ai.State = alertState;
                Assert.False(ai.Can(RobotAiState.Alert));
            }
        }
    }
}