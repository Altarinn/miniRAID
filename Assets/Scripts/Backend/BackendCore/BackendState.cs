using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Sirenix.Serialization;
using UnityEngine;

namespace miniRAID.Backend
{
    public class StateEntry<T>
    {
        // Store the current entry and return a tag for retrieval.
        // public StateTag Tag();
        
        // Setter
        // Getter

        // public struct HistoryNode<T>
        // {
        //     public T value;
        //     public HistoryNode<T> prev;
        //     public LinkedList<HistoryNode<T>> next;
        // }
    }
    
    public class BackendState
    {
        public Guid guid;

        public override int GetHashCode()
        {
            return guid.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            BackendState s = (obj as BackendState);
            return (s != null) && guid.Equals(s.guid);
        }

        public BackendState()
        {
            guid = Guid.NewGuid();
        }

        public void Register()
        {
            if (Globals.backend != null)
            {
                Globals.backend.RegisterState(this);
            }
        }

        [NonSerialized]
        public IStateRenderer renderer;

        public void DestroyRenderer()
        {
            renderer?.Destroy();
            renderer = null;
        }
    }

    public interface IColliderState
    {
        public GridCollider Collider { get; set; }

        public Vector3 Position
        {
            get => Collider.Position;
            set => Collider.Position = value;
        }
        
        public Consts.Direction Direction
        {
            get => Collider.Direction;
            set => Collider.Direction = value;
        }

        public void OnEnterCollider(BackendState other);
        public void OnExitCollider(BackendState other);
    }

    public interface IRenderableState
    {
        public void ConstructRenderer();
        public void UpdateRenderer();
    }

    public interface IStateRenderer
    {
        public abstract void Refresh();
        
        // Triggers when the renderer is being attached to new instance of BackendState
        // e.g., during deserialization.
        public virtual void OnReload() { }

        public abstract void Destroy();
    }
}
