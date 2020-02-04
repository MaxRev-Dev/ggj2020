using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class HistoryContainer : IDictionary<string, LinkedList<BlockTransform>>
    {
        private readonly Dictionary<string, LinkedList<BlockTransform>> Sequence
            = new Dictionary<string, LinkedList<BlockTransform>>();
        private readonly Dictionary<string, LinkedList<BlockTransform>> History
            = new Dictionary<string, LinkedList<BlockTransform>>();


        public void Add(string key, BlockTransform transformRotation)
        {
            if (!Sequence.ContainsKey(key))
            {
                Sequence[key] = new LinkedList<BlockTransform>();
            }
            Sequence[key].AddLast(transformRotation);
        }

        public bool Rewind(string key, out BlockTransform item)
        {
            if (Sequence.ContainsKey(key) && Sequence[key].Last != default)
            {
                item = Sequence[key].Last.Value;
                Sequence[key].RemoveLast();
                if (!History.ContainsKey(key))
                    History[key] = new LinkedList<BlockTransform>();
                History[key].AddFirst(item);
                return true;
            }
            item = default;
            return false;
        }

        public bool Forward(string key, out BlockTransform item)
        {
            if (History.ContainsKey(key) && History[key].First != default)
            {
                item = History[key].First.Value;
                History[key].RemoveFirst();
                if (!Sequence.ContainsKey(key))
                    Sequence[key] = new LinkedList<BlockTransform>();
                Sequence[key].AddLast(item);
                return true;
            }
            item = default;
            return false;
        }
         
        public IEnumerable<BlockTransform> GetForPercentage(string id, float percentage, bool allFrames)
        {
            var percentageLocal = GetPercentageForID(id) * .01f;
            var isRewind = percentageLocal >= percentage; 
            return isRewind ? RewindLoop(id, percentage, allFrames) : ForwardLoop(id, percentage, allFrames);
        }

        private IEnumerable<BlockTransform> ForwardLoop(string id, float perc, bool allFrames)
        {
            var breakPoint = (int)GetAllFrames(id) * perc;

            if (allFrames)
                foreach (var item in Sequence[id].Take((int) breakPoint))
                    yield return item;
            else
                yield return Sequence[id].ElementAt((int)breakPoint); 
        }

        private IEnumerable<BlockTransform> RewindLoop(string id, float perc, bool allFrames)
        {
            var breakPoint = (int)GetAllFrames(id) * perc;

            if (allFrames)
                foreach (var item in Sequence[id].Reverse().Take((int) breakPoint))
                    yield return item;
            else
                yield return Sequence[id].ElementAt((int)breakPoint); 
        } 

        private float GetPercentageForID(string id)
        {
            var sum = GetAllFrames(id);
            return Sequence[id].Count * 1f / sum * 100;
        }

        private float GetAllFrames(string id)
        {
            return (Sequence.ContainsKey(id) ? Sequence[id].Count : 0) +
                 (History.ContainsKey(id) ? History[id].Count : 0);
        }


        #region Dictionary Impl

        public IEnumerator<KeyValuePair<string, LinkedList<BlockTransform>>> GetEnumerator()
        {
            return Sequence.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Sequence).GetEnumerator();
        }

        public void Add(KeyValuePair<string, LinkedList<BlockTransform>> item)
        {
            Sequence.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            Sequence.Clear();
        }

        public bool Contains(KeyValuePair<string, LinkedList<BlockTransform>> item)
        {
            return Sequence.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, LinkedList<BlockTransform>>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, LinkedList<BlockTransform>> item)
        {
            return Sequence.Remove(item.Key);
        }

        public void Add(string key, LinkedList<BlockTransform> value)
        {
            Sequence.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return Sequence.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return Sequence.Remove(key);
        }

        public bool TryGetValue(string key, out LinkedList<BlockTransform> value)
        {
            return Sequence.TryGetValue(key, out value);
        }

        public LinkedList<BlockTransform> this[string key]
        {
            get => Sequence[key];
            set => Sequence[key] = value;
        }


        public int Count => Sequence.Count;

        public bool IsReadOnly => ((ICollection<KeyValuePair<string, LinkedList<BlockTransform>>>)Sequence).IsReadOnly;

        public ICollection<string> Keys => ((IDictionary<string, LinkedList<BlockTransform>>)Sequence).Keys;

        public ICollection<LinkedList<BlockTransform>> Values => ((IDictionary<string, LinkedList<BlockTransform>>)Sequence).Values;
         
        #endregion
    }
}