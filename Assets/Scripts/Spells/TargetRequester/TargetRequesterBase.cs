using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

using Sirenix.OdinInspector;

namespace miniRAID.UI.TargetRequester
{
    public enum RequestType
    {
        Target,
        Ground,
        TargetRange,
        Directional
    }

    public class RequestStage
    {
        public Dictionary<Vector2Int, GridOverlay.Types> map;
        public Dictionary<Vector2Int, GridOverlay.Types> cursor;
        public RequestType type;

        public RequestStage()
        {
            map = new Dictionary<Vector2Int, GridOverlay.Types>();
            cursor = new Dictionary<Vector2Int, GridOverlay.Types>();
            type = RequestType.Target;
        }
    }

    /// <summary>
    /// 1 UIState start a request, given a mob;
    /// 2 TargetRequester will get control of current UI
    /// 3 After finished, TargetRequester invokes onFinish callback and give back the control of current UI
    ///
    /// TODO: The requester is unique upto ActionDataSOs, i.e., it is shared among RuntimeActions. Is it okay?
    /// </summary>
    [System.Serializable]
    public abstract class TargetRequesterBase : UIState
    {
        protected MobData mob;
        protected RuntimeAction ract;

        [FoldoutGroup("Debug info")]
        [LabelText("Current Stage")]
        [ReadOnly]
        public int currentStageCompleted = -1;

        [FoldoutGroup("Debug info")]
        [ReadOnly]
        public Stack<RequestStage> query_stack = new Stack<RequestStage>();

        [FoldoutGroup("Debug info")]
        [ReadOnly]
        public Stack<Vector2Int> choice = new Stack<Vector2Int>();

        public delegate void OnRequestFinish(Spells.SpellTarget target);

        protected System.Action onCancel;
        protected OnRequestFinish onFinish;

        protected GridOverlay overlay;

        RequestStage currentQuery
        {
            get
            {
                if(query_stack != null && query_stack.Count > 0)
                {
                    return query_stack.Peek();
                }
                return null;
            }
        }

        /// <summary>
        /// This will modify current UI
        /// </summary>
        /// <param name="mob"></param>
        public virtual void Request(MobData mob, RuntimeAction ract, OnRequestFinish onFinish, System.Action onCancel)
        {
            this.mob = mob;
            this.ract = ract;
            currentStageCompleted = -1;
            query_stack.Clear();
            choice.Clear();

            this.onFinish = onFinish;
            this.onCancel = onCancel;

            ui.EnterState(this, true);
            _Next(mob.Position, false);
        }

        protected void _Next(Vector2Int coord, bool notFirst = true)
        {
            if (notFirst)
            {
                choice.Push(coord);
            }

            currentStageCompleted += 1;
            query_stack.Push(Next(coord, notFirst));
            ShowQuery();
        }

        /// <summary>
        /// Progress to next step
        /// </summary>
        /// <param name="coord">coord selected in this step by UI</param>
        public abstract RequestStage Next(Vector2Int coord, bool notFirst = true);

        public virtual void Back()
        {
            if(query_stack.Count > 0)
            {
                query_stack.Pop();
            }

            if(choice.Count > 0)
            {
                choice.Pop();
            }

            currentStageCompleted -= 1;

            ShowQuery();
        }

        public virtual bool IsChoiceValid(Vector2Int coord)
        {
            return currentQuery.map.ContainsKey(coord);
        }

        public override void Submit(InputValue input)
        {
            base.Submit(input);
            if(IsChoiceValid(ui.cursor.position))
            {
                _Next(ui.cursor.position);
            }
        }

        public override void Cancel(InputValue input)
        {
            if(currentStageCompleted > 0)
            {
                Back();
            }
            else
            {
                SafeKillOverlay();
                onCancel();
                base.Cancel(input);
            }
        }

        public virtual void Finish(Spells.SpellTarget result)
        {
            SafeKillOverlay();

            onFinish?.Invoke(result);
        }

        public void EndState()
        {
            ui.BackState();
        }

        public void SafeKillOverlay()
        {
            if (overlay != null)
            {
                GameObject.Destroy(overlay.gameObject);
            }
        }

        public override void OnStateExit()
        {
            base.OnStateExit();
            ui.cursor.cursorShape = new GridShape(Vector2Int.zero);
        }

        public void ShowQuery()
        {
            SafeKillOverlay();

            if (currentQuery != null)
            {
                overlay = Globals.overlayMgr.Instance.FromDictionary(currentQuery.map);
            }
            // TODO: change cursor
        }

        public virtual bool CheckTargets(MobData mob, Spells.SpellTarget target)
        {
            return true;
        }
    }
}
