using System;
using System.Collections.Generic;
using UnityEngine;

namespace miniRAID.Collections
{
    public class PrioritySet<T> where T: IComparable
    {
        [SerializeField]
        private List<T> _list = new();
        public List<T> List => _list;

        public bool AddUnique(T t)
        {
            if (_list.Contains(t))
            {
                return false;
            }
            
            _list.Add(t);
            _list.Sort();

            return true;
        }

        public void Remove(T t)
        {
            _list.Remove(t);
        }
    }
}