using System;
using UnityEngine;

public class GrassSpawner : MonoBehaviour {
    [SerializeField] GameObject grassPrefab;
    [SerializeField] float areaWidth;
    [SerializeField] Transform grassParent;

    void Start() {
        PlantGrassArea(transform.position, areaWidth);
    }

    private void PlantGrass(Vector3 position)
    {
        position.y += grassPrefab.transform.position.y;
        var angles = grassPrefab.transform.rotation.eulerAngles;
        var rotation = new Quaternion();
        angles.y = UnityEngine.Random.Range(0, 180.0f);
        rotation.eulerAngles = angles;

        var newGrass = Instantiate(grassPrefab, position, rotation);
        newGrass.transform.SetParent(grassParent, true);
    }

    private void PlantGrassArea(Vector3 originPosition, float areaWidth)
    {
        Action<Vector3> sampleAction = (samplePosition) =>
        {
            PlantGrass(samplePosition);
        };

        var ps = new PoissonSampler(sampleAction);
        ps.Sample(originPosition, areaWidth);
    }
}
