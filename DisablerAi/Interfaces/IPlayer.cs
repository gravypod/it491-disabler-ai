namespace DisablerAi.Interfaces
{
    public interface IPlayer
    {
        IDisabler Disabler { get; set; }
        ILocation Location { get; set; }
    }
}