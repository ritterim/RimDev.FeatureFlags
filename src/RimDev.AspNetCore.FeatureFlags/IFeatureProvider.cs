using System.Threading.Tasks;

namespace RimDev.AspNetCore.FeatureFlags
{
    public interface IFeatureProvider
    {
        Task Initialize();

        Task<TFeature> Get<TFeature>() where TFeature: Feature;

        Task Set<TFeature>(TFeature feature) where TFeature: Feature;
    }
}
