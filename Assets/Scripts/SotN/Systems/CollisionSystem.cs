namespace SotN
{
    using UnityEngine;
    using Unity.Entities;
    using Unity.Mathematics;

    [ UpdateAfter( typeof( GravitySystem ) ) ]
    [ UpdateBefore( typeof( MovementSystem ) ) ]    
    public class CollisionSystem : ComponentSystem
    {
        struct CollisionFilter
        {
            public Movement MovementComponent;
            public readonly Collision CollisionComponent;
            public readonly BoxCollider2D ColliderComponent;
        }

        public struct RaycastOrigins
        {
            public Vector2 TopLeft, TopRight, BottomLeft, BottomRight;
        }

        const float MAX_CLIMB_ANGLE = 65;

        protected override void OnUpdate()
        {
            float deltaTime = Time.deltaTime;

            foreach( CollisionFilter entity in GetEntities<CollisionFilter>() )
            {
                Movement movement = entity.MovementComponent;
                Collision collisionData = entity.CollisionComponent;
                BoxCollider2D collider = entity.ColliderComponent;

                ResetCollisionState( collisionData );

                float2 movementDelta = movement.Value;

                RaycastOrigins rayOrigins = CalculateRaycastOrigins( collisionData, collider );
                float2 raySpacing = CalculateRaySpacing( collisionData, collider );

                if( movementDelta.x != 0 )
                    HandleHorizontalCollisions( ref movementDelta, collisionData, rayOrigins, raySpacing.x );

                if( movementDelta.y != 0 )
                    HandleVerticalCollisions( ref movementDelta, collisionData, rayOrigins, raySpacing.y );
                
                movement.Value = movementDelta;
            }
        }

        private void HandleHorizontalCollisions( ref float2 movementDelta, Collision collisionData, RaycastOrigins rayOrigins, float raySpacing )
        {
            float direction = math.sign( movementDelta.x );
            float rayDistance = math.abs( movementDelta.x ) + collisionData.SkinWidth;
            Vector2 rayOrigin = ( direction == -1 ) ?  rayOrigins.BottomLeft : rayOrigins.BottomRight;

            for( int i = 0; i < collisionData.HorizontalRayCount; i++ )
            {
                Vector2 ray = rayOrigin + ( Vector2.up * raySpacing * i );

                Debug.DrawRay(ray, Vector2.right * direction * rayDistance,Color.red);

                RaycastHit2D hit = Physics2D.Raycast( ray, Vector2.right * direction, rayDistance, collisionData.Mask );

                if( hit )
                {
                    // Slope Handling
                    float slopeAngle = Vector2.Angle( hit.normal, Vector2.up );
                    if( i == 0 && slopeAngle <= MAX_CLIMB_ANGLE )
                    {
                        float distanceToSlopeStart = 0;
                        if( slopeAngle != collisionData.State.slopeAngle )
                        {
                            distanceToSlopeStart = hit.distance - collisionData.SkinWidth;
                            movementDelta.x -= distanceToSlopeStart;
                        }
                        HandleVerticalSlope( ref movementDelta, collisionData, slopeAngle );
                        movementDelta.x += distanceToSlopeStart * direction;
                    }
                    else
                    {
                        movementDelta.x = ( hit.distance - collisionData.SkinWidth ) * direction;
                        rayDistance = hit.distance;

                        collisionData.State.right = ( direction == 1 );
                        collisionData.State.left = ( direction == -1 );
                    }
                }
            }
        }

        private void HandleVerticalCollisions( ref float2 movementDelta, Collision collisionData, RaycastOrigins rayOrigins, float raySpacing )
        {
            float direction = math.sign( movementDelta.y );
            float rayDistance = math.abs( movementDelta.y ) + collisionData.SkinWidth;
            Vector2 rayOrigin = ( direction == -1 ) ?  rayOrigins.BottomLeft : rayOrigins.TopLeft;
        

            for( int i = 0; i < collisionData.VerticalRayCount; i++ )
            {
                Vector2 ray = rayOrigin + ( Vector2.right * ( raySpacing * i + movementDelta.x ) );

                Debug.DrawRay(ray, Vector2.up * direction * rayDistance,Color.red, 0.1f);

                RaycastHit2D hit = Physics2D.Raycast( ray, Vector2.up * direction, rayDistance, collisionData.Mask );

                if( hit )
                {
                    movementDelta.y = ( hit.distance - collisionData.SkinWidth ) * direction;
                    rayDistance = hit.distance;

                    collisionData.State.above = ( direction == 1 );
                    collisionData.State.below = ( direction == -1 );
                }
            }
        }

        private void HandleVerticalSlope( ref float2 movementDelta, Collision collisionData, float slopeAngle )
        {
            float moveDistance = math.abs( movementDelta.x );
            float climbVelocity = math.sin( math.radians( slopeAngle ) ) * moveDistance;

            if( movementDelta.y <= climbVelocity )
            {
                movementDelta.y = climbVelocity;
                movementDelta.x = math.cos( math.radians( slopeAngle ) ) * moveDistance * math.sign( movementDelta.x );
                collisionData.State.below = true;
                collisionData.State.slopeAngle = slopeAngle;
            }
        }

        private RaycastOrigins CalculateRaycastOrigins( Collision collisionData, BoxCollider2D collider )
        {
            Bounds bounds = collider.bounds;
            bounds.Expand( collisionData.SkinWidth * -2f );

            RaycastOrigins origins = new RaycastOrigins();

            origins.TopLeft = new Vector2( bounds.min.x, bounds.max.y );
            origins.TopRight = bounds.max;
            origins.BottomLeft = bounds.min;
            origins.BottomRight = new Vector2( bounds.max.x, bounds.min.y );

            return origins;
        }

        private float2 CalculateRaySpacing( Collision collisionData, BoxCollider2D collider )
        {
            Bounds bounds = collider.bounds;
            bounds.Expand( collisionData.SkinWidth * -2f );

            float hSpacing = bounds.size.y / ( collisionData.HorizontalRayCount - 1 );
            float vSpacing = bounds.size.x / ( collisionData.VerticalRayCount - 1 );

            return new float2( hSpacing, vSpacing );
        }


        private void ResetCollisionState( Collision collisionData )
        {
            
            collisionData.State.left = collisionData.State.right = collisionData.State.above = collisionData.State.below = false;
            
        }
        
    }
}