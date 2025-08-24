using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// ゲームの全体的な管理をするスクリプト
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager instance;             //インスタンスを保持する変数
    private Image _gameOverImage = default;
    private PlayerInput _playerAction = default;

    private void Awake()
    {
        // インスタンスがまだ存在しない場合、自分自身をインスタンスとする
        if (instance == null)
        {
            instance = this;
            
            // 他のシーンに移動してもシングルトンを破棄しない
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // すでにインスタンスが存在する場合は、自分自身を破棄する
            Destroy(gameObject);
        }

        _playerAction = new PlayerInput();
        _playerAction.Enable();

        Transform gameManagerChild = this.gameObject.transform.GetChild(0);
        Transform gameManagerGrandChild = gameManagerChild.transform.GetChild(0);
        _gameOverImage = gameManagerGrandChild.GetComponent<Image>();

    }

    private void Start()
    {
        _gameOverImage.enabled = false;
    }

    private void Update()
    {
        Retry();
    }

    private void OnDestroy()
    {
        _playerAction.Disable();
    }

    /// <summary>
    /// リトライを行う処理
    /// </summary>
    private void Retry()
    {
        //リトライキーが押されたら
        if (_playerAction.Player.Retry.WasPressedThisFrame())
        {
            //現在のシーンをロード
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

            _gameOverImage.enabled = false;
        }
    }

}
