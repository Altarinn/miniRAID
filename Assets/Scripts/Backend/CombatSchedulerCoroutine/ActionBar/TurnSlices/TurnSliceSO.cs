using System.Collections;
using UnityEngine;
using Sprite = UnityEngine.ProBuilder.Shapes.Sprite;

namespace miniRAID.TurnSchedule
{
    public abstract class AbstractTurnSliceSO : CustomIconScriptableObject
    {
        public Sprite barIcon;
        public Color mainColor;
        public string label;
        public bool showInUI = true;

        public TurnSliceCategory defaultCategory;

        public abstract IEnumerator Turn(TurnSlice slice, CombatSchedulerCoroutine coroutine);
    }

    public abstract class TurnSliceSO : AbstractTurnSliceSO
    {
        public virtual TurnSlice Wrap(TurnSliceMetadata metadata)
        {
            return new TurnSlice(this, metadata);
        }
    }

    public enum TurnSliceCategory
    {
        Inherited = 0, // Default value
        PlayerTurn,
        EnemyRegularTurn,
        EnemySpecialTurn,
        AllyRegularTurn,
        AllySpecialTurn,
        UtilityTurn,
        Uncategorized,
    }
    
    public struct TurnSliceMetadata
    {
        public TurnSliceCategory category;
        public int priority;
        
        // TODO: FIXME: Savedata? How to handle this? Is this okay?
        public MobData source;

        public TurnSliceMetadata(MobData source)
        {
            this.category = TurnSliceCategory.Inherited;
            this.priority = 0;

            this.source = source;
        }

        public TurnSliceMetadata(MobData source, int priority)
        {
            this.category = TurnSliceCategory.Inherited;

            this.priority = priority;
            this.source = source;
        }
        
        public TurnSliceMetadata(MobData source, int priority, TurnSliceCategory category)
        {
            this.category = category;
            this.priority = priority;
            this.source = source;
        }
    }

    public class TurnSlice
    {
        public AbstractTurnSliceSO data;
        public TurnSliceMetadata metadata;

        public virtual bool ShowInUI => data.showInUI;
        public virtual string Label => data.label;
        public virtual Color MainColor => data.mainColor;
        public virtual Sprite BarIcon => data.barIcon;
        
        protected CombatSchedulerCoroutine coroutine;

        public TurnSlice(AbstractTurnSliceSO data, TurnSliceMetadata metadata)
        {
            this.data = data;
            this.metadata = metadata;

            if (this.metadata.category == TurnSliceCategory.Inherited)
            {
                this.metadata.category = data.defaultCategory;
            }
        }
        
        public void RegisterTo(CombatSchedulerCoroutine coroutine)
        {
            this.coroutine = coroutine;
        }

        public virtual IEnumerator Turn() => data.Turn(this, coroutine);
    }
}