using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerCharacter = Platformer.Mechanics.PlayerCharacter;
using LSG.Utilities;

using Damageable = Platformer.Module.Damageable;
using EGameMessege = InGame.EInGameID;
using ECommand = LSG.ECommand;

public partial class GameMainController : MonoBehaviour
{
    [SerializeField]
    PlayerBlackboard m_playerBlackboard;
    [SerializeField]
    PlayerCharacter m_player;
    [SerializeField]
    Transform m_startingPoint;
    private void Awake()
    {
        m_playerBlackboard.player = m_player.transform;
        m_player.gameObject.SetActive(false);
    }

    private void Start()
    {
        Messenger<EGameMessege>.Initialize();
        Messenger<EGameMessege>.SendMessage(EGameMessege.HP, ECommand.ACTIVATE_WITH_PARAM, m_player.damageable.startingHealth);
        if (m_startingPoint)
            m_player.transform.position = m_startingPoint.position;
        m_player.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        Messenger<EGameMessege>.Dispose();
    }

    //private IEnumerator Start()
    //{

    //    AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("CastleScene", UnityEngine.SceneManagement.LoadSceneMode.Additive);
    //    yield return async;
    //    m_player.gameObject.SetActive(true);
    //}

}
