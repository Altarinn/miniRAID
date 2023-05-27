using System;
using System.Collections;
using System.Collections.Generic;
using miniRAID.Buff;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace miniRAID.ActionHelpers
{
    public class Essentials
    {
        public static List<MobData> CaptureAllTargetsWithinRange()
        {
            throw new NotImplementedException();
        }

        public static Mob MobAtGrid(Vector2Int pos)
        {
            return Globals.backend.getMap(pos.x, pos.y)?.mob;
        }
    }

    public enum FloatModifierType
    {
        Multiply,
        Add,
        Expression
    }
    
    [InlineProperty(LabelWidth = 40)]
    public struct FloatModifier
    {
        [HorizontalGroup(MaxWidth = 80)]
        [HideLabel]
        public FloatModifierType type;
        
        [HorizontalGroup]
        [HideIf("type", FloatModifierType.Expression)]
        [HideLabel]
        public float value;
        
        [HorizontalGroup]
        [ShowIf("type",FloatModifierType.Expression)]
        [HideLabel]
        public LuaGetter<float, float> expression;

        public FloatModifier(float val)
        {
            type = FloatModifierType.Multiply;
            value = val;
            expression = null;
        }

        public float Apply(float input)
        {
            switch (type)
            {
                case FloatModifierType.Add:
                    return input + value;
                case FloatModifierType.Multiply:
                    return input * value;
                case FloatModifierType.Expression:
                    return expression.Eval(input);
            }

            return 0;
        }
    }

    public class SpellDamageHeal
    {
        public Consts.Elements type;

        public FloatModifier power = new(1.0f);
        public FloatModifier crit = new(1.0f);
        public FloatModifier hit = new(1.0f);

        public IEnumerator Do(RuntimeAction spellContext, Mob src, Mob tgt)
        {
            yield return new JumpIn(Globals.backend.DealDmgHeal(
                tgt,
                new Consts.DamageHeal_FrontEndInput
                {
                    source = src,
                    value = power.Apply(spellContext.power),
                    type = type,

                    crit = crit.Apply(spellContext.crit),
                    hit = 1.0f,// hit.Eval(spellContext.hit),

                    sourceAction = spellContext,
                }));
        }
    }
    
    public class SpellBuff
    {
        public BuffSO buff;
        public FloatModifier power = new(1.0f), auxPower = new(1.0f), crit = new(1.0f);

        public IEnumerator Do(RuntimeAction spellContext, Mob src, Mob tgt)
        {
            // TODO: Change to coroutine.
            Debug.LogError("Refactor this to coroutine and find better solutions than (power, auxPower).");
            
            Buff.Buff rbuff = (Buff.Buff)buff.Wrap(src.data);
            rbuff.power = dNumber.CreateComposite(power.Apply(spellContext.power));
            rbuff.auxPower = dNumber.CreateComposite(auxPower.Apply(spellContext.auxPower));
            rbuff.crit = dNumber.CreateComposite(crit.Apply(spellContext.crit));

            tgt.ReceiveBuff(rbuff);

            yield return -1;
        }
    }

    public class Summon<T> where T : Component
    {
        public T mobPrefab;

        public Mob Do(Vector2Int position, bool findEmpty = true)
        {
            if (findEmpty)
            {
                position = Globals.backend.FindNearestEmptyGrid(position);
            }
            
            var summoned = GameObject.Instantiate(mobPrefab.gameObject, Globals.backend.GridToWorldPos(position) + Vector2.one * 0.5f, Quaternion.identity).GetComponent<Mob>();

            return summoned;
        }
    }

    public class CreateGridEffect
    {
        public Buff.GridEffectSO effect;
        public GridShape shape;

        public IEnumerator Do(RuntimeAction spellContext, Mob src, Vector2Int targetShapeOrigin)
        {
            // TODO: Animations?
            
            shape.position = targetShapeOrigin;
            var grids = shape.ApplyTransform();
            GridEffect rfx =
                (Buff.GridEffect)effect.WrapFx(src.data, new Vector3(targetShapeOrigin.x, targetShapeOrigin.y, 0));
            
            foreach (var grid in grids)
            {
                rfx.Extend(grid);
            }

            yield return -1;
        }
    }

    public class PlayFx
    {
        public GameObject fx;

        public IEnumerator Do(Vector2 position)
        {
            yield return -1;
        }
    }
}