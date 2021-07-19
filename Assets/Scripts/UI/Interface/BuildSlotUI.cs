using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildSlotUI : MonoBehaviour
{
    public enum objectType
    {
        station,
        ship,
        shipCancel
    }
    public objectType type;
    public int Num;
    public int slotNum;
    
    public GlobalInterfaceEventSystem playerInterface;

    private Image ownImage;
    private Button ownButton;

    public Image BuildIndicator;
    
    // Start is called before the first frame update
    void Start()
    {
        ownImage = GetComponent<Image>();
        ownButton = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (type)
        {
            case objectType.station:
                if (playerInterface.BuilderController != null)
                {
                    switch (Num)
                    {
                        case 1:
                            if (slotNum < playerInterface.Category1.Count)
                            {
                                ownImage.sprite = playerInterface.Category1[slotNum].Icon;
                                if (ownImage.color != Color.white) ownImage.color = Color.white;
                            }
                            else
                            {
                                if (ownImage.color != Color.clear) ownImage.color = Color.clear;
                            }

                            break;
                        case 2:
                            if (slotNum < playerInterface.Category2.Count)
                            {
                                ownImage.sprite = playerInterface.Category2[slotNum].Icon;
                                if (ownImage.color != Color.white) ownImage.color = Color.white;
                            }
                            else
                            {
                                if (ownImage.color != Color.clear) ownImage.color = Color.clear;
                            }

                            break;
                        case 3:
                            if (slotNum < playerInterface.Category3.Count)
                            {
                                ownImage.sprite = playerInterface.Category3[slotNum].Icon;
                                if (ownImage.color != Color.white) ownImage.color = Color.white;
                            }
                            else
                            {
                                if (ownImage.color != Color.clear) ownImage.color = Color.clear;
                            }

                            break;
                    }
                }
                break;
            case objectType.ship:
                if (playerInterface.DockingBuilderController != null)
                {
                    if (slotNum < playerInterface.NoneCategory.Count)
                    {
                        if (playerInterface.NoneCategory[slotNum].IndexList.Count >= playerInterface.NoneCategory[slotNum].MaxIndexCount)
                        {
                            ownButton.interactable = false;
                        }
                        else
                        {
                            ownButton.interactable = true;
                        }
                        ownImage.sprite = playerInterface.NoneCategory[slotNum].Icon;
                        if (ownImage.color != Color.white) ownImage.color = Color.white;
                    }
                    else
                    {
                        if (ownImage.color != Color.clear) ownImage.color = Color.clear;
                    }
                }
                break;
            case objectType.shipCancel:
                if (playerInterface.DockingBuilderController != null)
                {
                    if (slotNum < playerInterface.DockingBuilderController.ShipsInList.Count)
                    {
                        ownImage.sprite = playerInterface.DockingBuilderController.ShipsInList[slotNum].Icon;
                        if (ownImage.color != Color.white) ownImage.color = Color.white;
                        BuildIndicator.fillAmount =
                            1 - (playerInterface.DockingBuilderController.ShipsInList[slotNum].ConstructionTime /
                                 playerInterface.DockingBuilderController.ShipsInList[slotNum].MaxConstructionTime);
                    }
                    else
                    {
                        if (ownImage.color != Color.clear) ownImage.color = Color.clear;
                        BuildIndicator.fillAmount = 0;
                    }
                }
                break;
        }
    }

    public void OnButtonDown()
    {
        switch (type)
        {
            case objectType.station:
                if (playerInterface.BuilderController != null)
                {
                    switch (Num)
                    {
                        case 1:
                            if (slotNum < playerInterface.Category1.Count)
                            {
                                playerInterface.BuilderController.OnButton(playerInterface.Category1[slotNum]);
                            }

                            break;
                        case 2:
                            if (slotNum < playerInterface.Category2.Count)
                            {
                                playerInterface.BuilderController.OnButton(playerInterface.Category2[slotNum]);
                            }

                            break;
                        case 3:
                            if (slotNum < playerInterface.Category3.Count)
                            {
                                playerInterface.BuilderController.OnButton(playerInterface.Category3[slotNum]);
                            }

                            break;
                    }
                }
                break;
            case objectType.ship:
                if (playerInterface.DockingBuilderController != null)
                {
                    if (slotNum < playerInterface.NoneCategory.Count)
                    {
                        playerInterface.DockingBuilderController.BuildShip(playerInterface.NoneCategory[slotNum]);
                    }
                }
                break;
            case objectType.shipCancel:
                if (playerInterface.DockingBuilderController != null)
                {
                    if (slotNum < playerInterface.DockingBuilderController.ShipsInList.Count)
                    {
                        playerInterface.DockingBuilderController.CanselShip(playerInterface.DockingBuilderController.ShipsInList[slotNum]);
                        BuildIndicator.fillAmount = 0;
                    }
                }
                break;
        }
    }
}
