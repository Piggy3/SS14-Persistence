using System.Diagnostics.CodeAnalysis;
using Content.Shared._Persistence14.RandomTable.State;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared._Persistence14.RandomTable;

public sealed partial class RandomTableSystem : EntitySystem
{
    [Dependency] private IRobustRandom _random = default!;
    [Dependency] private IPrototypeManager _protoMan = default!;
    [Dependency] private IEntityManager _entMan = default!;

    public override void Initialize()
    {

    }

    # region Run
    /// <summary>
    /// Runs the random table, fetching all rolled and valid items from the table in the provided prototype.
    /// </summary>
    public IEnumerable<RandomTableValue> Run(ProtoId<RandomTablePrototype> tableProtoId, RandomTableContext? ctx = null, RandomTableStateComponent? state = null) => Run(_protoMan.Index(tableProtoId).Table, ctx, state);

    public IEnumerable<RandomTableValue> Run(RandomTableSelector table, RandomTableContext? ctx = null, RandomTableStateComponent? state = null)
    {
        ctx ??= AssembleContext(state);

        foreach (var item in table.Run(ctx))
            yield return item;
    }

    /// <summary>
    /// Runs the random table, fetching all rolled and valid integer items (including converted floats) from the table in the provided prototype.
    /// </summary>
    public IEnumerable<int> RunInt(ProtoId<RandomTablePrototype> tableProtoId, RandomTableContext? ctx = null, RandomTableStateComponent? state = null) => RunInt(_protoMan.Index(tableProtoId).Table, ctx, state);

    /// <summary>
    /// Runs the random table, fetching all rolled and valid integer items (including converted floats) from the table in the provided prototype.
    /// </summary>
    public IEnumerable<int> RunInt(RandomTableSelector table, RandomTableContext? ctx = null, RandomTableStateComponent? state = null)
    {
        foreach (var item in Run(table, ctx, state))
            if (item.TryGetInt(out var val))
                yield return val.Value;
    }

    /// <summary>
    /// Runs the random table, fetching all rolled and valid float items (including converted integers) from the table in the provided prototype.
    /// </summary>
    public IEnumerable<float> RunFloat(ProtoId<RandomTablePrototype> tableProtoId, RandomTableContext? ctx = null, RandomTableStateComponent? state = null) => RunFloat(_protoMan.Index(tableProtoId).Table, ctx, state);

    /// <summary>
    /// Runs the random table, fetching all rolled and valid float items (including converted integers) from the table in the provided prototype.
    /// </summary>
    public IEnumerable<float> RunFloat(RandomTableSelector table, RandomTableContext? ctx = null, RandomTableStateComponent? state = null)
    {
        foreach (var item in Run(table, ctx, state))
            if (item.TryGetFloat(out var val))
                yield return val.Value;
    }

    /// <summary>
    /// Runs the random table, fetching all rolled and valid integer string from the table in the provided prototype.
    /// </summary>
    public IEnumerable<string> RunString(ProtoId<RandomTablePrototype> tableProtoId, RandomTableContext? ctx = null, RandomTableStateComponent? state = null) => RunString(_protoMan.Index(tableProtoId).Table, ctx, state);

    /// <summary>
    /// Runs the random table, fetching all rolled and valid integer string from the table in the provided prototype.
    /// </summary>
    public IEnumerable<string> RunString(RandomTableSelector table, RandomTableContext? ctx = null, RandomTableStateComponent? state = null)
    {
        foreach (var item in Run(table, ctx, state))
            if (item.TryGetString(out var val))
                yield return val;
    }

    /// <summary>
    /// Runs the random table, fetching all rolled and valid prototype items from the table in the provided prototype.
    /// </summary>
    public IEnumerable<T> RunPrototype<T>(ProtoId<RandomTablePrototype> tableProtoId, RandomTableContext? ctx = null, RandomTableStateComponent? state = null) where T : class, IPrototype => RunPrototype<T>(_protoMan.Index(tableProtoId).Table, ctx, state);

    /// <summary>
    /// Runs the random table, fetching all rolled and valid prototype items from the table in the provided prototype.
    /// </summary>
    public IEnumerable<T> RunPrototype<T>(RandomTableSelector table, RandomTableContext? ctx = null, RandomTableStateComponent? state = null) where T : class, IPrototype
    {
        foreach (var item in Run(table, ctx, state))
            if (item.TryGetPrototype<T>(_protoMan, out var proto))
                yield return proto;
    }
    # endregion

