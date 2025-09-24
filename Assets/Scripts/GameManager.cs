using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// ゲームの全体的な管理をするスクリプト
/// ・シングルトン管理
/// ・ゲームオーバーUIの制御
/// ・リトライ処理
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager _gameManagerInstance;  // シングルトンインスタンス保持用
    private Image _gameOverImage = default;          // ゲームオーバー時に表示するイメージ
    private PlayerInput _playerAction = default;     // 入力管理（新Input System）

    private void Awake()
    {
        // シングルトンの初期化処理
        if (_gameManagerInstance == null)
        {
            _gameManagerInstance = this;

            // シーンをまたいでも破棄されないように設定
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 既に存在する場合は自分を破棄
            Destroy(gameObject);
        }

        // ゲーム開始時に時間を通常速度に戻す
        Time.timeScale = 1;

        // プレイヤー入力の有効化
        _playerAction = new PlayerInput();
        _playerAction.Enable();

        // 子オブジェクト階層からゲームオーバーUIを取得
        Transform gameManagerChild = this.gameObject.transform.GetChild(0);
        Transform gameManagerGrandChild = gameManagerChild.transform.GetChild(0);
        _gameOverImage = gameManagerGrandChild.GetComponent<Image>();
    }

    private void Start()
    {
        // ゲームオーバーUIは初期状態では非表示
        _gameOverImage.enabled = false;
    }

    private void Update()
    {
        // リトライ処理を監視
        Retry();
    }

    private void OnDestroy()
    {
        // シーン破棄時に入力を無効化
        _playerAction.Disable();
    }

    /// <summary>
    /// リトライ処理
    /// 現在のシーンを再読み込みし、ゲームオーバーUIをリセット
    /// </summary>
    private void Retry()
    {
        // リトライキー（InputAction）が押されたか確認
        if (_playerAction.Player.Retry.WasPressedThisFrame())
        {
            // 現在のシーンを再読み込み
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

            // ゲームオーバーUIを非表示に戻す
            _gameOverImage.enabled = false;
        }
    }
}
