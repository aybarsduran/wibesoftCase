using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float CameraMoveSpeed = 10f; 
    public float ZoomSpeed = 2f;
    public Vector2 MoveLimitMin, MoveLimitMax; //Borders
    public float MinZoom = 30f, MaxZoom = 60f;

    private Vector3 _lastMousePosition;
    private Vector3 _targetPosition;
    private float _targetZoom;

    void Start()
    {
        _targetPosition = transform.position;
        _targetZoom = Camera.main.fieldOfView;
    }

    void Update()
    {
        HandleMovement();
        HandleZoom();
        SmoothMove();
    }

    void HandleMovement()
    {
        Vector3 move = Vector3.zero;

        // Mouse Controls
        if (Input.GetMouseButtonDown(0))
        {
            _lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - _lastMousePosition;
            move = new Vector3(-delta.x, 0, -delta.y) * CameraMoveSpeed * Time.deltaTime;
            _lastMousePosition = Input.mousePosition;
        }

        // Mobil Controls
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                move = new Vector3(-touch.deltaPosition.x, 0, -touch.deltaPosition.y) * CameraMoveSpeed * 0.01f;
            }
        }

        _targetPosition += move;
        _targetPosition.x = Mathf.Clamp(_targetPosition.x, MoveLimitMin.x, MoveLimitMax.x);
        _targetPosition.z = Mathf.Clamp(_targetPosition.z, MoveLimitMin.y, MoveLimitMax.y);
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        float zoomChange = scroll * ZoomSpeed;

        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            float prevDistance = (touch0.position - touch0.deltaPosition - (touch1.position - touch1.deltaPosition)).magnitude;
            float currentDistance = (touch0.position - touch1.position).magnitude;
            zoomChange = (currentDistance - prevDistance) * 0.005f; 
        }

        _targetZoom = Mathf.Clamp(_targetZoom - zoomChange, MinZoom, MaxZoom);
    }

    void SmoothMove()
    {
        transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * 10f);
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, _targetZoom, Time.deltaTime * 5f);
    }
}
