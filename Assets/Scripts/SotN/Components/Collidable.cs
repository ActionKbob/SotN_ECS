namespace SotN
{
    using UnityEngine;

    public class Collidable : MonoBehaviour
    {
        public RaycastHit2D Hit;

        public LayerMask ValidCollisionLayers;

        public CollisionState State = new CollisionState();

        [ System.Serializable ]
        public class CollisionState
        {
            public bool right, left, above, below, becameGroundedThisFrame, wasGroundedLastFrame, moveDownSlope;
            public float slopeAngle;
            public bool hasCollision
            {
                get
                {
                    return right || left || above || below;
                }
            }
        }
    }
}