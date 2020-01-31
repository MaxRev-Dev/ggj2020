using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Explosions
{
    public class BombCassette : MonoBehaviour
    {
        private bool exploded;
        public float commonDelay;
        public LayerMask explosionLayers;


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (exploded) return;
            exploded = true;
            StartCoroutine(StartExplodeSequence());
        }

        private IEnumerator StartExplodeSequence()
        {
            yield return new WaitForSeconds(commonDelay);
            var bombs = GameObject.FindGameObjectsWithTag("Bomb");
            foreach (var bomb in bombs)
            {
                var explosion = bomb.GetComponent<Explosion>();
                yield return new WaitForSeconds(explosion.explodeDelay);
                explosion.Explode(explosionLayers);
            }
        }
    }
}