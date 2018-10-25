namespace SotN
{
    using UnityEngine;
    using Unity.Entities;
    using Unity.Mathematics;

    public class CharacterJumpSystem : ComponentSystem
    {
        struct CharacterJumpFilter
        {
            public Velocity VelocityComponent;
            public Gravity GravityComponent;
            public readonly PlayerInput InputComponent;
            public readonly Jump JumpComponent;
            public readonly Collision CollisionComponent;
        }

        protected override void OnUpdate()
        {
            float deltaTime = Time.deltaTime;

            foreach( CharacterJumpFilter entity in GetEntities<CharacterJumpFilter>() )
            {
                Velocity velocity = entity.VelocityComponent;
                PlayerInput input = entity.InputComponent;
                Collision collisionData = entity.CollisionComponent;
                Jump jump = entity.JumpComponent;
                Gravity gravity = entity.GravityComponent;
                
                if( gravity.Value == 0 )
                    gravity.Value = CalculateGravityForJumpValues( jump.Height, jump.TimeToApex );

                if( input.Jump && collisionData.State.below )
                {
                    float jumpVelocity = gravity.Value * jump.TimeToApex;
                    velocity.Value.y = jumpVelocity;
                }
            }
        }

        public float CalculateGravityForJumpValues( float height, float timeToApex )
        {
            float gravity = ( 2 * height ) / ( timeToApex * timeToApex );
            return gravity;
        }
    }
}