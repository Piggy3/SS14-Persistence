using System.Diagnostics.CodeAnalysis;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared._Persistence14.RandomTable;

/// <summary>
/// A data structure for providing values to a RandomTableSelector
/// </summary>
[DataDefinition]
public sealed partial class RandomTableValue : RandomTableSelector
{
    public static readonly string[] ValidDataTypes = { "string", "int", "float", "prototype" };

    [DataField("string")]
    private string? _string = null;

    [DataField("int")]
    private int? _int = null;

    [DataField("float")]
    private float? _float = null;

    [DataField("prototype")]
    private string? _protoid = null;


    public bool TryGetString([NotNullWhen(true)] out string? value)
    {
        value = _string;
        return _string is not null;
    }

    public bool TryGetInt([NotNullWhen(true)] out int? value)
    {
        value = _int ?? ((int?)_float);
        return _int is not null || _float is not null;
    }

    public bool TryGetFloat([NotNullWhen(true)] out float? value)
    {
        value = ((float?)_int) ?? _float;
        return _int is not null || _float is not null;
    }

    public bool TryGetPrototype<T>(IPrototypeManager protoMan, [NotNullWhen(true)] out T? value) where T : class, IPrototype
    {
        value = null;
        if (_protoid is not null && protoMan.TryIndex<T>(_protoid, out value))
            return value is not null;

        return false;
    }

    public static implicit operator RandomTableValue(int value) => new RandomTableValue() { _int = value };
    public static implicit operator RandomTableValue(string value) => new RandomTableValue() { _string = value };
    public static implicit operator RandomTableValue(float value) => new RandomTableValue() { _float = value };

    protected override IEnumerable<RandomTableValue> RunImplementation(RandomTableContext ctx)
    {
        yield return this;
    }

    public override string ToString()
    {
        if (_string is not null)
            return _string;

        if (_int is not null)
            return $"{_int}";

        if (_float is not null)
            return $"{_float}";

        if (_protoid is not null)
            return _protoid;

        return "[No Valid Data]";
    }

    public override IEnumerable<(RandomTableValue value, float prob)> List(RandomTableContext ctx, float probabilityMultipler)
    {
        yield return (this, probabilityMultipler);
    }
}