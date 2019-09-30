namespace DisablerAi.Interfaces
{
    public interface ILocation
    {
        ILocation RandomLocation(float distanceFromPlayer, float distanceFromRobots);
        float DistanceFrom(ILocation location);
    }
}