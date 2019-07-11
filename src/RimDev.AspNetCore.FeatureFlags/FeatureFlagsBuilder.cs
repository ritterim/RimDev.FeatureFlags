using Microsoft.Extensions.DependencyInjection;

namespace FeatureFlags
{
    public class FeatureFlagsBuilder : IFeatureFlagsBuilder
    {
        public FeatureFlagsBuilder(IServiceCollection serviceCollection)
        {
            ServiceCollection = serviceCollection;
        }

        public IServiceCollection ServiceCollection { get; }
    }
}
