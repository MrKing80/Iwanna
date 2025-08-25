using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
/// <summary>
/// プレイヤーの行動を管理するスクリプト
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 0f;             //プレイヤーの移動速度
    [SerializeField] private float _jumpForce = 0f;             //プレイヤーのジャンプ力
    [SerializeField] private PlayerStatus _playerStatus = PlayerStatus.FALLING;   //プレイヤーのステータスを管理する変数
    [SerializeField] private LayerMask _rayCastTargetLayer = default;

    private PlayerInput _playerAction = default;
    private Rigidbody2D _playerRigidbody2D = default;
    private Animator _playerAnimetor = default;
    private CapsuleCollider2D _capsuleCollider2D = default;

    private Vector2 _movement = Vector2.zero;

    private int _jumpCount = 0;                     //ジャンプした回数を数える変数
    private const int FRIST_JUMP = 1;               //一回目のジャンプを判定するための数字を格納した定数
    private const int SECOND_JUMP = 2;              //二回目のジャンプを判定するための数字を格納した定数
    private const int EXPONENT = 2;                 //べき指数
    private const int MAX_JUMP_NUMBER = 2;          //最大ジャンプ回数

    private float _jumpVelocity = default;          //プレイヤーの計算後のジャンプ速度を格納する変数
    private float _addGravity = 60f;                //重力加速度
    private float _jumpTimer = 0f;                  //時間計測用
    private float _lowerLimitTime = 0.1f;           //ジャンプ経過時間の下限値
    private float _colliderWidth = 0f;
    private float _colliderHeight = 0f;
    private float _groundCheckWidthScale = 0.9f;
    private float _groundCheckHeight = 0.1f;
    private float _maxRayDistance = 0.15f;                                 // レイの射出距離
    private const float INITIAL_TIMER_VALUE = 0.2f; //タイマーを初期化するときに使用する定数

    private bool _isJumpPressed = false;            //ジャンプキーが押されたか
    private bool _keyLock = false;                  //キーロックをしているか

    private const string RUN_ANIMATION_NAME = "Run";
    private const string JUMP_ANIMATION_NAME = "Jump";
    private const string FALL_ANIMATION_NAME = "Fall";

    /// <summary>
    /// ジャンプ時のプレイヤーの状態
    /// </summary>   
    private enum PlayerStatus
    {
        GROUND,         //接地状態
        JUMPING,        //1段目のジャンプ状態
        DUBBLE_JUMPING, //2段目のジャンプ状態
        FALLING         //落下状態
    }

    private void Awake()
    {
        //プレイヤーアクションのインスタンス
        _playerAction = new PlayerInput();

        //アクションイベントを登録
        _playerAction.Player.Horizontal.started += OnMove;
        _playerAction.Player.Horizontal.performed += OnMove;
        _playerAction.Player.Horizontal.canceled += OnMove;

        //InputActionを有効化
        _playerAction?.Enable();
    }

    private void Start()
    {
        _playerRigidbody2D = this.GetComponent<Rigidbody2D>();
        _playerAnimetor = this.GetComponent<Animator>();
        _capsuleCollider2D = this.GetComponent<CapsuleCollider2D>();

        _colliderWidth = _capsuleCollider2D.size.x * transform.localScale.x;
        _colliderHeight = _capsuleCollider2D.size.y * transform.localScale.y;
    }
    private void Update()
    {
        OnJumping();
        SpeedAssignment(PlayerMove(), PlayerJump());
    }

    private void OnDisable()
    {
        //InputActionを無効化
        _playerAction?.Disable();
    }

    /// <summary>
    /// 移動入力取得
    /// </summary>
    /// <param name="context"></param>
    private void OnMove(InputAction.CallbackContext context)
    {
        _movement = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// ジャンプ入力を取得＆管理
    /// </summary>
    private void OnJumping()
    {
        //ジャンプキーが入力されたか
        if (_playerAction.Player.Jump.WasPressedThisFrame())
        {
            //ジャンプ回数が最大に達しているか
            if (_jumpCount != MAX_JUMP_NUMBER)
            {

                //ジャンプした回数を増やす
                _jumpCount++;

                //初期化
                _jumpTimer = 0f;
                SpeedAssignment(PlayerMove(), 0f);

                //現在のジャンプが1段目か2段目かを判定
                if (_jumpCount == FRIST_JUMP)
                {
                    _playerStatus = PlayerStatus.JUMPING;
                }
                else if (_jumpCount == SECOND_JUMP)
                {
                    _playerStatus = PlayerStatus.DUBBLE_JUMPING;
                }
            }
        }
        else
        {
            GroundJugement();
        }

        //スペースキーが押されていたら以下の処理
        if (_playerAction.Player.Jump.IsPressed())
        {
            //ジャンプキーが押された状態にする
            _isJumpPressed = !_keyLock;
        }
        else
        {
            //ジャンプキーがされていない状態にする
            _isJumpPressed = false;

            //キーロック解除
            _keyLock = false;
        }

    }

    /// <summary>
    /// プレイヤーの移動処理
    /// </summary>
    /// <returns></returns>
    private float PlayerMove()
    {
        if (_movement != Vector2.zero)
        {
            _playerAnimetor.SetBool(RUN_ANIMATION_NAME, true);

            //移動方向に応じて向きを変える
            transform.localScale = new Vector3(_movement.x, this.transform.localScale.y, this.transform.localScale.z);
        }
        else
        {
            _playerAnimetor.SetBool(RUN_ANIMATION_NAME, false);
        }
        //X軸の速度を計算し、返す
        return _moveSpeed * _movement.x;
    }

    /// <summary>
    /// プレイヤーのジャンプ処理
    /// </summary>
    private float PlayerJump()
    {
        //現在のステータスに応じて処理を変える
        switch (_playerStatus)
        {
            //接地状態
            case PlayerStatus.GROUND:

                //キーが入力されたらジャンプ状態へ変更
                if (_isJumpPressed)
                {
                    _playerStatus = PlayerStatus.JUMPING;
                }

                break;

            //ジャンプ状態（1段目）
            case PlayerStatus.JUMPING:

                ApplyJumpPhysics();

                break;

            //ジャンプ状態（2段目）
            case PlayerStatus.DUBBLE_JUMPING:

                ApplyJumpPhysics();

                break;

            //落下状態
            case PlayerStatus.FALLING:

                //時間を計測
                _jumpTimer += Time.deltaTime;

                //y方向の速度をゼロにする
                _jumpVelocity = 0f;

                //y方向の速度を徐々に減らしていく
                _jumpVelocity -= _addGravity * Mathf.Pow(_jumpTimer, EXPONENT);

                break;

            default:
                break;
        }

        //Y軸の速度を返す
        return _jumpVelocity;
    }

    /// <summary>
    /// ジャンプ中の処理
    /// </summary>
    private void ApplyJumpPhysics()
    {
        _playerAnimetor.SetBool(JUMP_ANIMATION_NAME, true);

        //タイマーカウント
        _jumpTimer += Time.deltaTime;

        //小ジャンプ
        if (_isJumpPressed || _lowerLimitTime > _jumpTimer)
        {
            _jumpVelocity = _jumpForce;

            //徐々にY軸の速度を減らしていく
            _jumpVelocity -= _addGravity * Mathf.Pow(_jumpTimer, EXPONENT);
        }
        //大ジャンプ
        else
        {
            //タイマーカウント
            _jumpTimer += Time.deltaTime;

            _jumpVelocity = _jumpForce;

            //徐々にY軸の速度を減らしていく
            _jumpVelocity -= _addGravity * Mathf.Pow(_jumpTimer, EXPONENT);
        }

        FallingJugement();
    }

    /// <summary>
    /// 落下判定を行う処理
    /// </summary>
    private void FallingJugement()
    {
        //Y軸の速度が0を下回ったら
        if (_playerRigidbody2D.linearVelocityY < 0f)
        {
            _playerAnimetor.SetBool(FALL_ANIMATION_NAME, true);
            _playerAnimetor.SetBool(JUMP_ANIMATION_NAME, false);

            //落下状態に変更
            _playerStatus = PlayerStatus.FALLING;

            //y方向の速度をゼロにする
            _jumpVelocity = 0f;

            //計測する時間に初期値を代入
            _jumpTimer = INITIAL_TIMER_VALUE;

        }
    }

    /// <summary>
    /// 渡された速度をプレイヤーの速度に反映する処理
    /// </summary>
    /// <param name="moveSpeed">X軸方向に対する速度</param>
    /// <param name="jumpForse">Y軸方向に対する速度</param>
    private void SpeedAssignment(float moveSpeed, float jumpForse)
    {
        _playerRigidbody2D.linearVelocity = new Vector2(moveSpeed, jumpForse);
    }

    private void GroundJugement()
    {
        if (_isJumpPressed)
        {
            return;
        }

        Vector2 boxSize = new Vector2(_colliderWidth * _groundCheckWidthScale, _groundCheckHeight);
        RaycastHit2D hit = Physics2D.BoxCast
                    (
                        transform.position,   // 中心
                        boxSize,              // ボックスの大きさ
                        0f,                   // 角度
                        Vector2.down,         // 下方向にキャスト
                        _maxRayDistance,       // 距離
                        _rayCastTargetLayer   // レイヤー
                    );

        // 飛ばしたレイが何かにヒットしているか
        if (hit.collider)
        {
            _playerAnimetor.SetBool(JUMP_ANIMATION_NAME, false);
            _playerAnimetor.SetBool(FALL_ANIMATION_NAME, false);

            //接地状態に変更
            _playerStatus = PlayerStatus.GROUND;

            //各変数を初期化
            _jumpCount = 0;
            _jumpVelocity = 0f;
            _jumpTimer = 0f;

            //キーをロック
            _keyLock = true;
        }
        else if (_playerStatus == PlayerStatus.GROUND)
        {
            _playerStatus = PlayerStatus.FALLING;
            _jumpCount++;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // BoxCast のサイズ（コライダーを元に計算する場合）
        Vector2 boxSize = new Vector2(_colliderWidth * _groundCheckWidthScale, _groundCheckHeight);

        // キャストの開始位置
        Vector2 origin = transform.position;

        // キャスト後の位置
        Vector2 end = origin + Vector2.down * _maxRayDistance;

        // Sceneビューにワイヤーの四角を表示
        Gizmos.DrawWireCube(origin, boxSize); // 開始地点
        Gizmos.DrawWireCube(end, boxSize);    // キャスト後の位置
    }

}