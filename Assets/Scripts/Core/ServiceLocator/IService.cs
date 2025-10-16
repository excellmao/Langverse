using System;

namespace Core.ServiceLocator
{
    public interface IService
    {
        void Init();
        void Cleanup();
        bool isInitialized { get; }
        string serviceId { get; }
    }
}
