using System.Threading.Tasks;

namespace RimDev.AspNetCore.FeatureFlags
{
    public interface IFeatureProvider
    {
        Task<TFeature> Get<TFeature>();

        Task Set<TFeature>(TFeature feature);
    }
}
