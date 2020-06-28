using System;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Scripts
{
    public class Resource: MonoBehaviour
    {
        public string resourceName;

        public enum ResourceType
        {
            wood, stone, food, wheat,
        }

        public ResourceType type;

        public Sprite resourceIcon;

        public float resourceAmmount;

        public float maxResource;

        public float regenPerCycle;

        public enum State
        {
            none, little, half, full
        }

        public State state;

        public bool isExtracting;

        public event EventHandler StateChanged;

        public event EventHandler ammountChanged;

        public BaseTownTile owner;

        public void AddResources(float ammount)
        {
            resourceAmmount += ammount;

            if(resourceAmmount > maxResource)
            {
                resourceAmmount = maxResource;
            }
            
            UpdateState();

            ammountChanged?.Invoke(this, null);
        }

        public void UpdateState()
        {
            State prev = state;

            if (resourceAmmount <= 0)
                state = State.none;
            else
            if (resourceAmmount <= maxResource * 0.2)
                state = State.little;
            else
            if (resourceAmmount <= maxResource * 0.5)
                state = State.half;
            else
                state = State.full;

            if(prev != state)
            {
                StateChanged?.Invoke(this, null);
            }
        }

        public void RemoveResources(float ammount)
        {
            resourceAmmount -= ammount;

            if (resourceAmmount < 0)
            {
                resourceAmmount = 0;
            }
            
            UpdateState();

            ammountChanged?.Invoke(this, null);
        }



        public string GetInfo()
        {
            string str = $"{resourceName} ({resourceAmmount})\n{Mathf.RoundToInt(resourceAmmount/maxResource*100)}%";

            return str;
        }

        public void Start()
        {
            owner.newCycle += OnNewCycle;
        }

        public void OnNewCycle(object resourceHolder, EventArgs e)
        {
            AddResources(regenPerCycle);
        }
    }
}
