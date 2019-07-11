using System;

namespace FeatureFlags
{
    public interface IStorePointer
    {
        string Name { get; }
        Type Type { get; }
    }

    public class StorePointer : IStorePointer
    {
        public StorePointer(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; }
        public Type Type { get; }
    }
}
