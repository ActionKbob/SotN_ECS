namespace SotN
{
    using UnityEngine;
    using Unity.Entities;

    public class GravitySystem : ComponentSystem
    {
        struct GravityFilter
        {
            public ComponentArray<Movement> MovementComponents;
            public readonly ComponentArray<Gravity> GravityComponents;
            public readonly int Length;
        }

        [ Inject ]
        private GravityFilter gravityFilter;

        protected override void OnUpdate()
        {
            float deltaTime = Time.deltaTime;

            for( int i = 0; i < gravityFilter.Length; i++ )
            {
                Movement movement = gravityFilter.MovementComponents[i];
                Gravity gravity = gravityFilter.GravityComponents[i];

                movement.Value.x -= gravity.Value * deltaTime;
            }
        }
    }
}