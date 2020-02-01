using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Randomizer : MonoBehaviour
    {
        public int maxItems = 10;
        public List<GameObject> Items;

        // Start is called before the first frame update
        void Start()
        {
            Items = new List<GameObject>();
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
            return Items;
        }
    }
}
