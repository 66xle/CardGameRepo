using MyBox;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    [MustBeAssigned] public UIManager UIManager;

    public void ClaimGear()
    {
        UIManager.NextScene();
    }
}
