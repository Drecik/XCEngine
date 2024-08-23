using MemoryPack;
using System.Buffers;

namespace XCEngine.Core
{
    /// <summary>
    /// MeoryPack序列化
    /// </summary>
    public class MemoryPackSerializer : ISerializer
    {
        #region Serialize

        public byte[] Serialize<T>(T obj)
        {
            return MemoryPack.MemoryPackSerializer.Serialize(obj);
        }

        public byte[] Serialize<T1, T2>(T1 obj1, T2 obj2)
        {
            var bufferWriter = new ArrayBufferWriter<byte>();
            MemoryPack.MemoryPackSerializer.Serialize(bufferWriter, obj1);
            MemoryPack.MemoryPackSerializer.Serialize(bufferWriter, obj2);
            return bufferWriter.WrittenSpan.ToArray();
        }

        public byte[] Serialize<T1, T2, T3>(T1 obj1, T2 obj2, T3 obj3)
        {
            var bufferWriter = new ArrayBufferWriter<byte>();
            MemoryPack.MemoryPackSerializer.Serialize(bufferWriter, obj1);
            MemoryPack.MemoryPackSerializer.Serialize(bufferWriter, obj2);
            MemoryPack.MemoryPackSerializer.Serialize(bufferWriter, obj3);
            return bufferWriter.WrittenSpan.ToArray();
        }

        public byte[] Serialize<T1, T2, T3, T4>(T1 obj1, T2 obj2, T3 obj3, T4 obj4)
        {
            var bufferWriter = new ArrayBufferWriter<byte>();
            MemoryPack.MemoryPackSerializer.Serialize(bufferWriter, obj1);
            MemoryPack.MemoryPackSerializer.Serialize(bufferWriter, obj2);
            MemoryPack.MemoryPackSerializer.Serialize(bufferWriter, obj3);
            MemoryPack.MemoryPackSerializer.Serialize(bufferWriter, obj4);
            return bufferWriter.WrittenSpan.ToArray();
        }

        public byte[] Serialize<T1, T2, T3, T4, T5>(T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5)
        {
            var bufferWriter = new ArrayBufferWriter<byte>();
            MemoryPack.MemoryPackSerializer.Serialize(bufferWriter, obj1);
            MemoryPack.MemoryPackSerializer.Serialize(bufferWriter, obj2);
            MemoryPack.MemoryPackSerializer.Serialize(bufferWriter, obj3);
            MemoryPack.MemoryPackSerializer.Serialize(bufferWriter, obj4);
            MemoryPack.MemoryPackSerializer.Serialize(bufferWriter, obj5);
            return bufferWriter.WrittenSpan.ToArray();
        }

        public byte[] Serialize(Type type, object obj)
        {
            return MemoryPack.MemoryPackSerializer.Serialize(type, obj);
        }

        public byte[] Serialize(Type type1, object obj1, Type type2, object obj2)
        {
            var bufferWriter = new ArrayBufferWriter<byte>();
            MemoryPack.MemoryPackSerializer.Serialize(type1, bufferWriter, obj1);
            MemoryPack.MemoryPackSerializer.Serialize(type2, bufferWriter, obj2);
            return bufferWriter.WrittenSpan.ToArray();
        }

        public byte[] Serialize(Type type1, object obj1, Type type2, object obj2, Type type3, object obj3)
        {
            var bufferWriter = new ArrayBufferWriter<byte>();
            MemoryPack.MemoryPackSerializer.Serialize(type1, bufferWriter, obj1);
            MemoryPack.MemoryPackSerializer.Serialize(type2, bufferWriter, obj2);
            MemoryPack.MemoryPackSerializer.Serialize(type3, bufferWriter, obj3);
            return bufferWriter.WrittenSpan.ToArray();
        }

        public byte[] Serialize(Type type1, object obj1, Type type2, object obj2, Type type3, object obj3, Type type4, object obj4)
        {
            var bufferWriter = new ArrayBufferWriter<byte>();
            MemoryPack.MemoryPackSerializer.Serialize(type1, bufferWriter, obj1);
            MemoryPack.MemoryPackSerializer.Serialize(type2, bufferWriter, obj2);
            MemoryPack.MemoryPackSerializer.Serialize(type3, bufferWriter, obj3);
            MemoryPack.MemoryPackSerializer.Serialize(type4, bufferWriter, obj4);
            return bufferWriter.WrittenSpan.ToArray();
        }

