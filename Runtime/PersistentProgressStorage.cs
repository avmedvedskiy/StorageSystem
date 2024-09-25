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
        public bool IsNew { get; private set; }
        public TProgress Data { get; private set; }

        private readonly string _pp;
        private readonly bool _encrypt;
        private bool _inProcess;

        private readonly string _filePath;

        public PersistentProgressStorage(string path, string pp, string fileName, bool encrypt)
        {
            _pp = pp;
            _encrypt = encrypt;
            _filePath = $"{path}/{fileName}";
            PersistentProgressObserver.Create(this);
        }

        public async UniTask WriteSave()
        {
            if (Data == null || _inProcess)
                return;
            _inProcess = true;
            await WriteTextAsync(SerializeData());
            _inProcess = false;
        }

        public void WriteSaveImmediately()
        {
            if (Data == null || _inProcess)
                return;
            WriteText(SerializeData());
        }

        public async UniTask ReadSave()
        {
            TProgress data;

            if (File.Exists(_filePath))
            {
                var str = Encoding.UTF8.GetString(await File.ReadAllBytesAsync(_filePath));
                try
                {
                    if (_encrypt)
                        str = StringCipher.Decrypt(str, _pp);

                    data = JsonUtility.FromJson<TProgress>(str);
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

        private string SerializeData()
        {
            return _encrypt
                ? StringCipher.Encrypt(JsonUtility.ToJson(Data), _pp)
                : JsonUtility.ToJson(Data, true);
        }

        private async UniTask WriteTextAsync(string value)
        {
            await File.WriteAllTextAsync(_filePath, value);
        }
        
        private void WriteText(string value)
        {
            File.WriteAllText(_filePath, value);
        }
    }
}