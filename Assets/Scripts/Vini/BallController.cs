using UnityEngine;
using UnityEngine.InputSystem;

public class BallController : MonoBehaviour
{
    [Header("Physics")]
    [SerializeField] private float _tiltForce = 20f;
    [SerializeField] private float _maxSpeed = 12f;
    [SerializeField] private float _smoothing = 0.15f;

    [Header("Tilt Inversion")]
    [SerializeField] private bool _invertX;
    [SerializeField] private bool _invertY;

    private Rigidbody _rb;
    private Vector3 _smoothedAccel;
    private Vector3 _spawnPosition;
    private bool _active;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _spawnPosition = transform.position;
    }

    private void Start()
    {
        if (Accelerometer.current != null)
            InputSystem.EnableDevice(Accelerometer.current);

        SetActive(false);
    }

    /// <summary>Enable or disable ball physics (called by GameManager).</summary>
    public void SetActive(bool active)
    {
        _active = active;
        _rb.isKinematic = !active;
    }

    /// <summary>Resets ball to spawn position.</summary>
    public void ResetBall()
    {
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        transform.position = _spawnPosition;
    }

    private void FixedUpdate()
    {
        if (!_active) return;
        ApplyTilt();
        ClampSpeed();
    }

    private void ApplyTilt()
    {
        if (Accelerometer.current == null) return;

        Vector3 raw = Accelerometer.current.acceleration.ReadValue();
        _smoothedAccel = Vector3.Lerp(_smoothedAccel, raw, _smoothing);

        float x = _invertX ? _smoothedAccel.x : -_smoothedAccel.x;
        float z = _invertY ? _smoothedAccel.y : -_smoothedAccel.y;

        _rb.AddForce(new Vector3(x, 0f, z) * _tiltForce, ForceMode.Acceleration);
    }

    private void ClampSpeed()
    {
        Vector3 flat = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
        if (flat.magnitude > _maxSpeed)
        {
            flat = flat.normalized * _maxSpeed;
            _rb.linearVelocity = new Vector3(flat.x, _rb.linearVelocity.y, flat.z);
        }
    }
}
