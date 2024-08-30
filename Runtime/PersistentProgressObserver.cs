using SavingSystem;
using UnityEngine;

namespace StorageSystem
{
    public class PersistentProgressObserver : MonoBehaviour
    {
        private IPersistentProgressStorage _storage;

        private void Initialize(IPersistentProgressStorage storage)
        {
            _storage = storage;
        }

        void OnApplicationPause(bool pause)
        {
            if(pause && _storage != null)
                _storage.WriteSaveImmediately();
        }

        public static PersistentProgressObserver Create(IPersistentProgressStorage storage)
        {
            var observer = new GameObject("PersistentProgressObserver").AddComponent<PersistentProgressObserver>();
            observer.Initialize(storage);
            DontDestroyOnLoad(observer.gameObject);
            return observer;
        }
    }
}