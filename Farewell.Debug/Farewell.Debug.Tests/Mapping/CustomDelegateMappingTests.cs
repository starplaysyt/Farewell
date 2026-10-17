using Farewell.Abstractions.Mapping;
using Farewell.Mapping;

namespace Farewell.Debug.Tests.Mapping;

public class CustomDelegateMappingTests
{
    private readonly IMappingProvider _mapper = MappingProvider.Instance;

    [Fact]
    public void Map_WithCustomFactory_UsesFactory()
    {
        var source = new SimpleSource { Id = 1, Name = "Custom", Score = 3.0 };

        var result = _mapper.Map<SimpleSource, SimpleTarget>(source, s => new SimpleTarget
        {
            Id = s.Id * 100,
            Name = s.Name.ToUpper(),
            Score = s.Score + 1
        });

        Assert.Equal(100, result.Id);
        Assert.Equal("CUSTOM", result.Name);
        Assert.Equal(4.0, result.Score);
    }

    [Fact]
    public void Map_WithCustomFactory_DoesNotAffectCachedMapper()
    {
        var source = new SimpleSource { Id = 1, Name = "Test", Score = 2.0 };
        
        var custom = _mapper.Map<SimpleSource, SimpleTarget>
            (source, s => new SimpleTarget
        {
            Id = 999,
            Name = "overridden",
            Score = 0
        });
        
        var auto = _mapper.Map<SimpleSource, SimpleTarget>(source);

        Assert.Equal(999, custom.Id);
        Assert.Equal(1, auto.Id);
        Assert.Equal("Test", auto.Name);
    }

    [Fact]
    public void Map_WithCustomFactory_NullFactory_ThrowsArgumentNullException()
    {
        var source = new SimpleSource { Id = 1, Name = "A", Score = 1.0 };

        Assert.Throws<ArgumentNullException>(() =>
            _mapper.Map<SimpleSource, SimpleTarget>(source,
                (Func<SimpleSource, SimpleTarget>)null!));
    }
}