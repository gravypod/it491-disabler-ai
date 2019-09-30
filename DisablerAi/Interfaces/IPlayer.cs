using System.Collections.Generic;

namespace DisablerAi.Interfaces
{
    public interface IPlayer
    {
        IDisabler Disabler { get; set; }
        ILocation Location { get; set; }

        List<IRobot> NearestRobots();
        
        List<IItem> NearestItems();
    }
}