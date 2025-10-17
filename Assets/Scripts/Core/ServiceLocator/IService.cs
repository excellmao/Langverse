namespace Core.ServiceLocator
{
    public interface IService
    {
        void Initialize();
        void Cleanup();
        bool isInitialized { get; }
        string ServiceId { get; }
    }
}