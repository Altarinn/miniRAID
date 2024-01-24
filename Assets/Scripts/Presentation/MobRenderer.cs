using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

using System.Linq;

using Sirenix.Serialization;

namespace miniRAID
{
    [XLua.LuaCallCSharp]
    [ParameterDefaultName("mob")]
    public partial class MobRenderer : MonoBehaviour
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

        //// Combat-related

        #region Stats

        public int VIT => (int)data.baseStats.VIT.Value;
        public int STR => (int)data.baseStats.STR.Value;
        public int MAG => (int)data.baseStats.MAG.Value;
        public int INT => (int)data.baseStats.INT.Value;
        public int DEX => (int)data.baseStats.AGI.Value;
        public int TEC => (int)data.baseStats.TEC.Value;

        public int AttackPower => (int)data.attackPower.Value;
        public int SpellPower => (int)data.spellPower.Value;
        public int HealPower => (int)data.healPower.Value;
        public int BuffPower => (int)data.buffPower.Value;

        public int Def => (int)data.defense.Value;
        public int SpDef => (int)data.spDefense.Value;

        public float AggroMul => (float)data.aggroMul.Value;

        #endregion

        //public MobDHInputArgumentDelegate

        // Use this for initialization
        void Start()
        {
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
            
            data.mobRenderer = this;
            data.nickname = this.name;
            data.Init();

            if(isBoss)
            {
                Globals.ui.Instance.BindAsBoss(this);
            }
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

        public void SyncRendererPosition()
        {
            transform.position = backend.GridToWorldPosCenteredGrounded(data.Position);
        }

        public IEnumerator MoveTowards(Vector3Int targetPos)
        {
            // Vector3 targetPosReal = new Vector3(targetPos.x + 0.5f, targetPos.z + 0.5f, transform.position.z);
            Vector3 targetPosReal = Globals.backend.GridToWorldPosCenteredGrounded(targetPos);

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
            if (Globals.combatMgr.Instance.currentPhase == data.unitGroup)
            {
                GetComponentInChildren<SpriteRenderer>().color = data.isControllable ? Color.white : Color.blue;
            }
            else
            {
                GetComponentInChildren<SpriteRenderer>().color = Color.white;
            }
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
            GetComponentInChildren<SpriteRenderer>().color = Color.green;
            yield return new WaitForSeconds(.2f);
            UpdateStatusColor();
        }

        public IEnumerator DamageAnimation()
        {
            for (int i = 0; i < 2; i++)
            {
                GetComponentInChildren<SpriteRenderer>().color = Color.red;
                yield return new WaitForSeconds(.07f);
                GetComponentInChildren<SpriteRenderer>().color = Color.clear;
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
                GetComponentInChildren<SpriteRenderer>().enabled = false;
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
                GetComponentInChildren<SpriteRenderer>().color = Color.cyan;
                yield return new WaitForSeconds(.35f);
                UpdateStatusColor();
            }

            yield break;
        }
    }
}
