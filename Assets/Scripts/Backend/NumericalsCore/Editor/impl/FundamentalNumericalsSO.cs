using UnityEditor;
using UnityEngine;

namespace miniRAID.Backend.Numericals.Impl
{
    [FilePath("Numericals/FundamentalNumericals.asset", FilePathAttribute.Location.ProjectFolder)]
    public class FundamentalNumericalsSO : ScriptableSingleton<FundamentalNumericalsSO>
    {
        public float test, test2;
        
        [ContextMenu("Recompute")]
        public void ComputeNumericals()
        {
            
        }
        
        public void Save()
        {
            Save(true);
        }
    }
}