namespace Farewell.Abstractions.DI;

public static class ServiceBuilderExtensions
{
    extension(IServiceBuilder builder)
    {
        public IServiceBuilder AddSingleton<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService =>
            builder.AddService(typeof(TService), typeof(TImplementation),
                ServiceLifetimeType.Singleton);

        public IServiceBuilder AddSingleton<TService>()
            where TService : class =>
            builder.AddService(typeof(TService), typeof(TService),
                ServiceLifetimeType.Singleton);

        public IServiceBuilder AddSingleton<TService>(Func<IServiceProvider, TService> factory)
            where TService : class =>
            builder.AddService(typeof(TService), factory, ServiceLifetimeType.Singleton);

        public IServiceBuilder AddSingleton(Type serviceType, Type implementationType) =>
            builder.AddService(serviceType, implementationType, ServiceLifetimeType.Singleton);

        public IServiceBuilder AddScoped<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService =>
            builder.AddService(typeof(TService), typeof(TImplementation),
                ServiceLifetimeType.Scoped);

        public IServiceBuilder AddScoped<TService>()
            where TService : class =>
            builder.AddService(typeof(TService), typeof(TService), ServiceLifetimeType.Scoped);

        public IServiceBuilder AddScoped<TService>(Func<IServiceProvider, TService> factory)
            where TService : class =>
            builder.AddService(typeof(TService), factory, ServiceLifetimeType.Scoped);

        public IServiceBuilder AddScoped(Type serviceType, Type implementationType) =>
            builder.AddService(serviceType, implementationType, ServiceLifetimeType.Scoped);

        public IServiceBuilder AddTransient<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService =>
            builder.AddService(
                typeof(TService), typeof(TImplementation), ServiceLifetimeType.Transient);

        public IServiceBuilder AddTransient<TService>()
            where TService : class =>
            builder.AddService(
                typeof(TService), typeof(TService), ServiceLifetimeType.Transient);

        public IServiceBuilder AddTransient<TService>(Func<IServiceProvider, TService> factory)
            where TService : class =>
            builder.AddService(
                typeof(TService), factory, ServiceLifetimeType.Transient);

        public IServiceBuilder AddTransient(Type serviceType, Type implementationType) =>
            builder.AddService(
                serviceType, implementationType, ServiceLifetimeType.Transient);

        public IServiceBuilder AddKeyedSingleton<TService, TImplementation>(object key)
            where TService : class
            where TImplementation : class, TService =>
            builder.AddService(
                typeof(TService), typeof(TImplementation), ServiceLifetimeType.Singleton, key);

        public IServiceBuilder AddKeyedSingleton<TService>(object key,
            Func<IServiceProvider, TService> factory)
            where TService : class =>
            builder.AddService(
                typeof(TService), factory, ServiceLifetimeType.Singleton, key);

        public IServiceBuilder AddKeyedScoped<TService, TImplementation>(object key)
            where TService : class
            where TImplementation : class, TService =>
            builder.AddService(
                typeof(TService), typeof(TImplementation), ServiceLifetimeType.Scoped, key);

        public IServiceBuilder AddKeyedScoped<TService>(object key,
            Func<IServiceProvider, TService> factory)
            where TService : class =>
            builder.AddService(
                typeof(TService), factory, ServiceLifetimeType.Scoped, key);

        public IServiceBuilder AddKeyedTransient<TService, TImplementation>(object key)
            where TService : class
            where TImplementation : class, TService =>
            builder.AddService(
                typeof(TService), typeof(TImplementation), ServiceLifetimeType.Transient, key);

        public IServiceBuilder AddKeyedTransient<TService>(object key,
            Func<IServiceProvider, TService> factory)
            where TService : class =>
            builder.AddService(
                typeof(TService), factory, ServiceLifetimeType.Transient, key);
    }
}