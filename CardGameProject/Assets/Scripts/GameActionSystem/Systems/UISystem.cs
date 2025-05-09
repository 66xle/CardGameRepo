using DG.Tweening;
using MyBox;
using System.Collections;
using TMPro;
using UnityEngine;

public class UISystem : MonoBehaviour
{
    [MustBeAssigned] public CombatUIManager CombatUIManager;

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<SpawnDamageUIPopupGA>(SpawnDamageUIPopupPerformer);
        ActionSystem.AttachPerformer<TogglePlayerUIGA>(TogglePlayerUIPerformer);
        ActionSystem.AttachPerformer<ToggleEnemyUIGA>(ToggleEnemyUIPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<SpawnDamageUIPopupGA>();
        ActionSystem.DetachPerformer<TogglePlayerUIGA>();
        ActionSystem.DetachPerformer<ToggleEnemyUIGA>();
    }

    private IEnumerator SpawnDamageUIPopupPerformer(SpawnDamageUIPopupGA spawnDamageUIPopupGA)
    {
        Avatar avatar = spawnDamageUIPopupGA.AvatarTakingDamage;

        GameObject popupObj = Instantiate(CombatUIManager.DamagePopupPrefab, CombatUIManager.WorldSpaceCanvas);
        popupObj.transform.position = new Vector3(avatar.transform.position.x + Random.Range(-CombatUIManager.RandomOffsetHorizontal, CombatUIManager.RandomOffsetHorizontal),
                                                  avatar.transform.position.y + CombatUIManager.OffsetVertical,
                                                  avatar.transform.position.z + Random.Range(-CombatUIManager.RandomOffsetHorizontal, CombatUIManager.RandomOffsetHorizontal));
        Vector3 moveToPos = popupObj.transform.position;
        moveToPos.y += 1f;

        TextMeshProUGUI popupText = popupObj.GetComponent<TextMeshProUGUI>();
        popupText.text = spawnDamageUIPopupGA.Damage.ToString();
        popupText.color = spawnDamageUIPopupGA.Color;

        Tween tween = popupObj.transform.DOMoveY(popupObj.transform.position.y + CombatUIManager.MoveVertical, CombatUIManager.MoveDuration).SetEase(Ease.OutQuad);

        yield return tween.WaitForCompletion();

        popupText.DOFade(0, CombatUIManager.FadeDuration).OnComplete(() => { Destroy(popupObj); });
    }

    private IEnumerator TogglePlayerUIPerformer(TogglePlayerUIGA togglePlayerUIGA)
    {
        CombatUIManager.PlayerUI.SetActive(togglePlayerUIGA.Toggle);

        yield return null;
    }

    private IEnumerator ToggleEnemyUIPerformer(ToggleEnemyUIGA toggleEnemyUIGA)
    {
        CombatUIManager.DetailedUI.SetActive(toggleEnemyUIGA.Toggle);

        yield return null;
    }
}
