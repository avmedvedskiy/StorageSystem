using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SavingSystem
{ 
    public interface ISaveDataModel
    {
        void BeforeSerialize();
        void AfterDeserialize();
    }
}
