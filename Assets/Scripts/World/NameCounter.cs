using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public static class NameCounter
{
    public static NameSystem Atlantia = new NameSystem();

    public static NameSystem Defiant = new NameSystem();
    public static NameSystem Nova = new NameSystem();
    public static NameSystem Saber = new NameSystem();

    public static NameSystem Akira = new NameSystem();
    public static NameSystem Intrepid = new NameSystem();
    public static NameSystem Steamrunner = new NameSystem();

    public static NameSystem Luna = new NameSystem();
    public static NameSystem Prometheuse = new NameSystem();
    public static NameSystem Nebula = new NameSystem();
    public static NameSystem Galaxy = new NameSystem();
    public static NameSystem Sovereign = new NameSystem();
    public static NameSystem Excalibur = new NameSystem();

    static List<string> ReadNamesToList(string fileName)
    {
        string filePath = Path.Combine(Application.dataPath, Path.Combine("Data/Names/", fileName));
        return File.ReadAllLines(filePath).ToList();
    }

    static NameCounter() {
        InitNames(Atlantia, "AtlantiaNames.txt");

        InitNames(Defiant, "DefiantNames.txt");
        InitNames(Nova, "NovaNames.txt");
        InitNames(Saber, "SaberNames.txt");

        InitNames(Akira, "AkiraNames.txt");
        InitNames(Intrepid, "IntrepidNames.txt");
        InitNames(Steamrunner, "SteamRunnerNames.txt");

        InitNames(Luna, "LunaNames.txt");
        InitNames(Prometheuse, "PrometheuseNames.txt");
        InitNames(Nebula, "NebulaNames.txt");
        InitNames(Galaxy, "GalaxyNames.txt");
        InitNames(Sovereign, "SovereignNames.txt");
        InitNames(Excalibur, "ExcaliburNames.txt");
    }

    static void InitNames(NameSystem _ns, string fileName)
    {
        List<string> lines = ReadNamesToList(fileName);
        foreach (string name in lines)
        {
            string[] array = name.Split('*');

            _ns.Names.Add(array[0]);
            if (array.Length >= 2)
            {
                _ns.NamesOnHull.Add(array[1]);
            }
            else
            {
                _ns.NamesOnHull.Add(string.Empty);
            }

            if (array.Length == 3)
            {
                _ns.NCC.Add(array[2]);
            }
            else
            {
                _ns.NCC.Add(string.Empty);
            }
        }
    }
}
[System.Serializable]
public class NameSystem
{
    public List<string> Names = new List<string>();
    public List<string> NamesOnHull = new List<string>();
    public List<string> NCC = new List<string>();
}