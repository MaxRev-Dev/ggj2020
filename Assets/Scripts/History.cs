using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class History
    {
        public History(List<Quaternion> movements)
        {
            Movements = movements;
        }

        public List<Quaternion> Movements { get; }
    }
}