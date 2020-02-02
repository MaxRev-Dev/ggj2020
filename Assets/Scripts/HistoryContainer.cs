using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class HistoryContainer : IDictionary<string, LinkedList<BlockTransform>>
    {
        private readonly Dictionary<string, LinkedList<BlockTransform>> Pending
            = new Dictionary<string, LinkedList<BlockTransform>>();
        private readonly Dictionary<string, LinkedList<BlockTransform>> History
            = new Dictionary<string, LinkedList<BlockTransform>>();

        public IEnumerator<KeyValuePair<string, LinkedList<BlockTransform>>> GetEnumerator()
        {
            return Pending.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Pending).GetEnumerator();
        }

        public void Add(KeyValuePair<string, LinkedList<BlockTransform>> item)
        {
            Pending.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            Pending.Clear();
        }

        public bool Contains(KeyValuePair<string, LinkedList<BlockTransform>> item)
        {
            return Pending.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, LinkedList<BlockTransform>>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, LinkedList<BlockTransform>> item)
        {
            return Pending.Remove(item.Key);
        }

        public int Count => Pending.Count;

        public bool IsReadOnly => ((ICollection<KeyValuePair<string, LinkedList<BlockTransform>>>)Pending).IsReadOnly;

        public void Add(string key, LinkedList<BlockTransform> value)
        {
            Pending.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return Pending.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return Pending.Remove(key);
        }

        public bool TryGetValue(string key, out LinkedList<BlockTransform> value)
        {
            return Pending.TryGetValue(key, out value);
        }

        public LinkedList<BlockTransform> this[string key]
        {
            get => Pending[key];
            set => Pending[key] = value;
        }

        public ICollection<string> Keys => ((IDictionary<string, LinkedList<BlockTransform>>)Pending).Keys;

        public ICollection<LinkedList<BlockTransform>> Values => ((IDictionary<string, LinkedList<BlockTransform>>)Pending).Values;

        public void Add(string key, BlockTransform transformRotation)
        {
            if (!Pending.ContainsKey(key))
            {
                Pending[key] = new LinkedList<BlockTransform>();
            }
            Pending[key].AddLast(transformRotation);
        }

        public bool Rewind(string key, out BlockTransform item)
        {
            if (Pending.ContainsKey(key) && Pending[key].Last != default)
            {
                item = PopItemFrom(Pending, key);
                AddTo(History, key, item);
                return true;
            }
            item = default;
            return false;
        }

        public bool Forward(string key, out BlockTransform item)
        {
            if (History.ContainsKey(key) && History[key].Last != default)
            {
                item = PopItemFrom(History, key);
                AddTo(Pending, key, item);
                return true;
            }
            item = default;
            return false;
        }
        private BlockTransform PopItemFrom(Dictionary<string, LinkedList<BlockTransform>> list, string key)
        {
            var item = list[key].Last.Value;
            list[key].RemoveLast();
            return item;
        }

        void AddTo(Dictionary<string, LinkedList<BlockTransform>> list, string key, BlockTransform block)
        {
            if (!list.ContainsKey(key))
            {
                list[key] = new LinkedList<BlockTransform>();
            }

            list[key].AddLast(block);
        }

        public IEnumerable<BlockTransform> GetForPercentage(string id, float percentage)
        {
            var percentageLocal = GetPercentageForID(id) * .01f;
            var isRewind = percentageLocal > percentage;
            if (Mathf.Abs(percentage - percentageLocal) < .1)
                return Enumerable.Empty<BlockTransform>();
            return isRewind ? RewindLoop(id, percentage) : ForwardLoop(id, percentage);
        }

        private IEnumerable<BlockTransform> ForwardLoop(string id, float perc)
        {
            var breakPoint = (int)GetAllFrames(id) * perc;
            //if (History.ContainsKey(id) && Pending.ContainsKey(id))
            //    MoveFrom(Pending[id], History[id]);
            var forId = (History.ContainsKey(id) ? History[id].Count : 0);
            var diff = Math.Abs(forId - breakPoint);
            while (diff-- > 0)
            //    while (breakPoint-- > 0)
            {
                if (Forward(id, out var item))
                {
                    yield return item;
                }
                else
                {
                    yield break;
                }
            }
        }

        private IEnumerable<BlockTransform> RewindLoop(string id, float perc)
        {
            var breakPoint = (int)GetAllFrames(id) * perc;
            //if (History.ContainsKey(id) && Pending.ContainsKey(id))
            //    MoveFrom(History[id], Pending[id]);

            var forId = (Pending.ContainsKey(id) ? Pending[id].Count : 0);
            var diff = Math.Abs(forId - breakPoint);
            while (diff-- > 0)
            //  while (breakPoint-- > 0)
            {
                if (Rewind(id, out var item))
                {
                    yield return item;
                }
                else
                {
                    yield break;
                }
            }
        }

        private void MoveFrom(LinkedList<BlockTransform> source, LinkedList<BlockTransform> dest)
        {
            foreach (var item in source.Reverse())
            {
                dest.AddLast(item);
            }
            source.Clear();
        }

        private float GetPercentageForID(string id)
        {
            var sum = GetAllFrames(id);
            return Pending[id].Count * 1f / sum * 100;
        }

        private float GetAllFrames(string id)
        {
            return (Pending.ContainsKey(id) ? Pending[id].Count : 0) +
                 (History.ContainsKey(id) ? History[id].Count : 0);
        }
    }
}