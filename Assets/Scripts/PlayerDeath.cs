using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Assertions.Must;

/// <summary>
/// プレイヤーの死亡処理を行うスクリプト
/// </summary>
public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private GameObject _blood = default;   //死亡時に飛び散るオブジェクト
    private GameOverGenerate _gameOverGnerate = default;
    private GameObject _gameOverManager = default;
    private const int MAX_GENERATE_OBJECTS = 75;            //飛び散るオブジェクトの最大数
    private Vector2 _playerPosition = Vector2.zero;         //プレイヤーのポジション
    private const float MIN_FORCE = 5f;                     //オブジェクトを飛ばす最小の力
    private const float MAX_FORCE = 12.5f;                  //オブジェクトを飛ばす最大の力
    private const string KILLER_TAG = "Killer";     //プレイヤーを殺すオブジェクトのタグ名
    private const string GAMEOVER_TAG = "GameController";
    private bool _isDeath = false;                  //死んだかどうか

    private void Awake()
    {
        _gameOverManager = GameObject.FindGameObjectWithTag(GAMEOVER_TAG);

        _gameOverGnerate = _gameOverManager.GetComponent<GameOverGenerate>();
    }

    private void Start()
    {
        _isDeath = false;
    }

    private void FixedUpdate()
    {
        if (_isDeath)
        {
            DeathPlayer();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //キラーオブジェクトに触れたら
        if (collision.gameObject.CompareTag(KILLER_TAG))
        {
            //死亡判定をtrueに変更
            _isDeath = true;
        }
    }

    /// <summary>
    /// カメラ外に出たら処理を行う
    /// </summary>
    private void OnBecameInvisible()
    {
        //死亡判定をtrueに変更
        _isDeath = true;
    }

    /// <summary>
    /// 死亡したときの処理を行う
    /// </summary>
    private void DeathPlayer()
    {
        print("あなたは死にました");
        GenetateBlood();
        _gameOverGnerate.GameOverUI();
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// 死亡時にオブジェクトを飛ばす処理
    /// </summary>
    private void GenetateBlood()
    {
        GameObject generatedBlood = default;    //生成後のオブジェクトを格納する変数

        //オブジェクトを生成する
        for (int i = 0; i < MAX_GENERATE_OBJECTS; i++)
        {
            //プレイヤーのポジションを格納
            _playerPosition = transform.position;

            //オブジェクトを生成
            generatedBlood = Instantiate(_blood, _playerPosition, Quaternion.identity);
            Rigidbody2D bloodrig2D = generatedBlood.GetComponent<Rigidbody2D>();

            //飛ばす方向をランダムに決める
            Vector2 randomDirection = Random.insideUnitCircle.normalized;

            //飛ばすスピードをランダムに決める
            float randomSpeed = Random.Range(MIN_FORCE, MAX_FORCE);

            //ランダムに決めた値を使ってオブジェクトを飛ばす
            bloodrig2D.AddForce(randomDirection * randomSpeed, ForceMode2D.Impulse);
        }
    }
}
