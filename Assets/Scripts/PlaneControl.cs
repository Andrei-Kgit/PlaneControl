using UnityEngine;

[RequireComponent(typeof(IControllable))]
public class PlaneControl : MonoBehaviour
{
    private IControllable _controllable;
    private PlaneInputs _planeInputs;

    private void Awake()
    {
        _planeInputs = new PlaneInputs();
        _planeInputs.Enable();

        _controllable = GetComponent<IControllable>();
    }

    private void Update()
    {
        GetInputs();
    }

    private void GetInputs()
    {
        var moveDirection = _planeInputs.Flight.Move.ReadValue<Vector2>();
        var rotatonDirection = _planeInputs.Flight.Rotation.ReadValue<Vector2>();
        _controllable.Move(moveDirection);
        _controllable.Rotate(rotatonDirection);
    }
}
