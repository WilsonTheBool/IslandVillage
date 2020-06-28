using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class House: Structure
    {

        public void Update()
        {
            UpdateWorkers();
        }
        public override void SetUp()
        {
            game = FindObjectOfType<GameController>();

            resourcseObj = new List<ResourceObject>();

            if (maxWorkers == 0)
            maxWorkers = 4;

            if (resoursesPerWorker == 0)
            {
                resoursesPerWorker = 1;
            }

            game.maxPopulation += maxWorkers;



            onDelete += OnDelete;

        }

        //OnStart += добавить макс кол-во жителей (4);
        public override void OnNewCycle()
        {
            if (!isDeleted)
            {
                EatFood();

            }
        }

        private void OnDelete(object obj, EventArgs e)
        {
            ReduceMaxPopulation();
        }

        private void ReduceMaxPopulation()
        {
            game.maxPopulation -= maxWorkers;

        }

        public event EventHandler populationStarved;

        private void EatFood()
        {
            if (workers > 0)
            {


                float eatAmmount = workers * resoursesPerWorker;
                if (game.food >= eatAmmount)
                {
                    game.food -= eatAmmount;

                    ShowFoodAnimation();

                    GetNewVilagers();


                }
                else
                {
                    if (game.food != 0)
                    {
                        game.food = 0;
                        ShowFoodAnimation();
                    }

                    if (game.TryStarvePopulation())
                    {
                        workers -= 1;
                        game.population -= 1;
                        populationStarved?.Invoke(this, null);
                        ShowStarvePeopleAnim();
                    }
                }
            }
            else
            {
                if(game.food > 0)
                GetNewVilagers();
            }
        }

        private int AnimationCount;

        public GameObject fadeSpendFood;
        private void ShowFoodAnimation()
        {
            //StartCoroutine(ShowAnimation(fadeSpendFood, AnimationCount * 1, false));
        }

        private IEnumerator ShowAnimation(GameObject obj, float timeDelay, bool isIncome)
        {
            AnimationCount++;
            yield return new WaitForSeconds(timeDelay);

            if(isIncome)
            Instantiate(obj, transform.position, new Quaternion());
            else
            Instantiate(obj, transform.position + new Vector3(0,0.8f,0), new Quaternion());
            
            AnimationCount--;
        }

        public GameObject fadeNewPeople;
        private void ShowNewPeopleAnim()
        {
            StartCoroutine(ShowAnimation(fadeNewPeople, AnimationCount * 1, true));
        }

        public GameObject fadeDeadPeople;
        private void ShowStarvePeopleAnim()
        {
            StartCoroutine(ShowAnimation(fadeDeadPeople, AnimationCount * 1, false));
        }

        private void GetNewVilagers()
        {
            if(workers < maxWorkers)
            {
                if (game.TryIncreacePopulation())
                {
                    game.population++;
                    workers++;

                    ShowNewPeopleAnim();
                }
            }
        }

        private void UpdateWorkers()
        {
            int homeless = game.GetHomeLessPeople();

            if(homeless > 0)
            {

                if (homeless <= maxWorkers - workers)
                    AddPeople(homeless);
                else
                {
                    AddPeople(maxWorkers - workers);
                }
            }
        }

        private void AddPeople(int count)
        {
            workers += count;

        }

        public override void AddWorker(int count)
        {
            
        }

        public override void RemoveWorker(int count)
        {

        }
        public override void UpdateOwnedTiles(ResourceObject tile)
        {
            
        }

        public override void AddResourcesToPlayer(float ammount, Resource res)
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

        public override string GetInfo()
        {
            string str = tileName + "\n";

            str += $"People: {workers}/{maxWorkers}\n";

            str += $"Food consumption: ({workers * resoursesPerWorker})\n";


            return str;
        }
    }
}
