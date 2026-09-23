using Farewell.Abstractions.DI;

namespace Farewell.Abstractions.Extensions;

public static class KeyedServiceProviderExtensions
{
    extension(IKeyedServiceProvider provider)
    {
        /// <summary>
        /// Returns keyed service of type T, or null
        /// </summary>
        public T? GetKeyedService<T>(object key)
            where T : class
        {
            ArgumentNullException.ThrowIfNull(provider);
            ArgumentNullException.ThrowIfNull(key);
            return (T?)provider.GetKeyedService(typeof(T), key);
        }

        /// <summary>
        /// Returns keyed service of type T. Throws exception when there is no such.
        /// </summary>
        public T GetRequiredKeyedService<T>(object key)
            where T : class
        {
            ArgumentNullException.ThrowIfNull(provider);
            ArgumentNullException.ThrowIfNull(key);

            var service = (T?)provider.GetKeyedService(typeof(T), key);
            if (service is null)
                throw new InvalidOperationException(
                    $"Keyed service of type '{typeof(T)}' with key '{key}' is not registered.");
            return service;
        }

        /// <summary>
        /// Returns keyed service of type serviceType, or null
        /// </summary>
        public object GetRequiredKeyedService(Type serviceType, object key)
        {
            ArgumentNullException.ThrowIfNull(provider);
            ArgumentNullException.ThrowIfNull(serviceType);
            ArgumentNullException.ThrowIfNull(key);

            var service = provider.GetKeyedService(serviceType, key);
            if (service is null)
                throw new InvalidOperationException(
                    $"Keyed service of type '{serviceType}' with key '{key}' is not registered.");
            return service;
        }
    }
}