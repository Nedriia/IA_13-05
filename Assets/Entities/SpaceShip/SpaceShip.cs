using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShip : MonoBehaviour
{
    // Getters
    public int Owner { get { return _owner; } }
    public float Thrust { get { return _thrust; } }
    public Vector2 Velocity { get { return _rigidbody.velocity; } }
    public float SpeedMax { get { return _speedMax; } }
    public Vector2 Position { get { return (Vector2)(transform.position); } }
    public float Orientation { get { return transform.eulerAngles.z; } }
    public float Radius { get { return _collider.radius * _collider.transform.lossyScale.x; } }
    public float HitCountdown { get { return _hitCountdown; } }
    public float Energy { get { return _energy; } }
    public float MineEnergyCost { get { return _mineEnergyCost; } }
    public float ShootEnergyCost { get { return _shootEnergyCost; } }
    public int HitCount { get { return _hitCount; } }


    // Gameplay
    public Color color;
    public Mine minePrefab;
    public Bullet bulletPrefab;

    private int _owner;

    private const float _hitDuration = 3.0f;
    private float _hitSpeedFactor = 0.3f;
    private float _hitCountdown = 0.0f;
    private int _hitCount = 0;

    private float _energy = 1.0f;
    private float _mineEnergyCost = 0.4f;
    private float _shootEnergyCost = 0.15f;
    private float _energyPerSecond = 0.1f;

    // Visual
    public Material hitMaterial = null;
    private MeshRenderer _meshRenderer = null;

    // Spaceship speed
    private float _thrust = 0.0f;
    private const float _speedForThrust = 5.0f;
    private const float _speedMax = 2.5f;

    // Spaceship orientation
    private float _orientationTarget = 0.0f;
    private const float _rotationSpeed = 180.0f;

    private BaseSpaceShipController _controller = null;

    // Spaceship physics
    private Rigidbody2D _rigidbody = null;
    private CircleCollider2D _collider = null;
    private ParticleSystem _thrustFX = null;

    private Vector2 previousPosition = Vector2.zero;
    private float previousRotation = 0.0f;
    private bool willCheckRigidbody = false;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponentInChildren<CircleCollider2D>();
        previousPosition = _rigidbody.position;
        previousRotation = _rigidbody.rotation;
        _thrustFX = GetComponentInChildren<ParticleSystem>();
        ParticleSystem.EmissionModule emission = _thrustFX.emission;
        emission.enabled = false;
        _orientationTarget = transform.eulerAngles.z;
        _meshRenderer = GetComponentInChildren<MeshRenderer>();

        GameManager.Instance.GetGameData().SpaceShips.Add(this);
    }

    private void OnDestroy()
    {
        GameManager.Instance.GetGameData().SpaceShips.Remove(this);
    }


    private void Update()
    {
        if(IsHit())
        {
            _hitCountdown = Mathf.Max(_hitCountdown - Time.deltaTime, 0);
            if(_hitCountdown <= 0)
            {
                _meshRenderer.materials = new Material[1] { _meshRenderer.materials[0] };
            }
        }

        _energy = Mathf.Clamp01(_energy + _energyPerSecond * Time.deltaTime);

        if (_controller == null)
            return;

        InputData inputData = _controller.UpdateInput(this, GameManager.Instance.GetGameData());
        _thrust = Mathf.Clamp01(inputData.thrust);
        _orientationTarget = Mathf.Repeat(inputData.targetOrientation, 360.0f);
        ParticleSystem.EmissionModule emission = _thrustFX.emission;
        emission.enabled = _thrust > 0;

        if(inputData.shoot)
        {
            Shoot();
        }
        if (inputData.dropMine)
        {
            DropMine();
        }
    }

    private void LateUpdate()
    {
        willCheckRigidbody = true;
        previousPosition = _rigidbody.position;
        previousRotation = _rigidbody.rotation;
    }


    private void FixedUpdate()
    {
        /*if(!CheckRigidbody())
            return;*/
        float speedMax = _speedMax * (IsHit() ? _hitSpeedFactor : 1.0f);
        Vector2 direction = new Vector2(Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(transform.eulerAngles.z * Mathf.Deg2Rad));
        _rigidbody.velocity = Vector3.ClampMagnitude(_rigidbody.velocity + direction * _thrust * _speedForThrust * Time.fixedDeltaTime, speedMax);
        if(Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.z, _orientationTarget)) > Mathf.Epsilon)
        {
            float deltaRotation = _rotationSpeed * Time.fixedDeltaTime;
            if(Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.z, _orientationTarget)) < deltaRotation)
            {
                transform.eulerAngles = new Vector3(0.0f,0.0f,_orientationTarget);
            } else
            {
                transform.eulerAngles += new Vector3(0.0f, 0.0f, deltaRotation * Mathf.Sign(Mathf.DeltaAngle(transform.eulerAngles.z, _orientationTarget)));
            }
        }
    }

    private bool CheckRigidbody()
    {
        if(!willCheckRigidbody)
            return true;

        if (_rigidbody.position != previousPosition)
        {
            Debug.LogError("GameObject position was changed elsewhere. Be sure not to change position directly in your controller.");
            return false;
        }
        if (_rigidbody.rotation != previousRotation)
        {
            Debug.LogError("GameObject rotation was changed elsewhere. Be sure not to change rotation directly in your controller.");
            return false;
        }
        return true;
    }

    public void Initialize(BaseSpaceShipController controller, int owner)
    {
        _controller = controller;
        _owner = owner;
    }

    public void DropMine()
    {
        if (_energy < _mineEnergyCost)
            return;
        GameObject.Instantiate(minePrefab, transform.position, Quaternion.identity);
        _energy -= _mineEnergyCost;
    }

    public void Shoot()
    {
        if (_energy < _shootEnergyCost)
            return;
        Bullet spawned = GameObject.Instantiate<Bullet>(bulletPrefab, transform.position, transform.rotation);
        spawned.SetOwner(Owner);
        _energy -= _shootEnergyCost;
    }

    public void OnHitMine(Mine mine)
    {
        if (!mine.IsHitting())
            return;
        Hit();
    }

    public void OnHitBullet(Bullet bullet)
    {
        if (!bullet.IsHitting())
            return;
        Hit();
    }

    private void Hit()
    {
        if (!IsHit())
        {
            _hitCount++;
        }

        _hitCountdown = _hitDuration;
        _meshRenderer.materials = new Material[2] { _meshRenderer.materials[0], hitMaterial };
    }

    public bool IsHit() {
        return _hitCountdown > 0.0f;
    }
}
