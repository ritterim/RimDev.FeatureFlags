using System.Threading.Tasks;

namespace RimDev.AspNetCore.FeatureFlags
{
    public interface IFeatureProvider
    {
        Task<Feature> Get(string featureName);

        Task Set<TFeature>(TFeature feature);
    }
}
