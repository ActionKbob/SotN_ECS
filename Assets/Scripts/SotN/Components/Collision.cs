namespace SotN
{
    using UnityEngine;

    public class Collision : MonoBehaviour
    {
        [ System.Serializable ]
        public struct CollisionState
        {
            public bool left, right, above, below;
            public float slopeAngle;
        }

        public LayerMask Mask;

        public float SkinWidth = 0.015f;

        public CollisionState State = new CollisionState();

        public int HorizontalRayCount = 4;
        public int VerticalRayCount = 4;
    }
}