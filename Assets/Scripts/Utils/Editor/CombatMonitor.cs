using System;
using System.Collections.Generic;
using miniRAID;
using miniRAID.Agents;
using miniRAID.TurnSchedule;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine.Device;

namespace Utils.Editor
{
    public class CombatMonitor : OdinEditorWindow
    {
        [MenuItem("miniRAID/Combat Monitor")]
        private static void OpenWindow()
        {
            GetWindow<CombatMonitor>().Show();
        }
        
        [LabelText("Turn Summary")]
        [TableList]
        public CombatTracker.TurnSummary[] turnSummary;

        [LabelText("RNG History")] 
        [TableList] 
        public RNG.RNGHistoryEntry[] rngHistory;

        [Button(ButtonSizes.Large)]
        public void UpdateTurnSchedule()
        {
            turnSchedule = Globals.combatMgr?.Instance?._TurnScheduleView;
        }
        
        [LabelText("TurnSchedule")]
        public List<TurnSlice> turnSchedule;

        private void OnInspectorUpdate()
        {
            if (Application.isPlaying)
            {
                turnSummary = Globals.combatTracker?.turnSummaries.ToArray();
                rngHistory = Globals.combatCoroutine?.Instance.currentContext.rng?.history.ToArray();
            }
        }
    }
}