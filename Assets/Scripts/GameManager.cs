using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

/// <summary>
/// ゲームの全体的な管理をするスクリプト
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager instance;             //インスタンスを保持する変数
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

    }

    private void Start()
    {
        
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
        }
    }

}
