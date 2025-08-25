using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

using System.Linq;
using miniRAID.Backend;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace miniRAID
{
    public partial class MobRenderer : MonoBehaviour, IStateRenderer
    {
        // A bunch of grids occpied by this mob (for 1x1 mobs, only 1 grid; 2x2 -> 4 grids, etc.)
        // TODO: seperate GridBody into a single MonoBehaviour
        // public GridShape gridBody;

        Databackend backend;
        public bool isBoss;
        
        public MobData data;

        public Animator animator;

        public delegate void MobMenuGUIDelegate(MobRenderer mobRenderer, UI.UnitMenu state, UI.UIMenu_UIContainer menu);

        ///////////////////////////// Events /////////////////////////////
        /// TODO: Invoke order ......

        //// Timing

        //// Status calculation

        //// User Interface

        public event MobMenuGUIDelegate OnShowMobMenu;

        //public MobDHInputArgumentDelegate

        // Use this for initialization
        void Start()
        {
            Init();
        }

        private bool _inited = false;
        
        [InfoBox("Enable this when MobData should be initialized from Editor.")]
        public bool handleDataInit = false;
        public void Init()
        {
            if (_inited)
            {
                return;
            }
            
            animator = GetComponent<Animator>();

            backend = Databackend.GetSingleton();
            // UpdateGridPos();

            // // Find my bodies
            // data.gridBody = new GridShape();
            //
            // // TODO: FIXME: Move me to MobData.Init and use pre-defined grid bodies
            // foreach (MobGridProxy proxy in GetComponentsInChildren<MobGridProxy>())
            // {
            //     data.gridBody.AddGrid(Databackend.GetSingleton().GetGridPos(proxy.transform.position) - data.Position);
            // }

            if (handleDataInit)
            {
                data.renderer = this;
                data.nickname = this.name;
                data.Init(Globals.backend.RenderToBackendPos(transform.position) - Globals.halfGround, Consts.Direction.Up);
            }

            if(isBoss)
            {
                Globals.ui.Instance.BindAsBoss(this);
            }

            _inited = true;
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        // TODO: Move move-related to MobData
        // void UpdateGridPos()
        // {
        //     data.Position = backend.GetGridPos(transform.position);
        // }

        public void Refresh()
        {
            backend = Globals.backend;
            SyncRendererPosition();
            UpdateStatusColor();
        }

        public void SyncRendererPosition()
        {
            if (data == null)
            {
                Debug.LogError("!?");
            }

            transform.position = backend.BackendToRenderPosCenteredGrounded(data.Position);
        }

        public IEnumerator MoveTowards(Vector3Int targetGridPos) =>
            MoveTowards(Globals.backend.GridToBackendFloorPos(targetGridPos));

        public IEnumerator MoveTowards(Vector3 targetPos)
        {
            // Vector3 targetPosReal = new Vector3(targetPos.x + 0.5f, targetPos.z + 0.5f, transform.position.z);
            Vector3 targetPosReal = Globals.backend.BackendToRenderPosCenteredGrounded(targetPos);

            // Move until reached target
            while ((transform.position - targetPosReal).magnitude >= 1e-3)
            {
                // Move a little bit
                transform.position = Vector3.MoveTowards(transform.position, targetPosReal, 5.0f * Time.deltaTime);

                // Wait 1 frame before next movement
                yield return null;
            }

            SyncRendererPosition();
        }

        public void UpdateStatusColor()
        {
            // if (Globals.combatMgr.Instance.currentPhase == data.unitGroup)
            // {
            //     GetComponentInChildren<SpriteRenderer>().color = data.isControllable ? Color.white : Color.blue;
            // }
            // else
            // {
            GetComponentInChildren<BillboardSpriteRenderer>().Color = Color.white;
            // }
        }

        public void OnWakeUp()
        {
            UpdateStatusColor();
        }
        
        public void OnNewPhase()
        {
            UpdateStatusColor();
        }

        public IEnumerator HealAnimation()
        {
            // FIXME: Test animation
            GetComponentInChildren<BillboardSpriteRenderer>().Color = Color.green;
            yield return new WaitForSeconds(.2f);
            UpdateStatusColor();
        }

        public IEnumerator DamageAnimation()
        {
            for (int i = 0; i < 2; i++)
            {
                GetComponentInChildren<BillboardSpriteRenderer>().Color = Color.red;
                yield return new WaitForSeconds(.07f);
                GetComponentInChildren<BillboardSpriteRenderer>().Color = Color.clear;
                yield return new WaitForSeconds(.07f);
            }
            UpdateStatusColor();
        }

        public IEnumerator Killed(Consts.DamageHeal_Result info, bool destroy = true)
        {
            Globals.combatTracker.Record(new Consts.KillEvent
            {
                info = info
            });
            
            // TODO: Proper logic to destroy
            if (destroy)
            {
                GetComponentInChildren<BillboardSpriteRenderer>().enabled = false;
                Destroy(this.gameObject);
            }

            yield break;
        }
        
        public IEnumerator WaitForAnimation(string animationState)
        {
            if(animator == null) { yield break; }

            int hash = Animator.StringToHash($"Base Layer.{animationState}");
            animator.Play(hash);

            //Wait until we enter the current state
            while (animator.GetCurrentAnimatorStateInfo(0).fullPathHash != hash)
            {
                yield return null;
            }

            float counter = 0;
            float waitTime = animator.GetCurrentAnimatorStateInfo(0).length;

            //Now, Wait until the current state is done playing
            while (counter < (waitTime))
            {
                counter += Time.deltaTime;
                yield return null;
            }
        }

        public void ShowMenu(UI.UnitMenu menu, UI.UIMenu_UIContainer container)
        {
            OnShowMobMenu?.Invoke(this, menu, container);
        }

        public IEnumerator SetActiveAnim(bool value)
        {
            if (value == false)
            {
                // FIXME: Test animation
                GetComponentInChildren<BillboardSpriteRenderer>().Color = Color.cyan;
                yield return new WaitForSeconds(.15f);
                UpdateStatusColor();
            }

            yield break;
        }

        public void Destroy()
        {
            GameObject.Destroy(gameObject);
        }
    }
}
