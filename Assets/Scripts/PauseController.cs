using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    [SerializeField] private Canvas _pauseCanvas = default;
    private PlayerInput _playerAction = default;
    private bool _isPaused = false;
    private const string TITLE_SCENE_NAME = "TitleScene";

    private void Awake()
    {
        //プレイヤーアクションのインスタンス
        _playerAction = new PlayerInput();

        //InputActionを有効化
        _playerAction?.Enable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _pauseCanvas.enabled = false;
    }

    // Update is called once per frame
    private void Update()
    {
        PauseMenu();
    }

    private void PauseMenu()
    {
        if (_playerAction.Player.Pause.WasPressedThisFrame())
        {
            if (!_isPaused)
            {
                Time.timeScale = 0;
                _pauseCanvas.enabled = true;
                _isPaused = true;
            }
            else if (_isPaused)
            {
                Time.timeScale = 1;
                _pauseCanvas.enabled = false;
                _isPaused = false;
            }
        }
    }

    public void ReturnGame()
    {
        Time.timeScale = 1;
        _pauseCanvas.enabled = false;
    }

    public void BackToTheTitle()
    {
        SceneManager.LoadScene(TITLE_SCENE_NAME);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif

    }

    private void OnDisable()
    {
        //InputActionを無効化
        _playerAction?.Disable();
    }

}