    # region List
    public IEnumerable<(RandomTableValue value, float prob)> List(ProtoId<RandomTablePrototype> tableProtoId, RandomTableContext? ctx = null, RandomTableStateComponent? state = null) => List(_protoMan.Index(tableProtoId).Table, ctx, state);
    public IEnumerable<(RandomTableValue value, float prob)> List(RandomTableSelector table, RandomTableContext? ctx = null, RandomTableStateComponent? state = null)
    {
        ctx ??= AssembleContext(state);
        foreach (var (value, prob) in table.List(ctx))
            yield return (value, prob);
    }

    public IEnumerable<(int value, float prob)> ListInt(ProtoId<RandomTablePrototype> tableProtoId, RandomTableContext? ctx = null, RandomTableStateComponent? state = null) => ListInt(_protoMan.Index(tableProtoId).Table, ctx, state);
    public IEnumerable<(int value, float prob)> ListInt(RandomTableSelector table, RandomTableContext? ctx = null, RandomTableStateComponent? state = null)
    {
        foreach (var (value, prob) in List(table, ctx, state))
            if (value.TryGetInt(out var val))
                yield return (val.Value, prob);
    }

    public IEnumerable<(float value, float prob)> ListFloat(ProtoId<RandomTablePrototype> tableProtoId, RandomTableContext? ctx = null, RandomTableStateComponent? state = null) => ListFloat(_protoMan.Index(tableProtoId).Table, ctx, state);
    public IEnumerable<(float value, float prob)> ListFloat(RandomTableSelector table, RandomTableContext? ctx = null, RandomTableStateComponent? state = null)
    {
        foreach (var (value, prob) in List(table, ctx, state))
            if (value.TryGetFloat(out var val))
                yield return (val.Value, prob);
    }

    public IEnumerable<(string value, float prob)> ListString(ProtoId<RandomTablePrototype> tableProtoId, RandomTableContext? ctx = null, RandomTableStateComponent? state = null) => ListString(_protoMan.Index(tableProtoId).Table, ctx, state);
    public IEnumerable<(string value, float prob)> ListString(RandomTableSelector table, RandomTableContext? ctx = null, RandomTableStateComponent? state = null)
    {
        foreach (var (value, prob) in List(table, ctx, state))
            if (value.TryGetString(out var val))
                yield return (val, prob);
    }

    public IEnumerable<(T prototype, float prob)> ListPrototype<T>(ProtoId<RandomTablePrototype> tableProtoId, RandomTableContext ctx, RandomTableStateComponent? state = null) where T : class, IPrototype => ListPrototype<T>(_protoMan.Index(tableProtoId).Table, ctx, state);
    public IEnumerable<(T prototype, float prob)> ListPrototype<T>(RandomTableSelector table, RandomTableContext? ctx = null, RandomTableStateComponent? state = null) where T : class, IPrototype
    {
        foreach (var (value, prob) in List(table, ctx, state))
            if (value.TryGetPrototype<T>(_protoMan, out var proto))
                yield return (proto, prob);

    }
    #endregion

    /// <summary>
    /// Retrieves the state component from an entity
    /// </summary>
    /// <param name="uid"></param>
    /// <returns></returns>
    public RandomTableStateComponent? GetTableState(EntityUid uid)
    {
        if (TryComp<RandomTableStateComponent>(uid, out var comp)) return comp;
        return null;
    }

    /// <summary>
    /// Attempts to retrieve the state component from an entity
    /// </summary>
    public bool TryGetTableState(EntityUid uid, [NotNullWhen(true)] out RandomTableStateComponent? comp) => TryComp(uid, out comp);

    /// <summary>
    /// Creates the necessary context for executing a run or list of the table selector.
    /// </summary>
    private RandomTableContext AssembleContext(RandomTableStateComponent? state = null)
    {
        var ctx = new RandomTableContext()
        {
            Random = _random,
            PrototypeManager = _protoMan,
            RandomTableSystem = this,
            EntityManager = _entMan,
            State = state,
        };

        return ctx;
    }
}