using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class UISystem : MonoBehaviour
{
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
        CombatUIManager UIManager = spawnDamageUIPopupGA.combatUIManager;

        GameObject popupObj = Instantiate(UIManager.damagePopupPrefab, UIManager.worldSpaceCanvas);
        popupObj.transform.position = new Vector3(avatar.transform.position.x + Random.Range(-UIManager.randomOffsetHorizontal, UIManager.randomOffsetHorizontal),
                                                  avatar.transform.position.y + UIManager.offsetVertical,
                                                  avatar.transform.position.z + Random.Range(-UIManager.randomOffsetHorizontal, UIManager.randomOffsetHorizontal));
        Vector3 moveToPos = popupObj.transform.position;
        moveToPos.y += 1f;

        TextMeshProUGUI popupText = popupObj.GetComponent<TextMeshProUGUI>();
        popupText.text = spawnDamageUIPopupGA.damage.ToString();
        popupText.color = spawnDamageUIPopupGA.color;

        Tween tween = popupObj.transform.DOMoveY(popupObj.transform.position.y + UIManager.moveVertical, UIManager.moveDuration).SetEase(Ease.OutQuad);

        yield return tween.WaitForCompletion();

        popupText.DOFade(0, UIManager.fadeDuration).OnComplete(() => { Destroy(popupObj); });
    }

}
