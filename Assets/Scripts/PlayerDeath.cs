using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Assertions.Must;

/// <summary>
/// プレイヤーの死亡処理を行うスクリプト
/// ・トラップや画面外に落ちたときに死亡判定
/// ・血しぶきエフェクト生成
/// ・効果音再生
/// ・GameOverUI表示
/// ・プレイヤー本体を非アクティブ化
/// </summary>
public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private GameObject _blood = default;          // 死亡時に飛び散るオブジェクト（血しぶき）
    [SerializeField] private GameObject _soundGenerater = default; // 死亡時に再生するサウンド生成用オブジェクト

    private GameOverGenerate _gameOverGnerate = default;           // GameOverUI制御用
    private GameObject _gameManager = default;                     // GameManager参照用

    private const int MAX_GENERATE_OBJECTS = 40;                    // 血しぶきを生成する数
    private Vector2 _playerPosition = Vector2.zero;                 // プレイヤーの現在位置
    private const float MIN_FORCE = 5f;                             // 血しぶきの最小飛散力
    private const float MAX_FORCE = 12.5f;                          // 血しぶきの最大飛散力

    private const string KILLER_TAG = "Killer";                     // プレイヤーを殺すオブジェクトのタグ
    private const string GAMEMANAGER_TAG = "GameController";        // GameManagerを探すためのタグ

    private bool _isDeath = false;                                  // プレイヤーが死亡しているかどうか

    private void Start()
    {
        // GameManagerをタグから検索し、GameOverUIを扱えるようにする
        _gameManager = GameObject.FindGameObjectWithTag(GAMEMANAGER_TAG);

        if (_gameManager != null)
        {
            _gameOverGnerate = _gameManager.GetComponent<GameOverGenerate>();
        }

        _isDeath = false;
    }

    private void FixedUpdate()
    {
        // 死亡フラグが立っていたら死亡処理を実行
        if (_isDeath)
        {
            DeathPlayer();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 「Killer」タグのオブジェクトに触れたら死亡
        if (collision.gameObject.CompareTag(KILLER_TAG))
        {
            _isDeath = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 「Killer」タグのオブジェクトに触れたら死亡
        if (collision.gameObject.CompareTag(KILLER_TAG))
        {
            _isDeath = true;
        }
    }

    /// <summary>
    /// カメラ外に出た場合も死亡扱い
    /// （落下死などに対応）
    /// </summary>
    private void OnBecameInvisible()
    {
        _isDeath = true;
    }

    /// <summary>
    /// プレイヤー死亡時の一連の処理
    /// </summary>
    private void DeathPlayer()
    {
        // サウンド再生用オブジェクトを生成
        Instantiate(_soundGenerater);

        // 血しぶきをランダムに飛ばす
        GenetateBlood();

        // GameOverUIを表示
        if (_gameOverGnerate != null)
        {
            _gameOverGnerate.GameOverUI();
        }

        // プレイヤーを非アクティブ化
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// 血しぶきエフェクトを生成してランダムに飛ばす処理
    /// </summary>
    private void GenetateBlood()
    {
        GameObject generatedBlood = default; // 生成したオブジェクトを一時的に格納

        for (int i = 0; i < MAX_GENERATE_OBJECTS; i++)
        {
            // プレイヤーの現在位置を取得
            _playerPosition = transform.position;

            // 血しぶきオブジェクトを生成
            generatedBlood = Instantiate(_blood, _playerPosition, Quaternion.identity);
            Rigidbody2D bloodrig2D = generatedBlood.GetComponent<Rigidbody2D>();

            // ランダムな方向を決定
            Vector2 randomDirection = Random.insideUnitCircle.normalized;

            // ランダムなスピードを決定
            float randomSpeed = Random.Range(MIN_FORCE, MAX_FORCE);

            // ランダムな方向と速度で飛ばす
            bloodrig2D.AddForce(randomDirection * randomSpeed, ForceMode2D.Impulse);
        }
    }
}
