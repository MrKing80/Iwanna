using UnityEngine;
using UnityEngine.UIElements;

public class PlayerDeath : MonoBehaviour
{
    private const string KILLER_TAG = "Killer";
    private bool _isDeath = false;
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        DeathPlayer();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag(KILLER_TAG))
        {
            _isDeath = true;
        }
    }
    private void OnBecameInvisible()
    {
        _isDeath = true;
    }

    private void DeathPlayer()
    {
        if (_isDeath)
        {
            print("‚ ‚È‚½‚ÍŽ€‚É‚Ü‚µ‚½");
            this.gameObject.SetActive(false);
        }
    }
}
