using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;

namespace SavingSystem
{
    public static class SerializationHelper
    {
        [Serializable]
        public struct JSONSerializedElement
        {
            [SerializeField]
            public string fullName;

            [SerializeField]
            public string json;
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool IsValid()
            {
                return !string.IsNullOrEmpty(fullName) && !string.IsNullOrEmpty(json);
            }
        }

        public static JSONSerializedElement nullElement => new();

        static Type GetTypeFromSerializedString(string typeInfo)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var type = assembly.GetType(typeInfo);
                if (type != null)
                    return type;
            }

            return null;
        }

        private static JSONSerializedElement Serialize<T>(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Can not serialize null element");

            var typeInfo = item.GetType().FullName;
            var data = JsonUtility.ToJson(item);

            return new JSONSerializedElement
            {
                fullName = typeInfo,
                json = data
            };
        }

        private static T Deserialize<T>(JSONSerializedElement item, params object[] constructorArgs) where T : class
        {
            if (item.IsValid() == false)
                throw new ArgumentException($"Can not deserialize {item}, it is invalid");

            var type = GetTypeFromSerializedString(item.fullName);
            if (type == null)
                throw new ArgumentException($"Can not deserialize ({item.fullName}), type is invalid");

            var instance = (T)Activator.CreateInstance(type, constructorArgs);
            JsonUtility.FromJsonOverwrite(item.json, instance);
            return instance;
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
                    Debug.LogError(element.json);
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
                    Debug.LogError(element.json);
                }
            }
        }
    }
}
