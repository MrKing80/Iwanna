using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ポーズメニューの表示・制御を行うスクリプト
/// ・ポーズ入力でゲームを停止 / 再開
/// ・メニューからタイトルに戻る / 終了する
/// </summary>
public class PauseController : MonoBehaviour
{
    [SerializeField] private Canvas _pauseCanvas = default; // ポーズ画面用キャンバス
    private PlayerInput _playerAction = default;            // 新Input Systemの入力管理
    private bool _isPaused = false;                         // ポーズ中かどうかを判定

    private const string TITLE_SCENE_NAME = "TitleScene";   // タイトルシーン名

    private void Awake()
    {
        // プレイヤーアクションのインスタンス生成
        _playerAction = new PlayerInput();

        // InputActionを有効化
        _playerAction?.Enable();
    }

    private void Start()
    {
        // ゲーム開始時はポーズ画面を非表示
        _pauseCanvas.enabled = false;
    }

    private void Update()
    {
        // 毎フレームポーズ入力を監視
        PauseMenu();
    }

    /// <summary>
    /// ポーズ状態の切り替え処理
    /// </summary>
    private void PauseMenu()
    {
        // ポーズボタンが押された瞬間
        if (_playerAction.Player.Pause.WasPressedThisFrame())
        {
            if (!_isPaused)
            {
                // ポーズ開始
                Time.timeScale = 0;         // ゲームを停止
                _pauseCanvas.enabled = true;// ポーズ画面を表示
                _isPaused = true;
            }
            else if (_isPaused)
            {
                // ポーズ解除
                Time.timeScale = 1;          // ゲームを再開
                _pauseCanvas.enabled = false;// ポーズ画面を非表示
                _isPaused = false;
            }
        }
    }

    /// <summary>
    /// ボタン操作：ポーズ解除してゲームに戻る
    /// </summary>
    public void ReturnGame()
    {
        Time.timeScale = 1;
        _pauseCanvas.enabled = false;
    }

    /// <summary>
    /// ボタン操作：タイトルシーンに戻る
    /// </summary>
    public void BackToTheTitle()
    {
        SceneManager.LoadScene(TITLE_SCENE_NAME);
    }

    /// <summary>
    /// ボタン操作：ゲームを終了する
    /// （エディタとビルド版で処理を分岐）
    /// </summary>
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // エディタ実行終了
#else
        Application.Quit(); // ビルド実行終了
#endif
    }

    private void OnDisable()
    {
        // InputActionを無効化
        _playerAction?.Disable();
    }
}
