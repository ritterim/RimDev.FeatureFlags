using System.Threading.Tasks;

namespace FeatureFlags
{
    public interface IStore
    {
        Task<T> Save<T>(T feature) where T : Feature;
        Task<T> Get<T>() where T : Feature;
    }
}
