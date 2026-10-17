using Farewell.Abstractions.DI;

namespace Farewell.Abstractions.Extensions;

public static class ServiceBuilderExtensions
{
    extension(IServiceBuilder builder)
    {
        #region Singleton

        public IServiceBuilder AddSingleton<TService>()
            where TService : class =>
            builder.AddService(typeof(TService), typeof(TService),
                ServiceLifetimeType.Singleton);

        public IServiceBuilder AddSingleton<TService>(TService instance)
            where TService : class =>
            builder.AddService(typeof(TService), (_, _) => instance,
                ServiceLifetimeType.Singleton);

        public IServiceBuilder AddSingleton<TService>(Func<IServiceProvider, TService> factory)
            where TService : class =>
            builder.AddService(typeof(TService), factory, ServiceLifetimeType.Singleton);

        public IServiceBuilder AddSingleton<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService =>
            builder.AddService(typeof(TService), typeof(TImplementation),
                ServiceLifetimeType.Singleton);

        public IServiceBuilder AddSingleton(Type serviceType) =>
            builder.AddService(serviceType, serviceType, ServiceLifetimeType.Singleton);

        public IServiceBuilder AddSingleton(Type serviceType, Type implementationType) =>
            builder.AddService(serviceType, implementationType, ServiceLifetimeType.Singleton);

        public IServiceBuilder AddSingleton(Type serviceType,
            Func<IServiceProvider, object> factory) =>
            builder.AddService(serviceType, factory, ServiceLifetimeType.Singleton);

        public IServiceBuilder AddSingleton(Type serviceType, object instance) =>
            builder.AddService(serviceType, (_, _) => instance, ServiceLifetimeType.Singleton);

        #endregion

        #region Scoped

        public IServiceBuilder AddScoped<TService>()
            where TService : class =>
            builder.AddService(typeof(TService), typeof(TService), ServiceLifetimeType.Scoped);

        public IServiceBuilder AddScoped<TService>(Func<IServiceProvider, TService> factory)
            where TService : class =>
            builder.AddService(typeof(TService), factory, ServiceLifetimeType.Scoped);

        public IServiceBuilder AddScoped<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService =>
            builder.AddService(typeof(TService), typeof(TImplementation),
                ServiceLifetimeType.Scoped);

        public IServiceBuilder AddScoped(Type serviceType) =>
            builder.AddService(serviceType, serviceType, ServiceLifetimeType.Scoped);

        public IServiceBuilder AddScoped(Type serviceType, Type implementationType) =>
            builder.AddService(serviceType, implementationType, ServiceLifetimeType.Scoped);

        public IServiceBuilder AddScoped(Type serviceType,
            Func<IServiceProvider, object> factory) =>
            builder.AddService(serviceType, factory, ServiceLifetimeType.Scoped);

        #endregion

        #region Transient

        public IServiceBuilder AddTransient<TService>()
            where TService : class =>
            builder.AddService(
                typeof(TService), typeof(TService), ServiceLifetimeType.Transient);

        public IServiceBuilder AddTransient<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService =>
            builder.AddService(
                typeof(TService), typeof(TImplementation), ServiceLifetimeType.Transient);

        public IServiceBuilder AddTransient<TService>(Func<IServiceProvider, TService> factory)
            where TService : class =>
            builder.AddService(
                typeof(TService), factory, ServiceLifetimeType.Transient);

        public IServiceBuilder AddTransient(Type serviceType) =>
            builder.AddService(
                serviceType, serviceType, ServiceLifetimeType.Transient);

        public IServiceBuilder AddTransient(Type serviceType, Type implementationType) =>
            builder.AddService(
                serviceType, implementationType, ServiceLifetimeType.Transient);

        public IServiceBuilder AddTransient(Type serviceType,
            Func<IServiceProvider, object> factory) =>
            builder.AddService(
                serviceType, factory, ServiceLifetimeType.Transient);

        #endregion

        #region KeyedSingleton

        public IServiceBuilder AddKeyedSingleton<TService>(object key)
            where TService : class =>
            builder.AddService(typeof(TService), typeof(TService),
                ServiceLifetimeType.Singleton, key);

