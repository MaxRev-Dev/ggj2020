using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class Randomizer : MonoBehaviour
    {
        public int maxItems = 3;
        public List<GameObject> Items;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }


        public GameObject GenerateOne(GameObject[] blocks)
        {
            int index = Random.Range(0, blocks.Length);
            return blocks[index];
        }
        public List<GameObject> Generate(GameObject[] blocks)
        {
            for (int i = 0; i < maxItems; i++)
            {
                int index = Random.Range(0, blocks.Length);
                Items.Add(blocks[index]);
            }

            return Items = Items.OrderBy(x => Random.Range(0f, 1f)).ToList();
        }

        public GameObject GetCurrentRand()
        {
            int index = Random.Range(0, Items.Count);
            return Items[index];
        }
    }
}
