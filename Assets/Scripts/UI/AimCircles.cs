using System;
using System.Linq;
using miniRAID.Agents;
using miniRAID.Spells;
using UnityEngine;

namespace miniRAID.UI
{
    public class AimCircles : MonoBehaviour
    {
        public Transform[] circles;
        public MobData[] players;

        public void UpdateAllCircles()
        {
            players = Globals.backend.allMobs.Where(x => x.unitGroup == Consts.UnitGroup.Player).ToArray();
            for (int i = 0; i < Mathf.Min(circles.Length, players.Length); i++)
            {
                UpdateCircle(i);
            }
        }

        // TODO: FIXME: Performance concerns
        private void UpdateCircle(int i)
        {
            // Find current target for player #i
            SpellTarget target = players[i].FindListener<PlayerAutoAttackAgentBase>()?.GetTarget(players[i]);
            if (target == null) { circles[i].gameObject.SetActive(false); return; }

            // Just use the first target for now
            Vector3 worldPos = Globals.backend.GridToWorldPos(target.targetPos[0]) + Vector3.one * 0.5f;

            circles[i].gameObject.SetActive(true);
            circles[i].GetComponent<SpriteRenderer>().color = players[i].baseDescriptor.color;
            circles[i].localPosition = worldPos;
        }
    }
}