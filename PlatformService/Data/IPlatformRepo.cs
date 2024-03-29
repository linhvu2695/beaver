using PlatformService.Models;

namespace PlatformService.Data
{
    public interface IPlatformRepo
    {
        bool SaveChanges();

        IEnumerable<Platform> GetAllPlatforms();

        Platform? GetPlatformById(int id);

        void CreatePlatform(Platform p);

        bool PlatformExists(int platformId);

        void UpdatePlatform(Platform platform);
    }
}