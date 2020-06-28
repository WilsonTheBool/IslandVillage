using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Assets.Scripts;
using Assets.Scripts.TownTiles;

public class BuildModeController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
        game = FindObjectOfType<GameController>();
        cam = game.mainCamera;
        instances = new List<GameObject>();
    }
    public GameObject mainWindow;

    public Structure woodchoper;
    public Structure mine;
    public Structure fisher;
    public Structure forager;
    public Structure farm;
    public Structure windmill;

    public House house;

    public GameController game;

    public event EventHandler enterMode;

    public event EventHandler leaveMode;

    public bool isBuildMode;

    private Structure structureToBuild;

    public GameObject empty;

    private GameObject instance;

    public void StartBuildMine()
    {
        if(game.HaveResourcesFor(mine))
        EnterBuildMode(mine);
    }

    public void StartBuildForager()
    {
        if (game.HaveResourcesFor(forager))
            EnterBuildMode(forager);
    }

    public void StartBuildFisher()
    {
        if (game.HaveResourcesFor(fisher))
            EnterBuildMode(fisher);
    }
    public void StartBuildWoodChoper()
    {
        if (game.HaveResourcesFor(woodchoper))
            EnterBuildMode(woodchoper);
    }

    public void StartBuildHouse()
    {
        if (game.HaveResourcesFor(house))
            EnterBuildMode(house);
    }

    public void StartBuildFarmHouse()
    {
        if (game.HaveResourcesFor(farm))
            EnterBuildMode(farm);
    }
    public void StartBuildWindmill()
    {
        if (game.HaveResourcesFor(windmill))
            EnterBuildMode(windmill);
    }


    public void EnterBuildMode(Structure struc)
    {
        isBuildMode = true;
        HideWindow();
        structureToBuild = struc;
        
        instance = Instantiate(empty);
        instance.GetComponent<SpriteRenderer>().sprite = structureToBuild.structureSprite;

        

        enterMode?.Invoke(this, null);
    }

    public void LeaveBuildMode()
    {
        isBuildMode = false;

        Destroy(instance);

        instance = null;
        
        foreach (GameObject inst in instances)
        {
            Destroy(inst);
        }

        instances.Clear();

        //if (range_2.activeSelf == true)
        //{
        //    range_2.SetActive(false);
        //}
        ShowWindow();

        game.inputController.ClearSelectedObj();

        leaveMode?.Invoke(this, null);
    }

    public void HideShowWindow()
    {
        if (mainWindow.activeSelf)
        {
            HideWindow();
        }
        else
        {
            if(!isBuildMode)
            ShowWindow();
        }
    }
    private void HideWindow()
    {
        mainWindow.SetActive(false);
    }

    private void ShowWindow()
    {
        mainWindow.SetActive(true);
    }
    private Camera cam;


    public GameObject range_2;

    public GameObject canBuildTile;

    public GameObject notBuildTile;

    public void UpdateInput()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        Vector3 perfPos = game.structureMap.CellToWorld(game.structureMap.WorldToCell(mousePos));

        instance.transform.position = perfPos;

        
        if(structureToBuild is House || structureToBuild is Windmill)
        {

        }
        else
        {
            range_2.transform.position = perfPos;
        }
        

        DrawHelpTiles(perfPos);

       
        if (Input.GetMouseButtonDown(0))
        {
            Vector3Int mainTilePos = game.landMap.WorldToCell(perfPos);
            if (!game.structureMap.HasTile(mainTilePos) && structureToBuild.CanBuild(game.landMap.GetTile(mainTilePos) as TownGameTile) == true)
            {
                AcceptBuilding(mainTilePos);

                LeaveBuildMode();
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            LeaveBuildMode();
            game.inputController.HideRangeTile();
        }

    }

    private void AcceptBuilding(Vector3Int pos)
    {
        if (game.HaveResourcesFor(structureToBuild))
        {
            game.RemoveResources(structureToBuild.costWood, game.woodRes);
            game.RemoveResources(structureToBuild.costStone, game.stoneRes);

            Vector3 worldPos = game.structureMap.CellToWorld(pos);
            game.structureMap.SetTile(pos, structureToBuild.tile);

            GameObject obj = Instantiate<UnityEngine.GameObject>(structureToBuild.gameObject, worldPos, new Quaternion());

            game.AddStructure(obj.GetComponent<Structure>());

            game.structureInfo.SetPositionProperty(pos, "tileObj", obj as UnityEngine.Object);
            game.SpawnSmoke(worldPos);
            game.inputController.ClearSelectedObj();
            game.inputController.selectedTile = obj.GetComponent<Structure>();
            game.inputController.UpdateSelectedTile(null, null);
        }
        else
        {
            ShowError();
        }
    }

    public void ShowError()
    {

    }

    List<GameObject> instances;
    private void DrawHelpTiles(Vector3 perfPos)
    {
        foreach(GameObject inst in instances)
        {
            Destroy(inst);
        }

        instances.Clear();

        //main tile
        Vector3Int mainTilePos = game.landMap.WorldToCell(perfPos);
        if (!game.structureMap.HasTile(mainTilePos) && structureToBuild.CanBuild(game.landMap.GetTile(mainTilePos) as TownGameTile) == true)
        {
            instances.Add(Instantiate(canBuildTile,perfPos, new Quaternion()));
        }
        else
        {
            instances.Add(Instantiate(notBuildTile, perfPos, new Quaternion()));
        }

        if (!(structureToBuild is House))
        {
            if(structureToBuild is Farm)
            {
                Vector3Int[] posTiles = GetNeighboursPosInTileMap(perfPos);

                foreach (Vector3Int pos in posTiles)
                {

                    if (game.landMap.GetTile(pos) == game.grassTile)
                    {
                        if (game.structureMap.HasTile(pos))
                        {   

                            GameObject obj = game.structureInfo.GetPositionProperty<UnityEngine.Object>(pos, "tileObj", null) as GameObject;
                            if (obj != null)
                            {
                                WheatField tile = obj.GetComponent<WheatField>();

                                if (tile != null)
                                {
                                    instances.Add(Instantiate(canBuildTile, game.structureMap.CellToWorld(pos), new Quaternion()));
                                }
                            }
                        }
                        else
                        {
                            instances.Add(Instantiate(canBuildTile, game.structureMap.CellToWorld(pos), new Quaternion()));
                        }

                    }
                }
            }
            else
            {
                Vector3Int[] posTiles = GetNeighboursPosInTileMap(perfPos);

                foreach (Vector3Int pos in posTiles)
                {
                    if (game.structureMap.HasTile(pos))
                    {
                        GameObject obj = game.structureInfo.GetPositionProperty<UnityEngine.Object>(pos, "tileObj", null) as GameObject;
                        if (obj != null)
                        {
                            ResourceObject tile = obj.GetComponent<ResourceObject>();

                            if (tile != null && tile.HasResource(structureToBuild.resourse))
                            {
                                instances.Add(Instantiate(canBuildTile, game.structureMap.CellToWorld(pos), new Quaternion()));
                            }
                        }
                    }
                }
            }

            
        }
    }

    public Vector3Int[] GetNeighboursPosInTileMap(Vector3 pos)
    {
        Vector3Int[] neighbours = new Vector3Int[6];

        neighbours[0] = game.structureMap.WorldToCell(pos + new Vector3(-1, 0, 0));
        neighbours[1] = game.structureMap.WorldToCell(pos + new Vector3(1, 0, 0));
        neighbours[2] = game.structureMap.WorldToCell(pos + new Vector3(0.5f, 0.75f, 0));
        neighbours[3] = game.structureMap.WorldToCell(pos + new Vector3(-0.5f, 0.75f, 0));
        neighbours[4] = game.structureMap.WorldToCell(pos + new Vector3(0.5f, -0.75f, 0));
        neighbours[5] = game.structureMap.WorldToCell(pos + new Vector3(-0.5f, -0.75f, 0));

        return neighbours;
    }
}
