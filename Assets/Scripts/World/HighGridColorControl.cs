using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighGridColorControl : MonoBehaviour
{
    private Renderer _grid;
    // Start is called before the first frame update
    void Start()
    {
        _grid = gameObject.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y > 1)
        {
            if (!_grid.enabled) _grid.enabled = true;
            _grid.material.color = Color.green;
        }
        if (transform.position.y < -1)
        {
            if (!_grid.enabled) _grid.enabled = true;
            _grid.material.color = Color.red;
        }
        if (transform.position.y < 1 && transform.position.y > -1)
        {
            if (_grid.enabled) _grid.enabled = false;
        }
    }
}
