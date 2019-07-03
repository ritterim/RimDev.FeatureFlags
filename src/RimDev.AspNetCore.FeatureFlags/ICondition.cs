using System.Threading.Tasks;

namespace FeatureFlags
{
    public interface ICondition
    {
        /// <summary>
        /// Applying a condition on a feature
        /// could alter the feature that is passed in
        /// to make it conform to the requirements of the condition
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        Task Apply(Feature feature);
    }
}
