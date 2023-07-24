using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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

        public static MobData MobAtGrid(Vector3Int pos)
        {
            return Globals.backend.GetMap(pos.x, pos.y, pos.z)?.mob;
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

    [ColoredBox("#f77")]
    public class SpellDamageHeal
    {
        public Consts.DamageHealFlags flags;
        
        public Consts.Elements type;

        public FloatModifier power = new(1.0f);
        public FloatModifier crit = new(1.0f);
        public FloatModifier hit = new(1.0f);

        [Serializable]
        public class AdvancedSettings
        {
            public float aggroMul = 1.0f;
        }

        [SerializeField]
        public AdvancedSettings advancedSettings;

        public IEnumerator DoDamageHeal(RuntimeAction spellContext, Buff.Buff buffContext, MobData src, MobData tgt)
        {
            if (advancedSettings != null)
            {
                src.aggroMul.MulMul(dNumber.CreateStatic(advancedSettings.aggroMul, "Damage Aggro Multiplier"));
            }

            var input = new Consts.DamageHeal_FrontEndInput
            {
                source = src,
                type = type,

                sourceAction = spellContext,
                sourceBuff = buffContext,

                flags = flags,
            };

            if (input.IsAction)
            {
                input.value = power.Apply(spellContext.power);
                input.crit = crit.Apply(spellContext.crit);
                input.hit = hit.Apply(spellContext.hit);
            }
            else
            {
                input.value = power.Apply(buffContext.power);
                input.crit = crit.Apply(buffContext.crit);
                input.hit = hit.Apply(buffContext.hit);
            }
            
            yield return new JumpIn(Globals.backend.DealDmgHeal(tgt, input));

            if (advancedSettings != null)
            {
                src.aggroMul.MulMul(dNumber.CreateStatic(1.0f / advancedSettings.aggroMul, "Damage Aggro Multiplier"));
            }
        }

        public IEnumerator Do(RuntimeAction spellContext, MobData src, MobData tgt)
        {
            yield return new JumpIn(DoDamageHeal(spellContext, null, src, tgt));
        }
        
        public IEnumerator Do(Buff.Buff buffContext, MobData src, MobData tgt)
        {
            yield return new JumpIn(DoDamageHeal(null, buffContext, src, tgt));
        }
    }
    
    [ColoredBox("#bf7")]
    public class SpellBuff
    {
        public BuffSO buff;
        public bool inheritLevel = true;
        public bool inheritPower = true;
        
        [ShowIf("inheritPower")] 
        public FloatModifier power = new(1.0f);
        
        [ShowIf("inheritPower")]
        public FloatModifier auxPower = new(1.0f);
        
        [ShowIf("inheritPower")]
        public FloatModifier crit = new(1.0f);

        public IEnumerator Do(RuntimeAction spellContext, MobData src, MobData tgt)
        {
            // TODO: Change to coroutine.
            Debug.LogError("Refactor this to coroutine and find better solutions than (power, auxPower).");
            
            Buff.Buff rbuff = (Buff.Buff)buff.LeveledWrap(src, inheritLevel ? spellContext.level : 0);

            if (inheritPower)
            {
                rbuff.power = dNumber.CreateComposite(power.Apply(spellContext.power));
                rbuff.auxPower = dNumber.CreateComposite(auxPower.Apply(spellContext.auxPower));
                rbuff.crit = dNumber.CreateComposite(crit.Apply(spellContext.crit));
            }

            tgt.AddBuff(rbuff);

            yield return -1;
        }
    }

    // TODO: Make me to MobData-based.
    [ColoredBox("#bf7")]
    public class Summon<T> where T : Component
    {
        public T mobPrefab;

        // TODO: FIXME: Problematic
        public MobRenderer Do(Vector3Int position, bool findEmpty = true)
        {
            if (findEmpty)
            {
                position = Globals.backend.FindNearestEmptyGrid(position);
            }
            
            var summoned = GameObject.Instantiate(mobPrefab.gameObject, Globals.backend.GridToWorldPos(position) + Vector3.one * 0.5f, Quaternion.identity).GetComponent<MobRenderer>();

            return summoned;
        }
    }

    [ColoredBox("#bf7")]
    public class CreateGridEffect
    {
        public Buff.GridEffectSO effect;
        public GridShape shape;

        public bool inheritLevel = true;

        public IEnumerator Do(RuntimeAction spellContext, MobData src, Vector3Int targetShapeOrigin)
        {
            // TODO: Animations?
            
            shape.position = targetShapeOrigin;
            var grids = shape.ApplyTransform();
            GridEffect rfx =
                (Buff.GridEffect)effect.LeveledWrapFx(
                    src, 
                    inheritLevel ? spellContext.level : 1,
                    new Vector3(targetShapeOrigin.x, targetShapeOrigin.y, 0));
            
            foreach (var grid in grids)
            {
                rfx.Extend(grid);
            }

            yield return -1;
        }
    }

    [ColoredBox("#7fd")]
    public class UnitFilters
    {
        public bool toEnemy;
        public bool toAlly;

        public bool Check(MobData source, MobData target)
        {
            bool flag = false;
            
            if (toEnemy)
            {
                flag |= Consts.ApplyMask(Consts.EnemyMask(source.unitGroup), target.unitGroup);
            }

            if (toAlly)
            {
                flag |= Consts.ApplyMask(Consts.AllyMask(source.unitGroup), target.unitGroup);
            }

            return flag;
        }
    }

    [ColoredBox("#ff7")]
    public class SimpleExplosionFx
    {
        public Sprite image;
        public float size = 200.0f;
        public float triggerTime = 0.3f;
        public float time = 0.5f;

        public IEnumerator Do(Vector3 position)
        {
            if (Globals.cc.animation)
            {
                GameObject obj = new GameObject("explosion");
                obj.transform.position = new Vector3(position.x + 0.5f, position.y + 0.5f, position.z + 0.5f);
                var sr = obj.AddComponent<SpriteRenderer>();
                sr.sprite = image;

                sr.DOColor(Color.clear, time);
                DOTween.Sequence()
                    .Append(obj.transform.DOScale(Vector3.one * size, time)
                        .OnComplete(() => {GameObject.Destroy(obj);}))
                    .SetLink(obj);
                
                yield return new WaitForSeconds(triggerTime);
            }
            
            yield return -1;
        }

        public IEnumerator Do(Vector3Int gridPosition) => Do(Globals.backend.GridToWorldPos(gridPosition));
    }

    [ColoredBox("#ff7")]
    public class ShowImportantMessage
    {
        public string message;
        public float seconds;
        
        public IEnumerator Do()
        {
            if (Globals.cc.animation)
            {
                Globals.debugMessage.AddMessage($"!! {message} !!");
                yield return new JumpIn(Globals.ui.Instance.combatView.ShowImportantText(message, seconds));
                // Globals.ui.Instance.combatView.ShowImportantText(message);
                // yield return new WaitForSeconds(seconds);
                // Globals.ui.Instance.combatView.HideImportantText();
            }
        }
    }
}