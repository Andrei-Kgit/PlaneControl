using UnityEngine;

public class InputsVisualizer : MonoBehaviour
{
    [SerializeField] private Transform _leftObj;
    [SerializeField] private Transform _rightObj;
    [SerializeField] private float _distToBorder;
    private Vector2 _moveInput;
    private Vector2 _rotationInput;
    
    private void Start()
    {
        _leftObj.localPosition = Vector3.zero;
        _rightObj.localPosition = Vector3.zero;
    }

    public void SetInputs(Vector2 moveInput, Vector2 rotationInput)
    {
        _moveInput = moveInput;
        _rotationInput = rotationInput;
    }

    private void LateUpdate()
    {
        UpdateStickPosition(_leftObj, _moveInput.x, _moveInput.y);
        UpdateStickPosition(_rightObj, _rotationInput.x, _rotationInput.y);
    }

    private void UpdateStickPosition(Transform obj, float xInput, float yInput)
    {
        Vector3 newPos = Vector3.zero + new Vector3(xInput * obj.localScale.x, yInput * obj.localScale.y, 0);
        obj.localPosition = newPos * _distToBorder;
    }
}
