using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Laser/Renderer Settings")]
public class LaserRendererSettings : ScriptableObject
{
    [SerializeField] public Color color;
    [SerializeField] public float width;
    [SerializeField][Range(1f, 200f)] public float emissionAmount;

    //public void Apply(LineRenderer lineRenderer)
    //{
    //    lineRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));

    //    lineRenderer.material.EnableKeyword("_EMISSION");
    //    lineRenderer.material.SetColor("_EmissionColor", color * emissionAmount);
    //    lineRenderer.startWidth = width;
    //    lineRenderer.startColor = color;
    //}

    public void Apply(LineRenderer lineRenderer)
    {
        // Dùng Unlit để không bị ảnh hưởng bởi bóng tối
        lineRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));

        // Tạo màu HDR bằng cách lấy màu gốc nhân với độ chói
        Color hdrColor = color * emissionAmount;

        // Gán màu HDR vào thuộc tính _BaseColor của Unlit Shader
        lineRenderer.material.SetColor("_BaseColor", hdrColor);

        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width; // Thêm dòng này để đuôi laser không bị nhọn hoắt (nếu bạn muốn)
    }
}
