using Farewell.Abstractions.Mapping;
using Farewell.Mapping;
using static Xunit.TestContext;

namespace Farewell.Debug.Tests.Mapping;

public class CachingTests
{
    private readonly IMappingProvider _mapper = MappingProvider.Instance;

    [Fact]
    public void Map_CalledTwice_ReturnsSameResult()
    {
        var source = new SimpleSource { Id = 1, Name = "Cache", Score = 5.0 };

        var result1 = _mapper.Map<SimpleSource, SimpleTarget>(source);
        var result2 = _mapper.Map<SimpleSource, SimpleTarget>(source);

        Assert.Equal(result1.Id, result2.Id);
        Assert.Equal(result1.Name, result2.Name);
        Assert.Equal(result1.Score, result2.Score);
    }

    [Fact]
    public void Map_DifferentSourceInstances_BothWork()
    {
        var source1 = new SimpleSource { Id = 1, Name = "A", Score = 1.0 };
        var source2 = new SimpleSource { Id = 2, Name = "B", Score = 2.0 };

        var result1 = _mapper.Map<SimpleSource, SimpleTarget>(source1);
        var result2 = _mapper.Map<SimpleSource, SimpleTarget>(source2);

        Assert.Equal(1, result1.Id);
        Assert.Equal(2, result2.Id);
    }

    [Fact]
    public async Task Map_ThreadSafety_NoCrash()
    {
        var tasks = Enumerable.Range(0, 100).Select(i => Task.Run(() =>
        {
            var source = new SimpleSource { Id = i, Name = $"Thread{i}", Score = i * 0.1 };
            var result = _mapper.Map<SimpleSource, SimpleTarget>(source);
            Assert.Equal(i, result.Id);
        }));

        await Task.WhenAll(tasks);
    }
}