#nullable disable

using AutoMapper;
using CommandService.Models;
using Grpc.Core.Interceptors;
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
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            System.Console.WriteLine($"---> Calling GRPC service {_configuration[Const.CONFIG_GRPC_PLATFORM]}");
            var channel = GrpcChannel.ForAddress(_configuration[Const.CONFIG_GRPC_PLATFORM]);

            // channel.Intercept(interceptor);
            var invoker = channel.Intercept(new ClientLoggingInterceptor(loggerFactory));

            var client = new GrpcPlatform.GrpcPlatformClient(invoker);
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