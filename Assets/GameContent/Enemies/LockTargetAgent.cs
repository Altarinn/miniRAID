// using System;
// using System.Collections;
// using System.Collections.Generic;
// using miniRAID.TurnSchedule;
// using UnityEngine;
//
// namespace miniRAID.Agents
// {
//     public class LockTargetAgent : MonoBehaviourAgentComponent
//     {
//         public MobRenderer initialTarget;
//         public MobData target;
//
//         private void Awake()
//         {
//             if (initialTarget != null)
//             {
//                 target = initialTarget.data;
//             }
//         }
//
//         public override IEnumerator Turn(MobData mob, Timestamp turn)
//         {
//             if(target != null)
//             {
//                 var aggro = mob.FindListener<AggroCollector>();
//                 aggro.useAggro = false;
//                 aggro.currentTarget = target;
//                 yield return new JumpIn(agent.Turn());
//             }
//         }
//     }
// }
