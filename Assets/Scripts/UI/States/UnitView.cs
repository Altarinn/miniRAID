using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;

// Somehow not used anymore.

namespace miniRAID.UI
{
    /// <summary>
    /// Provide quick actions for units.
    /// </summary>
    public class UnitView : UIState
    {
        Mob currentUnit;
        GridOverlay overlay;

        HashSet<Vector2Int> moveRange;

        public UnitView(Mob mob)
        {
            stateStr = $"UnitView: {mob.name}";
            currentUnit = mob;

            if (currentUnit)
            {
                currentUnit.GetComponentInChildren<SpriteRenderer>().color = Color.red;
                ui.ShowMainMobStats(currentUnit);
            }
        }

        public override void OnStateEnter()
        {
            base.OnStateEnter();

            if (currentUnit)
            {
                // Highlight moveable grids
                UpdateRanges();
            }
        }

        private void UpdateRanges()
        {
            if (overlay)
            {
                GameObject.Destroy(overlay.gameObject);
            }

            moveRange = Databackend.GetSingleton().GetMoveableGrids(currentUnit);
            overlay = Globals.overlayMgr.Instance.FromMoveableRange(currentUnit, moveRange);
        }

        public override void OnStateExit()
        {
            base.OnStateExit();
            if (overlay)
            {
                GameObject.Destroy(overlay.gameObject);
            }
        }

        public override void OnStateDestroyed()
        {
            if (currentUnit)
            {
                currentUnit.GetComponentInChildren<SpriteRenderer>().color = Color.white;
            }

            ui.HideMainMobStats();
        }

        public override void Submit(InputValue input)
        {
            base.Submit(input);

            GridData grid = Databackend.GetSingleton().getMap(ui.currentGridPos.x, ui.currentGridPos.y);
            Mob mob = grid.mob;

            if (mob == currentUnit)
            {
                Debug.Log($"Unit menu: {currentUnit.gameObject.name}");
                //Debug.LogWarning("Unit menu not implemented.");
                //fsm.SetTrigger("Unit Menu");
                ui.EnterState(new UnitMenu(mob), true);
            }

            // Move
            // TODO: can move
            if (mob == null && grid.solid == false && PreCheckMovement(ui.currentGridPos))
            {
                //if(Globals.backend.IsPathValid(mob, path))
                if (true)
                {
                    if (overlay)
                    {
                        GameObject.Destroy(overlay.gameObject);
                    }

                    //WaitForAnimation();
                    currentUnit.MoveTo(ui.currentGridPos, () =>
                    {
                        //AnimationFinish();

                        if (enabled)
                        {
                            UpdateRanges();
                        }
                    });
                }
            }

            // TODO: Implement this with range & AP check etc.
            // Attack
            //if (mob != null && mob != currentUnit && currentUnit.data.mainWeapon.GetRegularAttackSpell().CheckWithCosts(currentUnit))
            //{
            //    // TODO: multi-target
            //    WaitForAnimation();
            //    currentUnit.RegularAttack(new Spells.SpellTarget(ui.currentGridPos), AnimationFinish);
            //}
        }

        private bool PreCheckMovement(Vector2Int currentGridPos)
        {
            return moveRange.Contains(currentGridPos);
        }
    }
}
