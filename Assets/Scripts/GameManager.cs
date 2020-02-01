using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Explosions;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        private GameObject[] _blocks;
        private bool _initialized;
        private List<GameObject> editable;

        // Start is called before the first frame update
        void Start()
        {
            _blocks = GameObject.FindGameObjectsWithTag("BuildingBlock");
        }

        // Update is called once per frame
        void Update()
        {
            if (_initialized) return;
            _initialized = true;
            RandomizeActiveObjects();

            var cassette = GameObject.FindObjectOfType<BombCassette>();
            StartRecordWithDelay(cassette.commonDelay);
            StartCoroutine(WaitAndStopRecord());
            TEST_StartInterceptor();
        }
        private void RandomizeActiveObjects()
        {
            var rand = GetComponent<Randomizer>();
            editable = rand.Generate(_blocks);
            foreach (var item in editable)
            {
                item.GetComponent<SpriteRenderer>().color = Color.blue;
            }
        }

        private void TEST_StartInterceptor()
        {
            IEnumerator _rootine()
            {
                yield return new WaitForSeconds(3);
                var history = GetHistory();
                history.StopPlaying();

                for (int i = 0; i < 10; i++)
                {
                    history.Seek(_blocks, .1f);
                    history.StopPlaying();
                    history.Seek(_blocks, .5f);
                    history.StopPlaying();
                }

                history.Seek(_blocks, 0);

                history.StopPlaying();
            }
            StartCoroutine(_rootine());
        }

        private void StartRecordWithDelay(float delay)
        {
            IEnumerator _rootine()
            {
                yield return new WaitForSeconds(delay);
                var history = GetHistory();
                history.StartRecord(_blocks);
            }
            StartCoroutine(_rootine());
        }

        private HistoryManager GetHistory()
        {
            var level = GameObject.FindGameObjectWithTag("Level");
            return level.GetComponent<HistoryManager>();
        }

        private IEnumerator WaitAndStopRecord()
        {
            yield return new WaitForSeconds(3);

            var history = GetHistory();
            history.StopRecord();
        }
    }

}