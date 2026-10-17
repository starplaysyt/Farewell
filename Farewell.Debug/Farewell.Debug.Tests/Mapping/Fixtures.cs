using Farewell.Abstractions.Attributes.Mapping;
using Farewell.Abstractions.Domain;

namespace Farewell.Debug.Tests.Mapping;

public class SimpleSource
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Score { get; set; }
}

public class SimpleTarget
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Score { get; set; }
}

public class TargetWithRequired
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public double Score { get; set; }
}

public class SourceMissingRequired
{
    public double Score { get; set; }
}

public class SourceWithId
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class TargetWithNullableProps
{
    public int Id { get; set; }
    public string? Description { get; set; }
    public int? OptionalCount { get; set; }
}

public class SourceWithoutNullableFields
{
    public int Id { get; set; }
}

public class TargetWithNullableField
{
    public int Id { get; set; }
    public NullableField<string>? OptionalName { get; set; }
    public NullableField<int>? OptionalAge { get; set; }
}

public class TargetWithNonNullableNullableField
{
    public int Id { get; set; }
    public NullableField<string> RequiredField { get; set; }
}

public class SourceForNullableField
{
    public int Id { get; set; }
    public string OptionalName { get; set; } = string.Empty;
    public int OptionalAge { get; set; }
}

public class SourcePartialForNullableField
{
    public int Id { get; set; }
}

public class SourceWithStringId
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class TargetWithIntId
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class SourceWithInt
{
    public int Value { get; set; }
}

public class TargetWithLong
{
    public long Value { get; set; }
}

public class TargetWithExtraProps
{
    public int Id { get; set; }
    public int ExtraNumber { get; set; }
    public bool ExtraFlag { get; set; }
    public string ExtraText { get; set; } = "original";
}

public class MinimalSource
{
    public int Id { get; set; }
}

public record SimpleRecord(int Id, string Name, double Score);

public record RecordWithNullable(int Id, string? Description, int? OptionalCount);

public record RecordWithNullableField(int Id, NullableField<string>? OptionalName);

public record struct SimpleRecordStruct(int Id, string Name);

[method: MapToConstructor]
public class ClassWithMappedConstructor(int id, string name)
{
    public int Id { get; } = id;
    public string Name { get; } = name;

    public ClassWithMappedConstructor() : this(0, string.Empty)
    {
    }
}

[method: MapToConstructor]
public class ClassWithMultipleCtors(int id, string name, double score)
{
    public int Id { get; } = id;
    public string Name { get; } = name;
    public double Score { get; } = score;

    public ClassWithMultipleCtors() : this(0, string.Empty, 0)
    {
    }

    public ClassWithMultipleCtors(int Id) : this(Id, string.Empty, 0)
    {
    }
}

public record StrictRecord(int Id, string Name, double Score);

public class SourceMissingForRecord
{
    public int Id { get; set; }
}

public record FlexibleRecord(int Id, string? Name, int? Count);

public class SourceOnlyId
{
    public int Id { get; set; }
}

public class BaseSource
{
    public int Id { get; set; }
}

public class DerivedSource : BaseSource
{
    public string Name { get; set; } = string.Empty;
}

public class BaseTarget
{
    public int Id { get; set; }
}

public class DerivedTarget : BaseTarget
{
    public string Name { get; set; } = string.Empty;
}

public class NoParameterlessCtor
{
    public int Id { get; set; }
    public NoParameterlessCtor(int id) { Id = id; }
}

public class SourceWithList
{
    public int Id { get; set; }
    public List<int> Numbers { get; set; } = new();
}

public class TargetWithList
{
    public int Id { get; set; }
    public List<int> Numbers { get; set; } = new();
}

public class EmptySource { }
public class EmptyTarget { }

public class SourceWithNullableInt
{
    public int Id { get; set; }
    public int? MaybeValue { get; set; }
}

public class TargetWithNullableFieldNullableInner
{
    public int Id { get; set; }
    public NullableField<int?>? MaybeValue { get; set; }
}

public record struct RecordStructWithNullableField(int Id, NullableField<string>? Tag);

public class SourceForRecordStruct
{
    public int Id { get; set; }
}

public class FullSourceForRecordStruct
{
    public int Id { get; set; }
    public string Tag { get; set; } = string.Empty;
}