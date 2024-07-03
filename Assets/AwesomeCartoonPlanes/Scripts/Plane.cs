using UnityEngine;

public class Plane : MonoBehaviour {
	public GameObject prop;
	public GameObject propBlured;
	private PlaneInputs _planeControl;

    private void Awake()
    {
        _planeControl = new PlaneInputs();
        _planeControl.Enable();
    }

    private void PropStop(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        prop.SetActive(true);
        propBlured.SetActive(false);
    }

    private void PropMove(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        prop.SetActive(false);
        propBlured.SetActive(true);
        propBlured.transform.Rotate(1000 * Time.deltaTime, 0, 0);
    }

    private void OnEnable()
    {
        _planeControl.Flight.Move.performed += PropMove;
        _planeControl.Flight.Move.canceled += PropStop;
    }

    private void OnDisable()
    {
        _planeControl.Flight.Move.performed -= PropMove;
        _planeControl.Flight.Move.canceled -= PropStop;
    }
}