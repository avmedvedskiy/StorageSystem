using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SavingSystem
{
    public static class SerializationHelper
    {
        [Serializable]
        public struct TypeSerializationInfo
        {
            [SerializeField]
            public string fullName;

            public bool IsValid()
            {
                return !string.IsNullOrEmpty(fullName);
            }
        }

        [Serializable]
        public struct JSONSerializedElement
        {
            [SerializeField]
            public TypeSerializationInfo typeInfo;

            [SerializeField]
            public string JSONnodeData;
        }

        public static JSONSerializedElement nullElement
        {
            get
            {
                return new JSONSerializedElement();
            }
        }

        public static TypeSerializationInfo GetTypeSerializableAsString(Type type)
        {
            return new TypeSerializationInfo
            {
                fullName = type.FullName
            };
        }

        static Type GetTypeFromSerializedString(TypeSerializationInfo typeInfo)
        {
            if (!typeInfo.IsValid())
                return null;

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var type = assembly.GetType(typeInfo.fullName);
                if (type != null)
                    return type;
            }

            return null;
        }

        public static JSONSerializedElement Serialize<T>(T item)
        {
            if (item == null)
                throw new ArgumentNullException("item", "Can not serialize null element");

            var typeInfo = GetTypeSerializableAsString(item.GetType());
            var data = JsonUtility.ToJson(item);

            if (string.IsNullOrEmpty(data))
                throw new ArgumentException(string.Format("Can not serialize {0}", item));
            ;

            return new JSONSerializedElement
            {
                typeInfo = typeInfo,
                JSONnodeData = data
            };
        }

        public static T Deserialize<T>(JSONSerializedElement item, params object[] constructorArgs) where T : class
        {
            if (!item.typeInfo.IsValid() || string.IsNullOrEmpty(item.JSONnodeData))
                throw new ArgumentException(string.Format("Can not deserialize {0}, it is invalid", item));

            TypeSerializationInfo info = item.typeInfo;

            var type = GetTypeFromSerializedString(info);
            if (type == null)
                throw new ArgumentException(string.Format("Can not deserialize ({0}), type is invalid", info.fullName));

            T instance;
            try
            {
                instance = Activator.CreateInstance(type, constructorArgs) as T;
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Could not construct instance of: {0}", type), e);
            }

            if (instance != null)
            {
                JsonUtility.FromJsonOverwrite(item.JSONnodeData, instance);
                return instance;
            }
            return null;
        }

        public static List<JSONSerializedElement> Serialize<T>(IEnumerable<T> list)
        {
            var result = new List<JSONSerializedElement>();
            if (list == null)
                return result;

            foreach (var element in list)
            {
                try
                {
                    result.Add(Serialize(element));
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
            return result;
        }


        public static void SerializeNoAlloc<T>(IEnumerable<T> list, ref List<JSONSerializedElement> result)
        {
            if (list == null)
            {
                result = new List<JSONSerializedElement>();
                return;
            }
            result.Clear();
            foreach (var element in list)
            {
                try
                {
                    result.Add(Serialize(element));
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
            return;
        }



        public static List<T> Deserialize<T>(IEnumerable<JSONSerializedElement> list, params object[] constructorArgs) where T : class
        {
            var result = new List<T>();
            if (list == null)
                return result;

            foreach (var element in list)
            {
                try
                {
                    result.Add(Deserialize<T>(element, constructorArgs));
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    Debug.LogError(element.JSONnodeData);
                }
            }
            return result;
        }

        public static void DeserializeNoAlloc<T>(IEnumerable<JSONSerializedElement> list,ref List<T> result, params object[] constructorArgs) where T : class
        {
            if (list == null)
            {
                result = new List<T>();
                return;
            }

            result.Clear();
            foreach (var element in list)
            {
                try
                {
                    result.Add(Deserialize<T>(element, constructorArgs));
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    Debug.LogError(element.JSONnodeData);
                }
            }
            return;
        }
    }
}
