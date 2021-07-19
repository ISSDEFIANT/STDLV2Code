using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderConstructionController : MonoBehaviour
{
    public BuilderConstructionSS system;
    
    public List<ConstructionContract> AbleToConstruct = new List<ConstructionContract>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnButton(ConstructionContract contract)
    {
        foreach (ConstructionContract cont in AbleToConstruct)
        {
            if (cont == contract)
            {
                PlayerCameraControll player = GameManager.instance.Players[system.Owner.PlayerNum - 1].CameraControll
                    .GetComponent<PlayerCameraControll>();
                if (player._ghost != null) Destroy(player._ghost);
                GameObject obj = Instantiate(cont.Ghost, Vector3.zero, Quaternion.identity);
                player._ghost = obj;
                player.CameraState = STMethods.PlayerCameraState.BuildingPlacement;
                player._buildingCommand = new BuildingCommand();
                player._buildingCommand.command = "Build";
                player._buildingCommand.target = cont;
            }
        }
    }
}