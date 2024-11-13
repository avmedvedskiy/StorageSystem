using UnityEngine;

namespace SavingSystem
{
    public class JsonUtilitySerializer : IJsonSerializer
    {
        public string Serialize<T>(T data, bool pretty = default) => JsonUtility.ToJson(data, pretty);

        public T Deserialize<T>(string data) => JsonUtility.FromJson<T>(data);
    }
}