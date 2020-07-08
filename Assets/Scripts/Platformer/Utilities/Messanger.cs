using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace LSG
{
    public interface IMessage: ICollection
    {
        void Send(params object[] param);
    }

    public class Messanger<TKey, TMessage> where TMessage : IMessage
    {
        TMessage this[TKey type]
        {
            get
            {
                return Messages[type];
            }
        }

        static Dictionary<TKey, TMessage> Messages => g_container;
   
        static Messanger<TKey, TMessage> Instance
        {
            get
            {
                if (g_instance == null && !g_isDestroyed)
                    g_instance =  new Messanger<TKey, TMessage>();
                return g_instance;
            }
        }

        static Messanger<TKey, TMessage> g_instance;
        static Dictionary<TKey, TMessage> g_container;
        static bool g_isDestroyed = false;

        Messanger()
        {
            g_isDestroyed = false;
            g_container = new Dictionary<TKey, TMessage>();
        }

        public static void Dispose()
        {
            g_isDestroyed = true;

            if (g_container != null)
                g_container.Clear();

            g_container = null;
            g_instance = null;
        }

        public static void Add(TKey key, TMessage item)
        {
            if(! ContainsKey(key) && item != null)
            {
                Messages.Add(key, item);
            }
        }

        public static void Initialize()
        {
            g_isDestroyed = false;
        }

        public static void GetFunction(TKey type, ref TMessage value)
        {
            if (ContainsKey(type))
                value = Instance[type];
        }

        public static void SendMessage(TKey type, params object[] param)
        {
            if(ContainsKey(type) && Instance[type] != null)
            {
                Instance[type].Send(param);
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogWarning(type.ToString() + " not founds");
            }
#endif
        }

        static bool ContainsKey(TKey type) /*this Messanger<TKey, TMessage> intstance*/
        {
            return Messages != null && Messages.ContainsKey(type);
        }
    }
}
