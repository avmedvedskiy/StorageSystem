namespace SavingSystem
{ 
    public interface IPersistentProgress
    {
        void BeforeSerialize();
        void AfterDeserialize();
    }
}
