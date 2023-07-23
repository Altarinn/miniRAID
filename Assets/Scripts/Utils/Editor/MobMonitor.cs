using System;
using System.Collections.Generic;
using miniRAID.Agents;
using miniRAID.Editor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

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
        public AggroAgentBase.AggroInfo[] targetAggro;

        public string agentInfo;

        private void OnInspectorUpdate()
        {
            if (targetMob != null)
            {
                var agent = targetMob.data.FindListener<AggroAgentBase>();
                if (agent != null)
                {
                    targetAggro = ((AggroAgentBase)agent).GetAggroListUtil().ToArray();

                    agentInfo = agent.GetInformationString();
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