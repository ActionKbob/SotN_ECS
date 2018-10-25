namespace SotN
{
    using UnityEngine;
    using Unity.Entities;
    using Unity.Mathematics;

    public class VelocitySystem : ComponentSystem
    {
        struct VelocityFilter
        {
            public Movement MovementComponent;
            public readonly Velocity VelocityComponent;
        }

        protected override void OnUpdate()
        {
            float deltaTime = Time.deltaTime;

            foreach( VelocityFilter entity in GetEntities<VelocityFilter>() )
            {
                Velocity velocity = entity.VelocityComponent;
                Movement movement = entity.MovementComponent;

                movement.Value = velocity.Value * deltaTime;
            }
        }
    }
}