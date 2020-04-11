using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{

    int[,] _level = new int[25, 50];

    private void Awake()
    {
        for (int z = 0; z < _level.GetLength(0); z++)
        {
            _level[z, 0] = 1;
            _level[z, _level.GetLength(1) - 1] = 1;
        }

        for (int x = 0; x < _level.GetLength(1); x++)
        {
            _level[0, x] = 1;
            _level[_level.GetLength(0) - 1, x] = 1;
        }

        for (int z = 0; z < _level.GetLength(0); z++)
        {
            for (int x = 0; x < _level.GetLength(1); x++)
            {
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = new Vector3(x, 0, -z);
                cube.transform.SetParent(transform);

                if(_level[z,x] == 1)
                {
                    var Wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Wall.transform.position = new Vector3(x, 1, -z);
                    Wall.transform.SetParent(cube.transform);
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetData(int x,int z)
    {
        return _level[z, x];
    }
}
