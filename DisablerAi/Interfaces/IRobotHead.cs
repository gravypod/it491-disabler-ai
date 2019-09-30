namespace DisablerAi.Interfaces
{
    public interface IRobotHead
    {
        ILocation Location { get; set; }
        bool Shot { get; set; }
    }
}