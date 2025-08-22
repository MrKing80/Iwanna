using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 0f;
    [SerializeField] private float _jumpForce = 0f;
    [SerializeField] private PlayerStatus _playerStatus = PlayerStatus.FALLING;   //プレイヤーのステータスを管理する変数
    private PlayerInput _playerAction = default;
    private Rigidbody2D _playerRigidbody2D = default;
    private Vector2 _movement = Vector2.zero;
    private float _jumpVelocity = default;              //プレイヤーの計算後のジャンプ速度を格納する変数
    private float _addGravity = 60f;                    //重力加速度
    private float _jumpTimer = 0f;                      //時間計測用
    private float _lowerLimitTime = 0.1f;            //ジャンプ経過時間の下限値
    private bool _isJumpPressed = false;
    private bool _keyLock = false;
    private const int EXPONENT = 2;                                 //べき指数
    private const float INITIAL_TIMER_VALUE = 0.2f;

    /// <summary>
    /// ジャンプ時のプレイヤーの状態
    /// </summary>   
    private enum PlayerStatus
    {
        GROUND,     //接地状態
        JUMPING,    //ジャンプ状態
        FALLING     //落下状態
    }

    private void Awake()
    {
        _playerAction = new PlayerInput();

        _playerAction.Player.Horizontal.started += OnMove;
        _playerAction.Player.Horizontal.performed += OnMove;
        _playerAction.Player.Horizontal.canceled += OnMove;

        _playerAction?.Enable();
    }

    private void Start()
    {
        _playerRigidbody2D = this.GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        //スペースキーが押されていたら以下の処理
        if (_playerAction.Player.Jump.IsPressed())
        {
            //ジャンプキーが押された状態にする
            _isJumpPressed = !_keyLock;
        }
        //押されていなければ以下の処理
        else
        {
            //ジャンプキーがされていない状態にする
            _isJumpPressed = false;

            //キーロック解除
            _keyLock = false;
        }

        SpeedAssignment(PlayerMove(), PlayerJump());
    }

    private void OnDisable()
    {
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

    private float PlayerMove()
    {
       return _moveSpeed * _movement.x;
    }

    /// <summary>
    /// プレイヤーのジャンプ処理
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

                _jumpTimer += Time.deltaTime;

                //小ジャンプ
                if (_isJumpPressed || _lowerLimitTime > _jumpTimer)
                {
                    _jumpVelocity = _jumpForce;
                    _jumpVelocity -= _addGravity * Mathf.Pow(_jumpTimer, EXPONENT);
                }
                //大ジャンプ
                else
                {
                    _jumpTimer += Time.deltaTime;
                    _jumpVelocity = _jumpForce;
                    _jumpVelocity -= _addGravity * Mathf.Pow(_jumpTimer, EXPONENT);
                }

                GroundingJudgment();

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

        return _jumpVelocity;
    }

    private void GroundingJudgment()
    {
        //ジャンプ状態から落下状態へ移行
        if (0f > _playerRigidbody2D.linearVelocityY)
        {
            //ステータスを落下状態に変更
            _playerStatus = PlayerStatus.FALLING;

            //y方向の速度をゼロにする
            _jumpVelocity = 0f;

            //計測する時間に初期値を代入
            _jumpTimer = INITIAL_TIMER_VALUE;
        }

    }

    private void SpeedAssignment(float moveSpeed,float jumpForse)
    {
        _playerRigidbody2D.linearVelocity = new Vector2(moveSpeed,jumpForse);

        print(_playerRigidbody2D.linearVelocity);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (_playerStatus == PlayerStatus.FALLING && collision.gameObject.CompareTag("Floor"))
        {
            _playerStatus = PlayerStatus.GROUND;
            _jumpTimer = 0f;                        //初期化
            _keyLock = true;                        //キー入力をロック
        }
    }
}
