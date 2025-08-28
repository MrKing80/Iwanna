using UnityEditor;
using UnityEngine;

public class SaveObjectController : MonoBehaviour
{
    private Animator _animator = default;
    private const string APPLY_SAVE_ANIMATION = "ApplySave";
    private const string BULLET_TAG = "Bullet";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag(BULLET_TAG))
        {
            _animator.SetTrigger(APPLY_SAVE_ANIMATION);

            SaveManager._saveManagerInstance.SavePoint = this.transform.position;
        }
    }
}
