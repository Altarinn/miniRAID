# Some Examples on Spells

## AoE

```C#
public class SimpleDirectionalAoE : ActionDataSO<FourDirectionalTarget>
{
    [SerializeField] private GridShape shape;
    [SerializeField] private UnitFilters filter;
    [SerializeField] private SpellDamageHeal damageOrHeal;
    [SerializeField] private SpellBuff buff;
    [SerializeField] private SimpleExplosionFx fx;

    public override GridShape MainShape => shape;

    public override Dictionary<string, object> LazyPrepareTooltipVariables(RuntimeAction ract)
    {
        var vars = base.LazyPrepareTooltipVariables(ract);
        vars.Add("HitPower", damageOrHeal.GetPower(ract));

        return vars;
    }

    public override IEnumerator OnPerform(RuntimeAction<FourDirectionalTarget> ract, MobData mob,
        FourDirectionalTarget target)
    {
        shape.position = mob.Position;
        shape.direction = target.Target;

        var capturedTargets = CaptureTargetsInGridShape.CaptureAllTargetsWithinRange(
            mob, filter, shape.ApplyTransform());

        yield return new JumpIn(fx.Do(mob.Position + 2 * Consts.DirectionVectors[(int)shape.direction]));

        yield return new JumpIn(MobListHelpers.WaitForAllMobs(
            capturedTargets,
            targetMob => JumpInHelper.Chain(
                damageOrHeal.Do(ract, mob, targetMob),
                buff.Do(ract, mob, targetMob)
            )
        ));
    }
}
```

