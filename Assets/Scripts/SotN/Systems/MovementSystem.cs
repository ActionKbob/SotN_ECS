namespace SotN
{
    using UnityEngine;
    using Unity.Entities;
    using Unity.Mathematics;

    [ UpdateBefore( typeof( UnityEngine.Experimental.PlayerLoop.FixedUpdate ) ) ]
    public class MovementSystem : ComponentSystem
    {
        struct MovementEntity
        {
            public Transform TransformComponent;
            public Movement MovementComponent;
        }

        protected override void OnUpdate()
        {
            float deltaTime = Time.deltaTime;

            foreach( MovementEntity entity in GetEntities<MovementEntity>() )
            {
                Transform transform = entity.TransformComponent;
                float2 movement = entity.MovementComponent.Value;
                Vector3 movementVector = new Vector3( movement.x, movement.y, 0 );

                transform.Translate( movementVector );
            }
        }
    }
}