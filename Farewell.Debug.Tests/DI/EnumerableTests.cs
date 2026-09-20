using Farewell.Abstractions.DI;
using Farewell.DI;

namespace Farewell.Debug.Tests.DI;

public class EnumerableTests
{
    [Fact]
    public void GetServices_MultipleRegistrations_ReturnsAll()
    {
        var provider = new ServiceBuilder()
            .AddSingleton<INotifier, EmailNotifier>()
            .AddSingleton<INotifier, SmsNotifier>()
            .AddSingleton<INotifier, PushNotifier>()
            .Build();

        var notifiers = provider.GetServices<INotifier>().ToList();
        Assert.Equal(3, notifiers.Count);
        Assert.Contains(notifiers, n => n.Name == "email");
        Assert.Contains(notifiers, n => n.Name == "sms");
        Assert.Contains(notifiers, n => n.Name == "push");
    }

    [Fact]
    public void GetServices_NoRegistrations_ReturnsEmpty()
    {
        var provider = new ServiceBuilder().Build();
        var result = provider.GetServices<INotifier>();
        Assert.Empty(result);
    }

    [Fact]
    public void GetServices_SingleRegistration_ReturnsOne()
    {
        var provider = new ServiceBuilder()
            .AddSingleton<INotifier, EmailNotifier>()
            .Build();

        var result = provider.GetServices<INotifier>().ToList();
        Assert.Single(result);
    }

    [Fact]
    public void Enumerable_InjectedIntoConstructor()
    {
        var provider = new ServiceBuilder()
            .AddSingleton<INotifier, EmailNotifier>()
            .AddSingleton<INotifier, SmsNotifier>()
            .AddSingleton<NotifierConsumer>()
            .Build();

        var consumer = provider.GetService<NotifierConsumer>()!;
        Assert.Equal(2, consumer.Notifiers.Count());
    }

    [Fact]
    public void Enumerable_WithTransients_NewInstancesEachTime()
    {
        var provider = new ServiceBuilder()
            .AddTransient<INotifier, EmailNotifier>()
            .AddTransient<INotifier, SmsNotifier>()
            .Build();

        var list1 = provider.GetServices<INotifier>().ToList();
        var list2 = provider.GetServices<INotifier>().ToList();

        Assert.NotSame(list1[0], list2[0]);
        Assert.NotSame(list1[1], list2[1]);
    }

    [Fact]
    public void Enumerable_MixedLifetimes_SingletonsSame()
    {
        var provider = new ServiceBuilder()
            .AddSingleton<INotifier, EmailNotifier>()
            .AddTransient<INotifier, SmsNotifier>()
            .Build();

        var list1 = provider.GetServices<INotifier>().ToList();
        var list2 = provider.GetServices<INotifier>().ToList();

        var email1 = list1.First(n => n.Name == "email");
        var email2 = list2.First(n => n.Name == "email");
        Assert.Same(email1, email2);

        var sms1 = list1.First(n => n.Name == "sms");
        var sms2 = list2.First(n => n.Name == "sms");
        Assert.NotSame(sms1, sms2);
    }
}