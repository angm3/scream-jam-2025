using UnityEngine;

public class Spawner : MonoBehaviour
{

    public GameObject prefabToSpawn;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnObjectsInVolume(4);
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    void SpawnObjectsInVolume(int count)
    {
        if (prefabToSpawn == null || count <= 0) return;

        // Get the actual world-space size and position of the volume
        Vector3 size = Vector3.Scale(gameObject.GetComponent<BoxCollider>().size, gameObject.transform.lossyScale);
        Vector3 center = gameObject.transform.TransformPoint(gameObject.transform.position);

        // Compute roughly cubic grid resolution
        int resolution = Mathf.CeilToInt(Mathf.Pow(count, 1f / 3f));
        Vector3 cellSize = size / resolution;

        int spawned = 0;
        for (int x = 0; x < resolution && spawned < count; x++)
        {
            for (int y = 0; y < resolution && spawned < count; y++)
            {
                for (int z = 0; z < resolution && spawned < count; z++)
                {
                    // Normalized local offset (-0.5 to 0.5)
                    Vector3 offset = new Vector3(
                        (x + 0.5f) / resolution - 0.5f,
                        (y + 0.5f) / resolution - 0.5f,
                        (z + 0.5f) / resolution - 0.5f
                    );

                    // Convert to world position inside the box
                    Vector3 localPos = Vector3.Scale(offset, size);
                    Vector3 worldPos = gameObject.transform.TransformPoint(localPos + gameObject.transform.position);

                    // Add random jitter within each grid cell
                    Vector3 jitter = new Vector3(
                        Random.Range(-0.5f, 0.5f),
                        Random.Range(-0.5f, 0.5f),
                        Random.Range(-0.5f, 0.5f)
                    );
                    worldPos += Vector3.Scale(jitter, cellSize * 1);

                    Instantiate(prefabToSpawn, worldPos, Quaternion.identity, transform);
                    spawned++;
                }
            }
        }
    }
}
