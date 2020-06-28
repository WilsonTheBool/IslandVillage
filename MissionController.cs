using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Tilemaps;
using Assets.Scripts;


public class MissionController : MonoBehaviour
{

    public List<Mission> missions;

    public float landmassDelay;

    public float structureDelay;

    private Vector3Int startTilePos;

    public Vector3 newCameraPos;


    public GameController game;

    private void Start()
    {
        game = FindObjectOfType<GameController>();

        
        foreach(Mission mission in missions)
        {
            mission.isActive = true;
            
            
        }
        missionUI.SetActive(false);
        missionUI2.SetActive(false);
        hasMission = false;
    }

    public GameObject missionUI;

    public GameObject missionUI2;

  
    public void Update()
    {
        
        UpdateMissions();

    }
    public void UpdateMissions()
    {
        if (hasMission)
        {
            foreach (Mission mission in missions)
            {
                if (mission.isActive && game.population >= mission.PopulationReq)
                {
                    hasMission = false;
                    if (missions.IndexOf(mission) == 0)
                    {
                        missionUI.SetActive(false);
                    }
                    if (missions.IndexOf(mission) == 1)
                    {
                        missionUI2.SetActive(false);
                    }
                    mission.isActive = false;
                    UnlockMission(mission);
                    return;

                }
            }
        }
        else
        {
            foreach (Mission mission in missions)
            {
                if (mission.isActive)
                {
                    hasMission = true;
                    if(missions.IndexOf(mission) == 0)
                    {
                        missionUI.SetActive(true);
                    }
                    if (missions.IndexOf(mission) == 1)
                    {
                        missionUI2.SetActive(true);
                    }
                        
                    
                    return;

                }
                
            }
        }
        


    }

    private bool hasMission;
    public void UnlockMission(Mission mission)
    {
        //Vector3Int startPos = FindMinPos(mission);
        
        
        StartCoroutine(UnlockAllTiles(mission));
    }

    private Vector3Int FindMinPos(Mission mission)
    {
        Vector3Int min = new Vector3Int(int.MaxValue, int.MaxValue, 0);
        foreach (Vector3Int pos in mission.landMap.cellBounds.allPositionsWithin)
        {
            
            if(mission.landMap.HasTile(pos))
            if((pos - startTilePos).magnitude <= (min - startTilePos).magnitude)
            {
                min = pos;
            }
        }

        return min;
    }
    
    private IEnumerator UnlockAllTiles(Mission mission)
    {
        yield return new WaitForSeconds(landmassDelay);

        newCameraPos = mission.cameraPos;

        StartCoroutine(TranslateCamera());

        foreach (Vector3Int pos in mission.landMap.cellBounds.allPositionsWithin)
        {
            

            if (mission.landMap.HasTile(pos))
            {

                if (game.structureMap.HasTile(pos))
                {   
                    
                    if(game.structureInfo == null)
                    {
                        Debug.LogError("NULL IN INFO");
                    }

                    GameObject obj = game.structureInfo.GetPositionProperty<UnityEngine.Object>(pos, "tileObj", null) as GameObject;
                    if(obj != null)
                    {
                        Destroy(obj);
                        game.structureInfo.ErasePositionProperty(pos, "tileObj");
                    }
                   

                    //добавить DELETE для тайлов?

                    game.structureMap.SetTile(pos, null);

                }

                if (game.landMap.HasTile(pos))
                {
                    game.landMap.SetTile(pos, null);
                }

                game.SpawnSmoke(game.landMap.CellToWorld(pos));

                game.landMap.SetTile(pos, mission.landMap.GetTile(pos));

                

                game.structureMap.SetTile(pos, mission.structureMap.GetTile(pos));
            }
        }

        game.LoadMap(mission.structureMap);
    }
    private IEnumerator UnlockTile(Vector3Int pos, Mission mission)
    {
        Debug.Log(pos);
        yield return new WaitForSeconds(landmassDelay);

        if (game.structureMap.HasTile(pos))
        {
            game.structureInfo.ErasePositionProperty(pos, "tileObj");

            //добавить DELETE для тайлов?

            game.structureMap.SetTile(pos, null);

        }

        if (game.landMap.HasTile(pos))
        {
            game.landMap.SetTile(pos, null);
        }

        game.SpawnSmoke(game.landMap.CellToWorld(pos));

        game.landMap.SetTile(pos, mission.landMap.GetTile(pos));

       

        yield return new WaitForSeconds(structureDelay);

        game.structureMap.SetTile(pos, mission.structureMap.GetTile(pos));


        Vector3Int[] vecs = game.GetNeighboursPosInTileMap(game.landMap.CellToWorld(pos));

        foreach (Vector3Int vec in vecs)
        {
            if (mission.landMap.HasTile(vec))
            {
                
                if(mission.landMap.GetTile(pos) != game.landMap.GetTile(pos))
                StartCoroutine(UnlockTile(vec, mission));
            }
        }


    }

    private IEnumerator TranslateCamera()
    {
        Camera cam = game.mainCamera;
        float T = 5;
        float t = 0;

        while(cam.transform.position != newCameraPos)
        {
            t += Time.deltaTime;

            cam.transform.position = Vector3.Lerp(cam.transform.position, newCameraPos, t / T);
            yield return null;
        }
    }
}
