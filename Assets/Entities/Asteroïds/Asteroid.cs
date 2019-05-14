using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public Vector2 Position { get { return (Vector2)(transform.position); } }
    public float Radius { get { return _collider.radius * _collider.transform.lossyScale.x; } }

    private CircleCollider2D _collider = null;

    void Awake()
    {
        _collider = GetComponentInChildren<CircleCollider2D>();

        GameManager.Instance.GetGameData().Asteroids.Add(this);
    }

    private void OnDestroy()
    {
        GameManager.Instance.GetGameData().Asteroids.Remove(this);
    }
}
