using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    const string ANIM_ON_CHANGE_OWNER = "OnChangeOwner";

    public int Owner { get { return _owner; } }
    public float Radius { get { return _collider.radius * _collider.transform.lossyScale.x; } }
    public Vector2 Position { get { return (Vector2)(transform.position); } }

    private int _owner = -1;
    private Animator _animator = null;
    private List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
    private CircleCollider2D _collider = null;

    void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        GetComponentsInChildren<SpriteRenderer>(spriteRenderers);
        _collider = GetComponentInChildren<CircleCollider2D>();

        GameManager.Instance.GetGameData().WayPoints.Add(this);
    }

    private void OnDestroy()
    {
        GameManager.Instance.GetGameData().WayPoints.Remove(this);
    }

    void LateUpdate()
    {
        _animator.ResetTrigger(ANIM_ON_CHANGE_OWNER);
    }

    void SetOwner(int newOwner, Color color)
    {
        _owner = newOwner;
        _animator.SetTrigger(ANIM_ON_CHANGE_OWNER);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {            
            SpaceShip spaceShip = collision.attachedRigidbody.GetComponent<SpaceShip>();
            if (spaceShip.Owner != _owner)
            {
                _owner = spaceShip.Owner;
                Color shipColor = spaceShip.color;
                _animator.SetTrigger(ANIM_ON_CHANGE_OWNER);
                foreach (SpriteRenderer spriteRenderer in spriteRenderers)
                {
                    shipColor.a = spriteRenderer.color.a;
                    spriteRenderer.color = shipColor;
                }
            }
        }
    }

}
