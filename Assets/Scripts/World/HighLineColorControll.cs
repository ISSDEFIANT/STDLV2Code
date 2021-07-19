using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighLineColorControll : MonoBehaviour
{
    private LineRenderer _line;
    private GameObject _grid;
    // Start is called before the first frame update
    void Start()
    {
        _line = gameObject.GetComponent<LineRenderer>();
        _grid = FindObjectOfType<HighGridMarker>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (_grid.transform.position.y > 1)
        {
            if (!_line.enabled) _line.enabled = true;
            _line.startColor = Color.green;
            _line.endColor = Color.green;
        }
        if (_grid.transform.position.y < -1)
        {
            if (!_line.enabled) _line.enabled = true;
            _line.startColor = Color.red;
            _line.endColor = Color.red;
        }
        if (_grid.transform.position.y < 1 && _grid.transform.position.y > -1)
        {
            if (_line.enabled) _line.enabled = false;
        }
        _line.SetPositions(new Vector3[2] {Vector3.zero, new Vector3(0, -transform.position.y, 0)});
    }
}
