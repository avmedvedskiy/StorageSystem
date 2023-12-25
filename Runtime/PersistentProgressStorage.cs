using UnityEngine;
using System.IO;
using System.Text;
using System;
using Cysharp.Threading.Tasks;
using EncryptStringSample;

namespace SavingSystem
{
    public class PersistentProgressStorage<T> : IPersistentProgressStorage<T> where T : class, IPersistentProgress, new()
    {
        public bool IsNew { get; private set; }
        public T Data { get; private set; }

        private readonly string _pp;
        private readonly string _fileName;
        private readonly bool _encrypt;
        private readonly string _path;
        private bool _inProcess;

        private string FilePath => $"{_path}/{_fileName}";

        public PersistentProgressStorage(string path, string pp, string fileName, bool encrypt)
        {
            _path = path;
            _pp = pp;
            _fileName = fileName;
            _encrypt = encrypt;
        }

        public async UniTask WriteSave()
        {
            if (Data == null || _inProcess)
                return;
            Data.BeforeSerialize();
            _inProcess = true;
            await WriteTextAsync(SerializeData());
            _inProcess = false;
        }

        public void WriteSaveImmediately()
        {
            if (Data == null || _inProcess)
                return;
            Data.BeforeSerialize();
            WriteText(SerializeData());
        }

        public async UniTask ReadSave()
        {
            T data;

            if (File.Exists(FilePath))
            {
                var str = Encoding.UTF8.GetString(await File.ReadAllBytesAsync(FilePath));
                try
                {
                    if (_encrypt)
                        str = StringCipher.Decrypt(str, _pp);

                    data = JsonUtility.FromJson<T>(str);
                    data.AfterDeserialize();
                    IsNew = false;
                }
                catch (Exception e)
                {
                    Debug.LogException(new Exception("Cant decrypt saves! Create New", e));
                    data = new T();
                    data.AfterDeserialize();
                    IsNew = true;
                }
            }
            else
            {
                data = new T();
                data.AfterDeserialize();
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
            await File.WriteAllTextAsync(FilePath, value);
        }
        
        private void WriteText(string value)
        {
            File.WriteAllText(FilePath, value);
        }
    }
}