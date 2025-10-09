using Core.ServiceLocator;

namespace Core.Services.Interfaces
{
    public interface IVRService: IService
    {
        void OnApplicationPause();
        void OnApplicationResume();
        void OnVREnabled();
        void OnVRDisabled();
    }
}