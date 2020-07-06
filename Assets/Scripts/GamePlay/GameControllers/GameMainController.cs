using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Mechanics.AI;

public class GameMainController : MonoBehaviour
{
    [SerializeField]
    PlayerBlackboard m_playerBlackboard;
    [SerializeField]
    PlayerCharacter m_player;

    private void Awake()
    {
        m_playerBlackboard.player = m_player.transform;
    }
}
