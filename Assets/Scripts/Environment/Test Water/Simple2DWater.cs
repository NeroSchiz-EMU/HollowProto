using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class Simple2DWater : MonoBehaviour
{
    // Set to height of water in game units
    public float height = 5;

    // Set to width of water in game units
    public float width = 5;

    // Increase for smoother looking water, decrease for faster performance
    public float pointsPerUnit = 10;

    // When using in edit mode, these track changes to public properites for automatic rebuilding
#if UNITY_EDITOR
    float prevHeight;
    float prevWidth;
    float prevPointsPerUnit;
#endif

    // Required component, you should also have a Mesh Renderer if you want to see the water
    //  I use a 2 pixel wide texture -- just simple vertical gradient with alpha channel and unlit/transparent render
    MeshFilter meshFilter;

    // Keep track of some physics info for each point
    float[] velocities;
    float[] accelerations;
    float[] leftDeltas;
    float[] rightDeltas;

    // Our point count, set by Build, just store here because used a lot.
    int pointCount;

    // Some constants, you may need to adjust.
    const float springConstant = 0.02f;
    const float damping = 0.04f;
    const float spread = 0.05f;

    // Build in OnEnable/Awake... OnEnable works better at design-time (edit mode), Awake better for game/play mode.
#if UNITY_EDITOR
    void OnEnable()
#else
    void Awake()
#endif
    {
        Build();
    }

    // Create very simple tris for our mesh.
    int[] GetMeshTriangles(Vector3[] verts)
    {
        int[] triangles = new int[verts.Length * 3];
        for (int i = 0; i < verts.Length - 3; i += 2)
        {
            int t = i * 3;
            triangles[t] = i;
            triangles[t + 1] = i + 1;
            triangles[t + 2] = i + 3;

            triangles[t + 3] = i + 3;
            triangles[t + 4] = i + 2;
            triangles[t + 5] = i;
        }
        return triangles;
    }

    // Create very simple UVs for our mesh.
    Vector2[] GetUVMapping(Vector3[] verts)
    {
        Vector2[] uvs = new Vector2[verts.Length];
        for (int i = 0; i < verts.Length; i++)
        {
            float xMapping;
            float yMapping;

            if (i % 2 == 0)
            {
                yMapping = 0;
                xMapping = 0;
            }
            else
            {
                yMapping = 1;
                xMapping = 1;
            }

            uvs[i] = new Vector2(xMapping, yMapping);
        }

        return uvs;
    }

    // Build our mesh, only done once in game mode, in edit mode rebuilds if public properties change.
    public void Build()
    {
        meshFilter = GetComponent<MeshFilter>();

        // Calculate number of points based on public properties
        pointCount = Mathf.CeilToInt(width * pointsPerUnit);
        if (pointCount < 1)
            return;

        // Calculate units per point
        float unitsPerPoint = width / (float)pointCount;

        // Add one extra point for the end
        pointCount++;

        // Create our arrays just once
        velocities = new float[pointCount];
        accelerations = new float[pointCount];
        leftDeltas = new float[pointCount];
        rightDeltas = new float[pointCount];

        // Initialize all mesh verts, we just use 0 base, you can move the transform around as needed to position mesh in game
        Vector3[] verts = new Vector3[pointCount * 2];
        for (int i = 0; i < pointCount; i++)
        {
            verts[i * 2].x = i * unitsPerPoint;
            verts[i * 2].y = -height;
            verts[i * 2].z = 0;
            verts[(i * 2) + 1].x = i * unitsPerPoint;
            verts[(i * 2) + 1].y = 0;
            verts[(i * 2) + 1].z = 0;
        }

        // Assign mesh to MeshFilter and create some basic tris and UVs
        Mesh mesh = new Mesh();
        mesh.vertices = verts;
        mesh.triangles = GetMeshTriangles(verts);
        mesh.uv = GetUVMapping(verts);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        GameObject.DestroyImmediate(meshFilter.sharedMesh);
        meshFilter.sharedMesh = mesh;
    }

    // I use fixed update since I plan on adding some extra physics here, and Unity says that should be done in FixedUpdate (not Update).
    void FixedUpdate()
    {
        if (pointCount < 1)
            return;

        // You can use this for testing, will apply a splash on click, based on current mouse position.
        //  This is a good place to try different velocity values to see what works best with your setup.
#if false
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 inputPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Splash(inputPos.x, 5f);
            Debug.Log("Splash: " + inputPos.x);
        }
#endif

        // Get verts from our mesh
        Vector3[] verts = meshFilter.sharedMesh.vertices;

        // Apply splash from contact by other objects.
        //  For each point except the last...
        for (int i = 0; i < pointCount - 1; i++)
        {
            Vector2 v1 = verts[(i * 2) + 1];
            v1 = transform.TransformPoint(v1);
            Vector2 v2 = verts[((i + 1) * 2) + 1];
            v2 = transform.TransformPoint(v2);

            // Check for objects between points.
            RaycastHit2D hit = Physics2D.Linecast(v1, v2);
            if (hit.rigidbody == null)
                continue;

            // If object found, apply splash.  You may need to adjust the velocity part.
            //Debug.Log(hit.rigidbody.gameObject);
            Splash(hit.point.x, hit.rigidbody.velocity.y / 20f);
        }

        // Apply damping, velocity, and acceleration.
        for (int i = 0; i < pointCount; i++)
        {
            float y = verts[(i * 2) + 1].y;
            float force = springConstant * y + velocities[i] * damping;
            accelerations[i] = -force;
            y += velocities[i];
            velocities[i] += accelerations[i];
            verts[(i * 2) + 1].y = y;
        }

        // Apply wave motion (adjust adjacent points), uses multiple passes for more realistic motion.
        for (int j = 0; j < 8; j++)
        {
            for (int i = 0; i < pointCount; i++)
            {
                float y = verts[(i * 2) + 1].y;
                if (i > 0)
                {
                    float prevY = verts[((i - 1) * 2) + 1].y;
                    leftDeltas[i] = spread * (y - prevY);
                    velocities[i - 1] += leftDeltas[i];
                }
                if (i < pointCount - 1)
                {
                    float nextY = verts[((i + 1) * 2) + 1].y;
                    rightDeltas[i] = spread * (y - nextY);
                    velocities[i + 1] += rightDeltas[i];
                }
            }
        }

        // Update verts based on wave motion we calculated above.
        for (int i = 0; i < pointCount; i++)
        {
            if (i > 0)
            {
                verts[((i - 1) * 2) + 1].y += leftDeltas[i];
            }
            if (i < pointCount - 1)
            {
                verts[((i + 1) * 2) + 1].y += rightDeltas[i];
            }
        }

        // Apply updates to mesh
        meshFilter.sharedMesh.vertices = verts;
    }

    // Cause a splash!
    void Splash(float xpos, float velocity)
    {
        //Debug.Log("Splash: " + velocity);
        if (pointCount < 1)
            return;

        xpos = transform.InverseTransformPoint(xpos, 0, 0).x;

        // If xpos is left or right of water, we just treat as end of water, you could ignore the hit
        //  instead, if you want.
        float unitsPerPoint = width / (float)pointCount;
        int pointIndex = Mathf.RoundToInt(xpos / unitsPerPoint);
        if (pointIndex < 0)
            pointIndex = 0;
        else if (pointIndex >= pointCount)
            pointIndex = pointCount - 1;

        Debug.Log("Splash: " + pointIndex + " " + velocity);

        // Just set the velocity of the point that was hit.
        velocities[pointIndex] = velocity;
    }

    // This only runs in edit mode (design-time) and just rebuilds our mesh anytime public properties change.
#if UNITY_EDITOR
    void OnRenderObject()
    {
        bool changed = false;
        if (prevHeight != height ||
            prevWidth != width ||
            prevPointsPerUnit != pointsPerUnit)
            changed = true;
        if (changed)
        {
            Build();
            UnityEditor.SceneView.RepaintAll();
            prevHeight = height;
            prevWidth = width;
            prevPointsPerUnit = pointsPerUnit;
        }
    }
#endif
}