        public byte[] Serialize(Type type1, object obj1, Type type2, object obj2, Type type3, object obj3, Type type4, object obj4, Type type5, object obj5)
        {
            var bufferWriter = new ArrayBufferWriter<byte>();
            MemoryPack.MemoryPackSerializer.Serialize(type1, bufferWriter, obj1);
            MemoryPack.MemoryPackSerializer.Serialize(type2, bufferWriter, obj2);
            MemoryPack.MemoryPackSerializer.Serialize(type3, bufferWriter, obj3);
            MemoryPack.MemoryPackSerializer.Serialize(type4, bufferWriter, obj4);
            MemoryPack.MemoryPackSerializer.Serialize(type5, bufferWriter, obj5);
            return bufferWriter.WrittenSpan.ToArray();
        }

        public void Serialize<T>(IBufferWriter<byte> bw, T obj)
        {
            MemoryPack.MemoryPackSerializer.Serialize(bw, obj);
        }

        public void Serialize<T1, T2>(IBufferWriter<byte> bw, T1 obj1, T2 obj2)
        {
            MemoryPack.MemoryPackSerializer.Serialize(bw, obj1);
            MemoryPack.MemoryPackSerializer.Serialize(bw, obj2);
        }

        public void Serialize<T1, T2, T3>(IBufferWriter<byte> bw, T1 obj1, T2 obj2, T3 obj3)
        {
            MemoryPack.MemoryPackSerializer.Serialize(bw, obj1);
            MemoryPack.MemoryPackSerializer.Serialize(bw, obj2);
            MemoryPack.MemoryPackSerializer.Serialize(bw, obj3);
        }

        public void Serialize<T1, T2, T3, T4>(IBufferWriter<byte> bw, T1 obj1, T2 obj2, T3 obj3, T4 obj4)
        {
            MemoryPack.MemoryPackSerializer.Serialize(bw, obj1);
            MemoryPack.MemoryPackSerializer.Serialize(bw, obj2);
            MemoryPack.MemoryPackSerializer.Serialize(bw, obj3);
            MemoryPack.MemoryPackSerializer.Serialize(bw, obj4);
        }

        public void Serialize<T1, T2, T3, T4, T5>(IBufferWriter<byte> bw, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5)
        {
            MemoryPack.MemoryPackSerializer.Serialize(bw, obj1);
            MemoryPack.MemoryPackSerializer.Serialize(bw, obj2);
            MemoryPack.MemoryPackSerializer.Serialize(bw, obj3);
            MemoryPack.MemoryPackSerializer.Serialize(bw, obj4);
            MemoryPack.MemoryPackSerializer.Serialize(bw, obj5);
        }

        public void Serialize(IBufferWriter<byte> bw, Type type, object obj)
        {
            MemoryPack.MemoryPackSerializer.Serialize(type, bw, obj);
        }

        public void Serialize(IBufferWriter<byte> bw, Type type1, object obj1, Type type2, object obj2)
        {
            MemoryPack.MemoryPackSerializer.Serialize(type1, bw, obj1);
            MemoryPack.MemoryPackSerializer.Serialize(type2, bw, obj2);
        }

        public void Serialize(IBufferWriter<byte> bw, Type type1, object obj1, Type type2, object obj2, Type type3, object obj3)
        {
            MemoryPack.MemoryPackSerializer.Serialize(type1, bw, obj1);
            MemoryPack.MemoryPackSerializer.Serialize(type2, bw, obj2);
            MemoryPack.MemoryPackSerializer.Serialize(type3, bw, obj3);
        }

        public void Serialize(IBufferWriter<byte> bw, Type type1, object obj1, Type type2, object obj2, Type type3, object obj3, Type type4, object obj4)
        {
            MemoryPack.MemoryPackSerializer.Serialize(type1, bw, obj1);
            MemoryPack.MemoryPackSerializer.Serialize(type2, bw, obj2);
            MemoryPack.MemoryPackSerializer.Serialize(type3, bw, obj3);
            MemoryPack.MemoryPackSerializer.Serialize(type4, bw, obj4);
        }

        public void Serialize(IBufferWriter<byte> bw, Type type1, object obj1, Type type2, object obj2, Type type3, object obj3, Type type4, object obj4, Type type5, object obj5)
        {
            MemoryPack.MemoryPackSerializer.Serialize(type1, bw, obj1);
            MemoryPack.MemoryPackSerializer.Serialize(type2, bw, obj2);
            MemoryPack.MemoryPackSerializer.Serialize(type3, bw, obj3);
            MemoryPack.MemoryPackSerializer.Serialize(type4, bw, obj4);
            MemoryPack.MemoryPackSerializer.Serialize(type5, bw, obj5);
        }

        #endregion

        #region Deserialize

