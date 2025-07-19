using System;
using System.Collections.Generic;
using miniRAID.Agents;
using miniRAID.Editor;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using SerializationUtility = Sirenix.Serialization.SerializationUtility;

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
            // var test = MemoryPackSerializer.Serialize(targetMob.data);

            // var settings = new JsonSerializerSettings
            // {
            //     ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            //     PreserveReferencesHandling = PreserveReferencesHandling.Objects
            // };
            //
            // string json = JsonConvert.SerializeObject(targetMob.data, settings);
            // Debug.Log(json);

            List<UnityEngine.Object> unityObjects;
            
            Profiler.BeginSample($"Serializing {targetMob.data.nickname}");
            
            byte[] result = 
                SerializationUtility.SerializeValue(targetMob.data, DataFormat.JSON, out unityObjects);
            
            Profiler.EndSample();
            
            string json = System.Text.Encoding.Default.GetString(result);

            Profiler.BeginSample($"Deserializing {targetMob.data.nickname}");
            
            MobData roundtrip = SerializationUtility.DeserializeValue<MobData>(result, DataFormat.JSON, unityObjects);
            roundtrip.RestoreFromDeserialization();
            
            /* It is essential to recalculate all mob first
             * then recalculate the MobListeners that mark them as source
             * to retrieve non-snapshot'd power correctly.
             * Sometimes, listener's source won't be recalculatestats'd since they are no longer available in the allmobs list.
             * In this case we leave it in incorrect state.
             * Otherwise, perhaps it can be down in 2-pass.
             */
            
            Profiler.EndSample();

            Debug.Log($"Packed: {result.Length * sizeof(byte) / 1024.0f} KiB");
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