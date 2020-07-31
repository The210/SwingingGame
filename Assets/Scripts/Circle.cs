using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Circle : MonoBehaviour 
{
    public int vertexCount = 40;
    public float lineWidth ;
    public float fadeOutSpeed;
    private Material material;
    public float radius;
    private LineRenderer lineRenderer;
    

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        setupCricle();
    }

    private void Update()
    {
        fadeOutSpeed += Time.deltaTime * 0.5f;
        Color m_color = Color.Lerp(new Color(1f, 1f, 1f, 1f), new Color(0f, 0f, 0f, 0f), fadeOutSpeed);
        Debug.Log(m_color);
        lineRenderer.material.color = m_color;
        radius += 0.01f;
        if (m_color == new Color(0f, 0f, 0f, 0f))
        {
            GameObject.Destroy(gameObject);
        }
        setupCricle();
    }

    void setupCricle()
    {
        lineRenderer.widthMultiplier = lineWidth;
        float deltaTheta = (2f * Mathf.PI) / vertexCount;
        float theta = 0f;
        lineRenderer.positionCount = vertexCount;
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            Vector3 pos = new Vector3(radius * Mathf.Cos(theta), radius * Mathf.Sin(theta), 0f) + transform.position;
            lineRenderer.SetPosition(i, pos);
            theta += deltaTheta;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        float deltaTheta = (2f * Mathf.PI) / vertexCount;
        float theta = 0f;

        Vector3 oldPos = Vector3.zero;
        for (int i = 0; i < vertexCount + 1; i++)
        {
            Vector3 pos = new Vector3(radius * Mathf.Cos(theta), radius * Mathf.Sin(theta), 0f);
            Gizmos.DrawLine(oldPos, transform.position + pos);
            oldPos = transform.position + pos;
            theta += deltaTheta;
        }
    }
#endif
}
