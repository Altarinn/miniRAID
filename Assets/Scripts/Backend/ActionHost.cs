using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miniRAID
{
    public class ActionHost : MonoBehaviour
    {
        List<Coroutine> Coroutines = new List<Coroutine>();

        public bool IsActionFinished
        {
            get
            {
                return Coroutines.Count <= 0;
            }
        }

        // Use this for initialization
        //void Start()
        //{

        //}

        // Update is called once per frame
        //void Update()
        //{

        //}

        public void StartActionCoroutine(IEnumerator corotine, Action.OnActionCorotineFinished callback)
        {
            StartCoroutine(CoWrapper(corotine, callback, false));
        }

        public void HostActionCoroutine(IEnumerator corotine, Action.OnActionCorotineFinished callback)
        {
            StartCoroutine(CoWrapper(corotine, callback, true));
        }

        protected IEnumerator CoWrapper(IEnumerator co, Action.OnActionCorotineFinished callback, bool isHost = false)
        {
            Coroutine c = StartCoroutine(co);
            Coroutines.Add(c);

            yield return c;

            Coroutines.Remove(c);

            if(isHost)
            {
                while(!IsActionFinished)
                {
                    yield return null;
                }
            }

            callback?.Invoke();
        }
    }
}