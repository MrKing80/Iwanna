using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 0f;
    [SerializeField] private float _jumpForce = 0f;
    [SerializeField] private PlayerStatus _playerStatus = PlayerStatus.FALLING;   //�v���C���[�̃X�e�[�^�X���Ǘ�����ϐ�
    private PlayerInput _playerAction = default;
    private Rigidbody2D _playerRigidbody2D = default;
    private Vector2 _movement = Vector2.zero;
    private float _jumpVelocity = default;              //�v���C���[�̌v�Z��̃W�����v���x���i�[����ϐ�
    private float _addGravity = 60f;                    //�d�͉����x
    private float _jumpTimer = 0f;                      //���Ԍv���p
    private float _lowerLimitTime = 0.1f;            //�W�����v�o�ߎ��Ԃ̉����l
    private bool _isJumpPressed = false;
    private bool _keyLock = false;
    private const int EXPONENT = 2;                                 //�ׂ��w��
    private const float INITIAL_TIMER_VALUE = 0.2f;

    /// <summary>
    /// �W�����v���̃v���C���[�̏��
    /// </summary>   
    private enum PlayerStatus
    {
        GROUND,     //�ڒn���
        JUMPING,    //�W�����v���
        FALLING     //�������
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
        //�X�y�[�X�L�[��������Ă�����ȉ��̏���
        if (_playerAction.Player.Jump.IsPressed())
        {
            //�W�����v�L�[�������ꂽ��Ԃɂ���
            _isJumpPressed = !_keyLock;
        }
        //������Ă��Ȃ���Έȉ��̏���
        else
        {
            //�W�����v�L�[������Ă��Ȃ���Ԃɂ���
            _isJumpPressed = false;

            //�L�[���b�N����
            _keyLock = false;
        }

        SpeedAssignment(PlayerMove(), PlayerJump());
    }

    private void OnDisable()
    {
        _playerAction?.Disable();
    }

    /// <summary>
    /// �ړ����͎擾
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
    /// �v���C���[�̃W�����v����
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

                //���W�����v
                if (_isJumpPressed || _lowerLimitTime > _jumpTimer)
                {
                    _jumpVelocity = _jumpForce;
                    _jumpVelocity -= _addGravity * Mathf.Pow(_jumpTimer, EXPONENT);
                }
                //��W�����v
                else
                {
                    _jumpTimer += Time.deltaTime;
                    _jumpVelocity = _jumpForce;
                    _jumpVelocity -= _addGravity * Mathf.Pow(_jumpTimer, EXPONENT);
                }

                GroundingJudgment();

                break;

            //�������
            case PlayerStatus.FALLING:

                //���Ԃ��v��
                _jumpTimer += Time.deltaTime;

                //y�����̑��x���[���ɂ���
                _jumpVelocity = 0f;

                //y�����̑��x�����X�Ɍ��炵�Ă���
                _jumpVelocity -= _addGravity * Mathf.Pow(_jumpTimer, EXPONENT);

                break;

            default:
                break;
        }

        return _jumpVelocity;
    }

    private void GroundingJudgment()
    {
        //�W�����v��Ԃ��痎����Ԃֈڍs
        if (0f > _playerRigidbody2D.linearVelocityY)
        {
            //�X�e�[�^�X�𗎉���ԂɕύX
            _playerStatus = PlayerStatus.FALLING;

            //y�����̑��x���[���ɂ���
            _jumpVelocity = 0f;

            //�v�����鎞�Ԃɏ����l����
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
            _jumpTimer = 0f;                        //������
            _keyLock = true;                        //�L�[���͂����b�N
        }
    }
}
