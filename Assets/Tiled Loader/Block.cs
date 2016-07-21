using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Block : MonoBehaviour
{
    public Vector2 index;

    public List<Vector2> positions = new List<Vector2>();
    private Vector2 gridSize = new Vector2(1, 1);
    public bool backGround = true;
    public int tileType;

    void Start()
    {
    }

    public void Initialize(int x, int y, bool b, int tileType, int xGridsize = 1, int yGridsize = 1)
    {
        index = new Vector2(x, y);
        positions.Add(index);
        backGround = b;
        this.tileType = tileType;
        transform.localScale = new Vector3(1, 1, 0);
        

        gridSize = new Vector2(xGridsize, yGridsize);
    }

    public Vector2 GridSize
    {
        get { return gridSize; }
        set { gridSize = value; }
    }
}
