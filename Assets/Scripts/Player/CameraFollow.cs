using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Camera Limits")]
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    [Header("Smoothing")]
    public float smoothSpeed = 0.1f;

    private Camera _cam;

    void Start()
    {
        _cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (player == null) return;

        // Calcula o tamanho da câmera para limitar corretamente
        float camHeight = _cam.orthographicSize;
        float camWidth  = camHeight * _cam.aspect;

        // Segue o player com suavização
        Vector3 target = new Vector3(player.position.x, player.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, target, smoothSpeed);

        // Trava nas bordas
        float clampedX = Mathf.Clamp(transform.position.x, minX + camWidth,  maxX - camWidth);
        float clampedY = Mathf.Clamp(transform.position.y, minY + camHeight, maxY - camHeight);

        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }
}