using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        private GameObject[] _blocks;

        // Start is called before the first frame update
        void Start()
        {
            _blocks = GameObject.FindGameObjectsWithTag("BuildingBlock");
        }

        private int _bombScales;

        private bool _bombTriggered;

        // Update is called once per frame
        void Update()
        { 
        }
         
        private GameObject[] GetBombs()
        {
            return GameObject.FindGameObjectsWithTag("Bomb");
        }
    }

}