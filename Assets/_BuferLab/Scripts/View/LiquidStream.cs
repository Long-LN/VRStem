using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LiquidStream : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private int resolution = 20; 
    public Material streamMaterial; // Keo Unlit Transparent Material vao day

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = resolution;
        if (streamMaterial != null) lineRenderer.material = streamMaterial;
        lineRenderer.enabled = false;
        lineRenderer.numCapVertices = 5; 
        lineRenderer.numCornerVertices = 5;
    }

    public void BeginPour(Color liquidColor, float width)
    {
        lineRenderer.enabled = true;
        lineRenderer.startColor = liquidColor;
        lineRenderer.endColor = liquidColor;
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width * 0.5f; 
    }

    public void UpdateParabola(Vector3 startPos, Vector3 endPos, Vector3 initialVelocity)
    {
        lineRenderer.positionCount = resolution;
        float heightDifference = Mathf.Abs(startPos.y - endPos.y);
        float timeToHit = Mathf.Sqrt(2f * heightDifference / Mathf.Abs(Physics.gravity.y));
        if (timeToHit <= 0.01f) timeToHit = 0.1f;

        for (int i = 0; i < resolution; i++)
        {
            float t = i / (float)(resolution - 1) * timeToHit;
            Vector3 currentPos = startPos + initialVelocity * t + 0.5f * Physics.gravity * t * t;
            if (currentPos.y < endPos.y) currentPos.y = endPos.y;
            lineRenderer.SetPosition(i, currentPos);
        }
    }

    public void EndPour()
    {
        lineRenderer.enabled = false;
    }
}