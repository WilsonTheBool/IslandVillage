using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class Windmill : Structure
{
   
    public override void SetUp()
    {
        game = FindObjectOfType<GameController>();

        resourcseObj = new List<ResourceObject>();

        maxWorkers = 4;

        workers = 0;

    }

    public Resource wheat;

    public override void OnNewCycle()
    {
        if(!isDeleted && workers > 0)
        {
            float ammount = workers * resoursesPerWorker;

            if(game.wheat > 0)
            {
                if(game.wheat > ammount)
                {
                    
                    AddResourcesToPlayer(ammount, resourse);
                    game.RemoveResources(ammount, wheat);
                }
                else
                {
                    AddResourcesToPlayer(game.wheat, resourse);
                    game.RemoveResources(game.wheat, wheat);
                }
            }
        }
    }




    public override void UpdateOwnedTiles(ResourceObject tile)
    {

    }


    public override void OnResourceRemoved(object resObj, EventArgs e)
    {

    }

    public override void UpdateOwnedTiles()
    {

    }

    public override void UpdateMaxWorkers()
    {

    }
}
