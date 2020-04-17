using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class MaterialEffectController : MonoBehaviour
{
    private SelectableObject _so;
    [HideInInspector] public Renderer _or;

    public int BorgIndex = 0;
    public int DamageIndex = 0;

    private Material borgMat;
    private Material damageLMat;
    private Material damageHMat;

    public bool Deactivate;

    // Start is called before the first frame update
    void Start()
    {
        _so = GetComponentInParent<SelectableObject>();

        _or = GetComponent<Renderer>();

        if (Deactivate) this.enabled = false;
        
        borgMat = (Material) Resources.Load("Effects/Borg/BorgEffect");
        damageLMat = (Material) Resources.Load("Effects/DamageAndDestructions/DamageEffect/Hull/Light/DamageEffect");
        damageHMat = (Material) Resources.Load("Effects/DamageAndDestructions/DamageEffect/Hull/Heavy/DamageEffect");
    }

    public void UpdateBorgMat()
    {
        if (_so.Assimilated)
        {
            if (BorgIndex == 0)
            {
                addBorgMaterial();
            }
        }
        else
        {
            if (BorgIndex != 0)
            {
                removeBorgMaterial();
            }
        }
    }

    public void UpdateDamageMat()
    {
        if (_so.healthSystem)
        {
            if (_so._hs.curHull > _so._hs.MaxHull / 1.5)
            {
                if (DamageIndex != 0)
                {
                    removeDamageMaterial();
                }
            }
            else if (_so._hs.curHull < _so._hs.MaxHull / 1.5 && _so._hs.curHull > _so._hs.MaxHull / 3)
            {
                if (DamageIndex == 0)
                {
                    addDamageMaterial(damageLMat);
                }
                else
                {
                    addDamageMaterial(damageLMat, true);
                }
            }
            else if (_so._hs.curHull < _so._hs.MaxHull / 3)
            {
                if (DamageIndex == 0)
                {
                    addDamageMaterial(damageHMat);
                }
                else
                {
                    addDamageMaterial(damageHMat, true);
                }
            }
        }
    }

    void addBorgMaterial()
    {
        List<Material> mats = _or.materials.ToList();

        Vector2 vec = new Vector2(Random.Range(0f, 100f), Random.Range(0f, 100f));

        mats.Add(borgMat);
        BorgIndex = mats.Count - 1;

        _or.materials = mats.ToArray();
        _or.materials[BorgIndex].mainTextureOffset = vec;
    }

    void removeBorgMaterial()
    {

        List<Material> mats = _or.materials.ToList();
        mats.RemoveAt(BorgIndex);
        _or.materials = mats.ToArray();
        if (BorgIndex < DamageIndex) DamageIndex--;
        BorgIndex = 0;
    }

    void addDamageMaterial(Material dm, bool change = false)
    {
        Vector2 vec;
        if (change)
        {
            List<Material> mats = _or.materials.ToList();
            vec = _or.materials[DamageIndex].mainTextureOffset;
            mats[DamageIndex] = dm;
            _or.materials = mats.ToArray();
            _or.materials[DamageIndex].mainTextureOffset = vec;
        }
        else
        {
            List<Material> mats = _or.materials.ToList();

            vec = new Vector2(Random.Range(0f, 100f), Random.Range(0f, 100f));

            mats.Add(dm);
            DamageIndex = mats.Count - 1;

            _or.materials = mats.ToArray();
            _or.materials[DamageIndex].mainTextureOffset = vec;
        }
    }

    void removeDamageMaterial()
    {
        List<Material> mats = _or.materials.ToList();
        mats.RemoveAt(DamageIndex);
        _or.materials = mats.ToArray();
        if (DamageIndex < BorgIndex) BorgIndex--;
        DamageIndex = 0;   
    }
}