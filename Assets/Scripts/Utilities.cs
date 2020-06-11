using System;
using UnityEditorInternal;

namespace LSG
{
    public struct Utilities
    {
        public struct BitField
        {
            static void Error<T>()
            {
                throw new System.Exception(string.Format("Invalid type %s to convert to int16", typeof(T)));
            }

            public static void MarkFlag<T>(ref ulong curState, T state) 
            {
                try
                {
                    curState |= 0x01ul << System.Convert.ToInt16(state);
                }finally
                {
                    Error<T>();
                }
            }

            public static void ReleaseFlag<T>(ref ulong curState, T state)
            {
                try { 
                    curState &= ~(0x01ul << System.Convert.ToInt16(state));
                }
                finally
                {
                    Error<T>();
                }
            }
            public static void SetFlag<T>(ref ulong curState, T state)
            {
                try
                {
                    curState = (0x01ul << System.Convert.ToInt16(state));
                }
                finally
                {
                    Error<T>();
                }
            }

            public static bool IsMarkedFlag<T>(ulong curState, T state)
            {
                try
                {
                    return (curState & 0x01ul << System.Convert.ToInt16(state)) != 0;
                }
                finally
                {
                    Error<T>();
                }
            }

            public static void ClearFlags(ref ulong curState)
            {
                curState = 0;
            }

            public static void MarkFlag<T>(ref uint curState, T state) 
            {
                try
                { 
                    curState |= 0x01u << System.Convert.ToInt16(state);
                }
                finally
                {
                    Error<T>();
                }
            }

            public static void ReleaseFlag<T>(ref uint curState, T state) 
            {
                try
                {
                    curState &= ~(0x01u << System.Convert.ToInt16(state));
                }
                finally
                {
                    Error<T>();
                }
            }
            public static void SetFlag<T>(ref uint curState, T state) 
            {
                try { 
                    curState = (0x01u << System.Convert.ToInt16(state));
                }
                finally
                {
                    Error<T>();
                }
            }
            public static bool IsMarkedFlag<T>(uint curState, T state)
            {
                try
                {
                    return (curState & 0x01u << System.Convert.ToInt16(state)) != 0;
                }
                finally
                {
                    Error<T>();
                }
            }

            public static void ClearFlags(ref uint curState)
            {
                curState = 0;
            }

            public static void MarkFlag<T>(ref int curState, T state)
            {
                try
                {
                    curState |= 0x01 << System.Convert.ToInt16(state);
                }
                finally
                {
                    Error<T>();
                }
            }

            public static void ReleaseFlag<T>(ref int curState, T state)
            {
                try
                { 
                    curState &= ~(0x01 << System.Convert.ToInt16(state));
                }
                finally
                {
                    Error<T>();
                }
            }
            public static void SetFlag<T>(ref int curState, T state) 
            {
                try
                {
                    curState = (0x01 << System.Convert.ToInt16(state));
                }
                finally
                {
                    Error<T>();
                }
            }
            public static bool IsMarkedFlag<T>(int curState, T state)
            {
                try
                {
                    return (curState & 0x01 << System.Convert.ToInt16(state)) != 0;
                }
                finally
                {
                    Error<T>();
                }
            }

            public static void ClearFlags(ref int curState)
            {
                curState = 0;
            }
        }
    }
}

