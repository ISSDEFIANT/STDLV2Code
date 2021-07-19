using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaserSheafController : MonoBehaviour
{
    public ArcReactor_Launcher[] Beams;

    private AudioSource audio;
    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    IEnumerator AttackSeqience(Transform target)
    {
        for (int i = 0; i < 4; i++)
        {
            foreach (ArcReactor_Launcher weapon in Beams)
            {
                weapon.PhaserFire(target);
                audio.Play();
            }
            yield return new WaitForSeconds(.6f);
        }
        yield return null;
    }
}
