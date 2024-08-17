using System.Buffers;

namespace XCEngine.Core
{
    /// <summary>
    /// 序列化接口
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        byte[] Serialize<T>(T obj);
        byte[] Serialize<T1, T2>(T1 obj1, T2 obj2);
        byte[] Serialize<T1, T2, T3>(T1 obj1, T2 obj2, T3 obj3);
        byte[] Serialize<T1, T2, T3, T4>(T1 obj1, T2 obj2, T3 obj3, T4 obj4);
        byte[] Serialize<T1, T2, T3, T4, T5>(T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5);
        void Serialize<T>(IBufferWriter<byte> bw, T obj);
        void Serialize<T1, T2>(IBufferWriter<byte> bw, T1 obj1, T2 obj2);
        void Serialize<T1, T2, T3>(IBufferWriter<byte> bw, T1 obj1, T2 obj2, T3 obj3);
        void Serialize<T1, T2, T3, T4>(IBufferWriter<byte> bw, T1 obj1, T2 obj2, T3 obj3, T4 obj4);
        void Serialize<T1, T2, T3, T4, T5>(IBufferWriter<byte> bw, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5);

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        T Deserialize<T>(byte[] data);
        (T1, T2) Deserialize<T1, T2>(byte[] data);
        (T1, T2, T3) Deserialize<T1, T2, T3>(byte[] data);
        (T1, T2, T3, T4) Deserialize<T1, T2, T3, T4>(byte[] data);
        (T1, T2, T3, T4, T5) Deserialize<T1, T2, T3, T4, T5>(byte[] data);
        T Deserialize<T>(ReadOnlySpan<byte> data);
        (T1, T2) Deserialize<T1, T2>(ReadOnlySpan<byte> data);
        (T1, T2, T3) Deserialize<T1, T2, T3>(ReadOnlySpan<byte> data);
        (T1, T2, T3, T4) Deserialize<T1, T2, T3, T4>(ReadOnlySpan<byte> data);
        (T1, T2, T3, T4, T5) Deserialize<T1, T2, T3, T4, T5>(ReadOnlySpan<byte> data);

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        object Deserialize(Type type, byte[] data);
        (object, object) Deserialize(Type type1, Type type2, byte[] data);
        (object, object, object) Deserialize(Type type1, Type type2, Type type3, byte[] data);
        (object, object, object, object) Deserialize(Type type1, Type type2, Type type3, Type type4, byte[] data);
        (object, object, object, object, object) Deserialize(Type type1, Type type2, Type type3, Type type4, Type type5, byte[] data);
        object Deserialize(Type type, ReadOnlySpan<byte> data);
        (object, object) Deserialize(Type type1, Type type2, ReadOnlySpan<byte> data);
        (object, object, object) Deserialize(Type type1, Type type2, Type type3, ReadOnlySpan<byte> data);
        (object, object, object, object) Deserialize(Type type1, Type type2, Type type3, Type type4, ReadOnlySpan<byte> data);
        (object, object, object, object, object) Deserialize(Type type1, Type type2, Type type3, Type type4, Type type5, ReadOnlySpan<byte> data);
    }
}
