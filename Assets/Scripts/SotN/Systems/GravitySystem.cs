namespace SotN
{
    using UnityEngine;
    using Unity.Entities;

    public class GravitySystem : ComponentSystem
    {
        struct GravityFilter
        {
            public Velocity VelocityComponent;
            public readonly Gravity GravityComponent;
            public readonly Collision CollisionComponent;
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = Time.deltaTime;
            
            foreach( GravityFilter entity in GetEntities<GravityFilter>() )
            {
                Gravity gravity = entity.GravityComponent;
                Velocity velocity = entity.VelocityComponent;
                Collision collisionData = entity.CollisionComponent;

                if( !collisionData.State.below )
                    velocity.Value.y -= gravity.Value * deltaTime;
            }
        }
    }
}