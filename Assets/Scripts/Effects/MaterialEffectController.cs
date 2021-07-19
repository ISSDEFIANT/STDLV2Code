using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class MaterialEffectController : MonoBehaviour
{
    public SelectableObject _so = null;
    [HideInInspector] public Renderer _or;

    public int BorgIndex = 0;
    public int DamageIndex = 0;

    private Material borgMat;
    private Material damageLMat;
    private Material damageHMat;

    public bool Deactivate;

    public List<MaterialEffectController> SubParts;

    // Start is called before the first frame update
    void Start()
    {
        _so = transform.root.gameObject.GetComponent<SelectableObject>();

        _or = GetComponent<Renderer>();

        if (Deactivate)
        {
            this.enabled = false;
            return;
        }
        
        borgMat = (Material) DataLoader.Instance.ResourcesCache["AssimilationMaterial"];
        damageLMat = (Material) DataLoader.Instance.ResourcesCache["LightDamageMaterial"];
        damageHMat = (Material) DataLoader.Instance.ResourcesCache["HeavyDamageMaterial"];
        
        UpdateBorgMat();
        UpdateDamageMat();
    }

    public void UpdateBorgMat()
    {
        if (Deactivate) return;
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
        if (SubParts.Count > 0)
        {
            foreach (MaterialEffectController mat in SubParts)
            {
                if (mat._so == null) mat._so = _so;
                mat.UpdateBorgMat();
            }
        }
    }

    public void UpdateDamageMat()
    {
        if (Deactivate) return;
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
        if (SubParts.Count > 0)
        {
            foreach (MaterialEffectController mat in SubParts)
            {
                if (mat._so == null) mat._so = _so;
                mat.UpdateDamageMat();
            }
        }
    }

    void addBorgMaterial()
    {
        if (Deactivate) return;
        List<Material> mats = _or.materials.ToList();

        Vector2 vec = new Vector2(Random.Range(0f, 100f), Random.Range(0f, 100f));

        mats.Add(borgMat);
        BorgIndex = mats.Count - 1;

        _or.materials = mats.ToArray();
        _or.materials[BorgIndex].mainTextureOffset = vec;
    }

    void removeBorgMaterial()
    {
        if (Deactivate) return;
        List<Material> mats = _or.materials.ToList();
        mats.RemoveAt(BorgIndex);
        _or.materials = mats.ToArray();
        if (BorgIndex < DamageIndex) DamageIndex--;
        BorgIndex = 0;
    }

    void addDamageMaterial(Material dm, bool change = false)
    {
        if (Deactivate) return;
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
        if (Deactivate) return;
        List<Material> mats = _or.materials.ToList();
        mats.RemoveAt(DamageIndex);
        _or.materials = mats.ToArray();
        if (DamageIndex < BorgIndex) BorgIndex--;
        DamageIndex = 0;   
    }
}