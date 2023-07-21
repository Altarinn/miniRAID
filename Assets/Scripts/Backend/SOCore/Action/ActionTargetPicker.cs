using System;
using System.Collections.Generic;
using System.Linq;
using miniRAID.ActionHelpers;
using miniRAID.Buff;
using miniRAID.Extensions;
using miniRAID.Spells;
using UnityEngine;

namespace miniRAID.Extensions
{
    public static class EnumerableExtensions
    {
        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> selector)
        {
            return source.MinBy(selector, null);
        }

        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");
            comparer ??= Comparer<TKey>.Default;

            using (var sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                {
                    throw new InvalidOperationException("Sequence contains no elements");
                }
                var min = sourceIterator.Current;
                var minKey = selector(min);
                while (sourceIterator.MoveNext())
                {
                    var candidate = sourceIterator.Current;
                    var candidateProjected = selector(candidate);
                    if (comparer.Compare(candidateProjected, minKey) < 0)
                    {
                        min = candidate;
                        minKey = candidateProjected;
                    }
                }
                return min;
            }
        }
        
        public static T RandomChoice<T>(this IEnumerable<T> source)
        {
            return source.ElementAt(Globals.cc.rng.NextInt() % source.Count());
        }
    }
}

namespace miniRAID.Weapon
{
    public abstract class ActionTargetPickerBase
    {
        public abstract SpellTarget Pick(MobData source, RuntimeAction ract);
    }
    
    public class FollowLastTurnPicker : ActionTargetPickerBase
    {
        public override SpellTarget Pick(MobData source, RuntimeAction ract)
        {
            if (source.lastTurnTarget == null)
            {
                return null;
            }
            
            return new SpellTarget(source.lastTurnTarget.Position);
        }
    }
    
    public class SelfPicker : ActionTargetPickerBase
    {
        public override SpellTarget Pick(MobData source, RuntimeAction ract)
        {
            return new SpellTarget(source.Position);
        }
    }
    
    // TODO: Heal priority?
    public class HealerPicker : ActionTargetPickerBase
    {
        [SerializeField] private UnitFilters unitFilter;
        [SerializeField] private MobListenerSO excludeMobWithListener;
        [SerializeField] private bool lowestHealthFirst = true;

        public override SpellTarget Pick(MobData source, RuntimeAction ract)
        {
            MobData target = null;
            var targets = Globals.backend.allMobs
                .Where(x => !x.isDead)
                .Where(x => unitFilter.Check(source, x))
                .Where(x => x.FindListener(excludeMobWithListener) == null)
                .Where(x => ract.data.CheckWithTargets(source, new SpellTarget(x.Position)));

            // No valid targets
            if (!targets.Any())
            {
                return null;
            }

            if (lowestHealthFirst)
            {
                target = targets.MinBy(m => GetHealPriority(source, m));
            }
            else
            {
                target = targets.RandomChoice();
            }

            return new SpellTarget(target.Position);
        }

        private float GetHealPriority(MobData source, MobData mob)
        {
            float healP = mob.healPriority;
            if (mob == source && mob.health < mob.maxHealth * Consts.HealerSelfFocusThresholdHPPercentage)
            {
                healP *= Consts.HealerSelfFocusPriorityBoost;
            }

            return (
                (float)mob.health / ((float)mob.maxHealth * healP) // % of max HP, while focusing on self more when in danger
                - 0.01f * mob.healPriority); // Focus on high priority targets when at full HP (or rarely, same %)
        }
    }
}