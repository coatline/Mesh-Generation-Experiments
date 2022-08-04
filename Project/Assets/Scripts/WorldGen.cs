using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGen : MonoBehaviour
{
    [SerializeField] Material grassmaterial;
    [SerializeField] Player playerPrefab;
    [SerializeField] Chunk chunkPrefab;

    [SerializeField] float terrainScale = 4;
    [SerializeField] int renderDist = 5;
    [SerializeField] int seed;
    int chunkCoordsOffset = 500;
    Vector2 noiseOffset;

    Player player;

    Chunk[,] chunkMap;

    void Awake()
    {
        previousPlayerChunkCoordinates = new Vector2Int(16, 16);

        chunkMap = new Chunk[1000, 1000];

        if (seed > 0)
        {
            Random.InitState(seed);
        }
        else
        {
            Random.InitState(System.DateTime.UtcNow.Millisecond);
        }

        noiseOffset = new Vector2(Random.Range(0, 99999), Random.Range(0, 99999));

        player = Instantiate(playerPrefab, new Vector3(0, 250, 0), Quaternion.identity);
    }

    Vector2Int previousPlayerChunkCoordinates;

    void HandleChunks()
    {
        Vector2Int playerChunkCoordinates = new Vector2Int((Mathf.RoundToInt(player.transform.position.x) / 16), (Mathf.FloorToInt(player.transform.position.z) / 16));

        if (playerChunkCoordinates == previousPlayerChunkCoordinates)
        {
            return;
        }

        for (int x = playerChunkCoordinates.x - renderDist; x < playerChunkCoordinates.x + renderDist; x++)
        {
            for (int y = playerChunkCoordinates.y - renderDist; y < playerChunkCoordinates.y + renderDist; y++)
            {
                if (!chunkMap[x + chunkCoordsOffset, y + chunkCoordsOffset])
                {
                    var newChunk = Instantiate(chunkPrefab, new Vector3(x * 16, 0, y * 16), Quaternion.identity, transform);
                    newChunk.Generate(new Vector2Int(x, y), terrainScale, noiseOffset, grassmaterial, null);
                    chunkMap[x + chunkCoordsOffset, y + chunkCoordsOffset] = newChunk;
                }
            }
        }

        for (int x = playerChunkCoordinates.x - renderDist - 1; x < playerChunkCoordinates.x + renderDist + 1; x++)
        {
            if (chunkMap[x + chunkCoordsOffset, playerChunkCoordinates.y - renderDist + chunkCoordsOffset])
            {
                Destroy(chunkMap[x + chunkCoordsOffset, playerChunkCoordinates.y - renderDist + chunkCoordsOffset].gameObject);
                chunkMap[x + chunkCoordsOffset, playerChunkCoordinates.y - renderDist + chunkCoordsOffset] = null;
            }

            if (chunkMap[x + chunkCoordsOffset, playerChunkCoordinates.y + renderDist + chunkCoordsOffset])
            {
                Destroy(chunkMap[x + chunkCoordsOffset, playerChunkCoordinates.y + renderDist + chunkCoordsOffset].gameObject);
                chunkMap[x + chunkCoordsOffset, playerChunkCoordinates.y - renderDist + chunkCoordsOffset] = null;
            }
        }

        for (int y = playerChunkCoordinates.y - renderDist - 1; y < playerChunkCoordinates.y + renderDist + 1; y++)
        {
            if (chunkMap[playerChunkCoordinates.x - renderDist + chunkCoordsOffset, y + chunkCoordsOffset])
            {
                Destroy(chunkMap[playerChunkCoordinates.x - renderDist + chunkCoordsOffset, y + chunkCoordsOffset].gameObject);
                chunkMap[playerChunkCoordinates.x - renderDist + chunkCoordsOffset, y + chunkCoordsOffset] = null;
            }

            if (chunkMap[playerChunkCoordinates.x + renderDist + chunkCoordsOffset, y + chunkCoordsOffset])
            {
                Destroy(chunkMap[playerChunkCoordinates.x + renderDist + chunkCoordsOffset, y + chunkCoordsOffset].gameObject);
                chunkMap[playerChunkCoordinates.x + renderDist + chunkCoordsOffset, y + chunkCoordsOffset] = null;
            }
        }

        previousPlayerChunkCoordinates = playerChunkCoordinates;
    }

    void Update()
    {
        HandleChunks();
    }
}
