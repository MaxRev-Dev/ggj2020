using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class TimelineController : MonoBehaviour
    {

        // Start is called before the first frame update
        void Start()
        {
            hman = GameObject.FindObjectOfType<HistoryManager>();
            gman = GameObject.FindObjectOfType<GameManager>();
            InitTimeline();
            GenerateStartPoints();
        }

        private HistoryManager hman;
        private GameManager gman;
        public GameObject cueGameObject;
        private LinkedList<int> points;
        private int _current = -1;
        private int maxScale = 26;
        public int padBetween = 2;
        public int padBounds = 3;
        public int maxCue = 3;
        private LayoutElement[] pointPlace;
        public bool CanRewind { get; set; }
        public Dictionary<int, GameObject> Map = new Dictionary<int, GameObject>();
        private int activeIndex;
        private bool _initialized;
        private bool _pointsReady;

        private void GenerateStartPoints()
        {
            points = new LinkedList<int>(GetPointsByRule());
            var timeline = GameObject.FindGameObjectWithTag("Timeline");
            pointPlace = timeline.GetComponentsInChildren<LayoutElement>()
              .Where(x => x.name.StartsWith("item_"))
              .ToArray();

            foreach (var point in points.OrderByDescending(x => x))
            {
                var place = pointPlace.ElementAt(point);
                var gm = Instantiate(cueGameObject, place.gameObject.transform);
                var btn = gm.gameObject.GetComponentInChildren<Button>();
                btn.name = btn.name + '.' + point;
                btn.onClick.AddListener(delegate { Callback(btn); });
            }
        }

        private void Callback(Button btn)
        {
            this.activeIndex = int.Parse(btn.name.Substring(btn.name.IndexOf('.') + 1));
            var index = points.ToList().FindIndex(x => x == activeIndex);

            var target = points.ElementAt(index) * 1.0f / maxScale;
            ChangeActive();
            gman.ShowPivot();
            Debug.Log($"Callback : {target}");
            StartCoroutine(hman.SeekInstant(gman.Blocks, target));

        }

        float perc(float num) => num * 1.0f / maxScale;

        IEnumerable<int> GetPointsByRule()
        {
            int GetNext() => Random.Range(1 + padBounds, maxScale - padBounds);

            var list = new List<int>();

            while (list.Count != maxCue)
            {
                var num = GetNext();
                if (list.Contains(num))
                    continue;
                bool ok = true;
                foreach (var i in list)
                {
                    if (i + padBetween == num) ok = false;
                    if (i - padBetween == num) ok = false;
                    if (Mathf.Abs(perc(num) - perc(i)) < 0.1) ok = false;
                } 

                if (ok)
                {
                    list.Add(num);
                }
            }

            return list;
        }


        private void InitTimeline()
        {
            var timeline = GameObject.FindGameObjectWithTag("Timeline");
            var ch = timeline.GetComponentsInChildren<Text>();
            var fill = timeline.GetComponentsInChildren<RectTransform>().Where(x => x.name.Contains("fill")).ToArray();

            // strange part. edit only if you know what you're doing
            var ir = new double[100];
            double _k = 5;
            for (int i = -50; i < 50; i++)
            {
                float v = i * .57f;
                ir[i + 50] = ((Mathf.Cos(v) - 1.5) * .5 / 1.6 * v) * 3;
            }

            for (int i = 0, j = 10; i < ch.Length; i++, j++)
            {
                ch[i].text = ir[j].ToString("F1");
                if (i > fill.Length - 1) continue;
                fill[i].sizeDelta = new Vector2(25, (float)ir[i]);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (_initialized) return;
            _initialized = true;
        }

        private void InitPointsForce()
        {
            foreach (var point in points)
                while (true)
                {
                    var item = gman.GetRandomEditableObject();
                    if (Map.ContainsValue(item)) continue;
                    Map.Add(point, item);
                    break;
                }
        }


        public bool RewindOnce()
        {
            if (!CanRewind) return false; 

            IEnumerator _rootine()
            { 
                yield return StartCoroutine(hman.Seek(gman.Blocks, 1));
                ChangeActive(); 
                Physics2D.autoSimulation = true;
            }
            StartCoroutine(_rootine());
            return true;
        }

        private void ChangeActive()
        {
            if (!Map.Any()) InitPointsForce();
            gman.activeOne = Map[activeIndex];

            foreach (var item in gman.editable)
            {
                //item.GetComponent<SpriteRenderer>().color = Color.blue;
            }
            //gman.activeOne.GetComponent<SpriteRenderer>().color = Color.green;
             
        }
    }
}