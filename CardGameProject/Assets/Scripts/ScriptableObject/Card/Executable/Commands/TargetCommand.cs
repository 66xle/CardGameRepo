using DG.Tweening;
using events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class TargetCommand : Command
{
    protected abstract List<Avatar> GetTargets();

    public abstract override IEnumerator Execute(Action<bool> onComplete);

    
}
