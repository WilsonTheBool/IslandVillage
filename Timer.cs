using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class Timer : MonoBehaviour
    {
        public BaseTownTile owner;

        private void Start()
        {
            owner = GetComponent<BaseTownTile>();
            

            if (owner.CycleTimeSec > 0)
            {
                StartCoroutine(CycleTimer());
            }
        }

        IEnumerator CycleTimer()
        {
            yield return new WaitForSeconds(owner.CycleTimeSec);

            owner.OnNewCycle();
            if(owner.gameObject.activeSelf)
            StartCoroutine(CycleTimer());
        }

    }
}
