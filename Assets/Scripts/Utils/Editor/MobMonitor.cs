using System;
using System.Collections.Generic;
using MemoryPack;
using miniRAID.Agents;
using miniRAID.Editor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace miniRAID
{
    public class MobMonitor : OdinEditorWindow
    {
        [MenuItem("miniRAID/Mob Monitor")]
        private static void OpenWindow()
        {
            GetWindow<MobMonitor>().Show();
        }

        [SceneObjectsOnly]
        [Required]
        public MobRenderer targetMob;

        [LabelText("Aggro")]
        [TableList]
        public AggroCollector.AggroInfo[] targetAggro;

        public string agentInfo;

        [Button(ButtonSizes.Large)]
        public void Serialize()
        {
            // TODO
            var test = MemoryPackSerializer.Serialize(targetMob.data);
            Debug.Log("Packed");
        }

        private void OnInspectorUpdate()
        {
            if (targetMob != null)
            {
                var aggro = targetMob.data.FindListener<AggroCollector>();
                if (aggro != null)
                {
                    targetAggro = aggro.GetAggroListUtil().ToArray();
                    agentInfo = aggro.GetInformationString();
                }
                else
                {
                    // targetAggro = "Not aggro-based behaviour";
                }
            }
            else
            {
                // targetAggro = "Please choose target Mob";
            }
        }
    }
}