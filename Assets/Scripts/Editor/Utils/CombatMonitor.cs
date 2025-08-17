using System;
using System.Collections.Generic;
using System.Linq;
using miniRAID;
using miniRAID.ActionHelpers;
using miniRAID.Agents;
using miniRAID.Buff;
using miniRAID.TurnSchedule;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using Application = UnityEngine.Device.Application;
using Random = UnityEngine.Random;
using SerializationUtility = Sirenix.Serialization.SerializationUtility;

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
            turnSchedule = Globals.combatMgr?.Instance?.turnSchedule?.ToList();
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
        
        // GridFx Test
        // public GridEffectSO effect;
        // public MobRenderer dummySrc;
        // [Button(ButtonSizes.Small)]
        // public void MakePools()
        // {
        //     Vector3Int pos = new Vector3Int(
        //         Random.Range(0, Globals.backend.mapSizeX),
        //         Random.Range(0, Globals.backend.mapHeight),
        //         Random.Range(0, Globals.backend.mapSizeZ));
        //
        //     var coll = new PointCollider();
        //     coll.Position = pos;
        //     GridEffect rfx = (GridEffect)effect.LeveledWrapFx(
        //         dummySrc.data, 1, coll);
        //     
        //     dummySrc.data.AddListener(rfx);
        // }
        
        [Button(ButtonSizes.Large)]
        public void SaveState()
        {
            SaveDataSerializer.saveSlot = SaveDataSerializer.saveSlotBackup;
        }

        [Button(ButtonSizes.Large)]
        public void LoadState()
        {
            SaveDataSerializer.BeginDeserializeEverything(SaveDataSerializer.saveSlot);
        }
    }
}