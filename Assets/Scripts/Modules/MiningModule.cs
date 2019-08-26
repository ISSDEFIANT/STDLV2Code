using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules
{
    public class MiningModule : Module
    {
        //Цель добычи

        //Точка своза ресурсов
        /// <summary> Максимальное количество ресурсво. </summary>
        public float MaxResources;

        /// <summary> Текущее количество ресурсов. </summary>
        public float curResources;

        /// <summary> Текущий тип ресурсов. </summary>
        public STMethods.ResourcesType curResourcesType;

        /// <summary> Может ли добывать титан. </summary>
        public bool Titanium;

        /// <summary> Может ли добывать дилитий. </summary>
        public bool Dilithium;

        /// <summary> Может ли добывать биоматерию. </summary>
        public bool Biomatter;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void Mine(ResourceSource Target)
        {
            //if()
        }
    }
}