using System.Collections.Generic;
using UnityEngine;

public class AvatarSpawnPosition : MonoBehaviour
{
    public List<Transform> EnemySpawnPositionList;
    public Transform PlayerSpawnPosition;

    public void Awake()
    {
        ServiceLocator.Register(this);
    }

    public void OnDestroy()
    {
        ServiceLocator.Unregister<AvatarSpawnPosition>();
    }
}
