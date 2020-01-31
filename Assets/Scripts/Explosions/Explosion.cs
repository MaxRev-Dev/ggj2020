using Assets.Scripts.Extensions;
using UnityEngine;

namespace Assets.Scripts.Explosions
{
    public class Explosion : MonoBehaviour
    {
        void Start()
        {

        }

        void Update()
        {
        }

        public void Explode(LayerMask layer)
        {
            var point = this.transform.position;

            colliders = Physics2D.OverlapCircleAll(point, explosionRadius, layer);

            foreach (var coll in colliders)
            {
                coll.GetComponent<Rigidbody2D>().isKinematic = false;
                coll.GetComponent<Rigidbody2D>().AddExplosionForce(explosionPower, point, explosionRadius);
            } 
        }

        public float explosionRadius = 10;
        public float explosionPower = 100;
        public Collider2D[] colliders;
        public float explodeDelay = 0;
    }

}