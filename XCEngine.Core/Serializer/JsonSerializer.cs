using Newtonsoft.Json;
using System.Text;

namespace XCEngine.Core
{
    /// <summary>
    /// Json序列化
    /// </summary>
    public class JsonSerializer : ISerializer
    {
        public virtual byte[] Serialize(object obj)
        {
            string data = JsonConvert.SerializeObject(obj, Formatting.None);
            return Encoding.UTF8.GetBytes(data);
        }

        public virtual T Deserialize<T>(byte[] data)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(data));
        }

        public virtual T Deserialize<T>(ReadOnlySpan<byte> data)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(data));
        }

        public virtual object Deserialize(Type type, byte[] data)
        {
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(data), type);
        }

        public virtual object Deserialize(Type type, ReadOnlySpan<byte> data)
        {
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(data), type);
        }
    }
}