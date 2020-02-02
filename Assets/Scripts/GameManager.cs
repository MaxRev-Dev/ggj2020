using Assets.Scripts.Explosions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    { 
        public float explosionDuration = 5;
        private GameObject[] _blocks;
        private bool _initialized;
        public List<GameObject> editable;
        public TimelineController Timeline;
        private Vector3 _cameraBase;
        public GameObject activeOne;

        private bool _cameraMovement;

        // Start is called before the first frame update
        void Start()
        {
            _blocks = GameObject.FindObjectsOfType<BuildingBlock>().Select(x => x.gameObject).ToArray(); 
        }

        private void OnPauseClick()
        {
            // enable pause
            CameraReset();
        }

        // Update is called once per frame
        void Update()
        {
            CameraMoveToActiveObject();
            if (_initialized) return;
            _initialized = true;
            RandomizeActiveObjects();
            var cassette = GameObject.FindObjectOfType<BombCassette>();
            StartRecordWithDelay(cassette.commonDelay);
            StartCoroutine(WaitAndStopRecord());
           // TEST_StartInterceptor();
        }


        private void RandomizeActiveObjects()
        {
            var rand = GetComponent<Randomizer>();
            editable = rand.Generate(_blocks);
            _cameraMovement = true;
            foreach (var item in editable)
            {
                item.tag = "ActiveItems";
                item.GetComponent<SpriteRenderer>().color = Color.blue;
            }
            activeOne = editable.Last();
            activeOne.tag = "ActiveItems";
            activeOne.GetComponent<SpriteRenderer>().color = Color.green;
        }

        private float _activeZoffset = 20;

        public void CameraMoveToActiveObject()
        {
            var item = GetActiveItem();
            if (item == default) return;
            if (!_cameraMovement) return;
            var v3 = item.transform.position;
            _activeZoffset -= 0.1f;
            if (_activeZoffset < 0) _cameraMovement = false;
            var vector3 = new Vector3(v3.x, v3.y, _activeZoffset);
            Camera.main.transform.position =
                Vector3.Lerp(Camera.main.transform.position, vector3, Time.deltaTime * .1f);
        }

        private GameObject GetActiveItem()
        {
            return activeOne;
        }


        public void CameraReset()
        {
            Camera.main.transform.position = _cameraBase;
        }

        private void TEST_StartInterceptor()
        {
            IEnumerator _rootine()
            {
                yield return new WaitForSeconds(explosionDuration);
                var history = GetHistory();
                history.StopPlaying();

                for (int i = 0; i < 3; i++)
                {
                    yield return StartCoroutine(history.Seek(_blocks, .1f));
                    yield return new WaitForSeconds(1f);
                    yield return StartCoroutine(history.SeekInstant(_blocks, .5f));
                    yield return new WaitForSeconds(1f);
                }

                yield return StartCoroutine(history.SeekInstant(_blocks, 1f));
                yield return new WaitForSeconds(1f);
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
            DisablePhysics();
        }

        private void EnablePhysics()
        {
            Physics2D.autoSimulation = false;
        }

        private void DisablePhysics()
        {
            Physics2D.autoSimulation = false;
        }

        public void TurnActiveObjectRight()
        {
            var m = GetActiveItem();
            GameObject.FindObjectOfType<UserAngleState>().TurnRight(m);
        }

        public void TurnActiveObjectLeft()
        {
            var m = GetActiveItem();
            GameObject.FindObjectOfType<UserAngleState>().TurnLeft(m);
        }

        public void RewindOnce()
        {
            GameObject.FindObjectOfType<TimelineController>().RewindOnce();
        }
    }

}