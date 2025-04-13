using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WedgeVisionGizmo : MonoBehaviour
{
    [SerializeField] int segmentCount = 10;
    [SerializeField] float GizmoRadiusForDetectedObjects = 0.2f;
    [SerializeField] Color color = Color.blue;

    Mesh mesh;
    Vision vision;
    Transform model;
    Collider[] colliders = new Collider[50];
    float distance = 5;
    float angle = 60;
    float height = 2;
    int count;

    public void Start()
    {
        mesh = CreateWedgeMesh();
        vision = this.GetComponent<Vision>(true);
        model = this.GetComponentInSiblings<BodyParts>().transform;
        (distance, angle, height) = vision.GetParameters();
    }



    void OnDrawGizmos()
    {
        if (!enabled)
            return;

        if (mesh != null)
        {
            Gizmos.color = color;
            Gizmos.DrawMesh(mesh, transform.position, model.rotation);
        }
        Gizmos.DrawWireSphere(transform.position, distance);
        for (int i = 0; i < count; i++)
            Gizmos.DrawSphere(colliders[i].transform.position, GizmoRadiusForDetectedObjects);

        Gizmos.color = Color.green;
        foreach (var obj in vision.detectedObjects)
            Gizmos.DrawSphere(obj.transform.position, GizmoRadiusForDetectedObjects);
    }

    Mesh CreateWedgeMesh()
    {
        Mesh mesh_ = new Mesh();

        int triangleCount = 4 * (segmentCount + 1);
        int vertixCount = 3 * triangleCount;

        int[] triangles = new int[vertixCount]; // not triangleCount ??
        Vector3[] vertices = new Vector3[vertixCount];

        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;
        Vector3 bottomLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * distance;

        Vector3 topCenter = bottomCenter + height * Vector3.up;
        Vector3 topRight = bottomRight + height * Vector3.up;
        Vector3 topLeft = bottomLeft + height * Vector3.up;

        int index = 0;

        // left side
        vertices[index++] = bottomCenter;
        vertices[index++] = bottomLeft;
        vertices[index++] = topLeft;

        vertices[index++] = topLeft;
        vertices[index++] = topCenter;
        vertices[index++] = bottomCenter;

        // right side
        vertices[index++] = bottomCenter;
        vertices[index++] = topCenter;
        vertices[index++] = topRight;

        vertices[index++] = topRight;
        vertices[index++] = bottomRight;
        vertices[index++] = bottomCenter;

        float currentAngle = -angle;
        float deltaAngle = (2 * angle) / segmentCount;
        for (int i = 0; i < segmentCount; i++)
        {
            bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * distance;
            bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * distance;
            topRight = bottomRight + height * Vector3.up;
            topLeft = bottomLeft + height * Vector3.up;

            currentAngle += deltaAngle;

            // far side
            vertices[index++] = bottomLeft;
            vertices[index++] = bottomRight;
            vertices[index++] = topRight;

            vertices[index++] = topRight;
            vertices[index++] = topLeft;
            vertices[index++] = bottomLeft;

            // top side
            vertices[index++] = topCenter;
            vertices[index++] = topLeft;
            vertices[index++] = topRight;

            // bottom side
            vertices[index++] = bottomCenter;
            vertices[index++] = bottomRight;
            vertices[index++] = bottomLeft;
        }


        for (int i = 0; i < vertixCount; i++)
            triangles[i] = i;

        mesh_.vertices = vertices;
        mesh_.triangles = triangles;
        mesh_.RecalculateNormals();

        return mesh_;
    }



}
