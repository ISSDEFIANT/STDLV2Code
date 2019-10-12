using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /// <summary> Команды игроков. </summary>
    public int[] PlayersTeam = new int[8];
    
    public List<SelectableObject> SelectableObjects;
    // Start is called before the first frame update
    void Awake()
    {
        UpdateList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateList()
    {
        SelectableObjects = GameObject.FindObjectsOfType<SelectableObject>().ToList();
    }
}