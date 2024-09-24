using Cysharp.Threading.Tasks;
using SavingSystem;

namespace StorageSystem
{
    public static class PersistentProgressExtension
    {
        public static async UniTask WriteOnThreadPool(this IPersistentProgressStorage storage)
        {
#if UNITY_WEBGL
            await storage.WriteSave();
#else
            await UniTask.RunOnThreadPool(storage.WriteSave);
#endif
        }
        
        public static async UniTask ReadOnThreadPool(this IPersistentProgressStorage storage)
        {
#if UNITY_WEBGL
            await storage.ReadSave();
#else
            await UniTask.RunOnThreadPool(storage.ReadSave);
#endif
        }
    }
}