using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SavingSystem
{ 
    public interface IPersistentProgress
    {
        void BeforeSerialize();
        void AfterDeserialize();
    }
}
