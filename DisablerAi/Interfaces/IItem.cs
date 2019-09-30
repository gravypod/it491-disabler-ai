namespace DisablerAi.Interfaces
{
    public interface IItem
    {
        ILocation Location { get; set; }

        /// <summary>
        /// Mark the item as visible for  the player
        /// </summary>
        void MarkForPlayer();
    }
}