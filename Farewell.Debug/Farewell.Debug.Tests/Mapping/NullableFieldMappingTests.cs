using Farewell.Abstractions.Mapping;
using Farewell.Mapping;
using Farewell.Mapping.Exceptions;

namespace Farewell.Debug.Tests.Mapping;

public class NullableFieldMappingTests
{
    private readonly IMappingProvider _mapper = MappingProvider.Instance;

    [Fact]
    public void NullableField_SourceHasValues_MapsCorrectly()
    {
        var source = new SourceForNullableField
        {
            Id = 1,
            OptionalName = "Hello",
            OptionalAge = 25
        };

        var result = _mapper.Map<SourceForNullableField, TargetWithNullableField>(source);

        Assert.Equal(1, result.Id);
        Assert.NotNull(result.OptionalName);
        Assert.Equal("Hello", result.OptionalName!.Value.Value);
        Assert.NotNull(result.OptionalAge);
        Assert.Equal(25, result.OptionalAge!.Value.Value);
    }

    [Fact]
    public void NullableField_SourceMissingProperties_SetsNull()
    {
        var source = new SourcePartialForNullableField { Id = 1 };

        var result = _mapper.Map<SourcePartialForNullableField, TargetWithNullableField>(source);

        Assert.Equal(1, result.Id);
        Assert.Null(result.OptionalName);
        Assert.Null(result.OptionalAge);
    }

    [Fact]
    public void NullableFieldNonNullable_SourceMissing_ThrowsMappingCompilationException()
    {
        var source = new MinimalSource { Id = 1 };

        var ex = Assert.Throws<MappingCompilationException>(() =>
            _mapper.Map<MinimalSource, TargetWithNonNullableNullableField>(source));

        Assert.Contains("RequiredField", ex.Message);
    }

    [Fact]
    public void NullableFieldWithNullableInner_IntNullable_MapsCorrectly()
    {
        var source = new SourceWithNullableInt { Id = 1, MaybeValue = 42 };

        var result =
            _mapper.Map<SourceWithNullableInt, TargetWithNullableFieldNullableInner>(source);

        Assert.Equal(1, result.Id);
        Assert.NotNull(result.MaybeValue);
        Assert.Equal(42, result.MaybeValue!.Value.Value);
    }

    [Fact]
    public void NullableFieldWithNullableInner_SourceNull_MapsCorrectly()
    {
        var source = new SourceWithNullableInt { Id = 1, MaybeValue = null };

        var result =
            _mapper.Map<SourceWithNullableInt, TargetWithNullableFieldNullableInner>(source);

        Assert.Equal(1, result.Id);
        Assert.NotNull(result.MaybeValue);
        Assert.Null(result.MaybeValue!.Value.Value);
    }
}