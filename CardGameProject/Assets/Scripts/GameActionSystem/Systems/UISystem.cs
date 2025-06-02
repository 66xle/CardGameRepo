using DG.Tweening;
using MyBox;
using System.Collections;
using TMPro;
using UnityEngine;

public class UISystem : MonoBehaviour
{
    [Header("References")]
    [MustBeAssigned] [SerializeField] CombatUIManager CombatUIManager;
    [MustBeAssigned] [SerializeField] Camera MainCamera;

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

        Vector3 spawnPos = new Vector3(avatar.transform.position.x + Random.Range(-CombatUIManager.RandomOffsetHorizontal, CombatUIManager.RandomOffsetHorizontal),
                                                  avatar.transform.position.y + CombatUIManager.OffsetVertical,
                                                  avatar.transform.position.z + Random.Range(-CombatUIManager.RandomOffsetHorizontal, CombatUIManager.RandomOffsetHorizontal));

        popupObj.transform.position = spawnPos;


        TextMeshProUGUI popupText = popupObj.GetComponent<TextMeshProUGUI>();
        popupText.text = spawnDamageUIPopupGA.Damage.ToString();
        popupText.color = spawnDamageUIPopupGA.Color;


        float distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        float scaleFactor = (CombatUIManager.baseScale / 10f) / distance;
        Vector3 endOffset = Vector3.up * CombatUIManager.MoveVertical * scaleFactor;
        Vector3 targetPos = spawnPos + endOffset;

        Tween tween = DOTween.To(() => 0f, t => {
            popupText.transform.localScale = Vector3.one * scaleFactor;
            popupText.transform.position = Vector3.Lerp(spawnPos, targetPos, t / CombatUIManager.MoveDuration);
        }, CombatUIManager.MoveDuration, CombatUIManager.MoveDuration).SetEase(Ease.OutQuad);

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
