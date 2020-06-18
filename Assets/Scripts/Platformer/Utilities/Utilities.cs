using System;
using Coroutine = UnityEngine.Coroutine;
using MonoBehaviour = UnityEngine.MonoBehaviour;
using IEnumerator = System.Collections.IEnumerator;

public static class StaticFunctions
{
    public static void SafeStopCoroutine(this MonoBehaviour monoBehaviour, Coroutine routine)
    {
        if (routine != null)
            monoBehaviour.StopCoroutine(routine);
    }
}

namespace LSG.Utilities
{
    public static class BitField
    {
        static void Error<T>(string s)
        {
            throw new System.Exception(string.Format("Invalid type {0} to convert to int16\n{1}", typeof(T).ToString(), s));
        }

        public static void MarkFlag<T>(ref ulong curState, T state)
        {
            try
            {
                curState |= 0x01ul << System.Convert.ToInt16(state);
            }
            catch (System.Exception e)
            {
                Error<T>(e.Message);
            }
        }

        public static void ReleaseFlag<T>(ref ulong curState, T state)
        {
            try
            {
                curState &= ~(0x01ul << System.Convert.ToInt16(state));
            }
            catch (System.Exception e)
            {
                Error<T>(e.Message);
            }
        }
        public static void SetFlag<T>(ref ulong curState, T state)
        {
            try
            {
                curState = (0x01ul << System.Convert.ToInt16(state));
            }
            catch (System.Exception e)
            {
                Error<T>(e.Message);
            }
        }

        public static bool IsMarkedFlag<T>(ulong curState, T state)
        {
            try
            {
                return (curState & 0x01ul << System.Convert.ToInt16(state)) != 0;
            }
            catch (System.Exception e)
            {
                Error<T>(e.Message);
            }
            return false;
        }

        public static void ClearFlags(ref ulong curState)
        {
            curState = 0;
        }

        public static void DirtBitFlag<T>(ref uint curState, T state)
        {
            try
            {
                curState |= 0x01u << System.Convert.ToInt16(state);
            }
            catch (System.Exception e)
            {
                Error<T>(e.Message);
            }
        }
        public static void ReleaseFlag<T>(ref uint curState, T state)
        {
            try
            {
                curState &= ~(0x01u << System.Convert.ToInt16(state));
            }
            catch (System.Exception e)
            {
                Error<T>(e.Message);
            }
        }
        public static void SetFlag<T>(ref uint curState, T state)
        {
            try
            {
                curState = (0x01u << System.Convert.ToInt16(state));
            }
            catch (System.Exception e)
            {
                Error<T>(e.Message);
            }
        }
        public static bool IsMarkedFlag<T>(uint curState, T state)
        {
            try
            {
                return (curState & 0x01u << System.Convert.ToInt16(state)) != 0;
            }
            catch (System.Exception e)
            {
                Error<T>(e.Message);
            }
            return false;
        }

        public static void ClearFlags(ref uint curState)
        {
            curState = 0;
        }

        public static void DirtBitFlag<T>(ref int curState, T state)
        {
            try
            {
                curState |= 0x01 << System.Convert.ToInt16(state);
            }
            catch (System.Exception e)
            {
                Error<T>(e.Message);
            }
        }

        public static void ReleaseFlag<T>(ref int curState, T state)
        {
            try
            {
                curState &= ~(0x01 << System.Convert.ToInt16(state));
            }
            catch (System.Exception e)
            {
                Error<T>(e.Message);
            }
        }
        public static void SetFlag<T>(ref int curState, T state)
        {
            try
            {
                curState = (0x01 << System.Convert.ToInt16(state));
            }
            catch (System.Exception e)
            {
                Error<T>(e.Message);
            }
        }

        public static bool IsMarkedFlag<T>(int curState, T state)
        {
            try
            {
                return (curState & 0x01 << System.Convert.ToInt16(state)) != 0;
            }
            catch (System.Exception e)
            {
                Error<T>(e.Message);
            }
            return false;
        }

        public static void ClearFlags(ref int curState)
        {
            curState = 0;
        }

        public static void DirtBitFlag(ref uint curState, int state)
        {
            curState |= 0x01u << state;
        }
        public static void ReleaseFlag(ref uint curState, int state)
        {
            curState &= ~(0x01u << state);
        }
        public static void SetFlag(ref uint curState, int state)
        {
            curState = (0x01u << state);
        }
        public static bool IsMarkedFlag(uint curState, int state)
        {
            return (curState & 0x01u << state) != 0;
        }


        public static void MarkFlag(ref int curState, int state)
        {
            curState |= 0x01 << state;
        }

        public static void ReleaseFlag(ref int curState, int state)
        {
            curState &= ~(0x01 << state);
        }
        public static void SetFlag(ref int curState, int state)
        {
            curState = 0x01 << state;
        }

        public static bool IsMarkedFlag(int curState, int state)
        {
            return (curState & 0x01 << state) != 0;
        }
    }
}

