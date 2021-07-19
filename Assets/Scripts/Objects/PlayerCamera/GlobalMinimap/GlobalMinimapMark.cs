using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GlobalMinimapMark : MonoBehaviour
{
    public List<SelectableObject> curClaster;
    
    public SpriteRenderer Icon;
    public TextMeshPro Text;
    
    public RacesLogo ShipLogo;
    public RacesLogo StarbaseLogo;
    public Sprite ConstructionStationsLogo;
    public Sprite DefenceStationsLogo;
    public Sprite MiningStationLogo;
    public Sprite ScienceStationsLogo;
    public Sprite StationsGroupLogo;
    public Sprite StationLogo;
    public Sprite StationInProgressLogo;

    public Sprite UnknownLogo;
    public Sprite StarLogo;
    public Sprite PlanetLogo;
    public Sprite AsteroidLogo;

    public UnitsMarksController _umc;

    public bool Hovered;

    public enum ShowingStats
    {
        Unknown,
        Ship,
        Starbase,
        ConstructionStation,
        DefenceStation,
        MiningStation,
        SciStation,
        StationGroup,
        Station,
        StationInProgress,
        Star,
        Planet,
        Asteroid
    }

    public void UpdateData(List<SelectableObject> claster, bool showNames, string groupName)
    {
        curClaster = claster;
        
        Color playerColor = Color.grey;

        ShowingStats stats = ShowingStats.Unknown;

        for (int i = 0; i < claster.Count;)
        {
            if (!claster[i].destroyed && claster[i].isVisible != STMethods.Visibility.Invisible)
            {
                if(claster[i].PlayerNum != 0 && claster[i].isVisible == STMethods.Visibility.Visible) playerColor = GameManager.instance.Players[claster[i].PlayerNum - 1].PlayerColor;
                if (stats == ShowingStats.Unknown)
                {
                    stats = claster[i].GlobalMinimapRender;
                }
                if (stats != ShowingStats.Unknown && stats != ShowingStats.Ship && stats != claster[i].GlobalMinimapRender)
                {
                    stats = ShowingStats.StationGroup;
                }
                i++;
            }
            else
            {
                claster.RemoveAt(i);
            }

            if (Hovered)
            {
                Icon.color = Color.Lerp(playerColor, Color.white, 0.4f);
                Text.color = Color.Lerp(playerColor, Color.white, 0.4f);
            }
            else
            {
                Icon.color = playerColor;
                Text.color = playerColor;
            }
            Icon.sprite = GetSprite(stats, claster[0].ControllingFraction);
            string names = "";
            if (showNames)
            {
                if (groupName != null && groupName.Length > 0) names += groupName + "\n";
                foreach (SelectableObject obj in claster)
                {
                    names += obj.ObjectName + "\n";
                }
            }

            Text.text = names;
        }
    }

    public Sprite GetSprite(ShowingStats stats, STMethods.Races curRace)
    {
        switch (stats)
        {
            case ShowingStats.Ship:
                switch (curRace)
                {
                    case STMethods.Races.Federation:
                        return ShipLogo.FederationLogo;
                    case STMethods.Races.Klingon:
                        return ShipLogo.KlingonLogo;
                    case STMethods.Races.Romulan:
                        return ShipLogo.RomulanLogo;
                    case STMethods.Races.Cardassian:
                        return ShipLogo.CardassianLogo;
                    case STMethods.Races.Borg:
                        return ShipLogo.BorgLogo;
                    case STMethods.Races.S8472:
                        return ShipLogo.UndineLogo;
                    case STMethods.Races.None:
                        return ShipLogo.NeutralLogo;
                    default:
                        return ShipLogo.NeutralLogo;
                }
            case ShowingStats.Starbase:
                switch (curRace)
                {
                    case STMethods.Races.Federation:
                        return StarbaseLogo.FederationLogo;
                    case STMethods.Races.Klingon:
                        return StarbaseLogo.KlingonLogo;
                    case STMethods.Races.Romulan:
                        return StarbaseLogo.RomulanLogo;
                    case STMethods.Races.Cardassian:
                        return StarbaseLogo.CardassianLogo;
                    case STMethods.Races.Borg:
                        return StarbaseLogo.BorgLogo;
                    case STMethods.Races.S8472:
                        return StarbaseLogo.UndineLogo;
                    case STMethods.Races.None:
                        return StarbaseLogo.NeutralLogo;
                    default:
                        return StarbaseLogo.NeutralLogo;
                }
            case ShowingStats.ConstructionStation:
                return ConstructionStationsLogo;
            case ShowingStats.DefenceStation:
                return DefenceStationsLogo;
            case ShowingStats.MiningStation:
                return MiningStationLogo;
            case ShowingStats.SciStation:
                return ScienceStationsLogo;
            case ShowingStats.StationGroup:
                return StationsGroupLogo;
            case ShowingStats.StationInProgress:
                return StationInProgressLogo;
            case ShowingStats.Station:
                return StationLogo;
            case ShowingStats.Star:
                return StarLogo;
            case ShowingStats.Planet:
                return PlanetLogo;
            case ShowingStats.Asteroid:
                return AsteroidLogo;
            default:
                return UnknownLogo;
        }
    }

    [Serializable]
    public class RacesLogo
    {
        public Sprite FederationLogo;
        public Sprite KlingonLogo;
        public Sprite RomulanLogo;
        public Sprite CardassianLogo;
        public Sprite BorgLogo;
        public Sprite UndineLogo;
        public Sprite NeutralLogo;
    }
}