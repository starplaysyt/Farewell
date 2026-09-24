using Farewell.Abstractions.DI;

namespace Farewell.Abstractions.Extensions;

public static class ServiceProviderExtensions
{
    extension(IServiceProvider provider)
    {
        /// <summary>
        /// Returns service T
        /// </summary>
        public T? GetService<T>() where T : class
        {
            ArgumentNullException.ThrowIfNull(provider);
            return (T?)provider.GetService(typeof(T));
        }

        /// <summary>
        /// Returns service T. Drops exception if not found.
        /// </summary>
        public T GetRequiredService<T>() where T : class
        {
            ArgumentNullException.ThrowIfNull(provider);
            var service = (T?)provider.GetService(typeof(T));
            if (service is null)
                throw new InvalidOperationException(
                    $"Service of type '{typeof(T)}' is not registered.");
            return service;
        }

        /// <summary>
        /// Non-generic version of GetRequiredService
        /// </summary>
        public object GetRequiredService(Type serviceType)
        {
            ArgumentNullException.ThrowIfNull(provider);
            ArgumentNullException.ThrowIfNull(serviceType);

            var service = provider.GetService(serviceType);
            if (service is null)
                throw new InvalidOperationException(
                    $"Service of type '{serviceType}' is not registered.");
            return service;
        }

        /// <summary>
        /// Returns all registered implementations of T
        /// </summary>
        public IEnumerable<T> GetServices<T>() where T : class
        {
            ArgumentNullException.ThrowIfNull(provider);
            var result = provider.GetService(typeof(IEnumerable<T>));
            return (IEnumerable<T>)(result ?? Array.Empty<T>());
        }

        /// <summary>
        /// Non-generic version of GetServices
        /// </summary>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            ArgumentNullException.ThrowIfNull(provider);
            ArgumentNullException.ThrowIfNull(serviceType);

            var enumerableType = typeof(IEnumerable<>).MakeGenericType(serviceType);
            var result = provider.GetService(enumerableType);
            return (IEnumerable<object>?)result ?? [];
        }
        
        /// <summary>
        /// Returns keyed service of type T, or null
        /// </summary>
        public T? GetKeyedService<T>(object key)
            where T : class
        {
            ArgumentNullException.ThrowIfNull(provider);
            ArgumentNullException.ThrowIfNull(key);
            if (provider is not IKeyedServiceProvider keyedServiceProvider)
                throw new InvalidOperationException("This service provider doesn't support keyed services.");
            
            return (T?)keyedServiceProvider.GetKeyedService(typeof(T), key);
        }

        /// <summary>
        /// Returns keyed service of type T. Throws exception when there is no such.
        /// </summary>
        public T GetRequiredKeyedService<T>(object key)
            where T : class
        {
            ArgumentNullException.ThrowIfNull(provider);
            ArgumentNullException.ThrowIfNull(key);
            if (provider is not IKeyedServiceProvider keyedServiceProvider)
                throw new InvalidOperationException("This service provider doesn't support keyed services.");

            var service = (T?)keyedServiceProvider.GetKeyedService(typeof(T), key);
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
            
            if (provider is not IKeyedServiceProvider keyedServiceProvider)
                throw new InvalidOperationException("This service provider doesn't support keyed services.");

            var service = keyedServiceProvider.GetKeyedService(serviceType, key);
            if (service is null)
                throw new InvalidOperationException(
                    $"Keyed service of type '{serviceType}' with key '{key}' is not registered.");
            return service;
        }
    }
}