namespace SotN
{
    using UnityEngine;

    public class MoveSpeed : MonoBehaviour
    {
        public float Value = 8;
        public float GroundAcceleration = 0.1f;
        public float AirAcceleration = 0.2f;
        public float Smoothing;
    }
}