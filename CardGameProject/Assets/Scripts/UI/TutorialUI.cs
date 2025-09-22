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
    [MustBeAssigned][SerializeField] TMP_Text ButtonText;

    public void DisplayTutorial(Sprite sprite, string title, string description, string buttonText = "Next")
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        Image.sprite = sprite;
        Title.text = title;
        Description.text = description;
        ButtonText.text = buttonText;
    }

    public void CloseTutorial()
    {
        gameObject.SetActive(false);
    }

    
}
