using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    const int MAX_HEIGHT = 256;
    const int SIZE = 16;
    GameObject terrain;
    float terrainScale;

    public Vector2Int chunkCoordinates;
    Vector2Int blockCoordinates;
    Vector2 noiseOffset;

    public int[,,] blockmap;
    public int[,,] verticiesMap;

    public void Generate(Vector2Int coords, float tScale, Vector2 nO, Material grassMaterial, Material grassSideMaterial)
    {
        verticiesMap = new int[((SIZE) + 1) * ((SIZE) + 1), ((SIZE) + 1) * ((SIZE) + 1), ((SIZE) + 1) * ((SIZE) + 1)];
        //blockmap = new int[SIZE, MAX_HEIGHT, SIZE];

        terrainScale = tScale;

        noiseOffset = nO;

        chunkCoordinates = coords;

        blockCoordinates = new Vector2Int((chunkCoordinates.x) * SIZE, (chunkCoordinates.y) * SIZE);

        //for (int x = 0; x < SIZE; x++)
        //{
        //    for (int z = 0; z < SIZE; z++)
        //    {
        //        blockmap[x, Height(x + blockCoordinates.x, z + blockCoordinates.y), z] = 1;
        //    }
        //}

        CreateMesh();

        GenerateMesh();

        terrain.GetComponent<MeshRenderer>().material = grassMaterial;

        transform.position = new Vector3(blockCoordinates.x, 0, blockCoordinates.y);
        transform.GetChild(0).position = new Vector3(blockCoordinates.x, 0, blockCoordinates.y);
    }

    int Height(int x, int y)
    {
        var h = Mathf.RoundToInt(((MAX_HEIGHT - 100) * Mathf.PerlinNoise((((float)x + noiseOffset.x) * terrainScale) / 750f, (((float)y + noiseOffset.y) * terrainScale) / 750f)));

        if (h > MAX_HEIGHT)
        {
            return MAX_HEIGHT;
        }

        return h;
    }

    public void GenerateMesh()
    {
        var mesh = terrain.GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        int[] triangles = new int[(((SIZE * 2) * (SIZE * 2)) * 24)];
        Vector3[] vertices = new Vector3[((SIZE * 4) * (SIZE * 4))];
        Vector2[] uvs = new Vector2[((SIZE + 30) * (SIZE + 30))];
        int trianglesIndex = 0;
        int vertexIndex = 0;

        for (int z = 0; z < SIZE; z++)
        {
            for (int x = 0; x < SIZE; x++)
            {
                var height = Height(x + blockCoordinates.x, z + blockCoordinates.y);

                vertices[vertexIndex] = new Vector3(x, height, z);
                verticiesMap[x, height, z] = vertexIndex;

                vertices[vertexIndex + 2] = new Vector3(x, height, z + 1);
                verticiesMap[x, height, z + 1] = vertexIndex + 2;

                vertices[vertexIndex + 1] = new Vector3(x + 1, height, z);
                verticiesMap[x + 1, height, z] = vertexIndex + 1;

                vertices[vertexIndex + 3] = new Vector3(x + 1, height, z + 1);
                verticiesMap[x + 1, height, z + 1] = vertexIndex + 3;

                AddTriangle(vertexIndex + 1, vertexIndex, vertexIndex + 2);
                AddTriangle(vertexIndex + 2, vertexIndex + 3, vertexIndex + 1);

                vertexIndex += 4;

                if (Height(x + blockCoordinates.x + 1, z + blockCoordinates.y) < height)
                {
                    vertices[vertexIndex] = new Vector3(x + 1, height - 1, z);
                    verticiesMap[x + 1, height - 1, z] = vertexIndex;

                    vertices[vertexIndex + 1] = new Vector3(x + 1, height - 1, z + 1);
                    verticiesMap[x + 1, height - 1, z + 1] = vertexIndex + 1;

                    AddTriangle(vertexIndex + 1, vertexIndex, verticiesMap[x + 1, height, z]);
                    AddTriangle(verticiesMap[x + 1, height, z + 1], vertexIndex + 1, verticiesMap[x + 1, height, z]);

                    vertexIndex += 2;
                }

                if (Height(x + blockCoordinates.x - 1, z + blockCoordinates.y) < height)
                {
                    vertices[vertexIndex] = new Vector3(x, height - 1, z);
                    verticiesMap[x, height - 1, z] = vertexIndex;

                    vertices[vertexIndex + 1] = new Vector3(x, height - 1, z + 1);
                    verticiesMap[x, height - 1, z + 1] = vertexIndex + 1;

                    AddTriangle(vertexIndex, vertexIndex + 1, verticiesMap[x, height, z]);
                    AddTriangle(verticiesMap[x, height, z], vertexIndex + 1, verticiesMap[x, height, z + 1]);

                    vertexIndex += 2;
                }

                if (Height(x + blockCoordinates.x, z + blockCoordinates.y + 1) < height)
                {
                    vertices[vertexIndex] = new Vector3(x + 1, height - 1, z + 1);
                    verticiesMap[x + 1, height - 1, z + 1] = vertexIndex;

                    vertices[vertexIndex + 1] = new Vector3(x, height - 1, z + 1);
                    verticiesMap[x, height - 1, z + 1] = vertexIndex + 1;

                    AddTriangle(vertexIndex + 1, vertexIndex, verticiesMap[x + 1, height, z + 1]);
                    AddTriangle(vertexIndex + 1, verticiesMap[x + 1, height, z + 1], verticiesMap[x, height, z + 1]);

                    vertexIndex += 2;
                }


                if (Height(x + blockCoordinates.x, z + blockCoordinates.y - 1) < height)
                {
                    vertices[vertexIndex] = new Vector3(x, height - 1, z);
                    verticiesMap[x, height - 1, z] = vertexIndex;

                    vertices[vertexIndex + 1] = new Vector3(x + 1, height - 1, z);
                    verticiesMap[x + 1, height - 1, z] = vertexIndex + 1;

                    AddTriangle(vertexIndex + 1, vertexIndex, verticiesMap[x + 1, height, z]);
                    AddTriangle(vertexIndex, verticiesMap[x, height, z], verticiesMap[x + 1, height, z]);

                    vertexIndex += 2;
                }

            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        //mesh.uv = uvs;

        void AddTriangle(int a, int b, int c)
        {
            triangles[trianglesIndex] = a;
            triangles[trianglesIndex + 1] = b;
            triangles[trianglesIndex + 2] = c;
            trianglesIndex += 3;
        }

        terrain.AddComponent<MeshCollider>();
    }

    private void OnDestroy()
    {
        Destroy(terrain);
    }

    public void CreateMesh()
    {
        terrain = new GameObject();
        terrain.transform.SetParent(transform);
        terrain.name = "Terrain";

        MeshRenderer meshRenderer = terrain.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));

        MeshFilter meshFilter = terrain.AddComponent<MeshFilter>();

        Mesh mesh = new Mesh();

        meshFilter.mesh = mesh;
    }
}
