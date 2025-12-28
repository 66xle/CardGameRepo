using DG.Tweening;
using MyBox;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    [MustBeAssigned] [SerializeField] Image Image;
    [MustBeAssigned] [SerializeField] TMP_Text Title;
    [MustBeAssigned] [SerializeField] TMP_Text Description;
    [MustBeAssigned] [SerializeField] TMP_Text ButtonText;
    [MustBeAssigned] [SerializeField] GameObject Panel;
    [MustBeAssigned] [SerializeField] GameObject Logo;
    [MustBeAssigned] [SerializeField] GameObject ImageObj;

    public void DisplayTutorial(Sprite sprite, string title, string description, float delay, string buttonText = "Next")
    {
        Image.sprite = sprite;
        Title.text = title;
        Description.text = description;
        ButtonText.text = buttonText;

        ImageObj.SetActive(true);
        Logo.SetActive(false);

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            DOVirtual.DelayedCall(delay, () => OpenTutorial());
        }
    }

    public void DisplayLogo(string title, string description, float delay)
    {
        Title.text = title;
        Description.text = description;
        ImageObj.SetActive(false);
        Logo.SetActive(true);

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            DOVirtual.DelayedCall(delay, () => OpenTutorial());
        }
    }

    public void OpenTutorial()
    {
        Panel.SetActive(true);
    }

    public void CloseTutorial()
    {
        gameObject.SetActive(false);
        Panel.SetActive(false);
    }

    
}
