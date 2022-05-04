using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapInfo
{

    public enum MapType
    {
        Spawn,   // Type/{MapType}, SpawnPoint, Ending
        Normalroom,  // Type/{MapType}, SpawnPoint, EnemySpawns/{id}, Begining, Ending
        BlockingGate
    }

    public GameObject MetaDataObject;
    public MapType mapType;
    public Vector3 Begining;
    public Vector3 Ending;
    public Vector3 Center;
    public Vector3 SpawnLocation;
    public List<Vector3> EnemySpawnLocations;

    public MapInfo(GameObject map)
    {
        MetaDataObject = map;
        Transform typeo = map.transform.Find("Type");
        string maptype = typeo.GetChild(0).gameObject.name;

        switch (maptype)
        {
            case "Spawn":
                // init spawn
                mapType = MapType.Spawn;
                Ending = map.transform.Find("Ending").position;
                SpawnLocation = map.transform.Find("SpawnPoint").position;
                break;
            case "Normalroom":
                // init norm room
                mapType = MapType.Normalroom;
                Ending = map.transform.Find("Ending").position;
                Begining = map.transform.Find("Begining").position;
                SpawnLocation = map.transform.Find("SpawnPoint").position;
                EnemySpawnLocations = new List<Vector3>();
                Transform[] ChildTransforms = map.transform.Find("EnemySpawns").GetComponentsInChildren<Transform>();
                foreach (Transform item in ChildTransforms)
                {
                    EnemySpawnLocations.Add(item.position);
                }
                break;
            case "BlockingGate":
                mapType = MapType.BlockingGate;
                Center = map.transform.Find("Center").position;
                break;
            default:
                break;
        }
    }

    public void Refresh()
    {
        Transform typeo = MetaDataObject.transform.Find("Type");
        string maptype = typeo.GetChild(0).gameObject.name;
        switch (maptype)
        {
            case "Spawn":
                // init spawn
                mapType = MapType.Spawn;
                Ending = MetaDataObject.transform.Find("Ending").position;
                SpawnLocation = MetaDataObject.transform.Find("SpawnPoint").position;
                break;
            case "Normalroom":
                // init norm room
                mapType = MapType.Normalroom;
                Ending = MetaDataObject.transform.Find("Ending").position;
                Begining = MetaDataObject.transform.Find("Begining").position;
                SpawnLocation = MetaDataObject.transform.Find("SpawnPoint").position;
                EnemySpawnLocations = new List<Vector3>();
                Transform[] ChildTransforms = MetaDataObject.transform.Find("EnemySpawns").GetComponentsInChildren<Transform>();
                foreach (Transform item in ChildTransforms)
                {
                    EnemySpawnLocations.Add(item.position);
                }
                break;
            default:
                break;
        }
    }
}
