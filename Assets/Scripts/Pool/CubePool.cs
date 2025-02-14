using System.Collections.Generic;
using UnityEngine;

public class CubePool : Singleton<CubePool>
{
    [SerializeField] private CubeItem cubePrefab;
    private Queue<CubeItem> pool = new Queue<CubeItem>();

    public CubeItem GetCube(Transform parent)
    {
        CubeItem cube;
        if (pool.Count > 0)
        {
            cube = pool.Dequeue();
            cube.gameObject.SetActive(true);
            cube.transform.SetParent(parent);
        }
        else
        {
            cube = Instantiate(cubePrefab, parent);
        }
        return cube;
    }

    public void ReturnCube(CubeItem cube)
    {
        cube.gameObject.SetActive(false);
        cube.transform.SetParent(transform);
        pool.Enqueue(cube);
    }
}
