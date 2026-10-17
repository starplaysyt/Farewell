using Farewell.Abstractions.Mapping;
using Farewell.Mapping;
using Farewell.Mapping.Exceptions;

namespace Farewell.Debug.Tests.Mapping;

public class PropertyMappingTests
{
    private readonly IMappingProvider _mapper = MappingProvider.Instance;

    [Fact]
    public void SimpleMapping_AllPropertiesMatch_MapsCorrectly()
    {
        var source = new SimpleSource { Id = 1, Name = "Alice", Score = 9.5 };

        var result = _mapper.Map<SimpleSource, SimpleTarget>(source);

        Assert.Equal(1, result.Id);
        Assert.Equal("Alice", result.Name);
        Assert.Equal(9.5, result.Score);
    }

    [Fact]
    public void EmptyTypes_MapsWithoutError()
    {
        var source = new EmptySource();

        var result = _mapper.Map<EmptySource, EmptyTarget>(source);

        Assert.NotNull(result);
    }

    [Fact]
    public void CollectionProperty_SameType_MapsReference()
    {
        var source = new SourceWithList
        {
            Id = 1,
            Numbers = [10, 20, 30]
        };

        var result = _mapper.Map<SourceWithList, TargetWithList>(source);

        Assert.Equal(1, result.Id);
        Assert.Equal([10, 20, 30], result.Numbers);
    }

    [Fact]
    public void RequiredProperty_SourceHasIt_MapsCorrectly()
    {
        var source = new SourceWithId { Id = 42, Name = "Bob" };

        var result = _mapper.Map<SourceWithId, TargetWithRequired>(source);

        Assert.Equal(42, result.Id);
        Assert.Equal("Bob", result.Name);
    }

    [Fact]
    public void RequiredProperty_SourceMissing_ThrowsMappingCompilationException()
    {
        var source = new SourceMissingRequired { Score = 3.14 };

        var ex = Assert.Throws<MappingCompilationException>(() =>
            _mapper.Map<SourceMissingRequired, TargetWithRequired>(source));

        Assert.Contains("required property", ex.Message);
        Assert.Contains("Id", ex.Message);
    }

    [Fact]
    public void NullableProperty_SourceMissing_SetsNull()
    {
        var source = new SourceWithoutNullableFields { Id = 5 };

        var result = _mapper.Map<SourceWithoutNullableFields, TargetWithNullableProps>(source);

        Assert.Equal(5, result.Id);
        Assert.Null(result.Description);
        Assert.Null(result.OptionalCount);
    }

    [Fact]
    public void NonRequiredNonNullable_SourceMissing_SetsDefault()
    {
        var source = new MinimalSource { Id = 7 };

        var result = _mapper.Map<MinimalSource, TargetWithExtraProps>(source);

        Assert.Equal(7, result.Id);
        Assert.Equal(0, result.ExtraNumber);
        Assert.False(result.ExtraFlag);
    }

    [Fact]
    public void TypeMismatch_NoConversion_ThrowsMappingCompilationException()
    {
        var source = new SourceWithStringId { Id = "not-an-int", Name = "X" };

        var ex = Assert.Throws<MappingCompilationException>(() =>
            _mapper.Map<SourceWithStringId, TargetWithIntId>(source));

        Assert.Contains("type mismatch", ex.Message);
        Assert.Contains("Id", ex.Message);
    }

    [Fact]
    public void ImplicitNumericWidening_IntToLong_MapsCorrectly()
    {
        var source = new SourceWithInt { Value = 42 };

        var result = _mapper.Map<SourceWithInt, TargetWithLong>(source);

        Assert.Equal(42L, result.Value);
    }

    [Fact]
    public void InheritedProperties_MappedCorrectly()
    {
        var source = new DerivedSource { Id = 10, Name = "Derived" };

        var result = _mapper.Map<DerivedSource, DerivedTarget>(source);

        Assert.Equal(10, result.Id);
        Assert.Equal("Derived", result.Name);
    }

    [Fact]
    public void NoParameterlessCtor_NoAttribute_ThrowsMappingCompilationException()
    {
        var source = new SimpleSource { Id = 1, Name = "A", Score = 1.0 };

        var ex = Assert.Throws<MappingCompilationException>(() =>
            _mapper.Map<SimpleSource, NoParameterlessCtor>(source));

        Assert.Contains("parameterless constructor", ex.Message);
    }

    [Fact]
    public void NullSource_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _mapper.Map<SimpleSource, SimpleTarget>(null!));
    }
}