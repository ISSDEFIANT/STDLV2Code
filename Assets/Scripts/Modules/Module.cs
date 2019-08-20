using UnityEngine;

namespace Modules
{
    public class Module : MonoBehaviour
    {
        /// <summary> Объект, которому модуль принадлежит. </summary>
        public SelectableObject Owner;
    
    
        /// <summary> Когда модуль активируется. </summary>
        public virtual void Active(){}
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
