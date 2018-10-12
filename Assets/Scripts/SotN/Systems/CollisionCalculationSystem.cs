namespace SotN
{
    using UnityEngine;
    using Unity.Entities;
    using Unity.Mathematics;

    [ UpdateBefore( typeof( MovementSystem ) ) ]
    [ UpdateAfter( typeof( GravitySystem ) ) ]
    public class CollisionCalculationSystem : ComponentSystem
    {
        struct CollidableEntity
        {
            public Movement MovementComponent;
            public readonly Collidable CollidableComponent;
        }

        struct RaycastOrigins
        {
            public Vector2 TopLeft, BottomRight, BottomLeft;
        }

        protected override void OnUpdate()
        {
            foreach( CollidableEntity entity in GetEntities<CollidableEntity>() )
            {
                Movement movement = entity.MovementComponent;
                Collidable collisionData = entity.CollidableComponent;

                collisionData.State.wasGroundedLastFrame = collisionData.State.below;

                ResetCollisionState( collisionData.State );

                RaycastOrigins rayOrigins = GetRaycastOrigins( collisionData.collider );
                float2 distanceBetweenRays = GetDistanceBetweenRays( collisionData.collider );

                Vector2 delta = new Vector2( movement.Value.x, movement.Value.y );

                if( delta.x != 0 )
                    HandleHorizontalMovement( ref delta, collisionData, rayOrigins, distanceBetweenRays.y );

                if( delta.y != 0 )
                    HandleVerticalMovement( ref delta, collisionData, rayOrigins, distanceBetweenRays.x );

                if( !collisionData.State.wasGroundedLastFrame && collisionData.State.below )
                    collisionData.State.becameGroundedThisFrame = true;

                movement.Value = new float2( delta.x, delta.y );
            }
        }

        private void HandleHorizontalMovement( ref Vector2 _delta, Collidable _collisionData, RaycastOrigins _rayOrigins, float _distanceBetweenRays )
        {
            float skinWidth = _collisionData.collider.edgeRadius;

            bool isGoingRight = _delta.x > 0;
            float rayDistance = math.abs( _delta.x ) + skinWidth;
            Vector2 rayDirection = ( isGoingRight ) ? Vector2.right : Vector2.left;
            Vector2 initialRayOrigin = ( isGoingRight ) ? _rayOrigins.BottomRight : _rayOrigins.BottomLeft;

            for( int i = 0; i < 3; i++ )
            {
                Vector2 ray = new Vector2( initialRayOrigin.x, initialRayOrigin.y + i * _distanceBetweenRays );

                Debug.DrawLine( ray, ray + ( rayDirection * rayDistance ), Color.red, 0f );

                _collisionData.Hit = Physics2D.Raycast( ray, rayDirection, rayDistance, _collisionData.ValidCollisionLayers );

                if( _collisionData.Hit )
                {

                    Debug.DrawRay( _collisionData.Hit.point, Vector2.up, Color.magenta, 0f );

                    _delta.x = _collisionData.Hit.point.x - ray.x;
                    rayDistance = math.abs( _delta.x );

                    if( isGoingRight )
                    {
                        _delta.x -= skinWidth;
                        _collisionData.State.right = true;
                    }
                    else
                    {
                        _delta.x += skinWidth;
                        _collisionData.State.left = true;
                    }

                    if( rayDistance < skinWidth + 0.001f )
                        break;
                }
            }
        }

        private void HandleVerticalMovement( ref Vector2 _delta, Collidable _collisionData, RaycastOrigins _rayOrigins, float _distanceBetweenRays )
        {
            float skinWidth = _collisionData.collider.edgeRadius;

            bool isGoingUp = _delta.x > 0;
            float rayDistance = math.abs( _delta.y ) + skinWidth;
            Vector2 rayDirection = ( isGoingUp ) ? Vector2.up : Vector2.down;
            Vector2 initialRayOrigin = ( isGoingUp ) ? _rayOrigins.TopLeft : _rayOrigins.BottomLeft;

            initialRayOrigin.x += _delta.x;

            for( int i = 0; i < 3; i++ )
            {
                Vector2 ray = new Vector2( initialRayOrigin.x, initialRayOrigin.y + i * _distanceBetweenRays );

                Debug.DrawLine( ray, ray + ( rayDirection * rayDistance ), Color.red, 0f );

                _collisionData.Hit = Physics2D.Raycast( ray, rayDirection, rayDistance, _collisionData.ValidCollisionLayers );

                if( _collisionData.Hit )
                {

                    Debug.DrawRay( _collisionData.Hit.point, Vector2.up, Color.magenta, 0f );

                    _delta.y = _collisionData.Hit.point.y - ray.y;
                    rayDistance = math.abs( _delta.y );

                    if( isGoingUp )
                    {
                        _delta.y -= skinWidth;
                        _collisionData.State.above = true;
                    }
                    else
                    {
                        _delta.y += skinWidth;
                        _collisionData.State.below = true;
                    }

                    if( rayDistance < skinWidth + 0.001f )
                        break;
                }
            }
        }

        private RaycastOrigins GetRaycastOrigins( BoxCollider2D _collider )
        {
            Bounds modifiedBounds = _collider.bounds;
            modifiedBounds.Expand( -2f * _collider.edgeRadius );

            RaycastOrigins origins = new RaycastOrigins();

            origins.TopLeft = new Vector2( modifiedBounds.min.x, modifiedBounds.max.y );
            origins.BottomRight = new Vector2( modifiedBounds.max.x, modifiedBounds.min.y );
            origins.BottomLeft = modifiedBounds.min;

            return origins;
        }

        private float2 GetDistanceBetweenRays( BoxCollider2D _collider )
        {
            float colliderWidth = _collider.size.x * math.abs( _collider.transform.localScale.x ) - ( 2f * _collider.edgeRadius );
            float colliderHeight = _collider.size.y * math.abs( _collider.transform.localScale.y ) - ( 2f * _collider.edgeRadius );
            float2 distanceBetweenRays = new float2( colliderWidth / 2, colliderHeight / 2 );

            return distanceBetweenRays;
        }

        private void ResetCollisionState( Collidable.CollisionState _state )
        {
            _state.right = _state.left = _state.above = _state.below = _state.becameGroundedThisFrame = _state.moveDownSlope = false;
            _state.slopeAngle = 0f;
        }
    }
}