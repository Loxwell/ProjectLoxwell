using DataStructure.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScheduleSystem.Core
{
    public static partial class Simulation
    {
        static HeapQueue<Simulation.Event> g_eventsQueue = new HeapQueue<Simulation.Event>();
        static Dictionary<System.Type, Stack<Simulation.Event>> g_eventPools = new Dictionary<System.Type, Stack<Simulation.Event>>();

        public static T New<T>(params object[] param) where T : Simulation.Event, new()
        {
            Stack<Simulation.Event> pool;
            if (!g_eventPools.TryGetValue(typeof(T), out pool))
            {
                pool = new Stack<Simulation.Event>(4);
                pool.Push(new T());
                g_eventPools[typeof(T)] = pool;
            }

            T t;
            if (pool.Count > 0)
                t = (T)pool.Pop();
            else
                t = new T();
            t.Initialize(param);
            return t;
        }

        public static void Clear()
        {
            g_eventsQueue.Clear();
        }

        public static void Destroy()
        {
            g_eventPools.Clear();
            g_eventsQueue.Clear();
        }

        /// <summary>
        /// Schedule an event for a future tick, and return it.
        /// </summary>
        /// <returns>The event.</returns>
        /// <param name="tick">Tick.</param>
        /// <typeparam name="T">The event type parameter.</typeparam>
        public static T Schedule<T>(float tick = 0, params object[] param) where T : Simulation.Event, new()
        {
            T ev = New<T>(param);

            if(ev)
            {
                ev.tick = Time.time + tick;
                g_eventsQueue.Push(ev);
            }

            return ev;
        }

        /// <summary>
        /// Reschedule an existing event for a future tick, and return it.
        /// </summary>
        /// <returns>The event.</returns>
        /// <param name="tick">Tick.</param>
        /// <typeparam name="T">The event type parameter.</typeparam>
        public static T Reshedule<T>(T theEvent, float tick) where T : Simulation.Event, new()
        {
            theEvent.tick = Time.time + tick;
            g_eventsQueue.Push(theEvent);
            return theEvent;
        }

        /// <summary>
        /// Return the simulation model instance for a class.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static T GetModel<T>() where T : class, new()
        {

#if UNITY_EDITOR
            T instance = InstanceRegister<T>.instance;
            //Debug.Log( string.Format("GetModel<{0}>.HashCode({1})", typeof(T), instance.GetHashCode()));
            return instance;
#else
            return InstanceRegister<T>.instance;
#endif
        }

        /// <summary>
        /// Set a simulation model instance for a class.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void SetModel<T>(T newInstance) where T : class, new()
        {
            InstanceRegister<T>.instance = newInstance;
        }

        public static void DestroyModel<T>() where T : class, new()
        {
            InstanceRegister<T>.instance = null;
        }

        /// <summary>
        /// Tick the simulation. Returns the count of remaining events.
        /// If remaining events is zero, the simulation is finished unless events are
        /// injected from an external system via a Schedule() call.
        /// </summary>
        /// <returns></returns>
        public static int Tick()
        {
            float time = Time.time;
            uint executedEventCount = 0;

            while(g_eventsQueue.Count > 0 && g_eventsQueue.Peek().tick <= time)
            {
                Simulation.Event ev = g_eventsQueue.Pop();
                float tick = ev.tick;
                ev.ExecuteEvent();
                if(ev.tick > tick)
                {
                    //event was rescheduled, so do not return it to the pool.
                }else
                {
                    ev.Cleanup();
                    try
                    {
                        g_eventPools[ev.GetType()].Push(ev);
                    }
                    catch (KeyNotFoundException)
                    {
                        //This really should never happen inside a production build.
                        Debug.LogError($"No Pool For : {ev.GetType()}");
                    }
                }
                ++executedEventCount;
            }

            return g_eventsQueue.Count;
        }
    }
}


