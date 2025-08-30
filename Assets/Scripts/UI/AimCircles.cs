using System;
using System.Linq;
using miniRAID.Agents;
using miniRAID.Spells;
using UnityEngine;

namespace miniRAID.UI
{
    public class AimCircles : MonoBehaviour
    {
        [SerializeField]
        private Transform[] circles;
        private MobData[] players;

        private void Awake()
        {
            players = new MobData[circles.Length];
        }

        public void UpdateAllCircles()
        {
            // players = Globals.backend.allMobs.Where(x => x.unitGroup == Consts.UnitGroup.Player).ToArray();
            for (int i = 0; i < Mathf.Min(circles.Length, players.Length); i++)
            {
                UpdateCircle(i);
            }
        }

        public void RegisterPlayer(int i, MobData playerMob)
        {
            players[i] = playerMob;
        }

        public void Clear()
        {
            players = new MobData[9];
        }

        // TODO: FIXME: Performance concerns
        private void UpdateCircle(int i)
        {
            if (i >= players.Length) return;
            if (players[i] == null) return;
            
            // Find current target for player #i
            SpellTarget target = players[i].FindListener<PlayerAutoAttackAgentBase>()?.GetTarget(players[i]);
            if (target == null) { circles[i].gameObject.SetActive(false); return; }

            Vector3Int targetGrid = Vector3Int.zero;
            if (typeof(SingleMobTarget).IsAssignableFrom(target.GetType()))
            {
                targetGrid = ((SingleMobTarget)target).Target.GridPosition;
            }
            else if (typeof(SingleCoordinateTarget).IsAssignableFrom(target.GetType()))
            {
                targetGrid = ((SingleCoordinateTarget)target).Target;
            }
            else
            {
                circles[i].gameObject.SetActive(false);
                return;
            }

            // Just use the first target for now
            Vector3 worldPos = Globals.backend.BackendToRenderPos(targetGrid) + Vector3.one * 0.5f;

            circles[i].gameObject.SetActive(true);
            circles[i].GetComponent<BillboardSpriteRenderer>().Color = players[i].baseDescriptor.color;
            circles[i].GetComponent<BillboardSpriteRenderer>().SortingOrder = 5;
            circles[i].localPosition = worldPos;
        }
    }
}