using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// プレイヤーの行動を管理するスクリプト
/// ・移動処理
/// ・ジャンプ（二段ジャンプ対応）
/// ・落下判定／天井判定
/// ・アニメーション制御
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 0f;                             // プレイヤーの移動速度
    [SerializeField] private float _jumpForce = 0f;                             // ジャンプ力
    [SerializeField] private PlayerStatus _playerStatus = PlayerStatus.FALLING; // 現在のプレイヤー状態
    [SerializeField] private LayerMask _rayCastTargetLayer = default;           // 地面判定対象のレイヤー
    [SerializeField] private AudioClip[] _jumpSound = default;                  // ジャンプ時の効果音

    private PlayerInput _playerAction = default;       // 新InputSystemの入力クラス
    private Rigidbody2D _playerRigidbody2D = default;  // Rigidbody2D（物理挙動）
    private Animator _playerAnimetor = default;        // Animator（アニメーション制御）
    private AudioSource _playerAudioSource = default;  // 効果音再生用
    private BoxCollider2D _boxCollider2D = default;    // プレイヤーのコライダー

    private Vector2 _movement = Vector2.zero;       // 入力方向（左右移動用）
    private Vector2 _boxSize = Vector2.zero;        // 地面／天井判定用の BoxCast サイズ

    // ジャンプ管理
    private int _jumpCount = 0;                     // 現在のジャンプ回数
    private const int FRIST_JUMP = 1;               // 1段目ジャンプ判定
    private const int SECOND_JUMP = 2;              // 2段目ジャンプ判定
    private const int MAX_JUMP_NUMBER = 2;          // 最大ジャンプ回数

    private const int EXPONENT = 2;                 // 重力計算用のべき指数
    private float _jumpVelocity = default;          // 現在のY方向速度
    private float _addGravity = 60f;                // 重力加速度
    private float _jumpTimer = 0f;                  // ジャンプ経過時間
    private float _lowerLimitTime = 0.1f;           // 小ジャンプ判定用の下限時間
    private float _fallSpeed = 5f;                  // 落下速度
    private float _colliderWidth = 0f;              // コライダー幅
    private float _colliderHeight = 0f;             // コライダー高さ
    private float _groundCheckWidthScale = 0.9f;    // 地面判定用の幅スケール
    private float _groundCheckHeight = 0.1f;        // 地面判定用の高さスケール
    private float _maxRayDistance = 0.15f;          // Rayの長さ

    // 入力フラグ
    private bool _isJumpPressed = false;            // ジャンプキーが押されているか
    private bool _keyLock = false;                  // キーロック（着地直後の誤動作防止）

    // アニメーションパラメータ名
    private const string RUN_ANIMATION_NAME = "Run";
    private const string JUMP_ANIMATION_NAME = "Jump";
    private const string FALL_ANIMATION_NAME = "Fall";

    /// <summary>
    /// プレイヤーの状態
    /// </summary>
    private enum PlayerStatus
    {
        GROUND,         // 接地中
        JUMPING,        // 1段目ジャンプ中
        DUBBLE_JUMPING, // 2段目ジャンプ中
        FALLING         // 落下中
    }

    private void Awake()
    {
        // 入力クラスのインスタンス生成
        _playerAction = new PlayerInput();

        // 横移動のイベント登録
        _playerAction.Player.Horizontal.started += OnMove;
        _playerAction.Player.Horizontal.performed += OnMove;
        _playerAction.Player.Horizontal.canceled += OnMove;

        // 入力有効化
        _playerAction?.Enable();
    }

    private void Start()
    {
        // コンポーネント参照の取得
        _playerRigidbody2D = GetComponent<Rigidbody2D>();
        _playerAnimetor = GetComponent<Animator>();
        _playerAudioSource = GetComponent<AudioSource>();
        _boxCollider2D = GetComponent<BoxCollider2D>();

        // コライダーのサイズから判定用Boxのサイズを計算
        _colliderWidth = _boxCollider2D.size.x * transform.localScale.x;
        _colliderHeight = _boxCollider2D.size.y * transform.localScale.y;
        _boxSize = new Vector2(_colliderWidth * _groundCheckWidthScale, _groundCheckHeight);
    }

    private void Update()
    {
        // タイトルシーンやポーズ中は操作無効化
        if (SceneManager.GetActiveScene().name == "TitleScene" || Time.timeScale == 0)
        {
            return;
        }

        // ジャンプ入力を確認
        OnJumping();

        // 移動・ジャンプ速度をRigidbody2Dに反映
        SpeedAssignment(PlayerMove(), PlayerJump());
    }

    private void OnDisable()
    {
        // 入力無効化
        _playerAction?.Disable();
    }

    /// <summary>
    /// 移動入力イベント
    /// </summary>
    private void OnMove(InputAction.CallbackContext context)
    {
        _movement = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// ジャンプ入力処理
    /// ・ジャンプ回数の管理
    /// ・1段/2段ジャンプの判定
    /// ・天井判定／地面判定
    /// </summary>
    private void OnJumping()
    {
        // ジャンプボタンを押した瞬間
        if (_playerAction.Player.Jump.WasPressedThisFrame())
        {
            // まだ最大ジャンプ回数に達していなければ実行
            if (_jumpCount != MAX_JUMP_NUMBER)
            {
                _jumpCount++;                       // ジャンプ回数加算
                _jumpTimer = 0f;                    // ジャンプ時間リセット
                SpeedAssignment(PlayerMove(), 0f);  // Y速度リセット

                // 1段目 or 2段目判定
                if (_jumpCount == FRIST_JUMP)
                {
                    _playerAudioSource.PlayOneShot(_jumpSound[_jumpCount - 1]);
                    _playerStatus = PlayerStatus.JUMPING;
                }
                else if (_jumpCount == SECOND_JUMP)
                {
                    _playerAudioSource.PlayOneShot(_jumpSound[_jumpCount - 1]);
                    _playerStatus = PlayerStatus.DUBBLE_JUMPING;
                }
            }
        }
        else
        {
            // 接地判定
            GroundJugement();
        }

        // ジャンプボタンが押されている間
        if (_playerAction.Player.Jump.IsPressed())
        {
            _isJumpPressed = !_keyLock; // キーロック中は無効
            CeilingJudgment();          // 天井にぶつかったか判定
        }
        else
        {
            _isJumpPressed = false;
            _keyLock = false; // キーロック解除
        }
    }

    /// <summary>
    /// 移動処理（左右）
    /// </summary>
    private float PlayerMove()
    {
        if (_movement != Vector2.zero)
        {
            _playerAnimetor.SetBool(RUN_ANIMATION_NAME, true);

            // 向きを移動方向に合わせる
            transform.localScale = new Vector3(_movement.x, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            _playerAnimetor.SetBool(RUN_ANIMATION_NAME, false);
        }

        // X軸方向の速度を返す
        return _moveSpeed * _movement.x;
    }

    /// <summary>
    /// ジャンプ処理
    /// ・状態に応じて処理を切り替える
    /// </summary>
    private float PlayerJump()
    {
        switch (_playerStatus)
        {
            case PlayerStatus.GROUND:
                if (_isJumpPressed)
                {
                    _playerStatus = PlayerStatus.JUMPING;
                }
                break;

            case PlayerStatus.JUMPING:
                ApplyJumpPhysics(); // 1段目ジャンプ処理
                break;

            case PlayerStatus.DUBBLE_JUMPING:
                ApplyJumpPhysics(); // 2段目ジャンプ処理
                break;

            case PlayerStatus.FALLING:
                // 落下アニメーション
                _playerAnimetor.SetBool(FALL_ANIMATION_NAME, true);
                _playerAnimetor.SetBool(JUMP_ANIMATION_NAME, false);

                // 落下速度を設定
                _jumpVelocity = 0f;
                _jumpVelocity -= _fallSpeed;
                break;
        }

        return _jumpVelocity;
    }

    /// <summary>
    /// ジャンプ中の物理挙動
    /// ・小ジャンプ／大ジャンプ判定
    /// ・時間経過による減速
    /// </summary>
    private void ApplyJumpPhysics()
    {
        _playerAnimetor.SetBool(JUMP_ANIMATION_NAME, true);
        _jumpTimer += Time.deltaTime;

        // 小ジャンプ（キーを短く押した時 or 下限時間未満）
        if (_isJumpPressed || _lowerLimitTime > _jumpTimer)
        {
            _jumpVelocity = _jumpForce;
            _jumpVelocity -= _addGravity * Mathf.Pow(_jumpTimer, EXPONENT);
        }
        // 大ジャンプ（キーを長押しした時）
        else
        {
            _jumpTimer += Time.deltaTime;
            _jumpVelocity = _jumpForce;
            _jumpVelocity -= _addGravity * Mathf.Pow(_jumpTimer, EXPONENT);
        }

        FallingJugement(); // 落下判定
    }

    /// <summary>
    /// 落下判定
    /// </summary>
    private void FallingJugement()
    {
        if (_playerRigidbody2D.linearVelocityY < 0f)
        {
            _playerStatus = PlayerStatus.FALLING;
            _jumpVelocity = 0f;
        }
    }

    /// <summary>
    /// 移動・ジャンプ速度を Rigidbody2D に反映
    /// </summary>
    private void SpeedAssignment(float moveSpeed, float jumpForse)
    {
        _playerRigidbody2D.linearVelocity = new Vector2(moveSpeed, jumpForse);
    }

    /// <summary>
    /// 接地判定
    /// </summary>
    private void GroundJugement()
    {
        if (_isJumpPressed) return;

        RaycastHit2D hitDown = Physics2D.BoxCast(
            transform.position, _boxSize, 0f, Vector2.down, _maxRayDistance, _rayCastTargetLayer
        );

        if (hitDown.collider)
        {
            // 接地時
            _playerAnimetor.SetBool(JUMP_ANIMATION_NAME, false);
            _playerAnimetor.SetBool(FALL_ANIMATION_NAME, false);
            _playerStatus = PlayerStatus.GROUND;

            // 各変数リセット
            _jumpCount = 0;
            _jumpVelocity = 0f;
            _jumpTimer = 0f;

            _keyLock = true; // 着地直後はキー入力無効
        }
        else if (_playerStatus == PlayerStatus.GROUND)
        {
            // 接地状態から地面がなくなったら落下扱い
            _playerStatus = PlayerStatus.FALLING;
            _jumpCount++;
        }
    }

    /// <summary>
    /// 天井判定
    /// </summary>
    private void CeilingJudgment()
    {
        RaycastHit2D hitUp = Physics2D.BoxCast(
            transform.position, _boxSize, 0f, Vector2.up, _maxRayDistance, _rayCastTargetLayer
        );

        if (hitUp.collider)
        {
            _playerAnimetor.SetBool(JUMP_ANIMATION_NAME, false);
            _playerAnimetor.SetBool(FALL_ANIMATION_NAME, true);
            _playerStatus = PlayerStatus.FALLING;

            // 各変数リセット
            _jumpVelocity = 0f;
            _jumpTimer = 0f;

            _keyLock = true;
        }
    }

    /// <summary>
    /// Sceneビュー上に判定Boxを可視化
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 _boxSize = new Vector2(_colliderWidth * _groundCheckWidthScale, _groundCheckHeight);

        Vector2 origin = transform.position;
        Vector2 endDown = origin + Vector2.down * _maxRayDistance;
        Vector2 endUp = origin + Vector2.up * _maxRayDistance;

        Gizmos.DrawWireCube(endUp, _boxSize);   // 天井判定
        Gizmos.DrawWireCube(endDown, _boxSize); // 地面判定
    }
}
