using UnityEngine;

/// <summary>
/// トラップイベントを制御するスクリプト
/// ・落とし穴
/// ・落下物
/// ・飛び出し物
/// ・移動物
/// ・平行移動
/// ・プレイヤー狙いの飛来物
/// など複数のパターンを一括管理する
/// </summary>
public class KillerEventController : MonoBehaviour
{
    [SerializeField, Header("トラップ化するオブジェクト")]
    private GameObject[] _killers = default;

    [SerializeField, Header("トラップの種類")]
    private TrapType _trapType = default; 

    [SerializeField, Header("トラップの射出速度")]
    private float _injectionspeed = 0f;

    [SerializeField, Header("トラップの移動速度")]
    private float _moveSpeed = 0f;

    [SerializeField, Header("トラップが移動する際の終点")]
    private Vector2 _moveEndPoint = Vector2.zero;

    [SerializeField, Header("トラップの効果音")]
    private AudioClip _killerSound = default;

    private Rigidbody2D[] _trapRigidbodys = default;    // 各トラップの Rigidbody2D
    private AudioSource _audioSource = default;         // 効果音再生用
    private CircleCollider2D _collider2D = default;     // 発動トリガー用の当たり判定

    private Vector2[] _myPosition = default;            // トラップの初期位置
    private Vector2 _playerPosition = Vector2.zero;     // プレイヤー位置（狙い撃ち用）

    private bool _isTrapMoving = false;                 // 移動中フラグ

    /// <summary>
    /// トラップの種類を列挙
    /// </summary>
    private enum TrapType
    {
        Pitfall,            // 落とし穴：オブジェクトを消す
        FallingObjects,     // オブジェクトが落下
        JumpOutObjects,     // オブジェクトが飛び出す（上方向へ）
        MoveObjects,        // 指定した終点まで移動
        ParallelTranslation,// 水平方向に平行移動
        TowardsThePlayer    // プレイヤー目掛けて飛んでくる
    }

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _collider2D = GetComponent<CircleCollider2D>();

        _trapRigidbodys = new Rigidbody2D[_killers.Length];

        _myPosition = new Vector2[_killers.Length];

        // 初期位置を記録
        for (int i = 0; i < _killers.Length; i++)
        {
            _myPosition[i] = _killers[i].transform.position;
        }
    }

    private void Update()
    {
        // 移動フラグが立っていれば移動処理を実行
        if (_isTrapMoving)
        {
            TrapMove();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // プレイヤーの位置を記録（狙い撃ち系で使用）
        _playerPosition = collision.transform.position;

        // トラップ種類ごとに処理を分岐
        switch (_trapType)
        {
            case TrapType.Pitfall:
                PlaySound();
                // 落とし穴 → オブジェクトを消す
                for (int i = 0; i < _killers.Length; i++)
                {
                    _killers[i].gameObject.SetActive(false);
                }
                _collider2D.enabled = false;
                break;

            case TrapType.FallingObjects:
                PlaySound();
                // 上から下に落下
                for (int i = 0; i < _killers.Length; i++)
                {
                    _trapRigidbodys[i] = _killers[i].GetComponent<Rigidbody2D>();
                    _trapRigidbodys[i].bodyType = RigidbodyType2D.Dynamic;
                    _trapRigidbodys[i].linearVelocity = Vector2.down * _injectionspeed;
                }
                _collider2D.enabled = false;
                break;

            case TrapType.JumpOutObjects:
                PlaySound();
                // 下から上に飛び出す
                for (int i = 0; i < _killers.Length; i++)
                {
                    _trapRigidbodys[i] = _killers[i].GetComponent<Rigidbody2D>();
                    _trapRigidbodys[i].bodyType = RigidbodyType2D.Dynamic;
                    _trapRigidbodys[i].linearVelocity = Vector2.up * _injectionspeed;
                }
                _collider2D.enabled = false;
                break;

            case TrapType.TowardsThePlayer:
                PlaySound();
                // プレイヤー位置に向かって飛んでくる
                for (int i = 0; i < _killers.Length; i++)
                {
                    _trapRigidbodys[i] = _killers[i].GetComponent<Rigidbody2D>();
                    _trapRigidbodys[i].bodyType = RigidbodyType2D.Dynamic;

                    // プレイヤー方向を計算して力を加える
                    Vector2 direction = (_playerPosition - _myPosition[i]).normalized;
                    _trapRigidbodys[i].linearVelocity = direction * _injectionspeed;
                }
                _collider2D.enabled = false;
                break;

            case TrapType.MoveObjects:
                PlaySound();
                // Update 内で MoveTowards させる
                _isTrapMoving = true;
                _collider2D.enabled = false;
                break;

            case TrapType.ParallelTranslation:
                PlaySound();
                // 横移動を開始
                _isTrapMoving = true;
                _collider2D.enabled = false;
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// トラップ移動処理（移動/平行移動）
    /// </summary>
    private void TrapMove()
    {
        for (int i = 0; i < _killers.Length; i++)
        {
            if (_trapType == TrapType.MoveObjects)
            {
                // 任意の終点に移動
                _killers[i].transform.position = Vector2.MoveTowards(
                    _killers[i].transform.position,
                    _moveEndPoint,
                    _moveSpeed * Time.deltaTime
                );

                if ((Vector2)_killers[i].transform.position == _moveEndPoint)
                {
                    _isTrapMoving = false;
                }
            }
            else if (_trapType == TrapType.ParallelTranslation)
            {
                // X座標だけを変化させて移動
                _killers[i].transform.position = Vector2.MoveTowards(
                    _killers[i].transform.position,
                    new Vector2(_moveEndPoint.x, _killers[i].transform.position.y),
                    _moveSpeed * Time.deltaTime
                );

                if (_killers[i].transform.position.x == _moveEndPoint.x)
                {
                    _isTrapMoving = false;
                }
            }
        }
    }

    private void OnBecameInvisible()
    {
        // 画面外に出たらトラップを非アクティブ化（落とし穴・移動系は除外）
        if (_trapType != TrapType.Pitfall && _trapType != TrapType.MoveObjects)
        {
            for (int i = 0; i < _killers.Length; i++)
            {
                _killers[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 効果音再生処理
    /// </summary>
    private void PlaySound()
    {
        if (_killerSound != null)
        {
            _audioSource.PlayOneShot(_killerSound);
        }
    }
}
