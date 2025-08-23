using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace miniRAID.Spells
{
    public abstract class SpellTarget
    {
        // public List<Vector3Int> targetPos = new List<Vector3Int>();

        // public SpellTarget() { }

        // public SpellTarget(Vector3Int point)
        // {
        //     targetPos.Add(point);
        // }

        // public SpellTarget(IEnumerable<Vector3Int> point)
        // {
        //     targetPos.AddRange(point);
        // }
        
        public abstract bool Valid { get; }
    }

    public class SingleCoordinateTarget : SpellTarget
    {
        public Vector3Int Target;

        public SingleCoordinateTarget(Vector3Int target)
        {
            Target = target;
        }

        public override string ToString()
        {
            return Target.ToString();
        }

        public override bool Valid => Globals.backend.InMap(Target);
    }

    public class SingleMobTarget : SpellTarget
    {
        public MobData Target;
        public Vector3Int TargetPosition;

        public SingleMobTarget(MobData target, Vector3Int targetPosition)
        {
            Target = target;
            TargetPosition = targetPosition;
        }

        public override string ToString()
        {
            return Target.nickname;
        }

        public override bool Valid => Target is { IsInWorld: true };
    }
    
    // public class SelfTarget : SingleMobTarget
    // {
    //     public SelfTarget(MobData target) : base(target)
    //     { }
    // }

    public class FourDirectionalTarget : SpellTarget
    {
        public Consts.Direction Target;

        public FourDirectionalTarget(Consts.Direction target)
        {
            Target = target;
        }

        public override string ToString()
        {
            switch (Target)
            {
                case Consts.Direction.Down:
                    return "South";
                case Consts.Direction.Up:
                    return "North";
                case Consts.Direction.Left:
                    return "West";
                case Consts.Direction.Right:
                    return "East";
            }

            return "";
        }

        public override bool Valid => true;
    }

    public class DynamicTypeTarget : SpellTarget
    {
        public SpellTarget Target;
        public override bool Valid => Target.Valid;
    }
}