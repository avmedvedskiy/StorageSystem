using UnityEngine;
using System.IO;
using System.Text;
using System;
using System.Threading;

namespace SavingSystem
{
    public class LocalStorage<T> where T: class,ISaveUserData, new()
    {
        /// <summary>
        /// User Storage
        /// </summary>
        public T UserData
        {
            get
            {
                if (_userData == null)
                    _userData = ReadLocalSaves();
                return _userData;
            }
            private set
            {
                _userData = value;
            }
        }

        private const string SAVE_FILENAME = "data.json";
        private const string PP = "rhhf3zzvlis8pmyc";
        private const bool DONT_ENCRYPT_LOCAL_SAVES = true;
        private string _path => Application.persistentDataPath;
        private T _userData;

        protected virtual void OnBeforeSerialize()
        {

        }

        protected virtual void OnAfterDeserialize()
        {

        }

        public void SaveLocalSaves()
        {
            OnBeforeSerialize();
            UserData.BeforeSerialize();
            Thread thread = new Thread(WriteLocalSaves);
            thread.Start();
        }

        public void SaveLocalSavesImmediately()
        {
            OnBeforeSerialize();
            UserData.BeforeSerialize();
            WriteLocalSaves();
        }

        private T ReadLocalSaves()
        {
            T data = null;

            if (File.Exists(_path + "/" + SAVE_FILENAME))
            {
                string str = Encoding.UTF8.GetString(File.ReadAllBytes(_path + "/" + SAVE_FILENAME));

#if UNITY_EDITOR
                if (DONT_ENCRYPT_LOCAL_SAVES)
                {
                    try
                    {
                        data = JsonUtility.FromJson<T>(str);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(new Exception("Cant parce json", e));
                    }

                    if (data != null)
                    {
                        data.AfterDeserialize();
                        OnAfterDeserialize();
                        return data;
                    }
                }
#endif
                try
                {
                    str = EncryptStringSample.StringCipher.Decrypt(str, PP);
                    data = JsonUtility.FromJson<T>(str);
                    data.AfterDeserialize();
                    OnAfterDeserialize();
                }
                catch (Exception e)
                {
                    Debug.LogException(new Exception("Cant decrypt saves! Create New", e));
                    data = new T();
                    data.AfterDeserialize();
                    OnAfterDeserialize();
                }
            }
            else
            {
                data = new T();
                data.AfterDeserialize();
                OnAfterDeserialize();
            }
            
            return data;
        }
        private void WriteLocalSaves()
        {
#if UNITY_EDITOR
            if (DONT_ENCRYPT_LOCAL_SAVES)
            {
                WriteLocalSaves(Encoding.UTF8.GetBytes(JsonUtility.ToJson(UserData, true)));
                return;
            }
#endif
#pragma warning disable CS0162 // Обнаружен недостижимый код

            var json = EncryptStringSample.StringCipher.Encrypt(JsonUtility.ToJson(UserData), PP);
            WriteLocalSaves(Encoding.UTF8.GetBytes(json));

#pragma warning restore CS0162 // Обнаружен недостижимый код
        }

        private void WriteLocalSaves(byte[] bytes)
        {
            WriteLocalSaves(bytes, SAVE_FILENAME);
        }

        private void WriteLocalSaves(byte[] bytes, string fileName)
        {
            if (bytes.Length < 1) return;

            using (FileStream fileStream = File.Create(_path + "/" + fileName))
            {
                fileStream.Write(bytes, 0, bytes.Length);
                fileStream.Flush();
            }
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