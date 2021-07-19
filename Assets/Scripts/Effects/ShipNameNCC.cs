using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShipNameNCC : MonoBehaviour
{
    public enum NameOrNCC
    {
        Name,
        NCC,
        NCCP1,
        NCCP2,
        NameP1,
        NameP2
    }

    public NameOrNCC textType;

    private TextMeshPro text;
    private SelectableObject owner;
    // Start is called before the first frame update
    void Start()
    {
        text = gameObject.GetComponent<TextMeshPro>();
        owner = transform.root.GetComponent<SelectableObject>();
    }

    public void UpdateData()
    {
        switch (textType)
        {
            case NameOrNCC.Name:
                text.text = owner.ObjectOnModelName;
                break;
            case NameOrNCC.NCC:
                text.text = owner.ObjectNCC;
                break;
            case NameOrNCC.NCCP1:
                string nccp1TextOrigin = owner.ObjectNCC;
                string nccp1half = "";
                if (nccp1TextOrigin.Length % 2 == 0)
                {
                    for (int i = 0; i < nccp1TextOrigin.Length/2; i++)
                    {
                        nccp1half += nccp1TextOrigin[i];
                    }
                }
                else
                {
                    for (int i = 0; i < (int)nccp1TextOrigin.Length/2+1; i++)
                    {
                        nccp1half += nccp1TextOrigin[i];
                    }
                }

                text.text = nccp1half;
                break;
            case NameOrNCC.NCCP2:
                string nccp2TextOrigin = owner.ObjectNCC;
                string nccp2half = "";
                if (nccp2TextOrigin.Length % 2 == 0)
                {
                    for (int i = nccp2TextOrigin.Length/2; i < nccp2TextOrigin.Length; i++)
                    {
                        nccp2half += nccp2TextOrigin[i];
                    }
                }
                else
                {
                    for (int i = nccp2TextOrigin.Length/2-1; i < nccp2TextOrigin.Length; i++)
                    {
                        nccp2half += nccp2TextOrigin[i];
                    }
                }

                text.text = nccp2half;
                break;
            case NameOrNCC.NameP1:
                string namep1TextOrigin = owner.ObjectOnModelName;
                string namep1half = "";
                if (namep1TextOrigin.Length % 2 == 0)
                {
                    for (int i = 0; i < namep1TextOrigin.Length/2; i++)
                    {
                        namep1half += namep1TextOrigin[i];
                    }
                }
                else
                {
                    for (int i = 0; i < (int)namep1TextOrigin.Length/2+1; i++)
                    {
                        namep1half += namep1TextOrigin[i];
                    }
                }

                text.text = namep1half;
                break;
            case NameOrNCC.NameP2:
                string namep2TextOrigin = owner.ObjectOnModelName;
                string namep2half = "";
                if (namep2TextOrigin.Length % 2 == 0)
                {
                    for (int i = namep2TextOrigin.Length/2; i < namep2TextOrigin.Length; i++)
                    {
                        namep2half += namep2TextOrigin[i];
                    }
                }
                else
                {
                    for (int i = namep2TextOrigin.Length/2-1; i < namep2TextOrigin.Length; i++)
                    {
                        namep2half += namep2TextOrigin[i];
                    }
                }

                text.text = namep2half;
                break;
        }
    }
}
