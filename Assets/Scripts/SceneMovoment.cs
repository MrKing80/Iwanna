using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMovoment : MonoBehaviour
{
    private PlayerInput _playerAction = default;
    private string _currentSceneName = "";
    private const string STAGE_SCENE_NAME = "StageScene";
    private const string TITLE_SCENE_NAME = "TitleScene";
    private void Awake()
    {
        //プレイヤーアクションのインスタンス
        _playerAction = new PlayerInput();

        //InputActionを有効化
        _playerAction?.Enable();
    }

    private void Start()
    {
        _currentSceneName = SceneManager.GetActiveScene().name;
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
            if (_currentSceneName == TITLE_SCENE_NAME)
            {
                SaveManager._saveManagerInstance.SavePoint = Vector2.zero;
                SceneManager.LoadScene(STAGE_SCENE_NAME);
            }
            else
            {
                SceneManager.LoadScene(TITLE_SCENE_NAME);
            }
        }
    }
}
