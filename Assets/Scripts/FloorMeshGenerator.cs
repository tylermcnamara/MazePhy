using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class FloorMeshGenerator : MonoBehaviour
{
    private Mesh mesh_;
    private Vector3[] vertices_;
    private int[] triangles_;

    [SerializeField] private float noiseScale = 0.5f;
    [SerializeField] private float heightScale = 0.5f;

    public void GenerateMesh(int mazeRows, int mazeCols)
    {
        int width = mazeCols;
        int height = mazeRows;

        mesh_ = new Mesh();
        mesh_.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        GetComponent<MeshFilter>().mesh = mesh_;

        vertices_ = new Vector3[(width + 1) * (height + 1)];
        triangles_ = new int[width * height * 6];

        int v = 0;
        for (int z = 0; z <= height; z++)
        {
            for (int x = 0; x <= width; x++)
            {
                float y = Mathf.PerlinNoise(x * noiseScale, z * noiseScale) * heightScale;
                vertices_[v++] = new Vector3(x, y, -z); // Flip Z for top-down alignment
            }
        }

        int t = 0;
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                int i = z * (width + 1) + x;

                triangles_[t++] = i;
                triangles_[t++] = i + 1;
                triangles_[t++] = i + width + 1;

                triangles_[t++] = i + 1;
                triangles_[t++] = i + width + 2;
                triangles_[t++] = i + width + 1;
            }
        }

        UpdateMesh_();
    }

    private void UpdateMesh_()
    {
        mesh_.Clear();
        mesh_.vertices = vertices_;
        mesh_.triangles = triangles_;
        mesh_.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh_;

        MeshCollider collider = GetComponent<MeshCollider>();
        if (collider == null)
            collider = gameObject.AddComponent<MeshCollider>();

        collider.sharedMesh = null;
        collider.sharedMesh = mesh_;
    }
}