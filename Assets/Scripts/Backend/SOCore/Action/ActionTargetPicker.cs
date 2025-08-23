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
    public abstract class ActionTargetPickerBase<TSpellTarget> where TSpellTarget : SpellTarget
    {
        public abstract TSpellTarget Pick(MobData source, RuntimeAction<TSpellTarget> ract);
    }
    
    // This won't work for e.g. Grimoires.
    // TODO: Move the logic to MobData / Weapon in a better way?
    // Or change to a enemy-priority + target-mob system with each mob favoring their previously attacked mobs?
    public class FollowLastPicker<TSpellTarget> : ActionTargetPickerBase<TSpellTarget>
        where TSpellTarget : SpellTarget
    {
        public override TSpellTarget Pick(MobData source, RuntimeAction<TSpellTarget> ract)
        {
            if (ract.LastTarget is not { Valid: true })
            {
                return null;
            }
            
            return ract.LastTarget;
        }
    }
    
    public class SelfPicker : ActionTargetPickerBase<SingleMobTarget>
    {
        public override SingleMobTarget Pick(MobData source, RuntimeAction<SingleMobTarget> ract)
        {
            return new SingleMobTarget(source, source.GridPosition);
        }
    }
    
    // TODO: Heal priority?
    public class HealerSingleMobPicker : ActionTargetPickerBase<SingleMobTarget>
    {
        [SerializeField] private UnitFilters unitFilter;
        [SerializeField] private MobListenerSO excludeMobWithListener;
        [SerializeField] private bool lowestHealthFirst = true;
        [SerializeField] private bool captureFullHealthTargets = false;

        public override SingleMobTarget Pick(MobData source, RuntimeAction<SingleMobTarget> ract)
        {
            SingleMobTarget target = null;
            var targets = Globals.backend.allMobs
                .Where(x => !x.isDead)
                .Where(x => unitFilter.Check(source, x))
                .Where(x => excludeMobWithListener == null || x.FindListener(excludeMobWithListener) == null)
                .Where(x => captureFullHealthTargets || x.health < x.maxHealth)
                .Select(x => ValidTargetFinder.FindValidSingleMobTarget(source, x, ract))
                .Where(x => x != null)
                .ToList();

            // No valid targets
            if (!targets.Any())
            {
                return null;
            }

            if (lowestHealthFirst)
            {
                target = targets.MinBy(m => Consts.GetPrioritizedHealthRatio(source, m.Target));
            }
            else
            {
                target = targets.RandomChoice();
            }

            return target;
        }
    }
}