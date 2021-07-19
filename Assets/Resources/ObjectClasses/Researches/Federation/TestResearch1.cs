using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestResearch1 : SciContract
{
    public override float TitaniumCost
    {
        get
        {
            base.TitaniumCost = 100;
            return base.TitaniumCost;
        }
    }
    public override float DilithiumCost
    {
        get
        {
            base.DilithiumCost = 200;
            return base.DilithiumCost;
        }
    }
    public override float BiomatterCost
    {
        get
        {
            base.BiomatterCost = 0;
            return base.BiomatterCost;
        }
    }
    public override float CrewCost
    {
        get
        {
            base.CrewCost = 0;
            return base.CrewCost;
        }
    }
    public override float ConstructionTime
    {
        get
        {
            base.ConstructionTime = 30;
            return base.ConstructionTime;
        }
    }
    public override Sprite Icon
    {
        get
        {
            base.Icon = Resources.Load <Sprite>("Textures/Icons/Federation/Ships/GalaxyIcon");
            return base.Icon;
        }
    }
    public override string Name
    {
        get
        {
            base.Name = "Test research";
            return base.Name;
        }
    }

    public override string Description
    {
        get
        {
            base.Description = "This research was created to test the research system. It's not doing any good things, only taking your resources.";
            return base.Description;
        }
    }
}