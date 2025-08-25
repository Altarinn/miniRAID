using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using miniRAID.Backend.Numericals;
using miniRAID.Buff;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using Object = System.Object;

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
        [PathBell]
        public float value;
        
        [HorizontalGroup]
        [ShowIf("type",FloatModifierType.Expression)]
        [HideLabel]
        public ValueGetter<float, float> expression;

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

            public AdvancedSettings(AdvancedSettings settings)
            {
                aggroMul = settings.aggroMul;
            }
        }

        [SerializeField]
        public AdvancedSettings advancedSettings;
        
        public SpellDamageHeal(SpellDamageHeal damageHeal)
        {
            flags = damageHeal.flags;
            type = damageHeal.type;
            power = damageHeal.power;
            crit = damageHeal.crit;
            hit = damageHeal.hit;
            advancedSettings = damageHeal.advancedSettings == null ? null : new AdvancedSettings(damageHeal.advancedSettings);
        }

        /// <summary>
        /// Directly get the damage/heal info.
        /// Note that advanced settings will not be applied.
        /// </summary>
        /// <param name="spellContext">Source spell</param>
        /// <param name="buffContext">Source buff (either spellContext or buffContext should be non-null)</param>
        /// <param name="src">The mob to cast this action</param>
        /// <returns></returns>
        public Consts.DamageHeal_FrontEndInput GetInfo(RuntimeAction spellContext, Buff.Buff buffContext, MobData src)
        {
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

            return input;
        }

        public IEnumerator DoDamageHeal(RuntimeAction spellContext, Buff.Buff buffContext, MobData src, MobData tgt)
        {
            if (advancedSettings != null)
            {
                src.aggroMul.MulMul(dNumber.CreateStatic(advancedSettings.aggroMul, "Damage Aggro Multiplier"));
            }

            var input = GetInfo(spellContext, buffContext, src);
            
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

        public int GetPower(RuntimeAction spell) => Mathf.CeilToInt(power.Apply(spell.power));
        public int GetPower(Buff.Buff buff) => Mathf.CeilToInt(power.Apply(buff.power));
    }
    
    [ColoredBox("#bf7")]
    public class SpellBuff
    {
        public BuffSO buff;
        public bool inheritLevel = true;
        public bool inheritPower = true;
        
        [Range(1, 10)]
        public int stacks = 1;
        
        [ShowIf("inheritPower")]
        public FloatModifier power = new(1.0f);
        
        [ShowIf("inheritPower")]
        public FloatModifier auxPower = new(1.0f);
        
        [ShowIf("inheritPower")]
        public FloatModifier crit = new(1.0f);

        public IEnumerator Do(RuntimeAction spellContext, MobData src, MobData tgt)
        {
            if (buff == null)
            {
                yield break;
            }

            // TODO: Change to coroutine.
            Debug.LogError("Refactor this to coroutine and find better solutions than (power, auxPower).");
            
            Buff.Buff rbuff = (Buff.Buff)buff.LeveledWrap(src, inheritLevel ? spellContext.level : 0);

            if (inheritPower)
            {
                rbuff.power = dNumber.CreateComposite(power.Apply(spellContext.power));
                rbuff.auxPower = dNumber.CreateComposite(auxPower.Apply(spellContext.auxPower));
                rbuff.crit = dNumber.CreateComposite(crit.Apply(spellContext.crit));
            }

            stacks = Mathf.Max(1, stacks);
            for(int i = 0; i < stacks; ++i)
                tgt.AddBuff(rbuff);

            yield return -1;
        }
        
        public int GetPower(RuntimeAction spell) => Mathf.CeilToInt(power.Apply(spell.power));
    }

    [Serializable]
    public enum SummonUnitGroupType
    {
        SameTeam,
        EnemyTeam,
        AllyTeam,
        Others,
        FixedPlayer,
        FixedEnemy,
        FixedAlly,
        FixedOthers
    }
    
    // TODO: Make me to MobData-based.
    [ColoredBox("#bf7")]
    public class SummonMob
    {
        public BaseMobDescriptorSO mobDescriptor;
        public SummonUnitGroupType groupChoice;

        public Vector3Int? FindValidGrid(Vector3Int center)
        {
            var position = Globals.backend.FindNearestEmptyGrid(center, mobDescriptor.gridBody,
                x => mobDescriptor.movement.CanEndTurnAt(x, null));
            
            if (position == -Vector3.one)
            {
                return null;
            }
            
            return position;
        }
        
        public MobData Do(MobData source, Vector3Int position, bool findEmpty = true)
        {
            if (findEmpty)
            {
                var p = FindValidGrid(position);
                if (!p.HasValue) return null;
                position = p.Value;
            }

            Consts.UnitGroup sourceGroup = source?.unitGroup ?? Consts.UnitGroup.Player;
            Consts.UnitGroup summonedGroup;
            
            switch (groupChoice)
            {
                case SummonUnitGroupType.SameTeam:
                    summonedGroup = sourceGroup;
                    break;
                default:
                    throw new NotImplementedException();
                    break;
            }

            var summoned = mobDescriptor.Wrap(position, Consts.Direction.Up, summonedGroup);
            return summoned;
        }
    }

    [ColoredBox("#bf7")]
    public class CreateGridEffect
    {
        public Buff.GridEffectSO effect;
        public GridCollider shape;
        
        [InfoBox("Put a MovementSO here to add grounding constraint, e.g., only apply effect to grids that a walking mob can end turn on. If null, ignore grounding constraint.")]
        public MovementSO OnlyGroundedAs;

        public bool inheritLevel = true;

        public IEnumerator Do(RuntimeAction spellContext, MobData src, Vector3 targetShapeOrigin)
        {
            // TODO: Animations?
            GridCollider collider = shape.CloneWithNewGuid();
            collider.Position = targetShapeOrigin;

            if (OnlyGroundedAs != null)
            {
                var validGrids = collider
                    .Where(x => OnlyGroundedAs.CanEndTurnAt(x, null));

                collider = new EnumerateGridCollider(new GridShape(validGrids));
            }
            
            GridEffect rfx =
                (Buff.GridEffect)effect.LeveledWrapFx(
                    src,
                    inheritLevel ? spellContext.level : 1,
                    collider);
            
            src.AddListener(rfx);

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

    [ColoredBox("#fa7")]
    public class KnockBack
    {
        public bool isInstant = false;
        
        public IEnumerator Do(MobData mob, Vector3Int knockback)
        {
            Vector3Int newPos = mob.GridPosition + knockback;
            
            // TODO: Check newPos validity
            if (Globals.backend.GetMap(newPos).mob != null)
            {
                yield break;
            }
            
            if ((!isInstant) && Globals.cc.animation)
            {
                // TODO: Animation?
                yield return new JumpIn(mob.SetPosition(newPos));
            }
            else
            {
                yield return new JumpIn(mob.SetPosition(newPos));
            }
        }
    }
}