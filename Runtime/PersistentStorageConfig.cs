namespace SavingSystem
{
    public class PersistentStorageConfig
    {
        public string FilePath { get; }
        public string Pp { get; }
        public bool Encrypt { get; }

        public PersistentStorageConfig(string filePath, string pp, bool encrypt)
        {
            FilePath = filePath;
            Pp = pp;
            Encrypt = encrypt;
        }
    }
}