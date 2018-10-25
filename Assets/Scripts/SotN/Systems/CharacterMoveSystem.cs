namespace SotN
{
    using UnityEngine;
    using Unity.Entities;
    using Unity.Mathematics;

    public class CharacterMoveSystem : ComponentSystem
    {
        struct CharacterMovementFilter
        {
            public Velocity VelocityComponent;
            public readonly PlayerInput InputComponent;
            public readonly MoveSpeed SpeedComponent;
            public readonly Collision CollisionComponent;
        }

        protected override void OnUpdate()
        {
            foreach( CharacterMovementFilter entity in GetEntities<CharacterMovementFilter>() )
            {
                Velocity velocity = entity.VelocityComponent;
                PlayerInput input = entity.InputComponent;
                MoveSpeed speed = entity.SpeedComponent;
                bool isGrounded = entity.CollisionComponent.State.below;

                float targetVelocity = input.Movement.x * speed.Value;
                float acceleration = ( isGrounded ) ? speed.GroundAcceleration : speed.AirAcceleration;

                velocity.Value.x = Mathf.SmoothDamp( velocity.Value.x, targetVelocity, ref speed.Smoothing, acceleration );
            }
        }
    }
}