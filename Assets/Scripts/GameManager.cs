using Assets.Scripts.Explosions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public float explosionDuration = 4;
        public GameObject[] Blocks;
        private bool _initialized;
        public List<GameObject> editable;
        public TimelineController Timeline;
        private Vector3 _cameraBase;
        public GameObject activeOne;

        private bool _cameraMovement;

        // Start is called before the first frame update
        void Start()
        {
            Blocks = GameObject.FindObjectsOfType<BuildingBlock>().Select(x => x.gameObject).ToArray();
            _cameraBase = Camera.main.transform.position;
            RandomizeActiveObjects();
            Timeline = GameObject.FindObjectOfType<TimelineController>();
            ShowBriefing();
        }

        // Update is called once per frame
        void Update()
        {
            if (_startRecord)
            {
                Debug.Log("Recording started");
                _startRecord = false;
                StartRecordWithDelay(0);
            }

            if (_cameraMovement)
            {
                CameraMoveToActiveObject();
            }
        }
        public GameObject GetRandomEditableObject()
        {
            var rand = FindObjectOfType<Randomizer>();
            return rand.GetCurrentRand();
        }

        public void StartExplosion()
        {
            var cassette = FindObjectOfType<BombCassette>();
            cassette.bombPlanted = true;
        }

        public void OnPauseClick()
        {
            // enable pause 
            ShowBriefing();
        }

        #region Briefing

        private void Briefing(bool isOn)
        {
            var briefing = GameObject.FindGameObjectWithTag("Briefing");
            briefing.GetComponentInChildren<Animator>().SetBool("is_ON", isOn);
        }


        public void ShowBriefing()
        {
            Briefing(true);
        }

        public void CloseBriefing()
        {
            Briefing(false);

            if (!_exploded)
            {
                _startRecord = true;
                _exploded = true;
                StartExplosion();
            }

        }

        #endregion

        #region Pivot


        private void Pivot(bool isOn)
        {
            var pivot = GameObject.FindGameObjectWithTag("Pivot");
            pivot.GetComponent<Animator>().SetBool("is_ON", isOn);
        }
        public void ShowPivot()
        {
            Pivot(true); 
        }

        public void ClosePivot()
        {
            Pivot(false);
        }

        #endregion


        private void RandomizeActiveObjects()
        {
            var rand = GameObject.FindObjectOfType<Randomizer>();
            editable = rand.Generate(Blocks);
            foreach (var item in editable)
            {
                item.tag = "ActiveItems";
                item.GetComponent<SpriteRenderer>().color = Color.blue;
            }

            activeOne = editable.Last(); 
        }

        private float _activeZoffset = 25;
        private bool _startRecord;
        private bool _exploded;
        private bool _enableZoom = true;

        public void CameraMoveToActiveObject()
        {
            var item = GetActiveItem();
            if (item == default) return;
            if (!_cameraMovement) return;
            var v3 = item.transform.position;
            if (_enableZoom && _activeZoffset > 20)
                _activeZoffset -= 0.1f;
            else
            {
                _enableZoom = false;
                _activeZoffset = 20;
            }
            var vector3 = new Vector3(v3.x, v3.y, _activeZoffset); Camera.main.transform.LookAt(v3);
            //Camera.main.transform.position =
            //    Vector3.Lerp(Camera.main.transform.position, vector3, Time.deltaTime * .1f);
        }

        private GameObject GetActiveItem()
        {
            return activeOne;
        }


        public void CameraReset()
        {
            Camera.main.transform.position = _cameraBase;
        }

        private void StartRecordWithDelay(float delay)
        {
            IEnumerator _rootine()
            {
                yield return new WaitForSeconds(delay);
                var history = GetHistory();
                history.StartRecord(Blocks);
                StartCoroutine(WaitAndStopRecord());
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
            yield return new WaitForSeconds(explosionDuration);

            var history = GetHistory();
            history.StopRecord();
            Timeline.CanRewind = true;
            DisablePhysics();  
            Debug.Log($"Recording stopped: {history.Movements.Count} items, frames: {history.Movements.First().Value.Count}");
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
            if (Timeline.RewindOnce())
            { 
                ShowPivot();
                RepositionCamera();

            }
        }


        public void RepositionCamera()
        {
            _cameraMovement = true;
        }


        #region TEST

        private void TEST_StartInterceptor()
        {
            IEnumerator _rootine()
            {
                yield return new WaitForSeconds(explosionDuration);
                var history = GetHistory();
                history.StopPlaying();

                for (int i = 0; i < 3; i++)
                {
                    yield return StartCoroutine(history.Seek(Blocks, .1f));
                    yield return new WaitForSeconds(1f);
                    yield return StartCoroutine(history.SeekInstant(Blocks, .5f));
                    yield return new WaitForSeconds(1f);
                }

                yield return StartCoroutine(history.SeekInstant(Blocks, 1f));
                yield return new WaitForSeconds(1f);
                history.StopPlaying();
            }
            StartCoroutine(_rootine());
        }



        #endregion

    }

}