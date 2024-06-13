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
        
        // TODO: Move me to another place
        public HashSet<IMobListenerIndicator> indicators;
        public T AddIndicator<T>(T indicator) where T : IMobListenerIndicator
        {
            if (indicators == null)
            {
                indicators = new();
            }
            
            if(indicators.Add(indicator))
            {
                indicator.Instantiate();
                return indicator;
            }

            return default;
        }

        public void RemoveIndicator(IMobListenerIndicator indicator)
        {
            if (indicators.Remove(indicator))
            {
                indicator.Destroy();
            }
        }
        
        public void RemoveAllIndicators()
        {
            if (indicators == null)
            {
                return;
            }
            
            foreach(var indicator in indicators)
            {
                indicator.Destroy();
            }
            
            indicators.Clear();
        }
        
    }
}
