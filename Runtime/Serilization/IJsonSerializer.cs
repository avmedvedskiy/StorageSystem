namespace SavingSystem
{
    public interface IJsonSerializer
    {
        public string Serialize<T>(T data, bool pretty = default);
        public T Deserialize<T>(string data);
    }
}