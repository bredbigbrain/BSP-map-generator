using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell: MonoBehaviour
{
    public Map map;
    public int id, x, y;

    public SpriteRenderer spriteRenderer;
    public Vector3 position;

    public void Init(int id, int x, int y)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        position = transform.position;

        this.id = id;
        this.x = x;
        this.y = y;
    }
}
