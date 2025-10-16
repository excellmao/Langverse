namespace Core.Services.Interfaces
{
    public interface IVRService
    {
        void Initialize();
        void Cleanup();
        bool isInitialized { get; }
    }
}