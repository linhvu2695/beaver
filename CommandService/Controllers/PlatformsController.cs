using AutoMapper;
using CommandService.Data;
using CommandService.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers
{
    [Route("api/c/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly  ICommandRepo _repository;
        private readonly IMapper _mapper;
        
        public PlatformsController(ICommandRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            System.Console.WriteLine("---> Getting Platforms from CommandService...");

            var platformItem = _repository.GetAllPlatforms();

            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItem));
        }

        [HttpPost]
        public ActionResult TestInboundConnection()
        {
            System.Console.WriteLine("---> Inbound POST # Command Service");

            return Ok("Inbound test from Platform Controller");
        }
    }
}