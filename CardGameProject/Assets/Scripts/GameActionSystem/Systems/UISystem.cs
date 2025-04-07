using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class UISystem : MonoBehaviour
{
    public CombatUIManager CombatUIManager;

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<SpawnDamageUIPopupGA>(SpawnDamageUIPopupPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<SpawnDamageUIPopupGA>();
    }

    private IEnumerator SpawnDamageUIPopupPerformer(SpawnDamageUIPopupGA spawnDamageUIPopupGA)
    {
        Avatar avatar = spawnDamageUIPopupGA.avatarTakingDamage;

        GameObject popupObj = Instantiate(CombatUIManager.damagePopupPrefab, CombatUIManager.worldSpaceCanvas);
        popupObj.transform.position = new Vector3(avatar.transform.position.x + Random.Range(-CombatUIManager.randomOffsetHorizontal, CombatUIManager.randomOffsetHorizontal),
                                                  avatar.transform.position.y + CombatUIManager.offsetVertical,
                                                  avatar.transform.position.z + Random.Range(-CombatUIManager.randomOffsetHorizontal, CombatUIManager.randomOffsetHorizontal));
        Vector3 moveToPos = popupObj.transform.position;
        moveToPos.y += 1f;

        TextMeshProUGUI popupText = popupObj.GetComponent<TextMeshProUGUI>();
        popupText.text = spawnDamageUIPopupGA.damage.ToString();
        popupText.color = spawnDamageUIPopupGA.color;

        Tween tween = popupObj.transform.DOMoveY(popupObj.transform.position.y + CombatUIManager.moveVertical, CombatUIManager.moveDuration).SetEase(Ease.OutQuad);

        yield return tween.WaitForCompletion();

        popupText.DOFade(0, CombatUIManager.fadeDuration).OnComplete(() => { Destroy(popupObj); });
    }

}
