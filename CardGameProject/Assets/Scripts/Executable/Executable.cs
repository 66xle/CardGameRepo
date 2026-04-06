using System;
using System.Collections;

public enum CardTarget
{
    PreviousTarget,
    Enemy,
    AllEnemies,
    Self,
    AllAllies
}




[Serializable]
public abstract class Executable
{
    public virtual float Value { get; }
    public virtual bool IsUsingValue => false;

    public virtual bool RequiresMovement => false;
    public virtual bool IsReactiveCondition => false;
    public virtual CardTarget CardTarget => CardTarget.Enemy;

    public abstract IEnumerator Execute(Action<bool> IsConditionTrue);


}
