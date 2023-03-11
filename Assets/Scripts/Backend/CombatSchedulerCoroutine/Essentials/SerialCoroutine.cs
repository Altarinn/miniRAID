using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;

namespace miniRAID
{
    public class SerialYieldReturns {}
    public class JumpIn : SerialYieldReturns
    {
        public IEnumerator dest;

        public struct JumpInDebugInfo
        {
            public int lineNum;
            public string member, file;

            public string ToString()
            {
                return $"JumpIn Debug Info:\n{member} : {lineNum} ( {file} )";
            }
        }

        public JumpInDebugInfo info;

        public JumpIn(
            IEnumerator obj,
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string caller = null,
            [CallerFilePath] string file = null)
        {
            dest = obj;

            info = new JumpInDebugInfo
            {
                lineNum = lineNumber,
                member = caller,
                file = file,
            };
        }
    }

    public class EndImmediate : SerialYieldReturns { }

    public class SerialCoroutineHandle
    {
        public IEnumerator handle;
        public SerialYieldReturns returns;

        public JumpIn.JumpInDebugInfo debugInfo;

        public SerialCoroutineHandle(IEnumerator obj,
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string caller = null,
            [CallerFilePath] string file = null)
        {
            debugInfo = new JumpIn.JumpInDebugInfo
            {
                lineNum = lineNumber,
                member = caller,
                file = file,
            };

            if (obj == null)
            {
                Debug.LogWarning($"Null IEnumerator encountered ... \n {debugInfo.ToString()}");
            }
            handle = obj;
        }

        public SerialCoroutineHandle(IEnumerator obj, JumpIn.JumpInDebugInfo info)
        {
            debugInfo = info;

            if (obj == null)
            {
                Debug.LogWarning($"Null IEnumerator encountered ... \n {debugInfo.ToString()}");
            }
            handle = obj;
        }
    }

    public class SerialCoroutine : MonoBehaviour
    {
        Stack<SerialCoroutineHandle> handleStack = new();

        /// <summary>
        /// yield return IEnumerator / yield return StartCoroutine(IEnumerator):
        ///     Treated as waiting a unity coroutine (+1 frame delay)
        /// 
        /// yield return JumpIn(IEnumerator):
        ///     Will return to previous coroutine immediately.
        ///     
        /// yield break; (?) / yield return -1; / yield return EndImmediate():
        ///     Ends immediately and returns to the previous coroutine.
        /// </summary>
        /// <param name="obj"></param>

        public void StartSerialCoroutine(IEnumerator obj)
        {
            if(handleStack.Count == 0)
            {
                handleStack.Push(new SerialCoroutineHandle(obj));
                StartCoroutine(Tick());
            }
            else
            {
                Debug.LogWarning("SerialCoroutine: Cannot start multiple serial coroutines at once!");
            }
        }

        public IEnumerator Tick()
        {
            while(handleStack.Count > 0)
            {
                var currentHandle = handleStack.Peek();

                //if(currentHandle.condition != null)
                //{
                //    bool shouldResume = currentHandle.condition.Tick(dt);
                //    if (!shouldResume) { break; }
                //}

                bool shouldPop = false;

                if (currentHandle.handle.MoveNext())
                {
                    object result = currentHandle.handle.Current;

                    // Unity instructions
                    if (
                        result is YieldInstruction ||
                        result is CustomYieldInstruction ||
                        result is Coroutine ||
                        result is IEnumerator)
                    {
                        yield return result;
                    }
                    else if (result is int num)
                    {
                        if (num >= 0) { yield return num; }
                        else // <0, treated as EndImmediate
                        {
                            shouldPop = true;
                        }
                    }
                    else if (result is EndImmediate)
                    {
                        shouldPop = true;
                    }
                    else if (result is JumpIn ji)
                    {
                        if(ji.dest != null)
                        {
                            handleStack.Push(new SerialCoroutineHandle(ji.dest, ji.info));
                        }
                        else
                        {
                            Debug.LogWarning($"JumpIn is Null. \n{StackTrace()}");
                        }
                    }
                    else // What is this?
                    {
                        yield return result;
                    }
                }
                else
                {
                    // We have finished.
                    shouldPop = true;
                }

                if(shouldPop)
                {
                    handleStack.Pop();
                }
            }
        }

        public string StackTrace()
        {
            string s = "JumpIn StackTrace:\n";
            foreach (var entry in handleStack)
            {
                s += $"{entry.debugInfo.member} : {entry.debugInfo.lineNum} ( {entry.debugInfo.file} )\n";
            }

            return s;
        }
    }

    //public class CoroutineEvent<T> where T : Delegate
    //{
    //    HashSet<T> listeners = new();

    //    public IEnumerator Invoke(params object[] vs)
    //    {
    //        foreach (var e in listeners)
    //        {
    //            yield return new JumpIn((IEnumerator)e.DynamicInvoke(vs));
    //        }
    //        yield break;
    //    }

    //    public static CoroutineEvent<T> operator +(CoroutineEvent<T> evt, T listener)
    //    {
    //        if(!evt.listeners.Contains(listener))
    //        {
    //            evt.listeners.Add(listener);
    //        }

    //        return evt;
    //    }

    //    public static CoroutineEvent<T> operator -(CoroutineEvent<T> evt, T listener)
    //    {
    //        if (!evt.listeners.Contains(listener))
    //        {
    //            evt.listeners.Remove(listener);
    //        }

    //        return evt;
    //    }
    //}

    public class Testaaa<T1, T2>
    {
        Func<T1, T2, IEnumerator> Wrapper(Action<T1, T2> f)
        {
            IEnumerator foo(T1 a, T2 b)
            {
                f.Invoke(a, b);
                yield break;
            }

            return foo;
        }
    }
}
