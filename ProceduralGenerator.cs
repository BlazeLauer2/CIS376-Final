using UnityEngine;

public class ProceduralGenerator : MonoBehaviour
{
    public GameObject platformPrefab;
    public GameObject wallRunPrefab;
    public Transform player;

    public int initialPlatforms = 6;
    public float spawnDistance = 15f; // Distance from the last platform to spawn a new one

    public float minDistance = 4f; // Distance and height between platforms
    public float maxDistance = 8f;
    public float minHeight = -1f;
    public float maxHeight = 2f;

    public Material platformMaterial;
    public Material wallRunMaterial;

    [Range(0f, 1f)]
    public float wallRunChance = 0.3f; // Chance to spawn a wall run

    private Vector3 lastSpawnPosition;

    void Start()
    {
        lastSpawnPosition = transform.position;

        // Spawn initial platforms
        for (int i = 0; i < initialPlatforms; i++)
        {
            SpawnNextPlatform();
        }
    }

    void Update()
    {
        // Only spawn if player is getting close to the last platform
        if (player.position.z + spawnDistance > lastSpawnPosition.z)
        {
            SpawnNextPlatform();
        }
    }

    void SpawnNextPlatform()
    {
        float distance = Random.Range(minDistance, maxDistance);
        float heightOffset = Random.Range(minHeight, maxHeight);

        Vector3 spawnPosition = lastSpawnPosition + new Vector3(0, heightOffset, distance); // Move forward on Z
        Quaternion rotation = Quaternion.Euler(0, Random.Range(-15f, 15f), 0);

        // Replace platform with wall run surface by chance
        bool spawnWallRun = Random.value < wallRunChance;

        if (spawnWallRun)
        {
            // Choose left or right (-1 or 1)
            int side = Random.value < 0.5f ? -1 : 1;

            // Offset wall to the side of the spawn position
            Vector3 wallOffset = new Vector3(2f * side, 0f, 0f);
            Vector3 wallPosition = spawnPosition + wallOffset;

            GameObject wall = Instantiate(wallRunPrefab, wallPosition, Quaternion.identity);

            MeshRenderer renderer = wall.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material = wallRunMaterial;
            }
        }
        else
        {
            GameObject platform = Instantiate(platformPrefab, spawnPosition, rotation);

            MeshRenderer renderer = platform.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material = platformMaterial;
            }
        }

        lastSpawnPosition = spawnPosition;
    }
}
