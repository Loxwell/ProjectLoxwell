using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using IMessege = LSG.Interface.IMessege;

namespace LSG.Interface
{
    public interface IMessege 
    {
        void Send(params object[] param);
    }

}

namespace LSG.Utilities
{
   
    public static class Messenger<TKey> where TKey : struct
    {
        static Dictionary<int, IMessege> Messeges
        {
            get
            {
                if (g_container == null && !g_isDestroyed)
                    g_container = new Dictionary<int, IMessege>();
                return g_container;
            }
        }

        static Dictionary<int, IMessege> g_container;
        static bool g_isDestroyed = false;

        public static void Dispose()
        {
            g_isDestroyed = true;

            if (g_container != null)
                g_container.Clear();

            g_container = null;
        }

        public static void Add(TKey key, IMessege item)
        {
            if (g_isDestroyed)
                return;

            if (!ContainsKey(key) && item != null)
            {
                Messeges.Add(Enum32ToInt<TKey>(key), item);
            }
        }

        public static void Initialize()
        {
            g_isDestroyed = false;
        }

        public static bool GetFunction(TKey type, ref IMessege messege)
        {
            if (g_isDestroyed)
                return false;

            messege = null;
            Messeges.TryGetValue(Enum32ToInt<TKey>(type), out messege);
            return messege != null;
        }

        public static void SendMessage(TKey type, params object[] param)
        {
            if (g_isDestroyed)
                return;

            LSG.Interface.IMessege messege = null;
            Messeges.TryGetValue(Enum32ToInt<TKey>(type), out messege);
            if (messege != null)
                messege.Send(param);
#if UNITY_EDITOR
            else
            {
                Debug.LogWarning(type.ToString() + " not founds");
            }
#endif
        }

        public static int Enum32ToInt<T>(T value) where T : struct
        {
            try
            {
                Shell<T> s =new Shell<T>();
                s.enumValue = value;
                //unsafe
                //{
                //    int* ptr = &s.intValue;
                //    return *ptr;
                //}
                return s.GetIntValue;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                throw e;
            }
        }

        static T IntToEnum32<T>(int value) where T : struct
        {
            Shell<T> s = new Shell<T>();
            //unsafe
            //{
            //    try
            //    {
            //        int* ptr = &s.intValue;
            //        *ptr = value;
            //    }catch(System.Exception e)
            //    {
            //        Debug.LogError(string.Format("IntToEnum32<{0}> invalid cast", typeof(T).ToString()));
            //        throw e;
            //    }
                
            //}

            return s.GetEnum;// s.enumValue;
        }

        static bool ContainsKey(TKey key)
        {
            return !g_isDestroyed && Messeges.ContainsKey(Enum32ToInt<TKey>(key));
        }

        //[StructLayout(LayoutKind.Explicit, Size = sizeof(int))]
        struct Shell<T> where T : struct
        {
#pragma warning disable
            //[FieldOffset(0)]
            public int intValue;
#pragma warning disable
            //[FieldOffset(0)]
            public T enumValue;

            public int GetIntValue
            {
                get => Convert.ToInt32(enumValue);
            }

            public T GetEnum { get => (T)Convert.ChangeType(intValue, typeof(T)); }
        }
    }
}
