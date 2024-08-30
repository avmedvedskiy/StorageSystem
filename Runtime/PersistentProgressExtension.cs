using Cysharp.Threading.Tasks;
using SavingSystem;

namespace StorageSystem
{
    public static class PersistentProgressExtension
    {
        public static async UniTask WriteOnThreadPool(this IPersistentProgressStorage storage)
        {
            await UniTask.RunOnThreadPool(storage.WriteSave);
        }
        
        public static async UniTask ReadOnThreadPool(this IPersistentProgressStorage storage)
        {
            await UniTask.RunOnThreadPool(storage.ReadSave);
        }
    }
}