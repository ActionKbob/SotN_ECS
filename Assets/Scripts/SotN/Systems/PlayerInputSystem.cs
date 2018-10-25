namespace SotN
{
    using Unity.Entities;
    using Rewired;

    public class PlayerInputSystem : ComponentSystem
    {
        struct PlayerInputFilter
        {
            public PlayerInput InputComponent;
        }

        protected override void OnUpdate()
        {
            foreach( PlayerInputFilter entity in GetEntities<PlayerInputFilter>() )
            {
                PlayerInput input = entity.InputComponent;

                Player player = ReInput.players.SystemPlayer;

                input.Movement.x = player.GetAxis( "Horizontal Movement" );
                input.Movement.y = player.GetAxis( "Vertical Movement" );

                input.Jump = player.GetButtonDown( "Jump" );
            }
        }
    }
}