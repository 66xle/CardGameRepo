using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [HideInInspector] public List<Card> playerDeck;
    [HideInInspector] public List<Card> discardPile;
    [HideInInspector] public List<Card> enemyCardQueue;
}