        public T Deserialize<T>(byte[] data)
        {
            return MemoryPack.MemoryPackSerializer.Deserialize<T>(data);
        }

        public (T1, T2) Deserialize<T1, T2>(byte[] data)
        {
            using (var state = MemoryPackReaderOptionalStatePool.Rent(MemoryPackSerializerOptions.Default))
            {
                var reader = new MemoryPackReader(data, state);
                var ret1 = reader.ReadValue<T1>();
                var ret2 = reader.ReadValue<T2>();
                return (ret1, ret2);
            }
        }

        public (T1, T2, T3) Deserialize<T1, T2, T3>(byte[] data)
        {
            using (var state = MemoryPackReaderOptionalStatePool.Rent(MemoryPackSerializerOptions.Default))
            {
                var reader = new MemoryPackReader(data, state);
                var ret1 = reader.ReadValue<T1>();
                var ret2 = reader.ReadValue<T2>();
                var ret3 = reader.ReadValue<T3>();
                return (ret1, ret2, ret3);
            }
        }

        public (T1, T2, T3, T4) Deserialize<T1, T2, T3, T4>(byte[] data)
        {
            using (var state = MemoryPackReaderOptionalStatePool.Rent(MemoryPackSerializerOptions.Default))
            {
                var reader = new MemoryPackReader(data, state);
                var ret1 = reader.ReadValue<T1>();
                var ret2 = reader.ReadValue<T2>();
                var ret3 = reader.ReadValue<T3>();
                var ret4 = reader.ReadValue<T4>();
                return (ret1, ret2, ret3, ret4);
            }
        }

        public (T1, T2, T3, T4, T5) Deserialize<T1, T2, T3, T4, T5>(byte[] data)
        {
            using (var state = MemoryPackReaderOptionalStatePool.Rent(MemoryPackSerializerOptions.Default))
            {
                var reader = new MemoryPackReader(data, state);
                var ret1 = reader.ReadValue<T1>();
                var ret2 = reader.ReadValue<T2>();
                var ret3 = reader.ReadValue<T3>();
                var ret4 = reader.ReadValue<T4>();
                var ret5 = reader.ReadValue<T5>();
                return (ret1, ret2, ret3, ret4, ret5);
            }
        }

        public T Deserialize<T>(ReadOnlySpan<byte> data)
        {
            return MemoryPack.MemoryPackSerializer.Deserialize<T>(data);
        }

        public (T1, T2) Deserialize<T1, T2>(ReadOnlySpan<byte> data)
        {
            using (var state = MemoryPackReaderOptionalStatePool.Rent(MemoryPackSerializerOptions.Default))
            {
                var reader = new MemoryPackReader(data, state);
                var ret1 = reader.ReadValue<T1>();
                var ret2 = reader.ReadValue<T2>();
                return (ret1, ret2);
            }
        }

        public (T1, T2, T3) Deserialize<T1, T2, T3>(ReadOnlySpan<byte> data)
        {
            using (var state = MemoryPackReaderOptionalStatePool.Rent(MemoryPackSerializerOptions.Default))
            {
                var reader = new MemoryPackReader(data, state);
                var ret1 = reader.ReadValue<T1>();
                var ret2 = reader.ReadValue<T2>();
                var ret3 = reader.ReadValue<T3>();
                return (ret1, ret2, ret3);
            }
        }

        public (T1, T2, T3, T4) Deserialize<T1, T2, T3, T4>(ReadOnlySpan<byte> data)
        {
            using (var state = MemoryPackReaderOptionalStatePool.Rent(MemoryPackSerializerOptions.Default))
            {
                var reader = new MemoryPackReader(data, state);
                var ret1 = reader.ReadValue<T1>();
                var ret2 = reader.ReadValue<T2>();
                var ret3 = reader.ReadValue<T3>();
                var ret4 = reader.ReadValue<T4>();
                return (ret1, ret2, ret3, ret4);
            }
        }

        public (T1, T2, T3, T4, T5) Deserialize<T1, T2, T3, T4, T5>(ReadOnlySpan<byte> data)
        {
            using (var state = MemoryPackReaderOptionalStatePool.Rent(MemoryPackSerializerOptions.Default))
            {
                var reader = new MemoryPackReader(data, state);
                var ret1 = reader.ReadValue<T1>();
                var ret2 = reader.ReadValue<T2>();
                var ret3 = reader.ReadValue<T3>();
                var ret4 = reader.ReadValue<T4>();
                var ret5 = reader.ReadValue<T5>();
                return (ret1, ret2, ret3, ret4, ret5);
            }
        }

