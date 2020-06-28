using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Assets.Scripts.TownTiles;
using Assets.Scripts;

public class Farm : Structure
{
    //свободные клетки - ресурсы
    // в них появляются клетки полей пшеницы
    // их собирают как обычные клетки ресурсов

    public float newFieldProb;

    public override void SetUp()
    {
        base.SetUp();

        if (TryCreateField())
        {
           maxWorkers = resourcseObj.Count * 2;
        }
        else
        {
            maxWorkers = 0;
        }

    }
    private bool TryCreateField()
    {
        Vector3Int[] vec = game.GetNeighboursPosInTileMap(transform.position);

        bool bl = false;

        foreach(Vector3Int pos in vec)
        {
            if(game.structureMap.HasTile(pos) == false && game.landMap.GetTile(pos) == game.grassTile)
            {
                CreateField(pos);
                bl = true;
            }
           
        }

        if (bl)
        {
            return true;
        }
        return false;
    }


    public WheatField wheatField;

    private void CreateField(Vector3Int pos)
    {
        Vector3 worldPos = game.structureMap.CellToWorld(pos);
        game.structureMap.SetTile(pos, wheatFieldTile);

        GameObject obj = Instantiate<UnityEngine.GameObject>(wheatField.gameObject, worldPos, new Quaternion());

        ResourceObject res = obj.GetComponent<WheatField>();
        resourcseObj.Add(res);
        res.deleted += RemoveFieldFromList;
        game.structureInfo.SetPositionProperty(pos, "tileObj", obj as UnityEngine.Object);
        game.SpawnSmoke(worldPos);

        UpdateMaxWorkers();
        //UpdateOwnedTiles();
    }

    private void RemoveFieldFromList(object field, EventArgs e)
    {
        resourcseObj.Remove(field as ResourceObject);
    }
    private void ChangeResources()
    {
        if (workers != 0)
        {
            int wk = workers;
            int t;
            for (int i = 0; i < resourcseObj.Count; i++)
            {
                ResourceObject res = resourcseObj[i];
                if (wk >= 2)
                {
                    wk -= 2;
                    t = 2;

                }
                else
                {
                    t = wk;
                }

                res.AddResources(t * resoursesPerWorker, resourse);
                

                if (t < 2)
                {
                    break;
                }
            }

            AddResourcesToPlayer(workers * resoursesPerWorker, resourse);
        }
    }

    //OnStart += добавить макс кол-во жителей (4);
    public override void OnNewCycle()
    {
        if (!isDeleted && workers > 0)
        {
            ChangeResources();

            //if (workers == maxWorkers)
            if(UnityEngine.Random.Range(0f,1f) <= newFieldProb)
            {
                TryCreateField();
                

            }

            
        }
    }

   

    public TownGameTile wheatFieldTile;

    public override void UpdateMaxWorkers()
    {
        Vector3Int[] pos = game.GetNeighboursPosInTileMap(transform.position);
        int workersCount = 0;
        foreach(Vector3Int vec in pos)
        {
            
            if(game.landMap.GetTile(vec) == game.grassTile && (game.structureMap.GetTile(vec) == wheatFieldTile))
            {
                
                workersCount += 2;
            }
        }
        maxWorkers = workersCount;

        if (workers > maxWorkers)
        {
            RemoveWorker(workers - maxWorkers);
        }
    }

    public override void UpdateOwnedTiles(ResourceObject tile)
    {

    }

    public override void UpdateOwnedTiles()
    {

    }

    public override void Delete()
    {
        for (int i = resourcseObj.Count - 1; i >= 0; i--)
        {

            resourcseObj[i].Delete();
        }
        base.Delete();
    }

    private IEnumerator DeleteAllFields()
    {
        foreach (ResourceObject obj in resourcseObj)
        {
            yield return new WaitForSeconds(0.5f);
            obj.Delete();
        }
    }
}
