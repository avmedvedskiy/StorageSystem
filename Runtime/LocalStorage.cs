using UnityEngine;
using System.IO;
using System.Text;
using System;
using System.Threading.Tasks;
using EncryptStringSample;

namespace SavingSystem
{
    public interface ILocalStorage<out T> where T : class, IPersistentProgress, new()
    {
        T UserData { get; }
        void WriteSave();
        void WriteSaveImmediately();
        T ReadSave(out bool isNew);
    }

    public class LocalStorage<T> : ILocalStorage<T> where T : class, IPersistentProgress, new()
    {
        public T UserData { get; private set; }

        private readonly string _pp;
        private readonly string _fileName;
        private readonly bool _encrypt;
        private readonly string _path;

        private string FilePath => _path + "/" + _fileName;

        public LocalStorage(string path, string pp, string fileName, bool encrypt)
        {
            _path = path;
            _pp = pp;
            _fileName = fileName;
            _encrypt = encrypt;
        }

        public void WriteSave()
        {
            if (UserData == null)
                return;
            UserData.BeforeSerialize();
            Task.Run(WriteOnDisk);
        }

        public void WriteSaveImmediately()
        {
            if (UserData == null)
                return;
            UserData.BeforeSerialize();
            WriteOnDisk();
        }


        public T ReadSave(out bool isNew)
        {
            T data;

            if (File.Exists(FilePath))
            {
                string str = Encoding.UTF8.GetString(File.ReadAllBytes(FilePath));

                try
                {
                    if (_encrypt)
                        str = StringCipher.Decrypt(str, _pp);

                    data = JsonUtility.FromJson<T>(str);
                    data.AfterDeserialize();
                    isNew = false;
                }
                catch (Exception e)
                {
                    Debug.LogException(new Exception("Cant decrypt saves! Create New", e));
                    data = new T();
                    data.AfterDeserialize();
                    isNew = true;
                }
            }
            else
            {
                data = new T();
                data.AfterDeserialize();
                isNew = true;
            }

            UserData = data;
            return data;
        }

        private void WriteOnDisk()
        {
            var json = _encrypt
                ? StringCipher.Encrypt(JsonUtility.ToJson(UserData), _pp)
                : JsonUtility.ToJson(UserData, true);
            WriteOnDisk(Encoding.UTF8.GetBytes(json));
        }

        private void WriteOnDisk(byte[] bytes)
        {
            if (bytes.Length < 1) return;

            using FileStream fileStream = File.Create(FilePath);
            fileStream.Write(bytes, 0, bytes.Length);
            fileStream.Flush();
        }

        //#if UNITY_EDITOR
        //    [UnityEditor.MenuItem("Tools/Save/EncryptSave")]
        //    public static void EncryptSaveCopy()
        //    {
        //        var settings = ResourceManager.GetGeneralSettings();
        //        bool value = settings.DontEncryptLocalSaves;
        //        settings.DontEncryptLocalSaves = true;

        //        LocalStorage storage = new LocalStorage();
        //        var save = storage.ReadLocalSaves();
        //        var data = EncryptStringSample.StringCipher.EncryptToBytes(JsonUtility.ToJson(save,true), Constants.PP);
        //        storage.WriteLocalSaves(data, StorageController.SAVE_FILENAME_COPY);
        //        storage = null;

        //        settings.DontEncryptLocalSaves = value;
        //    }

        //    [UnityEditor.MenuItem("Tools/Save/DecryptSave")]
        //    public static void DecryptSaveCopy()
        //    {
        //        var settings = ResourceManager.GetGeneralSettings();
        //        bool value = settings.DontEncryptLocalSaves;
        //        settings.DontEncryptLocalSaves = false;

        //        LocalStorage storage = new LocalStorage();
        //        var save = storage.ReadLocalSaves();
        //        storage.WriteLocalSaves(Encoding.UTF8.GetBytes(save.ToString()), StorageController.SAVE_FILENAME_COPY);
        //        storage = null;

        //        settings.DontEncryptLocalSaves = value;
        //    }
        //#endif
    }
}