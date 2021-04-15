using UnityEngine;

public class LayerLogic : MonoBehaviour
{

    [SerializeField] private Transform bottomOfObject;
    [SerializeField] private BoxCollider2D collider;

    private Vector2 _boxColliderOriginalOffset;
    private GameObject _player;

    private SpriteRenderer _renderer;
    
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindWithTag("Player");
        _renderer = this.GetComponent<SpriteRenderer>();
        
        if(collider != null)
            _boxColliderOriginalOffset = collider.offset;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag(_player.tag))
        {
            if (_player.transform.position.y > bottomOfObject.position.y)
            {
                if (collider != null)
                {
                    collider.offset = new Vector2(_boxColliderOriginalOffset.x, -1.07F);
                }

                _renderer.sortingOrder = 5;
            }
            else if (_player.transform.position.y < bottomOfObject.position.y)
            {
                if (collider != null)
                {
                    collider.offset = _boxColliderOriginalOffset;
                }


                _renderer.sortingOrder = 1;
            }
        }    
    }
}
