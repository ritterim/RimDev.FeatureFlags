using System.Threading.Tasks;

namespace FeatureFlags
{
    public interface IStore
    {
        Task<FeatureStoreData> Get<T>() where T : Feature;
        Task<T> Save<T>(T feature) where T : Feature;
        Task<bool> Enrich<T>(T feature) where T : Feature;
    }
}
