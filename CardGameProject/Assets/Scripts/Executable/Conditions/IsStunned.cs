using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsStunned : Condition
{ 
    public override bool Evaluate()
    {
        List<Avatar> currentTargets = ExecutableParameters.Targets;

        for (int i = 0; i < ExecutableParameters.Targets.Count; i++)
        {
            Avatar avatar = ExecutableParameters.Targets[i];

            if (!avatar.isGuardBroken())
            {
                ExecutableParameters.Targets.Remove(avatar);
            }
        }

        if (ExecutableParameters.Targets.Count > 0)
            return true;

        return false;
    }
}
