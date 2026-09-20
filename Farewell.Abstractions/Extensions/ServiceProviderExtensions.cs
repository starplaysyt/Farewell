namespace Farewell.Abstractions.DI;

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
    }
}