using UnityEngine;
using System.IO;
using System.Text;
using System;
using Cysharp.Threading.Tasks;
using EncryptStringSample;
using StorageSystem;

namespace SavingSystem
{
    public class PersistentProgressStorage<TProgress> : IPersistentProgressStorage<TProgress> where TProgress : class, new()
    {
        private readonly IJsonSerializer _serializer;
        private readonly PersistentStorageConfig _config;
        public bool IsNew { get; private set; }
        public TProgress Data { get; private set; }

        private string Pp => _config.Pp;
        private bool Encrypt => _config.Encrypt;
        private string FilePath => _config.FilePath;

        private bool _inProcess;

        public PersistentProgressStorage(IJsonSerializer serializer, PersistentStorageConfig config)
        {
            _serializer = serializer;
            _config = config;
            PersistentProgressObserver.Create(this);
        }

        public async UniTask WriteSave()
        {
            if (Data == null || _inProcess)
                return;
            _inProcess = true;
            await WriteTextAsync(Serialize());
            _inProcess = false;
        }

        public void WriteSaveImmediately()
        {
            if (Data == null || _inProcess)
                return;
            WriteText(Serialize());
        }

        public async UniTask ReadSave()
        {
            TProgress data;

            if (File.Exists(FilePath))
            {
                var str = Encoding.UTF8.GetString(await File.ReadAllBytesAsync(FilePath));
                try
                {
                    if (Encrypt)
                        str = StringCipher.Decrypt(str, Pp);

                    data = Deserialize(str);
                    IsNew = false;
                }
                catch (Exception e)
                {
                    Debug.LogException(new Exception("Cant decrypt saves! Create New", e));
                    data = new TProgress();
                    IsNew = true;
                }
            }
            else
            {
                data = new TProgress();
                IsNew = true;
            }

            Data = data;
        }

        private TProgress Deserialize(string str)
        {
            return _serializer.Deserialize<TProgress>(str);
            //return JsonUtility.FromJson<TProgress>(str);
        }

        private string Serialize()
        {
            
            return Encrypt
                ? StringCipher.Encrypt(_serializer.Serialize(Data), Pp)
                : _serializer.Serialize(Data, true);
        }

        private async UniTask WriteTextAsync(string value)
        {
            await File.WriteAllTextAsync(FilePath, value);
        }
        
        private void WriteText(string value)
        {
            File.WriteAllText(FilePath, value);
        }
    }
}