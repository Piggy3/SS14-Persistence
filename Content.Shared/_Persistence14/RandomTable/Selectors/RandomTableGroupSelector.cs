using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared._Persistence14.RandomTable.Selectors;

/// <summary>
/// Selects an item from the children of the selector base
/// </summary>
public sealed partial class RandomTableGroupSelector : RandomTableSelector
{
    [DataField]
    public List<RandomTableSelector> Children = new();

    /// <inheritdoc/>
    protected override IEnumerable<RandomTableValue> RunImplementation(RandomTableContext ctx)
    {
        var totalWeight = SumWeights(ctx, out var activeChildren);
        if (totalWeight <= 0f) // If there are no valid children do nothing.
            yield break;

        var rand = ctx.Random.NextFloat() * totalWeight;
        var acc = 0f;

        foreach (var child in activeChildren)
        {
            acc += child.Weight;
            if (acc > rand)
            {
                foreach (var item in child.Run(ctx))
                    yield return item;

                yield break;
            }
        }
    }

    /// <inheritdoc/>
    public override IEnumerable<(RandomTableValue value, float prob)> List(RandomTableContext ctx, float probabilityMultipler = 1f)
    {
        var totalWeight = SumWeights(ctx, out var activeChildren, useConditions: true);

        foreach (var child in activeChildren)
        {
            var childProbability = child.Weight / totalWeight;
            foreach (var (value, prob) in child.List(ctx, probabilityMultipler * childProbability))
                yield return (value, prob);
        }
    }

    /// <summary>
    /// Sums the weights of any active children. For utility, provides the list of active children as an output variable.
    /// </summary>
    private float SumWeights(RandomTableContext ctx, out List<RandomTableSelector> activeChildren, bool useConditions = false)
    {
        activeChildren = new();
        float sum = 0f;
        foreach (var child in Children)
        {
            if (child.CheckConditions(ctx) || !useConditions) // Ignore inactive children.
            {
                sum += child.Weight;
                activeChildren.Add(child);
            }
        }

        return sum;
    }
}