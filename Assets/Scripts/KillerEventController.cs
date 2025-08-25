using UnityEngine;

public class KillerEventController : MonoBehaviour
{
    [SerializeField] private GameObject[] _killers = default;
    [SerializeField] private TrapType _trapType = default;
    [SerializeField] private float _injectionspeed = 0f;
    [SerializeField] private Vector2 _moveEndPoint = Vector2.zero;
    private enum TrapType
    {
        Pitfall,        //���Ƃ���
        FallingObjects, //�I�u�W�F�N�g���������Ă���
        JumpOutObjects, //�I�u�W�F�N�g���򗈂��Ă���
        MoveObjects,    //�I�u�W�F�N�g���ړ�����
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (_trapType)
        {
            case TrapType.Pitfall:

                for (int i = 0; i < _killers.Length; i++)
                {
                    _killers[i].gameObject.SetActive(false);
                }

                break;
            case TrapType.FallingObjects:


                break;
            case TrapType.JumpOutObjects:
                break;

            case TrapType.MoveObjects:
                break;
            default:
                break;
        }



    }
}
