using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class DataLoader : Singleton<DataLoader>
{
    protected DataLoader() { }

    public bool init;
    
    public Dictionary<string, object> ResourcesCache = new Dictionary<string, object>();

    public void Awake()
    {
        #region Effects

        #region Player

        ResourcesCache.Add("MoveMark", Resources.Load("Effects/PlayerEffects/OrderEffects/MoveMark"));
        ResourcesCache.Add("AttackMark", Resources.Load("Effects/PlayerEffects/OrderEffects/AttackMark"));
        ResourcesCache.Add("GuardMark", Resources.Load("Effects/PlayerEffects/OrderEffects/GuardMark"));
        
        ResourcesCache.Add("MoveFlag", Resources.Load("Effects/PlayerEffects/OrderEffects/MoveFlag"));
        
        ResourcesCache.Add("RangeLine", Resources.Load("Effects/PlayerEffects/RangeLine"));
        
        ResourcesCache.Add("LocalMinimapMark", Resources.Load("Effects/PlayerEffects/MinimapMark"));
        ResourcesCache.Add("GlobalMinimapMark", Resources.Load("Effects/PlayerEffects/GlobalMinimapMarks"));

        #region Interfaces

        ResourcesCache.Add("Interface/Federation", Resources.Load("Interfaces/Federation/FederationInterface"));

        #endregion

        #region SelectionFrames

        ResourcesCache.Add("SelectionFrames/Federation", Resources.Load("Textures/RacesInterface/Federation/FedSelFrame"));
        ResourcesCache.Add("SelectionFrames/Borg", Resources.Load("Textures/RacesInterface/Federation/BorgSelFrame"));
        ResourcesCache.Add("SelectionFrames/Klingons", Resources.Load("Textures/RacesInterface/Federation/KliSelFrame"));

        #endregion

        #endregion
        
        #region Explosions
        ResourcesCache.Add("AsteroidExplosion", Resources.Load("Effects/DamageAndDestructions/Explosions/AsteroidExplosion"));
        
        ResourcesCache.Add("SmallShipExplosion", Resources.Load("Effects/DamageAndDestructions/Explosions/SmallShipExplosion"));
        ResourcesCache.Add("MediumShipExplosion", Resources.Load("Effects/DamageAndDestructions/Explosions/MediumShipExplosion"));
        ResourcesCache.Add("BigShipExplosion", Resources.Load("Effects/DamageAndDestructions/Explosions/BigShipExplosion"));
        #endregion

        #region CoreDestruction
        ResourcesCache.Add("FederationCoreDestruction", Resources.Load("Effects/DamageAndDestructions/WarpCoreDestroyingEffect/FedCoreDestroyed"));
        #endregion

        #region WarpBlinks
        ResourcesCache.Add("FederationWarpBlink", Resources.Load("Effects/Warp/FedWarpEffect"));
        ResourcesCache.Add("BorgWarpBlink", Resources.Load("Effects/Warp/BorgWarpEffect"));
        #endregion

        #region Assimilation and model damage materials
        ResourcesCache.Add("AssimilationMaterial", Resources.Load("Effects/Borg/BorgEffect"));
        ResourcesCache.Add("LightDamageMaterial", Resources.Load("Effects/DamageAndDestructions/DamageEffect/Hull/Light/DamageEffect"));
        ResourcesCache.Add("HeavyDamageMaterial", Resources.Load("Effects/DamageAndDestructions/DamageEffect/Hull/Heavy/DamageEffect"));
        #endregion

        #endregion



        #region NeutralObjects
        
        #region Resource sources
        ResourcesCache.Add("Asteroid 1", Resources.Load("Models/World/Asteroids/Asteroid1Pre"));
        ResourcesCache.Add("Asteroid 2", Resources.Load("Models/World/Asteroids/Asteroid2Pre"));
        ResourcesCache.Add("Asteroid 3", Resources.Load("Models/World/Asteroids/Asteroid3Pre"));
        ResourcesCache.Add("Asteroid 4", Resources.Load("Models/World/Asteroids/Asteroid4Pre"));
        ResourcesCache.Add("Asteroid 5", Resources.Load("Models/World/Asteroids/Asteroid5Pre"));
        ResourcesCache.Add("Asteroid 6", Resources.Load("Models/World/Asteroids/Asteroid6Pre"));
        ResourcesCache.Add("Asteroid Ring", Resources.Load("Models/World/ResourcesSpawningArea"));
        #endregion
        #region Central objects
        ResourcesCache.Add("Star/Blue", Resources.Load("Models/World/Stars/BlueSun"));
        ResourcesCache.Add("Star/Blue/Icon", Resources.Load <Sprite> ("Textures/Icons/World/Stars/Blue"));
        
        ResourcesCache.Add("Star/Dark", Resources.Load("Models/World/Stars/DarkSun"));
        ResourcesCache.Add("Star/Dark/Icon", Resources.Load <Sprite> ("Textures/Icons/World/Stars/Dark"));
        
        ResourcesCache.Add("Star/Green", Resources.Load("Models/World/Stars/GreenSun"));
        ResourcesCache.Add("Star/Green/Icon", Resources.Load <Sprite> ("Textures/Icons/World/Stars/Green"));
        
        ResourcesCache.Add("Star/Red", Resources.Load("Models/World/Stars/RedSun"));
        ResourcesCache.Add("Star/Red/Icon", Resources.Load <Sprite> ("Textures/Icons/World/Stars/Red"));
        
        ResourcesCache.Add("Star/White", Resources.Load("Models/World/Stars/WhiteSun"));
        ResourcesCache.Add("Star/White/Icon", Resources.Load <Sprite> ("Textures/Icons/World/Stars/White"));
        
        ResourcesCache.Add("Star/Yellow", Resources.Load("Models/World/Stars/YellowSun"));
        ResourcesCache.Add("Star/Yellow/Icon", Resources.Load <Sprite> ("Textures/Icons/World/Stars/Yellow"));
        #endregion
        
        #endregion

        #region Federation

        #region Ships

        #region Atlantia
        ResourcesCache.Add("Atlantia", Resources.Load("Models/Federation/Ships/STDL_Atlantia/AtlantiaPre"));
        ResourcesCache.Add("Atlantia/Animation", Resources.Load("Models/Federation/Ships/STDL_Atlantia/AtlantiaAnim"));
        ResourcesCache.Add("Atlantia/Icon", Resources.Load <Sprite> ("Textures/Icons/Federation/Ships/AtlantiaIcon"));
        ResourcesCache.Add("Atlantia/PrimaryWeapons", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Atlantia/PrimaryWeapons"));
        ResourcesCache.Add("Atlantia/ImpulseEngines", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Atlantia/ImpulseEngines"));
        ResourcesCache.Add("Atlantia/WarpEngines", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Atlantia/WarpEngines"));
        ResourcesCache.Add("Atlantia/WarpCore", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Atlantia/WarpCore"));
        ResourcesCache.Add("Atlantia/LifeSupport", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Atlantia/LifeSupport"));
        ResourcesCache.Add("Atlantia/Sensors", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Atlantia/Sensors"));
        ResourcesCache.Add("Atlantia/Tracktor", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Atlantia/Tracktor"));
        ResourcesCache.Add("Atlantia/Hull", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Atlantia/Hull"));
        #endregion
        #region Defiant
        ResourcesCache.Add("Defiant", Resources.Load("Models/Federation/Ships/STDL_Defiant/DefiantPre"));
        ResourcesCache.Add("Defiant/Animation", Resources.Load("Models/Federation/Ships/STDL_Defiant/DefiantAnim"));
        ResourcesCache.Add("Defiant/Icon", Resources.Load <Sprite> ("Textures/Icons/Federation/Ships/DefiantIcon"));
        ResourcesCache.Add("Defiant/PrimaryWeapons", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Defiant/PrimaryWeapons"));
        ResourcesCache.Add("Defiant/SecondaryWeapons", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Defiant/SecWeapons"));
        ResourcesCache.Add("Defiant/ImpulseEngines", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Defiant/ImpulseEngines"));
        ResourcesCache.Add("Defiant/WarpEngines", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Defiant/WarpEngines"));
        ResourcesCache.Add("Defiant/WarpCore", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Defiant/WarpCore"));
        ResourcesCache.Add("Defiant/LifeSupport", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Defiant/LifeSupport"));
        ResourcesCache.Add("Defiant/Sensors", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Defiant/Sensors"));
        ResourcesCache.Add("Defiant/Tracktor", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Defiant/Tracktor"));
        ResourcesCache.Add("Defiant/Hull", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Defiant/Hull"));
        
        ResourcesCache.Add("Defiant/Mines", Resources.Load("Effects/Skills/DefiantMine"));
        ResourcesCache.Add("Defiant/Mines/Icon", Resources.Load <Sprite> ("Textures/Icons/Federation/Skills/DefiantMinesIcon"));
        #endregion
        #region Nova
        ResourcesCache.Add("Nova", Resources.Load("Models/Federation/Ships/STDL_Nova/NovaPre"));
        ResourcesCache.Add("Nova/Animation", Resources.Load("Models/Federation/Ships/STDL_Nova/NovaAnim"));
        ResourcesCache.Add("Nova/Icon", Resources.Load <Sprite> ("Textures/Icons/Federation/Ships/NovaIcon"));
        /*ResourcesCache.Add("Nova/PrimaryWeapons", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Nova/PrimaryWeapons"));
        ResourcesCache.Add("Nova/SecondaryWeapons", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Nova/SecWeapons"));
        ResourcesCache.Add("Nova/ImpulseEngines", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Nova/ImpulseEngines"));
        ResourcesCache.Add("Nova/WarpEngines", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Nova/WarpEngines"));
        ResourcesCache.Add("Nova/WarpCore", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Nova/WarpCore"));
        ResourcesCache.Add("Nova/LifeSupport", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Nova/LifeSupport"));
        ResourcesCache.Add("Nova/Sensors", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Nova/Sensors"));
        ResourcesCache.Add("Nova/Tracktor", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Nova/Tracktor"));
        ResourcesCache.Add("Nova/Hull", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Nova/Hull"));*/
        #endregion
        #region Saber
        ResourcesCache.Add("Saber", Resources.Load("Models/Federation/Ships/STDL_Saber/SaberPre"));
        ResourcesCache.Add("Saber/Animation", Resources.Load("Models/Federation/Ships/STDL_Saber/SaberAnim"));
        ResourcesCache.Add("Saber/Icon", Resources.Load <Sprite> ("Textures/Icons/Federation/Ships/SaberIcon"));
        /*ResourcesCache.Add("Saber/PrimaryWeapons", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Saber/PrimaryWeapons"));
        ResourcesCache.Add("Saber/SecondaryWeapons", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Saber/SecWeapons"));
        ResourcesCache.Add("Saber/ImpulseEngines", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Saber/ImpulseEngines"));
        ResourcesCache.Add("Saber/WarpEngines", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Saber/WarpEngines"));
        ResourcesCache.Add("Saber/WarpCore", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Saber/WarpCore"));
        ResourcesCache.Add("Saber/LifeSupport", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Saber/LifeSupport"));
        ResourcesCache.Add("Saber/Sensors", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Saber/Sensors"));
        ResourcesCache.Add("Saber/Tracktor", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Saber/Tracktor"));
        ResourcesCache.Add("Saber/Hull", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Saber/Hull"));*/
        #endregion
        #region Luna
        ResourcesCache.Add("Luna", Resources.Load("Models/Federation/Ships/STDL_Luna/LunaPre"));
        ResourcesCache.Add("Luna/Animation", Resources.Load("Models/Federation/Ships/STDL_Luna/LunaAnim"));
        ResourcesCache.Add("Luna/Icon", Resources.Load <Sprite> ("Textures/Icons/Federation/Ships/LunaIcon"));
        /*ResourcesCache.Add("Luna/PrimaryWeapons", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Luna/PrimaryWeapons"));
        ResourcesCache.Add("Luna/SecondaryWeapons", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Luna/SecWeapons"));
        ResourcesCache.Add("Luna/ImpulseEngines", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Luna/ImpulseEngines"));
        ResourcesCache.Add("Luna/WarpEngines", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Luna/WarpEngines"));
        ResourcesCache.Add("Luna/WarpCore", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Luna/WarpCore"));
        ResourcesCache.Add("Luna/LifeSupport", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Luna/LifeSupport"));
        ResourcesCache.Add("Luna/Sensors", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Luna/Sensors"));
        ResourcesCache.Add("Luna/Tracktor", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Luna/Tracktor"));
        ResourcesCache.Add("Luna/Hull", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Luna/Hull"));*/
        #endregion
        #region Steamrunner
        ResourcesCache.Add("Steamrunner", Resources.Load("Models/Federation/Ships/STDL_Steamrunner/SteamrunnerPre"));
        ResourcesCache.Add("Steamrunner/Animation", Resources.Load("Models/Federation/Ships/STDL_Steamrunner/SteamrunnerAnim"));
        ResourcesCache.Add("Steamrunner/Icon", Resources.Load <Sprite> ("Textures/Icons/Federation/Ships/SteamrunnerIcon"));
        /*ResourcesCache.Add("Steamrunner/PrimaryWeapons", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Steamrunner/PrimaryWeapons"));
        ResourcesCache.Add("Steamrunner/SecondaryWeapons", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Steamrunner/SecWeapons"));
        ResourcesCache.Add("Steamrunner/ImpulseEngines", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Steamrunner/ImpulseEngines"));
        ResourcesCache.Add("Steamrunner/WarpEngines", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Steamrunner/WarpEngines"));
        ResourcesCache.Add("Steamrunner/WarpCore", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Steamrunner/WarpCore"));
        ResourcesCache.Add("Steamrunner/LifeSupport", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Steamrunner/LifeSupport"));
        ResourcesCache.Add("Steamrunner/Sensors", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Steamrunner/Sensors"));
        ResourcesCache.Add("Steamrunner/Tracktor", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Steamrunner/Tracktor"));
        ResourcesCache.Add("Steamrunner/Hull", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Steamrunner/Hull"));*/
        #endregion
        #region Akira
        ResourcesCache.Add("Akira", Resources.Load("Models/Federation/Ships/STDL_Akira/AkiraPre"));
        ResourcesCache.Add("Akira/Animation", Resources.Load("Models/Federation/Ships/STDL_Akira/AkiraAnim"));
        ResourcesCache.Add("Akira/Icon", Resources.Load <Sprite> ("Textures/Icons/Federation/Ships/AkiraIcon"));
        /*ResourcesCache.Add("Akira/PrimaryWeapons", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Akira/PrimaryWeapons"));
        ResourcesCache.Add("Akira/SecondaryWeapons", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Akira/SecWeapons"));
        ResourcesCache.Add("Akira/ImpulseEngines", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Akira/ImpulseEngines"));
        ResourcesCache.Add("Akira/WarpEngines", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Akira/WarpEngines"));
        ResourcesCache.Add("Akira/WarpCore", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Akira/WarpCore"));
        ResourcesCache.Add("Akira/LifeSupport", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Akira/LifeSupport"));
        ResourcesCache.Add("Akira/Sensors", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Akira/Sensors"));
        ResourcesCache.Add("Akira/Tracktor", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Akira/Tracktor"));
        ResourcesCache.Add("Akira/Hull", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Akira/Hull"));*/
        #endregion
        #region Prometheus
        ResourcesCache.Add("Prometheus", Resources.Load("Models/Federation/Ships/STDL_Prometheus/PrometheusPre"));
        ResourcesCache.Add("Prometheus/Animation", Resources.Load("Models/Federation/Ships/STDL_Prometheus/PrometheusAnim"));
        ResourcesCache.Add("Prometheus/Icon", Resources.Load <Sprite> ("Textures/Icons/Federation/Ships/PrometheusIcon"));
        /*ResourcesCache.Add("Prometheus/PrimaryWeapons", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Prometheus/PrimaryWeapons"));
        ResourcesCache.Add("Prometheus/SecondaryWeapons", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Prometheus/SecWeapons"));
        ResourcesCache.Add("Prometheus/ImpulseEngines", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Prometheus/ImpulseEngines"));
        ResourcesCache.Add("Prometheus/WarpEngines", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Prometheus/WarpEngines"));
        ResourcesCache.Add("Prometheus/WarpCore", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Prometheus/WarpCore"));
        ResourcesCache.Add("Prometheus/LifeSupport", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Prometheus/LifeSupport"));
        ResourcesCache.Add("Prometheus/Sensors", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Prometheus/Sensors"));
        ResourcesCache.Add("Prometheus/Tracktor", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Prometheus/Tracktor"));
        ResourcesCache.Add("Prometheus/Hull", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Prometheus/Hull"));*/
        #endregion
        #region Nebula
        ResourcesCache.Add("Nebula", Resources.Load("Models/Federation/Ships/STDL_Nebula/NebulaPre"));
        ResourcesCache.Add("Nebula/Animation", Resources.Load("Models/Federation/Ships/STDL_Nebula/NebulaAnim"));
        ResourcesCache.Add("Nebula/Icon", Resources.Load <Sprite> ("Textures/Icons/Federation/Ships/NebulaIcon"));
        /*ResourcesCache.Add("Nebula/PrimaryWeapons", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Nebula/PrimaryWeapons"));
        ResourcesCache.Add("Nebula/SecondaryWeapons", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Nebula/SecWeapons"));
        ResourcesCache.Add("Nebula/ImpulseEngines", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Nebula/ImpulseEngines"));
        ResourcesCache.Add("Nebula/WarpEngines", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Nebula/WarpEngines"));
        ResourcesCache.Add("Nebula/WarpCore", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Nebula/WarpCore"));
        ResourcesCache.Add("Nebula/LifeSupport", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Nebula/LifeSupport"));
        ResourcesCache.Add("Nebula/Sensors", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Nebula/Sensors"));
        ResourcesCache.Add("Nebula/Tracktor", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Nebula/Tracktor"));
        ResourcesCache.Add("Nebula/Hull", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Nebula/Hull"));*/
        #endregion
        #region Galaxy
        ResourcesCache.Add("Galaxy", Resources.Load("Models/Federation/Ships/STDL_Galaxy/GalaxyPre"));
        ResourcesCache.Add("Galaxy/Animation", Resources.Load("Models/Federation/Ships/STDL_Galaxy/GalaxyAnim"));
        ResourcesCache.Add("Galaxy/Icon", Resources.Load <Sprite> ("Textures/Icons/Federation/Ships/GalaxyIcon"));
        /*ResourcesCache.Add("Galaxy/PrimaryWeapons", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Galaxy/PrimaryWeapons"));
        ResourcesCache.Add("Galaxy/SecondaryWeapons", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Galaxy/SecWeapons"));
        ResourcesCache.Add("Galaxy/ImpulseEngines", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Galaxy/ImpulseEngines"));
        ResourcesCache.Add("Galaxy/WarpEngines", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Galaxy/WarpEngines"));
        ResourcesCache.Add("Galaxy/WarpCore", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Galaxy/WarpCore"));
        ResourcesCache.Add("Galaxy/LifeSupport", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Galaxy/LifeSupport"));
        ResourcesCache.Add("Galaxy/Sensors", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Galaxy/Sensors"));
        ResourcesCache.Add("Galaxy/Tracktor", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Galaxy/Tracktor"));
        ResourcesCache.Add("Galaxy/Hull", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Galaxy/Hull"));*/
        #endregion
        #region Sovereign
        ResourcesCache.Add("Sovereign", Resources.Load("Models/Federation/Ships/STDL_Sovereign/SovereignPre"));
        ResourcesCache.Add("Sovereign/Animation", Resources.Load("Models/Federation/Ships/STDL_Sovereign/SovereignAnim"));
        ResourcesCache.Add("Sovereign/Icon", Resources.Load <Sprite> ("Textures/Icons/Federation/Ships/SovereignIcon"));
        /*ResourcesCache.Add("Sovereign/PrimaryWeapons", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Sovereign/PrimaryWeapons"));
        ResourcesCache.Add("Sovereign/SecondaryWeapons", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Sovereign/SecWeapons"));
        ResourcesCache.Add("Sovereign/ImpulseEngines", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Sovereign/ImpulseEngines"));
        ResourcesCache.Add("Sovereign/WarpEngines", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Sovereign/WarpEngines"));
        ResourcesCache.Add("Sovereign/WarpCore", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Sovereign/WarpCore"));
        ResourcesCache.Add("Sovereign/LifeSupport", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Sovereign/LifeSupport"));
        ResourcesCache.Add("Sovereign/Sensors", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Sovereign/Sensors"));
        ResourcesCache.Add("Sovereign/Tracktor", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Sovereign/Tracktor"));
        ResourcesCache.Add("Sovereign/Hull", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Sovereign/Hull"));*/
        #endregion
        #region Excalibur
        ResourcesCache.Add("Excalibur", Resources.Load("Models/Federation/Ships/STDL_Excalibur/ExcaliburPre"));
        ResourcesCache.Add("Excalibur/Animation", Resources.Load("Models/Federation/Ships/STDL_Excalibur/ExcaliburAnim"));
        ResourcesCache.Add("Excalibur/Icon", Resources.Load <Sprite> ("Textures/Icons/Federation/Ships/ExcaliburIcon"));
        /*ResourcesCache.Add("Excalibur/PrimaryWeapons", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Excalibur/PrimaryWeapons"));
        ResourcesCache.Add("Excalibur/SecondaryWeapons", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Excalibur/SecWeapons"));
        ResourcesCache.Add("Excalibur/ImpulseEngines", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Excalibur/ImpulseEngines"));
        ResourcesCache.Add("Excalibur/WarpEngines", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Excalibur/WarpEngines"));
        ResourcesCache.Add("Excalibur/WarpCore", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Excalibur/WarpCore"));
        ResourcesCache.Add("Excalibur/LifeSupport", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Excalibur/LifeSupport"));
        ResourcesCache.Add("Excalibur/Sensors", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Excalibur/Sensors"));
        ResourcesCache.Add("Excalibur/Tracktor", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Excalibur/Tracktor"));
        ResourcesCache.Add("Excalibur/Hull", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Ships/Excalibur/Hull"));*/
        #endregion
        
        #endregion
        #region Stations

        #region Starbase
        ResourcesCache.Add("FederationStarbase", Resources.Load("Models/Federation/Stations/STDL_Starbase/FedStarbasePre"));
        ResourcesCache.Add("FederationStarbase/InProgress", Resources.Load("Models/Federation/Stations/STDL_Starbase/FedStarbaseInProgress"));
        ResourcesCache.Add("FederationStarbase/Animation", Resources.Load("Models/Federation/Stations/STDL_Starbase/FedStarbaseAnim"));
        ResourcesCache.Add("FederationStarbase/Ghost", Resources.Load("Models/Federation/Stations/STDL_Starbase/FedStarbaseGhost"));
        ResourcesCache.Add("FederationStarbase/Icon", Resources.Load <Sprite> ("Textures/Icons/Federation/Stations/StarbaseIcon"));
        /*ResourcesCache.Add("FederationStarbase/PrimaryWeapons", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Stations/Starbase/PrimaryWeapons"));
        ResourcesCache.Add("FederationStarbase/SecondaryWeapons", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Stations/Starbase/SecWeapons"));
        ResourcesCache.Add("FederationStarbase/ImpulseEngines", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Stations/Starbase/ImpulseEngines"));
        ResourcesCache.Add("FederationStarbase/WarpCore", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Stations/Starbase/WarpCore"));
        ResourcesCache.Add("FederationStarbase/LifeSupport", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Stations/Starbase/LifeSupport"));
        ResourcesCache.Add("FederationStarbase/Sensors", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Stations/Starbase/Sensors"));
        ResourcesCache.Add("FederationStarbase/Hull", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Stations/Starbase/Hull"));*/
        ResourcesCache.Add("FederationStarbase/UpgradeButton1", Resources.Load <Sprite> ("Textures/RacesInterface/Federation/Station1UpdateButton"));
        ResourcesCache.Add("FederationStarbase/UpgradeButton2", Resources.Load <Sprite> ("Textures/RacesInterface/Federation/Station2UpdateButton"));
        #endregion
        #region SciStation
        ResourcesCache.Add("FederationSciStation", Resources.Load("Models/Federation/Stations/STDL_SciStation/FedSciStationPre"));
        ResourcesCache.Add("FederationSciStation/InProgress", Resources.Load("Models/Federation/Stations/STDL_SciStation/FedSciStationInProgress"));
        ResourcesCache.Add("FederationSciStation/Animation", Resources.Load("Models/Federation/Stations/STDL_SciStation/FedSciStationAnim"));
        ResourcesCache.Add("FederationSciStation/Ghost", Resources.Load("Models/Federation/Stations/STDL_SciStation/FedSciStationGhost"));
        ResourcesCache.Add("FederationSciStation/Icon", Resources.Load <Sprite> ("Textures/Icons/Federation/Stations/SciStationIcon"));
        /*ResourcesCache.Add("FederationSciStation/ImpulseEngines", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Stations/SciStation/ImpulseEngines"));
        ResourcesCache.Add("FederationSciStation/WarpCore", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Stations/SciStation/WarpCore"));
        ResourcesCache.Add("FederationSciStation/LifeSupport", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Stations/SciStation/LifeSupport"));
        ResourcesCache.Add("FederationSciStation/Sensors", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Stations/SciStation/Sensors"));
        ResourcesCache.Add("FederationSciStation/Hull", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Stations/SciStation/Hull"));*/
        #endregion
        #region Outpost
        ResourcesCache.Add("FederationOutpost", Resources.Load("Models/Federation/Stations/STDL_Outpost/FedOutpostPre"));
        ResourcesCache.Add("FederationOutpost/InProgress", Resources.Load("Models/Federation/Stations/STDL_Outpost/FedOutpostInProgress"));
        ResourcesCache.Add("FederationOutpost/Animation", Resources.Load("Models/Federation/Stations/STDL_Outpost/FedOutpostAnim"));
        ResourcesCache.Add("FederationOutpost/Ghost", Resources.Load("Models/Federation/Stations/STDL_Outpost/FedOutpostGhost"));
        ResourcesCache.Add("FederationOutpost/Icon", Resources.Load <Sprite> ("Textures/Icons/Federation/Stations/OutpostIcon"));
        /*ResourcesCache.Add("FederationOutpost/ImpulseEngines", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Stations/Outpost/ImpulseEngines"));
        ResourcesCache.Add("FederationOutpost/WarpCore", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Stations/Outpost/WarpCore"));
        ResourcesCache.Add("FederationOutpost/LifeSupport", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Stations/Outpost/LifeSupport"));
        ResourcesCache.Add("FederationOutpost/Sensors", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Stations/Outpost/Sensors"));
        ResourcesCache.Add("FederationOutpost/Hull", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Stations/Outpost/Hull"));*/
        #endregion
        #region MiningStation
        ResourcesCache.Add("FederationMiningStation", Resources.Load("Models/Federation/Stations/STDL_MineStation/FederationMiningStationPre"));
        ResourcesCache.Add("FederationMiningStation/InProgress", Resources.Load("Models/Federation/Stations/STDL_MineStation/FederationMiningStationInProgress"));
        ResourcesCache.Add("FederationMiningStation/Animation", Resources.Load("Models/Federation/Stations/STDL_MineStation/FederationMiningStationAnim"));
        ResourcesCache.Add("FederationMiningStation/Ghost", Resources.Load("Models/Federation/Stations/STDL_MineStation/FederationMiningStationGhost"));
        ResourcesCache.Add("FederationMiningStation/Icon", Resources.Load <Sprite> ("Textures/Icons/Federation/Stations/MiningStationIcon"));
        /*ResourcesCache.Add("FederationMiningStation/ImpulseEngines", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Stations/MiningStation/ImpulseEngines"));
        ResourcesCache.Add("FederationMiningStation/WarpCore", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Stations/MiningStation/WarpCore"));
        ResourcesCache.Add("FederationMiningStation/LifeSupport", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Stations/MiningStation/LifeSupport"));
        ResourcesCache.Add("FederationMiningStation/Sensors", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Stations/MiningStation/Sensors"));
        ResourcesCache.Add("FederationMiningStation/Hull", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Stations/MiningStation/Hull"));*/
        #endregion
        #region Drydock
        ResourcesCache.Add("FederationDrydock", Resources.Load("Models/Federation/Stations/STDL_Drydock/FedDrydockPre"));
        ResourcesCache.Add("FederationDrydock/InProgress", Resources.Load("Models/Federation/Stations/STDL_Drydock/FedDrydockInProgress"));
        ResourcesCache.Add("FederationDrydock/Animation", Resources.Load("Models/Federation/Stations/STDL_Drydock/FedDrydockAnim"));
        ResourcesCache.Add("FederationDrydock/Ghost", Resources.Load("Models/Federation/Stations/STDL_Drydock/FedDrydockGhost"));
        ResourcesCache.Add("FederationDrydock/Icon", Resources.Load <Sprite> ("Textures/Icons/Federation/Stations/DrydockIcon"));
        /*ResourcesCache.Add("FederationDrydock/ImpulseEngines", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Stations/Drydock/ImpulseEngines"));
        ResourcesCache.Add("FederationDrydock/WarpCore", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Stations/Drydock/WarpCore"));
        ResourcesCache.Add("FederationDrydock/LifeSupport", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Stations/Drydock/LifeSupport"));
        ResourcesCache.Add("FederationDrydock/Sensors", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Stations/Drydock/Sensors"));
        ResourcesCache.Add("FederationDrydock/Hull", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Stations/Drydock/Hull"));*/
        #endregion
        #region AdvancedDrydock
        ResourcesCache.Add("FederationAdvancedDrydock", Resources.Load("Models/Federation/Stations/STDL_AdvancedDrydock/FedAdvancedDrydockPre"));
        ResourcesCache.Add("FederationAdvancedDrydock/InProgress", Resources.Load("Models/Federation/Stations/STDL_AdvancedDrydock/FedAdvancedDrydockInProgress"));
        ResourcesCache.Add("FederationAdvancedDrydock/Animation", Resources.Load("Models/Federation/Stations/STDL_AdvancedDrydock/FedAdvancedDrydockAnim"));
        ResourcesCache.Add("FederationAdvancedDrydock/Ghost", Resources.Load("Models/Federation/Stations/STDL_AdvancedDrydock/FedAdvancedDrydockGhost"));
        ResourcesCache.Add("FederationAdvancedDrydock/Icon", Resources.Load <Sprite> ("Textures/Icons/Federation/Stations/ShipyardIcon"));
        /*ResourcesCache.Add("FederationAdvancedDrydock/ImpulseEngines", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Stations/AdvancedDrydock/ImpulseEngines"));
        ResourcesCache.Add("FederationAdvancedDrydock/WarpCore", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Stations/AdvancedDrydock/WarpCore"));
        ResourcesCache.Add("FederationAdvancedDrydock/LifeSupport", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Stations/AdvancedDrydock/LifeSupport"));
        ResourcesCache.Add("FederationAdvancedDrydock/Sensors", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Stations/AdvancedDrydock/Sensors"));
        ResourcesCache.Add("FederationAdvancedDrydock/Hull", Resources.Load <Sprite> ("Textures/Blueprints/Federation/Stations/AdvancedDrydock/Hull"));*/
        #endregion

        #endregion

        #endregion

        #region Sounds

        #region ShipsSounds
        ResourcesCache.Add("FederationShipSound", Resources.Load ("Sounds/FederationShipSound"));
        ResourcesCache.Add("BorgShipSound", Resources.Load ("Sounds/BorgShipSound"));
        #endregion

        #endregion
    }
}