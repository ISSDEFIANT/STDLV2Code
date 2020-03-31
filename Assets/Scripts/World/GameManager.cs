using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /// <summary> Игроки. </summary>
    public PlayerInfo[] Players = new PlayerInfo[8];
    
    public List<SelectableObject> SelectableObjects;
    // Start is called before the first frame update
    void Awake()
    {
        UpdateList();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (SelectableObject objects in SelectableObjects)
        {
            foreach (PlayerInfo _cp in Players)
            {
                if (_cp.CameraControll != null)
                {
                    if (!_cp.CameraControll.GetComponent<PlayerCameraControll>().SelectionList.Any(x => x == objects))
                    {
                        if (!objects.isHovering)
                        {
                            objects.ShowSelectionEffect(0);
                        }

                        if (objects.isSelected) objects.isSelected = false;
                    }
                }                
            }
        }
    }

    public void UpdateList()
    {
        SelectableObjects = GameObject.FindObjectsOfType<SelectableObject>().ToList();
    }
}
[System.Serializable]
public class PlayerInfo
{
    public int TeamNum;
    public STMethods.Races race;
    public GameObject CameraControll;
    public Color PlayerColor;
}