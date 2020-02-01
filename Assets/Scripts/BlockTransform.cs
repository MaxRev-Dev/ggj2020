using UnityEngine;

namespace Assets.Scripts
{
    public class BlockTransform
    {
        public Quaternion Rotation;
        public Vector3 Position; 

        public BlockTransform(Vector3 transformPosition, Quaternion transformRotation)
        {
            Position = transformPosition;
            Rotation = transformRotation;
        } 
    }
}