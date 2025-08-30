using System.Collections;
using System.Collections.Generic;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Profiling;

namespace miniRAID
{
    public static class SaveDataSerializer
    {
        public class SaveData
        {
            public List<UnityEngine.Object> unityObjects;
            public byte[] serializedState;
        }
        
        public struct SaveState
        {
            public Databackend backend;
            public CombatSchedulerCoroutine.SerializableInfo scheduler;
            public CombatTracker tracker;
            public RNG rng;
        }
        
        public static SaveData saveSlot, saveSlotBackup;
        
        public static SaveData SerializeEverything()
        {
            SaveData data = new();
            
            Profiler.BeginSample($"Serializing entire DataBackend");

            SaveState state = new SaveState()
            {
                backend = Globals.backend,
                scheduler = Globals.combatMgr.Instance.PrepareSerializationInfo(),
                tracker = Globals.combatTracker,
                rng = Globals.cc.rng
            };
            
            data.serializedState = 
                SerializationUtility.SerializeValue(state, DataFormat.Binary, out data.unityObjects);
            
            Profiler.EndSample();
            
            // string json = System.Text.Encoding.Default.GetString(result);

            Debug.Log($"Packed: {data.serializedState.Length * sizeof(byte) / 1024.0f} KiB");
            return data;
        }

        public static IEnumerator DeserializeEverything(SaveData save)
        {
            Profiler.BeginSample($"Deserializing entire DataBackend");
            
            SaveState roundtrip = SerializationUtility.DeserializeValue<SaveState>(
                save.serializedState, DataFormat.Binary, save.unityObjects);

            yield return new JumpIn(roundtrip.backend.Initialize());
            
            Globals.combatMgr.Instance.RestoreFromSerialization(roundtrip.scheduler);
            roundtrip.tracker.RestoreFromSerialization();
            Globals.combatTracker = roundtrip.tracker;
            Databackend.ReplaceSingleton(roundtrip.backend);
            // Globals.backend.RestoreFromDeserialization();
            
            /* It is essential to recalculate all mob first
             * then recalculate the MobListeners that mark them as source
             * to retrieve non-snapshot'd power correctly.
             * Sometimes, listener's source won't be recalculatestats'd since they are no longer available in the allmobs list.
             * In this case we leave it in incorrect state.
             * Otherwise, perhaps it can be down in 2-pass.
             */
            
            Profiler.EndSample();
        }

        public static void BeginDeserializeEverything(SaveData save)
        {
            if (save.serializedState == null || save.unityObjects == null)
            {
                Debug.LogWarning("Please do serialize everything first.");
                return;
            }
            
            Globals.combatMgr.Instance.OnNextSnapshot(DeserializeEverything(save));
            
            // Wait until we can safely load-in
            Globals.combatMgr.Instance.MarkPlayerTurnEnd();
        }
    }
}