using System;
using Cysharp.Threading.Tasks;
using EncryptStringSample;
using StorageSystem;
using UnityEngine;

namespace SavingSystem
{
    public class PlayerPrefsPersistentProgressStorage<TProgress> : IPersistentProgressStorage<TProgress>
        where TProgress : class, new()
    {

        private readonly IJsonSerializer _serializer;
        private readonly PersistentStorageConfig _config;
        public bool IsNew { get; private set; }
        public TProgress Data { get; private set; }

        private string Pp => _config.Pp;
        private bool Encrypt => _config.Encrypt;

        private bool _inProcess;
        private const string SAVE_KEY = "SAVE_KEY";

        public PlayerPrefsPersistentProgressStorage(IJsonSerializer serializer, PersistentStorageConfig config)
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

        public UniTask ReadSave()
        {
            TProgress data;

            if (PlayerPrefs.HasKey(SAVE_KEY))
            {
                var str = PlayerPrefs.GetString(SAVE_KEY);
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
            return UniTask.CompletedTask;
        }

        private TProgress Deserialize(string str)
        {
            return _serializer.Deserialize<TProgress>(str);
            //return JsonUtility.FromJson<TProgress>(str);
        }

        private string Serialize() =>
            Encrypt
                ? StringCipher.Encrypt(_serializer.Serialize(Data), Pp)
                : _serializer.Serialize(Data, true);

        private UniTask WriteTextAsync(string value)
        {
            PlayerPrefs.SetString(SAVE_KEY, value);
            return UniTask.CompletedTask;
        }
        
        private void WriteText(string value)
        {
            PlayerPrefs.SetString(SAVE_KEY, value);
        }
    }
}