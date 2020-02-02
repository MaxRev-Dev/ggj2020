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
            var gameManager = GameObject.FindObjectOfType<GameManager>();

            foreach (var item in gameManager.editable)
            {
                var key = HistoryManager.GetItemId(item);

                if (!List.ContainsKey(key))
                {
                    List[key] = 0;
                }
                else
                {
                    var angle = List[key];
                    item.transform.Rotate(0, 0, angle);
                    List[key] = 0;
                }
            }
        }

        public void TurnRight(GameObject o)
        {
            var id = HistoryManager.GetItemId(o);
            if (!List.ContainsKey(id))
            {
                List[id] = rotateAmount;
            }
            else
            {
                var current = List[id];
                if (current < maxRotateAmount)
                {
                    List[id] += rotateAmount;
                }
            }
        }

        public void TurnLeft(GameObject o)
        {
            var id = HistoryManager.GetItemId(o);
            if (!List.ContainsKey(id))
            {
                List[id] = -rotateAmount;
            }
            else
            {
                var current = List[id];
                if (-Mathf.Abs(current) > -maxRotateAmount)
                {
                    List[id] -= rotateAmount;
                }
            }
        }
    }
}
