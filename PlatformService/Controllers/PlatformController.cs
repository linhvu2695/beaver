using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataService;
using PlatformService.Data;
using PlatformService.DTOs;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{   
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        # region Const
        public class Const
        {
            public const string EVENT_PLATFORM_CREATE = "Platform_Create";
            public const string EVENT_PLATFORM_UPDATE = "Platform_Update";
        }        
        # endregion

        private readonly IPlatformRepo _repository;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;
        private readonly IMessageBusClient _messageBusClient;

        public PlatformsController(IPlatformRepo repository, IMapper mapper, ICommandDataClient commandDataCLient, IMessageBusClient messageBusClient)
        {
            _repository = repository;
            _mapper = mapper;
            _commandDataClient = commandDataCLient;
            _messageBusClient = messageBusClient;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            System.Console.WriteLine("--> Getting Platforms...");
            
            var platformItem = _repository.GetAllPlatforms();

            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItem)); 
        }

        [HttpGet("{id}", Name="GetPlatformById")]
        public ActionResult<PlatformReadDto> GetPlatformById(int id)
        {
            System.Console.WriteLine("--> Getting Platform by Id...");

            var platformItem = _repository.GetPlatformById(id);

            if (platformItem != null) {
                return Ok(_mapper.Map<PlatformReadDto>(platformItem));
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<PlatformCreateDto>> CreatePlatform(PlatformCreateDto platformCreateDto) 
        {
            System.Console.WriteLine("--> Creating Platform...");
            var platformModel = _mapper.Map<Platform>(platformCreateDto);

            _repository.CreatePlatform(platformModel);
            _repository.SaveChanges();

            var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

            // Synchronously send data to CommandService
            try
            {
                await _commandDataClient.SendPlatformToCommand(platformReadDto);
            }
            catch(Exception ex)
            {
                System.Console.WriteLine($"---> Could not send synchronously: {ex.Message}");
            }

            // Asynchronously send data to RabbitMQ
            try 
            {
                var platformPublishedDto = _mapper.Map<PlatformPublishedDto>(platformReadDto);
                platformPublishedDto.Event = Const.EVENT_PLATFORM_CREATE;
                _messageBusClient.PublishPlatform(platformPublishedDto);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"---> Could not send asynchronously: {ex.Message}");
            }

            return CreatedAtRoute(nameof(GetPlatformById), new { Id = platformReadDto.Id}, platformReadDto);
        }

        // Can only update Platform.Name
        [HttpPut("{id}")]
        public ActionResult<PlatformUpdateDto> UpdatePlatform(int id, PlatformUpdateDto platformUpdateDto)
        {
            System.Console.WriteLine($"--> Updating Platform {id}...");

            if (!_repository.PlatformExists(id))
            {
                return NotFound();
            }

            var platformModel = _mapper.Map<Platform>(platformUpdateDto);
            platformModel.Id = id;
            _repository.UpdatePlatform(platformModel);
            _repository.SaveChanges();

            var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

            // Asynchronously send data to RabbitMQ
            try 
            {
                var platformPublishedDto = _mapper.Map<PlatformPublishedDto>(platformReadDto);
                platformPublishedDto.Event = Const.EVENT_PLATFORM_UPDATE;
                _messageBusClient.PublishPlatform(platformPublishedDto);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"---> Could not send asynchronously: {ex.Message}");
            }

            return CreatedAtRoute(nameof(GetPlatformById), new { Id = platformReadDto.Id}, platformReadDto);
        }
    }
}