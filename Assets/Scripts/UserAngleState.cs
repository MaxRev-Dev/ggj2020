using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class UserAngleState : MonoBehaviour
    {
        public Dictionary<string, float> List = new Dictionary<string, float>();

        public float rotateAmount = 30;
        public float rotateMultiplier = 6;
        public double maxRotateAmount => rotateMultiplier * 6;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        { 

        }

        public void TurnRight(GameObject o)
        {
            var id = HistoryManager.GetItemId(o);
            if (!List.ContainsKey(id))
            {
                List[id] = -rotateAmount;
                o.transform.Rotate(0, 0, -rotateAmount);
            }
            else
            {
                var current = List[id];
                if (current < maxRotateAmount)
                {
                    List[id] -= rotateAmount;
                    o.transform.Rotate(0, 0, -rotateAmount);
                }
            }
        }

        public void TurnLeft(GameObject o)
        {
            var id = HistoryManager.GetItemId(o);
            if (!List.ContainsKey(id))
            {
                o.transform.Rotate(0, 0, rotateAmount);
                List[id] = rotateAmount;
            }
            else
            {
                var current = List[id];
                if (-Mathf.Abs(current) > -maxRotateAmount)
                {
                    List[id] += rotateAmount;
                    o.transform.Rotate(0, 0, rotateAmount);
                }
            }
        }

        public float GetRotation(GameObject item)
        {
            var id = HistoryManager.GetItemId(item);
            if (List.ContainsKey(id))
            {
                var val = List[id];
                List[id] = 0;
                return val;
            }

            return 0;
        }
    }
}
