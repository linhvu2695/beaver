using PlatformService.DTOs;

namespace PlatformService.AsyncDataService
{
    public interface IMessageBusClient
    {
        void PublishPlatform(PlatformPublishedDto platformPublishedDto);
    }
}