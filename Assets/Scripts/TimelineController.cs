using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class TimelineController : MonoBehaviour
    {

        // Start is called before the first frame update
        void Start()
        {
            InitTimeline();

            GenerateStartPoints();
            hman = GameObject.FindObjectOfType<HistoryManager>();
            gman = GameObject.FindObjectOfType<GameManager>();
        }

        private HistoryManager hman;
        private GameManager gman;
        public GameObject cueGameObject;
        private LinkedList<int> points;

        private void GenerateStartPoints()
        {
            points = new LinkedList<int>(GetPointsByRule());
            var timeline = GameObject.FindGameObjectWithTag("Timeline");
            var pointPlace = timeline.GetComponentsInChildren<LayoutElement>().Where(x => x.name.StartsWith("item_"))
                .ToArray();

            foreach (var point in points)
            {
                var place = pointPlace.ElementAt(point);
                Instantiate(cueGameObject, place.gameObject.transform);
            }
        }

        IEnumerable<int> GetPointsByRule()
        {
            int padBetween = 1;
            int padBounds = 3;
            int maxCue = 3;

            int GetNext() => Random.Range(1 + padBounds, 26 - padBounds);

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

        }


        public void RewindOnce()
        {
            var next = points.Last();
            StartCoroutine(hman.Seek(gman.Blocks, (next * 1.0f/ 26) * .1f));
        }
    }
}