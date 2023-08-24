using UnityEngine;

public class LineRendererExample : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public float animationSpeed = 1f;
    public bool Reset = false;

    private LineRenderer lineRenderer;
    private float t = 0f;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
    }

    private void Update()
    {
        t += Time.deltaTime * animationSpeed;
        if (t > 1f)
        {
            t = 1f;
            // Animación completada, hacer algo si es necesario
        }

        Vector3 startPos = Vector3.Lerp(startPoint.position, endPoint.position, t);
        Vector3 endPos = endPoint.position;

        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);

        if (Reset)
        {
            t = 0;
            Reset = false;
        }
    }
}
