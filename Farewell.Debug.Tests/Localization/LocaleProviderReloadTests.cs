using System.Collections.Concurrent;

namespace Farewell.Debug.Tests.Localization;

[Collection("Localization")]
public class LocaleProviderReloadTests : LocalizationTestBase
{
    [Fact]
    public void Reload_UpdatesValuesFromFile()
    {
        WriteLocaleFile("ru-ru", new Dictionary<string, string?>
        {
            [TestLocaleModule.TestMessage] = "Старое значение",
            [TestLocaleModule.HelloWorld] = "Привет мир",
            [TestLocaleModule.ErrorOccurred] = "Произошла ошибка"
        });

        var sp = BuildServiceProvider(
            m => m.AddModule(typeof(TestLocaleModule)),
            "ru-ru"
        );

        var provider = GetProvider(sp, "ru-ru");

        Assert.Equal("Старое значение", provider[TestLocaleModule.TestMessage]);

        WriteLocaleFile("ru-ru", new Dictionary<string, string?>
        {
            [TestLocaleModule.TestMessage] = "Новое значение",
            [TestLocaleModule.HelloWorld] = "Привет мир",
            [TestLocaleModule.ErrorOccurred] = "Произошла ошибка"
        });

        provider.Reload();

        Assert.Equal("Новое значение", provider[TestLocaleModule.TestMessage]);
    }

    [Fact]
    public void Reload_ValueBecomesNull_FallsBackToDefault()
    {
        WriteLocaleFile("ru-ru", new Dictionary<string, string?>
        {
            [TestLocaleModule.TestMessage] = "Тестовое сообщение",
            [TestLocaleModule.HelloWorld] = "Привет мир",
            [TestLocaleModule.ErrorOccurred] = "Произошла ошибка"
        });

        var sp = BuildServiceProvider(
            m => m.AddModule(typeof(TestLocaleModule)),
            "ru-ru"
        );

        var provider = GetProvider(sp, "ru-ru");

        WriteLocaleFile("ru-ru", new Dictionary<string, string?>
        {
            [TestLocaleModule.TestMessage] = null,
            [TestLocaleModule.HelloWorld] = "Привет мир",
            [TestLocaleModule.ErrorOccurred] = "Произошла ошибка"
        });

        provider.Reload();

        Assert.Equal("Test message", provider[TestLocaleModule.TestMessage]);
    }

    [Fact]
    public void Reload_FileMissingKeys_SynchronizesFile()
    {
        WriteLocaleFile("ru-ru", new Dictionary<string, string?>
        {
            [TestLocaleModule.TestMessage] = "Тестовое сообщение",
            [TestLocaleModule.HelloWorld] = "Привет мир",
            [TestLocaleModule.ErrorOccurred] = "Произошла ошибка"
        });

        var sp = BuildServiceProvider(
            m => m.AddModule(typeof(TestLocaleModule)),
            "ru-ru"
        );

        var provider = GetProvider(sp, "ru-ru");

        WriteLocaleFile("ru-ru", new Dictionary<string, string?>
        {
            [TestLocaleModule.TestMessage] = "Тестовое сообщение"
        });

        provider.Reload();

        var dict = ReadLocaleFileAsDict("ru-ru");

        Assert.True(dict.ContainsKey(TestLocaleModule.HelloWorld));
        Assert.True(dict.ContainsKey(TestLocaleModule.ErrorOccurred));
    }

    [Fact]
    public void Reload_FileDeletedBeforeReload_RecreatesFile()
    {
        WriteLocaleFile("ru-ru", new Dictionary<string, string?>
        {
            [TestLocaleModule.TestMessage] = "Тестовое сообщение",
            [TestLocaleModule.HelloWorld] = "Привет мир",
            [TestLocaleModule.ErrorOccurred] = "Произошла ошибка"
        });

        var sp = BuildServiceProvider(
            m => m.AddModule(typeof(TestLocaleModule)),
            "ru-ru"
        );

        var provider = GetProvider(sp, "ru-ru");

        File.Delete(GetLocalePath("ru-ru"));
        provider.Reload();

        Assert.True(File.Exists(GetLocalePath("ru-ru")));
        Assert.Equal("Test message", provider[TestLocaleModule.TestMessage]);
    }

    [Fact]
    public async Task Reload_ConcurrentReloads_NoExceptions()
    {
        WriteLocaleFile("ru-ru", new Dictionary<string, string?>
        {
            [TestLocaleModule.TestMessage] = "Тестовое сообщение",
            [TestLocaleModule.HelloWorld] = "Привет мир",
            [TestLocaleModule.ErrorOccurred] = "Произошла ошибка"
        });

        var sp = BuildServiceProvider(
            m => m.AddModule(typeof(TestLocaleModule)),
            "ru-ru"
        );

        var provider = GetProvider(sp, "ru-ru");
        var exceptions = new ConcurrentBag<Exception>();

        var tasks = Enumerable.Range(0, 10).Select(_ => Task.Run(() =>
        {
            try
            {
                for (var i = 0; i < 50; i++)
                {
                    provider.Reload();
                    var any = provider[TestLocaleModule.TestMessage];
                    var test = provider[TestLocaleModule.HelloWorld];
                }
            }
            catch (Exception ex) { exceptions.Add(ex); }
        }));
        
        await Task.WhenAll(tasks);

        Assert.Empty(exceptions);
    }

    [Fact]
    public async Task Reload_ConcurrentReadsAndReload_NoExceptions()
    {
        WriteLocaleFile("ru-ru", new Dictionary<string, string?>
        {
            [TestLocaleModule.TestMessage] = "Тестовое сообщение",
            [TestLocaleModule.HelloWorld] = "Привет мир",
            [TestLocaleModule.ErrorOccurred] = "Произошла ошибка"
        });

        var sp = BuildServiceProvider(
            m => m.AddModule(typeof(TestLocaleModule)),
            "ru-ru"
        );

        var provider = GetProvider(sp, "ru-ru");
        var exceptions = new ConcurrentBag<Exception>();
        var cts = new CancellationTokenSource();

        var readTasks = Enumerable.Range(0, 5).Select(_ => Task.Run(() =>
        {
            try
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    var message = provider[TestLocaleModule.TestMessage];
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex) { exceptions.Add(ex); }
        }, cts.Token));

        var reloadTask = Task.Run(() =>
        {
            try
            {
                for (var i = 0; i < 50; i++)
                {
                    provider.Reload();
                    Thread.Sleep(1);
                }
            }
            catch (Exception ex) { exceptions.Add(ex); }
            finally { cts.Cancel(); }
        }, cts.Token);

        await Task.WhenAll(readTasks.Append(reloadTask).ToArray());

        Assert.Empty(exceptions);
    }
}