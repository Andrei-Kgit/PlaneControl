using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlaneMovement : MonoBehaviour, IControllable
{
    [Header("Movement")]
    [SerializeField] private float _throttleIncrement = 10f;
    [SerializeField] private float _minThrustSpeed = 20f;
    [SerializeField] private float _maxThrustSpeed = 100f;
    [SerializeField] private float _liftForceMultiplier = 0.1f;
    [Header("Rotation")]
    [SerializeField, Range(0.1f, 10f)] private float _responsivness = 2f;
    [SerializeField] private float _maxResponsivnessModifier = 50f;
    [Header("Alignment")]
    [SerializeField] private float _smoothAlignmentSpeed = 5f;
    [Header("GroundCheck")]
    [SerializeField] private LayerMask _isGround;
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private float _checkDistance = 1.5f;

    private Rigidbody _rb;

    private Vector2 _rotationInput;

    private float _thrustForce;
    private float _rollForce;
    private float _yawForce;
    private float _pitchForce;

    private float _liftForce;

    private bool _isLanding;
    private bool _isLanded;
    private bool _isCrashed;
    private bool _inActiveFly;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        if (_groundCheckPoint == null)
            _groundCheckPoint = transform;

        _isLanded = Physics.Raycast(_groundCheckPoint.position, -transform.up, _checkDistance, _isGround);
    }

    private void Update()
    {
        GroundCheck();
        ActiveFlyCheck();
        LandingCheck();
    }

    private void FixedUpdate()
    {
        ApplyForces();
        SmoothAlignment();
    }

    public void Move(Vector2 move)
    {
        ThrottleForce(move.y);
        YawForce(move.x);
    }

    public void Rotate(Vector2 rotate)
    {
        _rotationInput = rotate;
        PitchForce(rotate.y);
        RollForce(rotate.x);
    }

    private void SmoothAlignment()
    {
        if (!_isCrashed)
        {
            Quaternion quaternion = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);
            Quaternion deltaRotation = Quaternion.RotateTowards(transform.rotation, quaternion, _smoothAlignmentSpeed * Time.deltaTime);

            if (_rotationInput.magnitude < 1)
                _rb.MoveRotation(deltaRotation);
        }
    }

    private void LandingCheck()
    {
        if (_isLanded && _thrustForce > _minThrustSpeed)
            _isLanding = false;
        else
            _isLanding = true;
    }

    private void ActiveFlyCheck()
    {
        if (_isLanded)
        {
            _inActiveFly = false;
        }
        else if (_thrustForce > _minThrustSpeed)
        {
            _inActiveFly = true;
        }
    }

    private void GroundCheck()
    {
        _isLanded = Physics.Raycast(_groundCheckPoint.position, Vector3.down, _checkDistance, _isGround);
    }

    public void ApplyForces()
    {
        if (!_isCrashed)
        {
            float magnitude = _rb.velocity.magnitude;
            if (_isLanded && !_isLanding)
            {
                if (magnitude >= _thrustForce - 1f)
                    _liftForce = Mathf.Lerp(_liftForce, 1, 0.3f * Time.deltaTime);
                else
                    _liftForce = Mathf.Lerp(_liftForce, 0, 0.3f * Time.deltaTime);
            }
            else
            {
                _liftForce = Mathf.Lerp(_liftForce, 0, 0.6f * Time.deltaTime);
            }
            Vector3 moveDir = new Vector3(0, _liftForce * _liftForceMultiplier, 0);
            _rb.velocity = (transform.forward + moveDir) * _thrustForce;

            float responseModifier = Mathf.Clamp(magnitude, 0.5f, _maxResponsivnessModifier);

            _rb.AddTorque(transform.up * _yawForce * _responsivness * responseModifier);
            _rb.AddTorque(transform.right * _pitchForce * _responsivness * responseModifier);
            _rb.AddTorque(-transform.forward * _rollForce * _responsivness * responseModifier);

        }
        else
        {
            Debug.Log($"Is crashed {_isCrashed} || Is landed {_isLanded}");
        }
    }

    public void ThrottleForce(float throttleInput)
    {
        _thrustForce += throttleInput * _throttleIncrement * Time.deltaTime;

        _thrustForce = Mathf.Clamp(_thrustForce, _inActiveFly ? _minThrustSpeed : 0, _maxThrustSpeed);
    }
    public void RollForce(float rollInput)
    {
        _rollForce = rollInput;
    }
    public void YawForce(float yawInput)
    {
        _yawForce = yawInput;
    }
    public void PitchForce(float pitchInput)
    {
        _pitchForce = pitchInput;
    }

    private void Crash()
    {
        _isCrashed = true;
    }
    private void Rise()
    {
        _isCrashed = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_isCrashed && !_isLanded)
        {
            Crash();
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (_isCrashed)
        {
            Invoke("Rise", 2f);
        }
    }
}
