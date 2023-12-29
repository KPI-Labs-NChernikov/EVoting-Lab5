using Algorithms.Abstractions;
using Algorithms.Common;
using System.Buffers.Binary;

namespace Modelling.CustomTransformers;
public sealed class ModellingTransformer : IObjectToByteArrayTransformer
{
    public bool CanTransform(Type type)
    {
        return type == typeof(int);
    }

    private readonly GuidTransformer _guidTransformer = new ();

    public T? ReverseTransform<T>(byte[] data)
    {
        var span = data.AsSpan();
        if (typeof(T) == typeof(int))
        {
            if (span.Length < sizeof(int))
            {
                var necessaryTrailingZerosCount = sizeof(int) - span.Length;
                var preparedData = new byte[sizeof(int)];
                Buffer.BlockCopy(data, 0, preparedData, necessaryTrailingZerosCount, data.Length);
                data = preparedData;
            }
            return (T)(object)BinaryPrimitives.ReadInt32BigEndian(data);
        }

        throw new NotSupportedException($"The type {typeof(T)} is not supported.");
    }

    public byte[] Transform(object obj)
    {
        if (obj.GetType() == typeof(int))
        {
            var data = new byte[sizeof(int)];
            BinaryPrimitives.WriteInt32BigEndian(data, (int)obj);
            return data;
        }

        throw new NotSupportedException($"The type {obj.GetType()} is not supported.");
    }
}
