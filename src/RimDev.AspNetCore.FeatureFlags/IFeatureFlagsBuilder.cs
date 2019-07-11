using Microsoft.Extensions.DependencyInjection;

namespace FeatureFlags
{
    public interface IFeatureFlagsBuilder
    {
        IServiceCollection ServiceCollection { get; }
    }
}