        public IServiceBuilder AddKeyedSingleton<TService>(object key, TService instance)
            where TService : class =>
            builder.AddService(typeof(TService), (_, _) => instance,
                ServiceLifetimeType.Singleton, key);

        public IServiceBuilder AddKeyedSingleton<TService>(object key,
            Func<IServiceProvider, TService> factory)
            where TService : class =>
            builder.AddService(typeof(TService), factory, ServiceLifetimeType.Singleton, key);

        public IServiceBuilder AddKeyedSingleton<TService, TImplementation>(object key)
            where TService : class
            where TImplementation : class, TService =>
            builder.AddService(typeof(TService), typeof(TImplementation),
                ServiceLifetimeType.Singleton, key);

        public IServiceBuilder AddKeyedSingleton(object key, Type serviceType) =>
            builder.AddService(serviceType, serviceType, ServiceLifetimeType.Singleton, key);

        public IServiceBuilder AddKeyedSingleton(object key, Type serviceType,
            Type implementationType) =>
            builder.AddService(serviceType, implementationType, ServiceLifetimeType.Singleton, key);

        public IServiceBuilder AddKeyedSingleton(object key, Type serviceType,
            Func<IServiceProvider, object> factory) =>
            builder.AddService(serviceType, factory, ServiceLifetimeType.Singleton, key);

        public IServiceBuilder AddKeyedSingleton(object key, Type serviceType, object instance) =>
            builder.AddService(serviceType, (_, _) => instance, ServiceLifetimeType.Singleton, key);

        #endregion

        #region KeyedScoped

        public IServiceBuilder AddKeyedScoped<TService>(object key)
            where TService : class =>
            builder.AddService(typeof(TService), typeof(TService), ServiceLifetimeType.Scoped, key);

        public IServiceBuilder AddKeyedScoped<TService>(object key, Func<IServiceProvider, TService> factory)
            where TService : class =>
            builder.AddService(typeof(TService), factory, ServiceLifetimeType.Scoped, key);

        public IServiceBuilder AddKeyedScoped<TService, TImplementation>(object key)
            where TService : class
            where TImplementation : class, TService =>
            builder.AddService(typeof(TService), typeof(TImplementation),
                ServiceLifetimeType.Scoped, key);

        public IServiceBuilder AddKeyedScoped(object key, Type serviceType) =>
            builder.AddService(serviceType, serviceType, ServiceLifetimeType.Scoped, key);

        public IServiceBuilder AddKeyedScoped(object key, Type serviceType, Type implementationType) =>
            builder.AddService(serviceType, implementationType, ServiceLifetimeType.Scoped, key);

        public IServiceBuilder AddKeyedScoped(object key, Type serviceType,
            Func<IServiceProvider, object> factory) =>
            builder.AddService(serviceType, factory, ServiceLifetimeType.Scoped, key);

        #endregion

        #region KeyedTransient

        public IServiceBuilder AddKeyedTransient<TService>(object key)
            where TService : class =>
            builder.AddService(
                typeof(TService), typeof(TService), ServiceLifetimeType.Transient, key);

        public IServiceBuilder AddKeyedTransient<TService, TImplementation>(object key)
            where TService : class
            where TImplementation : class, TService =>
            builder.AddService(
                typeof(TService), typeof(TImplementation), ServiceLifetimeType.Transient, key);

        public IServiceBuilder AddKeyedTransient<TService>(object key, Func<IServiceProvider, TService> factory)
            where TService : class =>
            builder.AddService(
                typeof(TService), factory, ServiceLifetimeType.Transient, key);

        public IServiceBuilder AddKeyedTransient(object key, Type serviceType) =>
            builder.AddService(
                serviceType, serviceType, ServiceLifetimeType.Transient, key);

        public IServiceBuilder AddKeyedTransient(object key, Type serviceType, Type implementationType) =>
            builder.AddService(
                serviceType, implementationType, ServiceLifetimeType.Transient, key);

        public IServiceBuilder AddKeyedTransient(object key, Type serviceType,
            Func<IServiceProvider, object> factory) =>
            builder.AddService(
                serviceType, factory, ServiceLifetimeType.Transient, key);

        #endregion
    }
}