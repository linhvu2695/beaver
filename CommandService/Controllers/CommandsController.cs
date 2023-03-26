using AutoMapper;
using CommandService.Data;
using CommandService.DTOs;
using CommandService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers
{
    [Route("api/c/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly  ICommandRepo _repository;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
        {
            System.Console.WriteLine($"---> Get All Commands for Platform {platformId}...");

            if (!_repository.PlatformExists(platformId))
            {
                return NotFound();
            }

            var commands = _repository.GetCommandsForPlatform(platformId);

            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
        }

        [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
        public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
        {
            System.Console.WriteLine($"---> Get Command {commandId} for Platform {platformId}...");

            if (!_repository.PlatformExists(platformId))
            {
                return NotFound();
            }

            var command = _repository.GetCommand(platformId, commandId);

            if (command == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CommandReadDto>(command));
        }

        [HttpPost]
        public ActionResult<CommandCreateDto> CreateCommand(int platformId, CommandCreateDto commandCreateDto)
        {
            System.Console.WriteLine("--> Creating Command...");
            if (!_repository.PlatformExists(platformId))
            {
                return NotFound();
            }
            var commandModel = _mapper.Map<Command>(commandCreateDto);

            _repository.CreateCommand(platformId, commandModel);
            _repository.SaveChanges();

            var commandReadDto = _mapper.Map<CommandReadDto>(commandModel);

            return CreatedAtRoute(nameof(GetCommandForPlatform), 
                new { platformId = platformId, commandId = commandReadDto.Id}, commandReadDto);
        }
    }
}