using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
/// <summary>
/// �v���C���[�̍s�����Ǘ�����X�N���v�g
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 0f;             //�v���C���[�̈ړ����x
    [SerializeField] private float _jumpForce = 0f;             //�v���C���[�̃W�����v��
    [SerializeField] private PlayerStatus _playerStatus = PlayerStatus.FALLING;   //�v���C���[�̃X�e�[�^�X���Ǘ�����ϐ�

    private PlayerInput _playerAction = default;
    private Rigidbody2D _playerRigidbody2D = default;

    private Vector2 _movement = Vector2.zero;

    private int _jumpCount = 0;                     //�W�����v�����񐔂𐔂���ϐ�
    private const int FRIST_JUMP = 1;               //���ڂ̃W�����v�𔻒肷�邽�߂̐������i�[�����萔
    private const int SECOND_JUMP = 2;              //���ڂ̃W�����v�𔻒肷�邽�߂̐������i�[�����萔
    private const int EXPONENT = 2;                 //�ׂ��w��
    private const int MAX_JUMP_NUMBER = 2;          //�ő�W�����v��

    private float _jumpVelocity = default;          //�v���C���[�̌v�Z��̃W�����v���x���i�[����ϐ�
    private float _addGravity = 60f;                //�d�͉����x
    private float _jumpTimer = 0f;                  //���Ԍv���p
    private float _lowerLimitTime = 0.1f;           //�W�����v�o�ߎ��Ԃ̉����l
    private const float INITIAL_TIMER_VALUE = 0.2f; //�^�C�}�[������������Ƃ��Ɏg�p����萔

    private bool _isJumpPressed = false;            //�W�����v�L�[�������ꂽ��
    private bool _keyLock = false;                  //�L�[���b�N�����Ă��邩

    private const string GROUND_TAG = "Floor";      //�n�ʂ̃^�O��

    /// <summary>
    /// �W�����v���̃v���C���[�̏��
    /// </summary>   
    private enum PlayerStatus
    {
        GROUND,         //�ڒn���
        JUMPING,        //1�i�ڂ̃W�����v���
        DUBBLE_JUMPING, //2�i�ڂ̃W�����v���
        FALLING         //�������
    }

    private void Awake()
    {
        //�v���C���[�A�N�V�����̃C���X�^���X
        _playerAction = new PlayerInput();

        //�A�N�V�����C�x���g��o�^
        _playerAction.Player.Horizontal.started += OnMove;
        _playerAction.Player.Horizontal.performed += OnMove;
        _playerAction.Player.Horizontal.canceled += OnMove;

        //InputAction��L����
        _playerAction?.Enable();
    }

    private void Start()
    {
        _playerRigidbody2D = this.GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        OnJumping();
        SpeedAssignment(PlayerMove(), PlayerJump());
    }

    private void OnDisable()
    {
        //InputAction�𖳌���
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

    /// <summary>
    /// �W�����v���͂��擾���Ǘ�
    /// </summary>
    private void OnJumping()
    {
        //�W�����v�L�[�����͂��ꂽ��
        if (_playerAction.Player.Jump.WasPressedThisFrame())
        {
            //�W�����v�񐔂��ő�ɒB���Ă��邩
            if (_jumpCount != MAX_JUMP_NUMBER)
            {
                //�W�����v�����񐔂𑝂₷
                _jumpCount++;

                //������
                _jumpTimer = 0f;
                SpeedAssignment(PlayerMove(), 0f);

                //���݂̃W�����v��1�i�ڂ�2�i�ڂ��𔻒�
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

        //�X�y�[�X�L�[��������Ă�����ȉ��̏���
        if (_playerAction.Player.Jump.IsPressed())
        {
            //�W�����v�L�[�������ꂽ��Ԃɂ���
            _isJumpPressed = !_keyLock;
        }
        else
        {
            //�W�����v�L�[������Ă��Ȃ���Ԃɂ���
            _isJumpPressed = false;

            //�L�[���b�N����
            _keyLock = false;
        }

    }

    /// <summary>
    /// �v���C���[�̈ړ�����
    /// </summary>
    /// <returns></returns>
    private float PlayerMove()
    {
        if (_movement != Vector2.zero)
        {
            //�ړ������ɉ����Č�����ς���
            transform.localScale = new Vector3(_movement.x, this.transform.localScale.y, this.transform.localScale.z);
        }

        //X���̑��x���v�Z���A�Ԃ�
        return _moveSpeed * _movement.x;
    }

    /// <summary>
    /// �v���C���[�̃W�����v����
    /// </summary>
    private float PlayerJump()
    {
        //���݂̃X�e�[�^�X�ɉ����ď�����ς���
        switch (_playerStatus)
        {
            //�ڒn���
            case PlayerStatus.GROUND:

                //�L�[�����͂��ꂽ��W�����v��Ԃ֕ύX
                if (_isJumpPressed)
                {
                    _playerStatus = PlayerStatus.JUMPING;
                }

                break;

            //�W�����v��ԁi1�i�ځj
            case PlayerStatus.JUMPING:

                ApplyJumpPhysics();

                break;

            //�W�����v��ԁi2�i�ځj
            case PlayerStatus.DUBBLE_JUMPING:

                ApplyJumpPhysics();

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

        //Y���̑��x��Ԃ�
        return _jumpVelocity;
    }

    /// <summary>
    /// �W�����v���̏���
    /// </summary>
    private void ApplyJumpPhysics()
    {
        //�^�C�}�[�J�E���g
        _jumpTimer += Time.deltaTime;

        //���W�����v
        if (_isJumpPressed || _lowerLimitTime > _jumpTimer)
        {
            _jumpVelocity = _jumpForce;

            //���X��Y���̑��x�����炵�Ă���
            _jumpVelocity -= _addGravity * Mathf.Pow(_jumpTimer, EXPONENT);
        }
        //��W�����v
        else
        {
            //�^�C�}�[�J�E���g
            _jumpTimer += Time.deltaTime;

            _jumpVelocity = _jumpForce;

            //���X��Y���̑��x�����炵�Ă���
            _jumpVelocity -= _addGravity * Mathf.Pow(_jumpTimer, EXPONENT);
        }

        FallingJugement();
    }

    /// <summary>
    /// ����������s������
    /// </summary>
    private void FallingJugement()
    {
        //Y���̑��x��0�����������
        if(_playerRigidbody2D.linearVelocityY < 0f)
        {
            //������ԂɕύX
            _playerStatus = PlayerStatus.FALLING;

            //y�����̑��x���[���ɂ���
            _jumpVelocity = 0f;

            //�v�����鎞�Ԃɏ����l����
            _jumpTimer = INITIAL_TIMER_VALUE;

        }
    }

    /// <summary>
    /// �n���ꂽ���x���v���C���[�̑��x�ɔ��f���鏈��
    /// </summary>
    /// <param name="moveSpeed">X�������ɑ΂��鑬�x</param>
    /// <param name="jumpForse">Y�������ɑ΂��鑬�x</param>
    private void SpeedAssignment(float moveSpeed, float jumpForse)
    {
        _playerRigidbody2D.linearVelocity = new Vector2(moveSpeed, jumpForse);
    }

    /// <summary>
    /// �n�ʂɐڂ��Ă�ԍs������
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionStay2D(Collision2D collision)
    {
        //������Ԃ��ڐG�������肪�n�ʂ̏ꍇ
        if (_playerStatus == PlayerStatus.FALLING && collision.gameObject.CompareTag(GROUND_TAG))
        {
            //�ڒn��ԂɕύX
            _playerStatus = PlayerStatus.GROUND;

            //�e�ϐ���������
            _jumpCount = 0;
            _jumpVelocity = 0f;
            _jumpTimer = 0f;

            //�L�[�����b�N
            _keyLock = true;
        }
    }
}