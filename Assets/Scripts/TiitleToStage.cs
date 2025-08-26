using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TiitleToStage : MonoBehaviour
{
    private PlayerInput _playerAction = default;
    private const string NEXT_SCENE_NAME = "StageScene";
    private void Awake()
    {
        //プレイヤーアクションのインスタンス
        _playerAction = new PlayerInput();

        //InputActionを有効化
        _playerAction?.Enable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnDisable()
    {
        _playerAction.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerAction.Player.Attack.WasPressedThisFrame())
        {
            SceneManager.LoadScene(NEXT_SCENE_NAME);
        }
    }
}
