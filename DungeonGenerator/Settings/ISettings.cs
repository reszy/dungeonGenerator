namespace DungeonGenerator.Settings
{
    public interface ISettings
    {
        ISettings GetClone();
        void SetSettings(ISettings settings);
    }
}