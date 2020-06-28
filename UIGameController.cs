using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Assets.Scripts
{
    public class UIGameController : MonoBehaviour
    {
        public GameController game;

        public Text woodCounted;
        public Text stoneCounted;
        public Text foodCounted;
        public Text wheatCount;
        public Text workersCounted;
        public Text maxWorkersCounted;

        public Text foodDiff;

        public Text noWorkPeople;
        public Text WorkPeople;
        public void Update()
        {
            UpdateUI();
        }

        public void Start()
        {
            //woodIcon.sprite = game.woodRes.resourceIcon;
            //stoneIcon.sprite = game.stoneRes.resourceIcon;
            game = FindObjectOfType<GameController>();

            StartCoroutine(FoodDefCycle());
        }

        public void OnResourceAdded(Resource res, Structure struc)
        {
            //struc.gameobject.transform.position;
        }
        public void UpdateUI()
        {
            

            woodCounted.text = game.wood.ToString();
           
            stoneCounted.text = game.stone.ToString();

            foodCounted.text = game.food.ToString();

            wheatCount.text = game.wheat.ToString();

            workersCounted.text = game.population.ToString();

            maxWorkersCounted.text = game.maxPopulation.ToString();

            noWorkPeople.text = (game.population - game.workingPeople).ToString();
            WorkPeople.text = ( game.workingPeople).ToString();
        }

        public float FoodCycleTime;
        private IEnumerator FoodDefCycle()
        {
            yield return new WaitForSeconds(FoodCycleTime);

            GetFoodDifText();
            
            StartCoroutine(FoodDefCycle());
        }
        private void GetFoodDifText()
        {
            float dif = game.GetFoodCycleDif();
            if(dif < 0)
            {
                foodDiff.color = Color.red;
                foodDiff.text = "-";
            }
            else
            {
                if (dif == 0)
                {
                    foodDiff.color = Color.white;
                }
                else
                {
                    foodDiff.color = Color.green;
                }

                foodDiff.text = "+";
            }

            foodDiff.text += dif.ToString();
        }
    }
}
