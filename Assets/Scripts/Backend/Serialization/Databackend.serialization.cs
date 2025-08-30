using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Backend.Map;
using miniRAID.Backend;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEngine;

namespace miniRAID
{
    public partial class Databackend
    {
        [OdinSerialize] public HashSet<BackendState> allStates = new();

        public void RegisterState(BackendState state)
        {
            Debug.Log($"Registered {state}");
            allStates.Add(state);

            Globals.combatCoroutine.Instance.RequireOnNextFrameEnd(() =>
            {
                if (state.renderer == null)
                {
                    (state as IRenderableState)?.ConstructRenderer();
                }
            });
        }

        public void RemoveState(BackendState state)
        {
            allStates.Remove(state);
        }
        
        // Used for deserialization only
        static public void ReplaceSingleton(Databackend d)
        {
            if (d.mapSystem == null)
            {
                Debug.LogError("Please call Databackend.Initialize (is a Coroutine) before actually use it!");
                return;
            }
            instance = d;
        }

        [OnDeserialized]
        public void RestoreFromDeserialization()
        {
            gridEffectChanges = new();

            // TODO: Properly restore map modifications from memory
            mapSystem = new MapSystem();
            Globals.ui.Instance.circles.Clear();

            // Pass 1
            foreach (var mob in allMobs)
            {
                mob.RestoreFromDeserialization();
            }

            // Pass 2: Update listeners that sourced from other non-initialized Mobs
            foreach (var mob in allMobs)
            {
                mob.RestoreFromDeserialization();
                if (mob.mobRenderer != null)
                {
                    mob.mobRenderer.data = mob;
                    mob.mobRenderer.Refresh();
                }
                
                TryAddMobToPlayer(mob);
            }
            
            // Handle global events
            onMobAdded.RestoreListener();
            onMobRemoved.RestoreListener();
            onGlobalActionPostcast.RestoreListener();

            // Pass 3: Handle renderers
            Databackend previousBackend = Globals.backend;
            ReplaceSingleton(this);

            Dictionary<Guid, IStateRenderer> previousRenderers;

            if (previousBackend != null)
            {
                // Grab and reuse previous renderers
                previousRenderers = previousBackend.allStates
                    .Where(x => x.renderer != null)
                    .ToDictionary(x => x.guid, x => x.renderer);
            }
            else
            {
                previousRenderers = new();
            }

            var statesToRender = allStates
                .Where(x => x is IRenderableState);

            foreach (var state in statesToRender)
            {
                if (previousRenderers.ContainsKey(state.guid))
                {
                    state.renderer = previousRenderers[state.guid];
                    state.renderer.OnReload();
                    previousRenderers.Remove(state.guid);
                }
                else
                {
                    (state as IRenderableState)?.ConstructRenderer();
                }
                
                (state as IRenderableState)?.UpdateRenderer();
            }

            // Destroy orphan renderers
            previousRenderers.ForEach(x => x.Value.Destroy());
        }
    }
}