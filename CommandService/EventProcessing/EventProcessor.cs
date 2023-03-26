#nullable disable

using System.Text.Json;
using AutoMapper;
using CommandService.Data;
using CommandService.DTOs;
using CommandService.Models;

namespace CommandService.EventProcessing
{
    # region Const
    public class Const
    {
        public const string EVENT_PLATFORM_CREATE = "Platform_Create";
        public const string EVENT_PLATFORM_UPDATE = "Platform_Update";
    }
    # endregion

    enum EventType
    {
        PlaformCreate,
        PlatformUpdate,
        Undetermined
    }

    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;   
        }
        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);

            switch (eventType)
            {
                case EventType.PlaformCreate:
                    addPlatform(message);
                    break;
                case EventType.PlatformUpdate:
                    updatePlatform(message);
                    break;
                default:
                    break;
            }
        }

        private EventType DetermineEvent(string notificationMessage)
        {
            System.Console.WriteLine("---> Determining Event");
            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

            switch(eventType.Event)
            {
                case Const.EVENT_PLATFORM_CREATE:
                    System.Console.WriteLine("---> Platform Create Event detected");
                    return EventType.PlaformCreate;
                case Const.EVENT_PLATFORM_UPDATE:
                    System.Console.WriteLine("---> Platform Update Event detected");
                    return EventType.PlatformUpdate;
                default:
                    System.Console.WriteLine("---> Could not determine the event type");
                    return EventType.Undetermined;
            }
        }

        private void addPlatform(string platformPublishedMessage)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

                var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);
                try
                {
                    var plat = _mapper.Map<Platform>(platformPublishedDto);
                    if (!repo.ExternalPlatformExists(plat.ExternalID))
                    {
                        repo.CreatePlatform(plat);
                        repo.SaveChanges();
                        System.Console.WriteLine("---> Platform Added");
                    }
                    else
                    {
                        System.Console.WriteLine("---> Platform already exists...");
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"---> Could not add Platform to DB: {ex.Message}");
                }
            }
        }

        private void updatePlatform(string platformPublishedMessage)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

                var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);
                try
                {
                    var plat = _mapper.Map<Platform>(platformPublishedDto);
                    if (!repo.ExternalPlatformExists(plat.ExternalID))
                    {
                        repo.CreatePlatform(plat);
                        repo.SaveChanges();
                        System.Console.WriteLine("---> Platform Added");
                    }
                    else
                    {
                        repo.UpdatePlatform(plat);
                        repo.SaveChanges();
                        System.Console.WriteLine("---> Platform Updated");
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"---> Could not add Platform to DB: {ex.Message}");
                }
            }
        }
    }
}