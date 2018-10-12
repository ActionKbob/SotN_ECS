namespace SotN
{
    using UnityEngine;

    [ RequireComponent( typeof( BoxCollider2D ) ) ]
    public class Collidable : MonoBehaviour
    {
        public RaycastHit2D Hit;

        public LayerMask ValidCollisionLayers;

        public CollisionState State = new CollisionState();

        public BoxCollider2D collider
        {
            get
            {
                return GetComponent<BoxCollider2D>();
            }
        }

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