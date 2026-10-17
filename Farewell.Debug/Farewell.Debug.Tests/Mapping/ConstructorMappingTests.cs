using Farewell.Abstractions.Mapping;
using Farewell.Mapping;
using Farewell.Mapping.Exceptions;

namespace Farewell.Debug.Tests.Mapping;

public class ConstructorMappingTests
{
    private readonly IMappingProvider _mapper = MappingProvider.Instance;

    [Fact]
    public void Record_AllPropertiesMatch_MapsCorrectly()
    {
        var source = new SimpleSource { Id = 1, Name = "Alice", Score = 9.5 };

        var result = _mapper.Map<SimpleSource, SimpleRecord>(source);

        Assert.Equal(1, result.Id);
        Assert.Equal("Alice", result.Name);
        Assert.Equal(9.5, result.Score);
    }

    [Fact]
    public void Record_NullableParams_SourceMissing_SetsNull()
    {
        var source = new SourceOnlyId { Id = 1 };

        var result = _mapper.Map<SourceOnlyId, FlexibleRecord>(source);

        Assert.Equal(1, result.Id);
        Assert.Null(result.Name);
        Assert.Null(result.Count);
    }

    [Fact]
    public void Record_NonNullableParams_SourceMissing_ThrowsMappingCompilationException()
    {
        var source = new SourceMissingForRecord { Id = 1 };

        var ex = Assert.Throws<MappingCompilationException>(
            () => _mapper.Map<SourceMissingForRecord, StrictRecord>(source));

        Assert.Contains("constructor parameter", ex.Message);
        Assert.Contains("non-nullable", ex.Message);
    }

    [Fact]
    public void Record_WithNullableField_SourceMissing_SetsNull()
    {
        var source = new SourceOnlyId { Id = 1 };

        var result = _mapper.Map<SourceOnlyId, RecordWithNullableField>(source);

        Assert.Equal(1, result.Id);
        Assert.Null(result.OptionalName);
    }

    [Fact]
    public void Record_NullableParams_SourceHasValues_MapsCorrectly()
    {
        var source = new SourceWithoutNullableFields { Id = 5 };

        var result = _mapper.Map<SourceWithoutNullableFields, RecordWithNullable>(source);

        Assert.Equal(5, result.Id);
        Assert.Null(result.Description);
        Assert.Null(result.OptionalCount);
    }

    [Fact]
    public void RecordStruct_AllMatch_MapsCorrectly()
    {
        var source = new SourceWithId { Id = 1, Name = "Struct" };

        var result = _mapper.Map<SourceWithId, SimpleRecordStruct>(source);

        Assert.Equal(1, result.Id);
        Assert.Equal("Struct", result.Name);
    }

    [Fact]
    public void RecordStruct_WithNullableField_SourceMissing_SetsNull()
    {
        var source = new SourceForRecordStruct { Id = 1 };

        var result = _mapper.Map<SourceForRecordStruct, RecordStructWithNullableField>(source);

        Assert.Equal(1, result.Id);
        Assert.Null(result.Tag);
    }

    [Fact]
    public void RecordStruct_WithNullableField_SourceHasValue_MapsCorrectly()
    {
        var source = new FullSourceForRecordStruct { Id = 1, Tag = "hello" };

        var result = _mapper.Map<FullSourceForRecordStruct, RecordStructWithNullableField>(source);

        Assert.Equal(1, result.Id);
        Assert.NotNull(result.Tag);
        Assert.Equal("hello", result.Tag!.Value.Value);
    }

    [Fact]
    public void ClassWithMapToConstructor_UsesMarkedCtor()
    {
        var source = new SimpleSource { Id = 1, Name = "Bob", Score = 5.0 };

        var result = _mapper.Map<SimpleSource, ClassWithMappedConstructor>(source);

        Assert.Equal(1, result.Id);
        Assert.Equal("Bob", result.Name);
    }

    [Fact]
    public void ClassWithMultipleCtors_UsesMarkedCtor()
    {
        var source = new SimpleSource { Id = 1, Name = "Multi", Score = 7.7 };

        var result = _mapper.Map<SimpleSource, ClassWithMultipleCtors>(source);

        Assert.Equal(1, result.Id);
        Assert.Equal("Multi", result.Name);
        Assert.Equal(7.7, result.Score);
    }
}