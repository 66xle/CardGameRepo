using System.Collections.Generic;
using MyBox;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<EnemyData> Enemies = new();

    private void Awake()
    {
        if (Enemies.Count == 0)
        {
            Debug.LogAssertion("No Enemy in List.");
        }
    }
}
