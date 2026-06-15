using System.IO;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Content.Shared._Persistence14.RandomTable;

[TypeSerializer]
public sealed class RandomTableTypeSerializer :
    ITypeReader<RandomTableSelector, MappingDataNode>
{
    public RandomTableSelector Read(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<RandomTableSelector>? instanceProvider = null)
    {
        var type = typeof(RandomTableSelector);

        foreach (var acceptedType in RandomTableValue.ValidDataTypes)
        {
            if (!node.Has(acceptedType))
                continue;

            return (RandomTableSelector)serializationManager.Read(typeof(RandomTableValue), node, context)!;
        }

        throw new InvalidDataException("Custom validation not supported! Please specify the type manually!");
    }

    public ValidationNode Validate(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
    {
        foreach (var type in RandomTableValue.ValidDataTypes)
            if (node.Has(type))
                return serializationManager.ValidateNode<RandomTableValue>(node, context);

        return new ErrorNode(node, "Custom validation not supported! Please specify the type manually!");
    }
}

[TypeSerializer]
public sealed class RandomTableValueSerializer : ITypeReader<RandomTableSelector, ValueDataNode>
{
    public RandomTableSelector Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<RandomTableSelector>? instanceProvider = null)
    {
        if (int.TryParse(node.Value, out var intValue))
            return (RandomTableValue)intValue;

        if (float.TryParse(node.Value, out var floatVal))
            return (RandomTableValue)floatVal;

        return (RandomTableValue)node.Value;
    }

    public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
    {
        return new ValidatedValueNode(node);
    }
}