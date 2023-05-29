using System.Collections;
using System.Collections.Generic;
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
        public BackendState()
        {
            Globals.backend.RegisterState(this);
        }
    }
}
