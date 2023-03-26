#nullable disable

using AutoMapper;
using CommandService.Models;
using Grpc.Net.Client;
using PlatformService;

namespace CommandService.SyncDataServices.Grpc
{
    # region Const
    public class Const
    {
        public const string CONFIG_GRPC_PLATFORM = "GrpcPlatform";
    }
    # endregion

    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public PlatformDataClient(IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _mapper = mapper;
        }
        public IEnumerable<Platform> ReturnAllPlatforms()
        {
            System.Console.WriteLine($"---> Calling GRPC service {_configuration[Const.CONFIG_GRPC_PLATFORM]}");
            var channel = GrpcChannel.ForAddress(_configuration[Const.CONFIG_GRPC_PLATFORM]);
            var client = new GrpcPlatform.GrpcPlatformClient(channel);
            var request = new GetAllRequest();

            try 
            {
                var reply = client.GetAllPlatform(request);
                return _mapper.Map<IEnumerable<Platform>>(reply.Platform);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"---> Couldnt call GRPC server: {ex.Message}");
            }
            return null;
        }
    }
}