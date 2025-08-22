using MyBox;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAudioType : MonoBehaviour
{
    [MustBeAssigned] [SerializeField] AudioType AudioType = AudioType.None;
    private Button Button;

    public void Awake()
    {
        if (AudioType == AudioType.None) Debug.LogError($"Didn't set Audio Type: '{gameObject.name}'");

        Button = GetComponent<Button>();

        if (Button == null) Debug.LogError($"Button component doesn't exist: '{gameObject.name}'" );

        Button.onClick.AddListener(() => AudioManager.Instance.PlaySound(AudioType));
    }
}
