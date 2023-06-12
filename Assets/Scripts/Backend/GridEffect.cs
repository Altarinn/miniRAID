using UnityEngine;
using System.Collections;

namespace miniRAID
{
    //public abstract class GridEffect
    //{
    //    public enum InvokeTiming
    //    {
    //        Passive,        // Will not be invoked
    //        Manual,         // Will be invoked manually; currently no difference with Passive except a warning.
    //        PhaseStart,     // At the beginning of specified phase, by GridEffect.invokePhase
    //        EveryPhaseStart,// Invokes at the beginning of every phase
    //        BeSleptOn,      // Invokes when a mob has "slept" on the grid
    //        BeAwakenOn,     // Invokes when a mob has "awake" on the grid
    //        BeStartedTurnOn,// Invokes when a mob starts its turn on the grid
    //        BePassedByOnce, // Invokes when a mob's moving path covers the grid, or be "slept" on, but maximum once / turn
    //        BePassedBy,     // Invokes when a mob's moving path covers the grid, or be "slept" on, with no limitaions on invoke times
    //    }

    //    public bool stackable
    //    {
    //        get;
    //        protected set;
    //    }

    //    public int stacks
    //    {
    //        get;
    //        set;
    //    }

    //    public Mob source;
    //    protected string name;

    //    public static GridEffect PlaceAt<T>(int x, int y, Mob source) where T : GridEffect, new()
    //    {
    //        // Initialization
    //        GridEffect e = new T();
    //        e.source = source;

    //        GridData grid = Globals.backend.getMap(x, y);

    //        if (grid.effects.ContainsKey(e))
    //        {
    //            GridEffect original_e = grid.effects[e];
    //            // TODO: Rewrite as a Stack() method instead of simply +1
    //            if (original_e.stackable)
    //            {
    //                original_e.stacks += 1;
    //            }

    //            return original_e;
    //        }

    //        e.Init();
    //        grid.effects.Add(e, e);
    //        return e;
    //    }

    //    protected GridEffect()
    //    {
    //        name = this.GetType().Name;
    //    }

    //    public virtual void Init() { }

    //    public override string ToString()
    //    {
    //        return $"GFx:{source.name}-{name}";
    //    }

    //    public override bool Equals(object obj)
    //    {
    //        return (obj is GridEffect) && (name == ((GridEffect)obj).name) && (source = ((GridEffect)obj).source);
    //    }

    //    public override int GetHashCode()
    //    {
    //        return name.GetHashCode() ^ source.GetHashCode();
    //    }

    //    public abstract void Invoke(Vector3Int pos, GridData grid);
    //}
}
