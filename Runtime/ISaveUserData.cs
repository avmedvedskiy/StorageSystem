using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SavingSystem
{ 
    public interface ISaveUserData
    {
        void BeforeSerialize();
        void AfterDeserialize();
    }
}
