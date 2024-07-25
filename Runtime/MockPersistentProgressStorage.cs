using Cysharp.Threading.Tasks;

namespace SavingSystem
{
    public class MockPersistentProgressStorage<TProgress> : IPersistentProgressStorage<TProgress>
        where TProgress : class, IPersistentProgress, new()
    {
        public bool IsNew => true;
        public TProgress Data { get; } = new();
        public UniTask WriteSave()
        {
            return UniTask.CompletedTask;
        }

        public void WriteSaveImmediately()
        {
            
        }

        public UniTask ReadSave()
        {
            return UniTask.FromResult(Data);
        }
    }
}