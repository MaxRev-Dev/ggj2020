using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class HistoryManager : MonoBehaviour
    {
        public bool isRecording;
        public bool isPlaying;
        float maxAmount = 15;
        private float slowMoFactor => 1.0f / maxAmount;
        public readonly HistoryContainer Movements = new HistoryContainer();
        private bool _inSlowmo;
        private Coroutine[] _rootines;
        private Coroutine _recordRootine;

        private BlockTransform target;

        // Update is called once per frame
        void Update()
        {

            //if (enableSlowmo)
            //{

            //    if (!_inSlowmo)
            //    {
            //        _inSlowmo = true;
            //        if (Math.Abs(Time.timeScale - 1.0f) < 0.001)
            //            Time.timeScale = 0.5f;
            //        else
            //            Time.timeScale = 1.0f;
            //        Time.fixedDeltaTime = 0.02f * Time.timeScale;
            //    }
            //}
            //if (Math.Abs(Time.timeScale - 0.5f) < 0.001)
            //{
            //    currentAmount += Time.deltaTime;
            //}

            //if (currentAmount > maxAmount)
            //{
            //    currentAmount = 0f;
            //    Time.timeScale = 1.0f;
            //}
        }


        public void Reset()
        {
            Movements.Clear();
        }

        #region Recording
        public void StartRecord(GameObject[] items)
        {
            isRecording = true;
            _recordRootine = StartCoroutine(Record(items));
        }

        public void StopRecord()
        {
            if (_recordRootine != default)
                StopCoroutine(_recordRootine);
            isRecording = false;
        }

        private IEnumerator Record(GameObject[] items)
        {
            while (true)
            {
                if (!isRecording)
                    break;
                yield return 0;
                foreach (var item in items)
                    Movements.Add(GetItemId(item), new BlockTransform(item.transform.position, item.transform.rotation));
            }
        }
        #endregion

        private string GetItemId(GameObject item)
        {
            return item.tag + '.' + item.name;
        }

        #region Controls

        public IEnumerator RewindFor(GameObject[] blocks)
        {
            Physics2D.autoSimulation = false;
            isPlaying = true;
            var active = 0;
            IEnumerator _rootine(GameObject item)
            {
                var id = GetItemId(item);

                while (Movements.Rewind(id, out var itemTransform))
                {
                    for (int i = 0; i < (_inSlowmo ? maxAmount : 1); i++)
                    {
                        if (!isPlaying)
                            break;
                        item.GetComponent<BuildingBlock>().SetTransform(itemTransform);
                        if (_inSlowmo)
                            yield return new WaitForSeconds(Time.deltaTime * slowMoFactor);
                        yield return new WaitForEndOfFrame();
                    }
                    if (!isPlaying)
                        break;
                }

                active++;
                SetZeroVelocity(item);
            }
            StartRootines(blocks.Select(_rootine));
            yield return new WaitWhile(() => active != blocks.Length);
            isPlaying = false;
        }


        public void StopPlaying()
        {
            StopRootines();
            isPlaying = false;
            Physics2D.autoSimulation = true;
            _inSlowmo = false;
        }

        public IEnumerator StartPlaying(GameObject[] blocks)
        {
            if (!isPlaying)
                return RewindFor(blocks);
            return null;
        }


        public IEnumerator SeekInstant(GameObject[] blocks, float percentage)
        {
            if (percentage < 0) throw new ArgumentOutOfRangeException(nameof(percentage));
            if (percentage > 1) throw new ArgumentOutOfRangeException(nameof(percentage));
            if (isPlaying)
                yield return new WaitWhile(() => isPlaying);
            Physics2D.autoSimulation = false;
            isPlaying = true;
            var active = 0;
            IEnumerator _rootine(GameObject block)
            {
                active++;
                var id = GetItemId(block);
                BlockTransform current = null;
                foreach (var itemsTransform in Movements.GetForPercentage(id, percentage))
                {
                    current = itemsTransform;
                    yield return 0;
                }

                if (current != null)
                    block.GetComponent<BuildingBlock>().SetTransform(current);
                yield return 0;

                SetZeroVelocity(block);
            }
            StartRootines(blocks.Select(_rootine));
            /*var bb = blocks.Select(x => x.GetComponent<BuildingBlock>());
            foreach (var block in bb)
                StartCoroutine(block.FromInstantMovement(Movements, percentage));*/

            yield return new WaitWhile(() => active != blocks.Length);
            isPlaying = false;
        }

        public void Seek(GameObject[] blocks, float percentage)
        {
            if (percentage < 0) throw new ArgumentOutOfRangeException(nameof(percentage));
            if (percentage > 1) throw new ArgumentOutOfRangeException(nameof(percentage)); 
            Physics2D.autoSimulation = false;
            isPlaying = true;
            var active = 0;
            IEnumerator _rootine(GameObject block)
            {
                yield return new WaitForSeconds(.5f);
                var id = GetItemId(block);
                active++;
                foreach (var itemsTransform in Movements.GetForPercentage(id, percentage))
                {
                    block.GetComponent<BuildingBlock>().SetTransform(itemsTransform);
                    yield return new WaitForEndOfFrame();
                }

                SetZeroVelocity(block);
            }
            StartRootines(blocks.Select(_rootine));

            /*var bb = blocks.Select(x => x.GetComponent<BuildingBlock>());
            foreach (var block in bb)
                StartCoroutine(block.FromMovement(Movements, percentage));*/
                 
            isPlaying = false;
            Physics2D.autoSimulation = true;
        }

        private void SetZeroVelocity(GameObject block)
        {
            block.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }


        public IEnumerator StartSlowmo(GameObject[] blocks)
        {
            _inSlowmo = true;
            yield return RewindFor(blocks);
        }

        #endregion

        private void StartRootines(IEnumerable<IEnumerator> rootine)
        {
            StopRootines();
            _rootines = rootine.Select(StartCoroutine).ToArray();
        }

        private void StopRootines()
        {
            if (_rootines == default || !_rootines.Any()) return;
            var tmp = _rootines.Where(x => !(x is null)).ToArray();
            foreach (Coroutine rootine in tmp)
            {
                StopCoroutine(rootine);
            }

            _rootines = tmp;
        }


    }
}
