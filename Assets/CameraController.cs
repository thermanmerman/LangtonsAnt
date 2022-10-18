using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Ant ant;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector2(ant.columns / 2, ant.rows / 2);
        Camera.main.orthographicSize = (ant.columns + ant.rows) / 2 / 2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