        public object Deserialize(Type type, byte[] data)
        {
            return MemoryPack.MemoryPackSerializer.Deserialize(type, data);
        }

        public (object, object) Deserialize(Type type1, Type type2, byte[] data)
        {
            using (var state = MemoryPackReaderOptionalStatePool.Rent(MemoryPackSerializerOptions.Default))
            {
                var reader = new MemoryPackReader(data, state);
                var ret1 = reader.ReadValue(type1);
                var ret2 = reader.ReadValue(type2);
                return (ret1, ret2);
            }
        }

        public (object, object, object) Deserialize(Type type1, Type type2, Type type3, byte[] data)
        {
            using (var state = MemoryPackReaderOptionalStatePool.Rent(MemoryPackSerializerOptions.Default))
            {
                var reader = new MemoryPackReader(data, state);
                var ret1 = reader.ReadValue(type1);
                var ret2 = reader.ReadValue(type2);
                var ret3 = reader.ReadValue(type3);
                return (ret1, ret2, ret3);
            }
        }

        public (object, object, object, object) Deserialize(Type type1, Type type2, Type type3, Type type4, byte[] data)
        {
            using (var state = MemoryPackReaderOptionalStatePool.Rent(MemoryPackSerializerOptions.Default))
            {
                var reader = new MemoryPackReader(data, state);
                var ret1 = reader.ReadValue(type1);
                var ret2 = reader.ReadValue(type2);
                var ret3 = reader.ReadValue(type3);
                var ret4 = reader.ReadValue(type4);
                return (ret1, ret2, ret3, ret4);
            }
        }

        public (object, object, object, object, object) Deserialize(Type type1, Type type2, Type type3, Type type4, Type type5, byte[] data)
        {
            using (var state = MemoryPackReaderOptionalStatePool.Rent(MemoryPackSerializerOptions.Default))
            {
                var reader = new MemoryPackReader(data, state);
                var ret1 = reader.ReadValue(type1);
                var ret2 = reader.ReadValue(type2);
                var ret3 = reader.ReadValue(type3);
                var ret4 = reader.ReadValue(type4);
                var ret5 = reader.ReadValue(type5);
                return (ret1, ret2, ret3, ret4, ret5);
            }
        }

        public object Deserialize(Type type, ReadOnlySpan<byte> data)
        {
            return MemoryPack.MemoryPackSerializer.Deserialize(type, data);
        }

        public (object, object) Deserialize(Type type1, Type type2, ReadOnlySpan<byte> data)
        {
            using (var state = MemoryPackReaderOptionalStatePool.Rent(MemoryPackSerializerOptions.Default))
            {
                var reader = new MemoryPackReader(data, state);
                var ret1 = reader.ReadValue(type1);
                var ret2 = reader.ReadValue(type2);
                return (ret1, ret2);
            }
        }

        public (object, object, object) Deserialize(Type type1, Type type2, Type type3, ReadOnlySpan<byte> data)
        {
            using (var state = MemoryPackReaderOptionalStatePool.Rent(MemoryPackSerializerOptions.Default))
            {
                var reader = new MemoryPackReader(data, state);
                var ret1 = reader.ReadValue(type1);
                var ret2 = reader.ReadValue(type2);
                var ret3 = reader.ReadValue(type3);
                return (ret1, ret2, ret3);
            }
        }

        public (object, object, object, object) Deserialize(Type type1, Type type2, Type type3, Type type4, ReadOnlySpan<byte> data)
        {
            using (var state = MemoryPackReaderOptionalStatePool.Rent(MemoryPackSerializerOptions.Default))
            {
                var reader = new MemoryPackReader(data, state);
                var ret1 = reader.ReadValue(type1);
                var ret2 = reader.ReadValue(type2);
                var ret3 = reader.ReadValue(type3);
                var ret4 = reader.ReadValue(type4);
                return (ret1, ret2, ret3, ret4);
            }
        }

        public (object, object, object, object, object) Deserialize(Type type1, Type type2, Type type3, Type type4, Type type5, ReadOnlySpan<byte> data)
        {
            using (var state = MemoryPackReaderOptionalStatePool.Rent(MemoryPackSerializerOptions.Default))
            {
                var reader = new MemoryPackReader(data, state);
                var ret1 = reader.ReadValue(type1);
                var ret2 = reader.ReadValue(type2);
                var ret3 = reader.ReadValue(type3);
                var ret4 = reader.ReadValue(type4);
                var ret5 = reader.ReadValue(type5);
                return (ret1, ret2, ret3, ret4, ret5);
            }
        }

        #endregion
    }
}
