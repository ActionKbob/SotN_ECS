namespace SotN
{
    using UnityEngine;

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance{ get; private set; }
        
        void Awake()
        {
            if( Instance == null )
                Instance = this;
            else if( Instance != this )
                Destroy( gameObject );
        }
    }
}