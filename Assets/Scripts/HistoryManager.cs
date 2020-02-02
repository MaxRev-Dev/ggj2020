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
        public readonly HistoryContainer Movements = new HistoryContainer();
        private Coroutine[] _routines;
        private Coroutine _recordroutine;

        private BlockTransform target;

        // Update is called once per frame
        void Update()
        {

        }

        public void Reset()
        {
            Movements.Clear();
        }

        #region Recording
        public void StartRecord(GameObject[] items)
        {
            isRecording = true;
            _recordroutine = StartCoroutine(Record(items));
        }

        public void StopRecord()
        {
            if (_recordroutine != default)
                StopCoroutine(_recordroutine);
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

        public static string GetItemId(GameObject item)
        {
            return item.tag + '.' + item.name;
        }

        #region Controls

        public IEnumerator RewindFor(GameObject[] blocks)
        {
            BooleansEnter();
            var active = 0;

            IEnumerator _mainroutine(GameObject item)
            {
                var id = GetItemId(item);

                while (Movements.Rewind(id, out var itemTransform))
                {
                    SetTransform(item, itemTransform);
                    yield return 0;
                }

                active++;
                SetZeroVelocity(item);
            }


            IEnumerator _exit()
            {
                Startroutines(blocks.Select(_mainroutine));
                yield return new WaitWhile(() => active != blocks.Length);
                BooleansExit();
            }

            yield return StartCoroutine(_exit());
        }

        private void BooleansExit()
        {
            isPlaying = false;
        }

        private void BooleansEnter()
        {
            Physics2D.autoSimulation = false;
            isPlaying = true;
        }

        internal void SetTransform(GameObject item, BlockTransform itemTransform)
        {
            if (itemTransform == default) return;
            item.transform.position = itemTransform.Position;
            if (item.tag == "ActiveItem")
            {
                var m = GameObject.FindObjectOfType<UserAngleState>();
                item.transform.rotation =
                    new Quaternion(itemTransform.Rotation.x, itemTransform.Rotation.y,
                        itemTransform.Rotation.z + m.GetRotation(item), itemTransform.Rotation.w);
            }
            else
            {
                item.transform.rotation = itemTransform.Rotation;
            }
        }

        public void StopPlaying()
        {
            Stoproutines();
            BooleansExit();
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
            BooleansEnter();

            var active = 0;
            IEnumerator _mainroutine(GameObject block)
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
                    SetTransform(block, current);
                yield return new WaitForEndOfFrame();

                SetZeroVelocity(block);
            }

            IEnumerator _exit()
            {
                Startroutines(blocks.Select(_mainroutine));
                yield return new WaitWhile(() => active != blocks.Length);
                BooleansExit();
            }

            yield return StartCoroutine(_exit());
        }

        public IEnumerator Seek(GameObject[] blocks, float percentage)
        {
            if (percentage < 0) throw new ArgumentOutOfRangeException(nameof(percentage));
            if (percentage > 1) throw new ArgumentOutOfRangeException(nameof(percentage));
            BooleansEnter();
            var active = 0;

            IEnumerator _mainroutine(GameObject block)
            {
                yield return InvokeUserActiveObjects();
                var id = GetItemId(block);
                foreach (var itemsTransform in Movements.GetForPercentage(id, percentage))
                {
                    // slowmo effect will not affect this blocks
                    SetTransform(block, itemsTransform);
                    yield return new WaitForEndOfFrame();
                }

                active++;
                SetZeroVelocity(block);
                yield return 0;
            }


            IEnumerator _exit()
            {
                Startroutines(blocks.Select(_mainroutine));
                yield return new WaitWhile(() => active != blocks.Length);
                BooleansExit();
            }

            yield return StartCoroutine(_exit());
        }

        private IEnumerator InvokeUserActiveObjects()
        {
            var editable = GameObject.FindObjectOfType<GameManager>().editable;
            foreach (var item in editable)
            {
            }

            yield return 0;
        }

        private void SetZeroVelocity(GameObject block)
        {
            block.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }

        /// <summary>
        /// There some issues with coroutines
        /// </summary>
        /*public IEnumerator StartSlowmo(GameObject[] blocks)
        {
            _inSlowmo = true;
            yield return RewindFor(blocks);
        }*/

        #endregion

        private void Startroutines(IEnumerable<IEnumerator> routine)
        {
            Stoproutines();
            _routines = routine.Select(StartCoroutine).ToArray();
        }

        private void Stoproutines()
        {
            if (_routines == default || !_routines.Any()) return;
            var tmp = _routines.Where(x => !(x is null)).ToArray();
            foreach (Coroutine routine in tmp)
            {
                StopCoroutine(routine);
            }

            _routines = tmp;
        }


    }
}
