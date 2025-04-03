using UnityEngine;

public class SpawnDamageUIPopupGA : GameAction
{
    public CombatUIManager combatUIManager;
    public Avatar avatarTakingDamage;
    public float damage;
    public Color color;

    public SpawnDamageUIPopupGA(CombatUIManager combatUIManager, Avatar avatarTakingDamage, float damage, Color color)
    {
        this.combatUIManager = combatUIManager;
        this.avatarTakingDamage = avatarTakingDamage;
        this.damage = damage;
        this.color = color;
    }
}
