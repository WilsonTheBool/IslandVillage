using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Assets.Scripts.TownTiles;
using Assets.Scripts;
public class GameController : MonoBehaviour
{
    // Start is called before the first frame update
    public float wood;
    public Resource woodRes;

    public float stone;
    public Resource stoneRes;

    public float food;
    public Resource foodRes;

    private float foodLast;

    public float wheat;
    public Resource wheatREs;

    public int population;

    public int maxPopulation;

    public int workingPeople;

    public Tilemap structureMap;
    public GridInformation structureInfo;

    public Tilemap landMap;
    public GridInformation landInfo;

    public Timer TimerPrefab;

    public UIGameController ui;

    public OnTileHoverScript inputController;
    private BuildModeController buildModeController;

    public Camera mainCamera;

    //populatiion
    public float newVilagerProb;
    public float starvePosibility;

    public float GetFoodCycleDif()
    {
        float d = food - foodLast;
        foodLast = food;
        return d;
    }
    public int GetHomeLessPeople()
    {
        int home = 0;
        foreach (Structure struc in allStrucutures)
        {
            if ((struc is House))
            {
                home += struc.workers;
            }
        }

        return population - home;
    }

    public void UpdateWorkingPopulation()
    {
        int working = 0;
        foreach(Structure struc in allStrucutures)
        {
            if(!(struc is House))
            {
                working += struc.workers;
            }
        }
        int dif = 0;
        if(working > population)
        {
            working = population;
            dif = working - population;

            for(int i = 0; i < dif; i++)
            foreach(Structure s in allStrucutures)
            {
                if((!(s is House)) && (s.workers > 0))
                {
                    s.RemoveWorker(1);
                }
            }
        }

        workingPeople = working;
    }
    public bool TryIncreacePopulation()
    {
        //множитель - соотношение оставшийся еды на всю популяцию
        float scale = food / population;

        float probability = newVilagerProb * scale;

        if(UnityEngine.Random.Range(0f,1f) <= probability)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool TryStarvePopulation()
    {
        if (UnityEngine.Random.Range(0f, 1f) <= starvePosibility)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void Start()
    {
        

        ui = GetComponent<UIGameController>();
        
        structureInfo = structureMap.GetComponent<GridInformation>();
        landInfo = landMap.GetComponent<GridInformation>();
        inputController = FindObjectOfType<OnTileHoverScript>();
        buildModeController = FindObjectOfType<BuildModeController>();

        buildModeController.enterMode += OnBuildModdeEnter;
        buildModeController.leaveMode += OnBuildModdeLeave;
        //CreateMap();
        allStrucutures = new List<Structure>();
        foodLast = food;
        LoadMap();
    }
    public void SetTimers()
    {
       
    }
    public TownGameTile grassTile;
    public TownGameTile sandTile;

    public TownGameTile forestTile;
    public GameObject forest;

    public TownGameTile woodChoperTile;
    public GameObject woodChoper;

    public TownGameTile FisherHomeTile;
    public GameObject FisherHome;

    public TownGameTile FishTile;
    public GameObject Fish;

    public TownGameTile FruitTile;
    public GameObject Fruit;

    public TownGameTile mountainTile;
    public GameObject mountain;

    public TownGameTile mineTile;
    public GameObject mine;

    public List<Structure> allStrucutures;

    void CreateMap()
    {
        GridInformation info = structureMap.GetComponent<GridInformation>();
        Vector3Int pos;
        for (int x = -2; x <= 2; x++)
        {
            for(int y = -2; y <= 2; y++)
            {
                pos = new Vector3Int(x, y, 0);
                if(pos != new Vector3Int(1, 1, 0) && pos != new Vector3Int(-1, -1, 0))
                {
                    if(UnityEngine.Random.Range(0f, 1f) <= 0.5f)
                    {
                        structureMap.SetTile(pos, forestTile);
                        info.SetPositionProperty(pos, "tileObj", Instantiate(forest, structureMap.CellToWorld(pos), new Quaternion()) as UnityEngine.Object);
                    }
                    else
                    {
                        structureMap.SetTile(pos, mountainTile);
                        info.SetPositionProperty(pos, "tileObj", Instantiate(mountain, structureMap.CellToWorld(pos), new Quaternion()) as UnityEngine.Object);
                    }
                    
                }
               
                
            }
        }

        pos = new Vector3Int(1, 1, 0);
        structureMap.SetTile(pos, woodChoperTile);
        info.SetPositionProperty(pos, "tileObj", Instantiate(woodChoper, structureMap.CellToWorld(pos), new Quaternion()) as UnityEngine.Object);

        pos = new Vector3Int(-1, -1, 0);
        structureMap.SetTile(pos, mineTile);
        info.SetPositionProperty(pos, "tileObj", Instantiate(mine, structureMap.CellToWorld(pos), new Quaternion()) as UnityEngine.Object);




    }

    void LoadMap()
    {
        GridInformation info = structureMap.GetComponent<GridInformation>();

        
        //create resources
        foreach (Vector3Int pos in structureMap.cellBounds.allPositionsWithin)
        {
            TileBase tile = structureMap.GetTile(pos);
            if(tile != null && tile is TownGameTile)
            {
                if(tile == forestTile)
                {

                    info.SetPositionProperty(pos, "tileObj", Instantiate(forest, structureMap.CellToWorld(pos), new Quaternion()) as UnityEngine.Object);
                }
                else
                if (tile == mountainTile)
                {

                    info.SetPositionProperty(pos, "tileObj", Instantiate(mountain, structureMap.CellToWorld(pos), new Quaternion()) as UnityEngine.Object);
                }
                if (tile == FishTile)
                {

                    info.SetPositionProperty(pos, "tileObj", Instantiate(Fish, structureMap.CellToWorld(pos), new Quaternion()) as UnityEngine.Object);
                }
                if (tile == FruitTile)
                {

                    info.SetPositionProperty(pos, "tileObj", Instantiate(Fruit, structureMap.CellToWorld(pos), new Quaternion()) as UnityEngine.Object);
                }


            }
        }


        //create structures
        foreach (Vector3Int pos in structureMap.cellBounds.allPositionsWithin)
        {
            TileBase tile = structureMap.GetTile(pos);
            if (tile != null && tile is TownGameTile)
            {
                if (tile == woodChoperTile)
                {

                    info.SetPositionProperty(pos, "tileObj", Instantiate(woodChoper, structureMap.CellToWorld(pos), new Quaternion()) as UnityEngine.Object);
                }
                else
                if (tile == mineTile)
                {

                    info.SetPositionProperty(pos, "tileObj", Instantiate(mine, structureMap.CellToWorld(pos), new Quaternion()) as UnityEngine.Object);

                }
                if (tile == FisherHomeTile)
                {

                    info.SetPositionProperty(pos, "tileObj", Instantiate(FisherHome, structureMap.CellToWorld(pos), new Quaternion()) as UnityEngine.Object);

                }


            }
        }
    }

    public void LoadMap(Tilemap structure)
    {
        GridInformation info = structureMap.GetComponent<GridInformation>();


        //create resources
        foreach (Vector3Int pos in structure.cellBounds.allPositionsWithin)
        {
            if (structure.HasTile(pos))
            {
                TileBase tile = structure.GetTile(pos);
                if (tile != null && tile is TownGameTile)
                {
                    if (tile == forestTile)
                    {

                        info.SetPositionProperty(pos, "tileObj", Instantiate(forest, structureMap.CellToWorld(pos), new Quaternion()) as UnityEngine.Object);
                    }
                    else
                    if (tile == mountainTile)
                    {

                        info.SetPositionProperty(pos, "tileObj", Instantiate(mountain, structureMap.CellToWorld(pos), new Quaternion()) as UnityEngine.Object);
                    }
                    if (tile == FishTile)
                    {

                        info.SetPositionProperty(pos, "tileObj", Instantiate(Fish, structureMap.CellToWorld(pos), new Quaternion()) as UnityEngine.Object);
                    }
                    if (tile == FruitTile)
                    {

                        info.SetPositionProperty(pos, "tileObj", Instantiate(Fruit, structureMap.CellToWorld(pos), new Quaternion()) as UnityEngine.Object);
                    }


                }
            }
        }

        ////create structures
        //foreach (Vector3Int pos in structureMap.cellBounds.allPositionsWithin)
        //{
        //    TileBase tile = structureMap.GetTile(pos);
        //    if (tile != null && tile is TownGameTile)
        //    {
        //        if (tile == woodChoperTile)
        //        {

        //            info.SetPositionProperty(pos, "tileObj", Instantiate(woodChoper, structureMap.CellToWorld(pos), new Quaternion()) as UnityEngine.Object);
        //        }
        //        else
        //        if (tile == mineTile)
        //        {

        //            info.SetPositionProperty(pos, "tileObj", Instantiate(mine, structureMap.CellToWorld(pos), new Quaternion()) as UnityEngine.Object);

        //        }
        //        if (tile == FisherHomeTile)
        //        {

        //            info.SetPositionProperty(pos, "tileObj", Instantiate(FisherHome, structureMap.CellToWorld(pos), new Quaternion()) as UnityEngine.Object);

        //        }


        //    }
        //}
    }

    public void AddResources(float ammount, Resource res, Structure struc)
    {
        switch (res.type)
        {
            case Resource.ResourceType.wood:
                {
                    wood += ammount;
                    break;
                }
            case Resource.ResourceType.stone:
                {
                    stone += ammount;
                    break;
                }
            case Resource.ResourceType.food:
                {
                    food += ammount;
                    break;
                }
            case Resource.ResourceType.wheat:
                {
                    wheat += ammount;
                    break;
                }
        }



        ui.OnResourceAdded(res, struc);
    }

    public void AddStructure(Structure structure)
    {
        allStrucutures.Add(structure);
        structure.onDelete += OnStructureDelete;
    }

   

    public void OnStructureDelete(object structure, EventArgs e)
    {
        RemoveStructure(structure as Structure);
    }
    public void RemoveStructure(Structure structure)
    {
        allStrucutures.Remove(structure);
    }


    public bool HaveResourcesFor(Structure structure)
    {
        if(structure.costStone <= stone && structure.costWood <= wood)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void RemoveResources(float ammount, Resource res)
    {
        switch (res.type)
        {
            case Resource.ResourceType.wood:
                {
                    wood -= ammount;
                    if(wood < 0)
                    {
                        wood = 0;
                    }
                    break;
                }
            case Resource.ResourceType.stone:
                {
                    stone -= ammount;
                    if (stone < 0)
                    {
                        stone = 0;
                    }
                    break;
                }
            case Resource.ResourceType.food:
                {
                    food -= ammount;
                    if (food < 0)
                    {
                        food = 0;
                    }
                    break;
                }
            case Resource.ResourceType.wheat:
                {
                    wheat -= ammount;
                    if (wheat < 0)
                    {
                        wheat = 0;
                    }
                    break;
                }
        }



        
    }

    public ResourceObject[] GetAllResourceTilesOfType(Resource type, Structure struc)
    {
        List<ResourceObject> list = new List<ResourceObject>();

        Vector3 worldPos = struc.transform.position;
        Vector3Int pos = landMap.WorldToCell(worldPos);
        
       
        ResourceObject test;

        Vector3Int neighborPos = landMap.WorldToCell(worldPos + new Vector3(-1, 0, 0));
        if(HasResource(neighborPos, type, out test))
        {
            list.Add(test);
        }
        neighborPos = landMap.WorldToCell(worldPos + new Vector3(1, 0, 0));
        if (HasResource(neighborPos, type, out test))
        {
            list.Add(test);
        }
        neighborPos = landMap.WorldToCell(worldPos + new Vector3(0.5f, 0.75f, 0));
        if (HasResource(neighborPos, type, out test))
        {
            list.Add(test);
        }
        neighborPos = landMap.WorldToCell(worldPos + new Vector3(-0.5f, 0.75f, 0));
        if (HasResource(neighborPos, type, out test))
        {
            list.Add(test);
        }
        neighborPos = landMap.WorldToCell(worldPos + new Vector3(0.5f, -0.75f, 0));
        if (HasResource(neighborPos, type, out test))
        {
            list.Add(test);
        }
        neighborPos = landMap.WorldToCell(worldPos + new Vector3(-0.5f, -0.75f, 0));
        if (HasResource(neighborPos, type, out test))
        {
            list.Add(test);
        }

        return list.ToArray();

    }

    private bool HasResource(Vector3Int pos, Resource type, out ResourceObject resObj)
    {
        if (structureMap.HasTile(pos))
        {
            
            GameObject obj = structureInfo.GetPositionProperty<UnityEngine.Object>(pos, "tileObj", null) as GameObject;
            if (obj != null)
            {


                var tile = obj.GetComponent<ResourceObject>();
                if (tile != null)
                {
                    if (tile != null && tile.HasResource(type))
                    {

                        resObj = tile;
                        return true;
                    }
                    
                }
            }
        }
        
        resObj = null;
        return false;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitMenu();
        }

        UpdateWorkingPopulation();

        if (!isBuildMode)
        {
            inputController.UpdateInput();
        }
        else
        {
            buildModeController.UpdateInput();
        }
    }

    private bool isBuildMode;
    public void OnBuildModdeEnter(object mode, EventArgs e)
    {
        isBuildMode = true;
    }

    public void OnBuildModdeLeave(object mode, EventArgs e)
    {
        isBuildMode = false;
    }

    public GameObject smoke;
    float smokeTime = 0.8f;
    public void SpawnSmoke(Vector3 pos)
    {
        Destroy(Instantiate(smoke, pos, new Quaternion(), this.transform), smokeTime);
    }

    public Vector3Int[] GetNeighboursPosInTileMap(Vector3 pos)
    {
        Vector3Int[] neighbours = new Vector3Int[6];

        neighbours[0] = structureMap.WorldToCell(pos + new Vector3(-1, 0, 0));
        neighbours[1] = structureMap.WorldToCell(pos + new Vector3(1, 0, 0));
        neighbours[2] = structureMap.WorldToCell(pos + new Vector3(0.5f, 0.75f, 0));
        neighbours[3] = structureMap.WorldToCell(pos + new Vector3(-0.5f, 0.75f, 0));
        neighbours[4] = structureMap.WorldToCell(pos + new Vector3(0.5f, -0.75f, 0));
        neighbours[5] = structureMap.WorldToCell(pos + new Vector3(-0.5f, -0.75f, 0));

        return neighbours;
    }

    public void ExitMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
