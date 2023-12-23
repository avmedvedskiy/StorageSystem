using Cysharp.Threading.Tasks;

namespace SavingSystem
{
    public interface IPersistentProgressStorage<T> where T : class, IPersistentProgress, new()
    {
        bool IsNew { get; }
        T Data { get; }
        UniTask WriteSave();
        void WriteSaveImmediately();
        UniTask ReadSave();
    }
}