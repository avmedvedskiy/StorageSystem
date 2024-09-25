using Cysharp.Threading.Tasks;

namespace SavingSystem
{
    public interface IPersistentProgressStorage
    {
        UniTask WriteSave();
        void WriteSaveImmediately();
        UniTask ReadSave();
        
    }
    
    public interface IPersistentProgressStorage<out T> : IPersistentProgressStorage where T : class, new()
    {
        bool IsNew { get; }
        T Data { get; }
    }
}