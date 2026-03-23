using System;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class OvalRing : MonoBehaviour
{
    [Header("Ellipse Shape (Base Shape)")]
    public float radiusX = 2f;
    public float radiusY = 1f;

    [Header("Ring Settings")]
    [Range(10, 200)]
    public int segments = 100;
    public float lineWidth = 0.05f;
    
    private SolarSystemFocus solarSystemFocus;

    public float worldRadiusX;
    public float worldRadiusY;

    private LineRenderer lineRenderer;
    // private MeshCollider meshCollider;

    private Vector3[] points;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        // meshCollider = GetComponent<MeshCollider>();

        lineRenderer.loop = true;
        lineRenderer.useWorldSpace = false;
        lineRenderer.widthMultiplier = lineWidth;
    }

    void Start()
    {
        solarSystemFocus = SolarSystemFocus.Instance;
        DrawOval();
    }

    private void Update()
    {
        worldRadiusX = solarSystemFocus.solarRoot.transform.lossyScale.x;
        worldRadiusY = solarSystemFocus.solarRoot.transform.lossyScale.y;
    }

    public void SetRingVisible(bool visible)
    {
        lineRenderer.enabled = visible;
    }

    public Vector3 GetPoint(float angle)
    {
        float x = Mathf.Cos(angle) * radiusX;
        float z = Mathf.Sin(angle) * radiusY;

        return transform.TransformPoint(new Vector3(x, 0, z));
    }

    void DrawOval()
    {
        lineRenderer.positionCount = segments;
        points = new Vector3[segments];

        float angle = 0f;

        for (int i = 0; i < segments; i++)
        {
            float x = Mathf.Cos(angle) * radiusX;
            float z = Mathf.Sin(angle) * radiusY;

            Vector3 pos = new Vector3(x, 0, z);
            points[i] = pos;

            lineRenderer.SetPosition(i, pos);

            angle += (2 * Mathf.PI) / segments;
        }
    }
    
    [ContextMenu("Rebuild Ring")]
    public void RebuildRing()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();
    
        lineRenderer.loop = true;
        lineRenderer.useWorldSpace = false;
        lineRenderer.widthMultiplier = lineWidth;
    
        DrawOval();
    }

    public void SetVisibility(float alpha)
    {
        Color c = lineRenderer.startColor;
        c.a = alpha;

        lineRenderer.startColor = c;
        lineRenderer.endColor = c;
    }
    
    public float GetRadius()
    {
        float scaleX = transform.lossyScale.x;
        float scaleZ = transform.lossyScale.z;

        float rX = radiusX * scaleX;
        float rY = radiusY * scaleZ;

        return (rX + rY) * 0.5f;
    }
}