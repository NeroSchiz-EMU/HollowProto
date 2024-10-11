using UnityEngine;

[CreateAssetMenu(fileName = "PhysicsProperties", menuName = "Scriptable Objects/Physics Properties")]
public class PhysicsProperties : ScriptableObject
{
    [Header("Ground Physics")]
    [SerializeField] private float groundDefaultAcceleration;
    [SerializeField] private float groundDecelerationTowardDefaultSpeed;
    [SerializeField] private float groundDecelerationTowardZero;

    [Header("Air Physics")]
    [SerializeField] private bool usesAirMultiplier = true;
    [ShowWhen("usesAirMultiplier", true)]
    [SerializeField] private float airMultiplier = 0.6f;

    [ShowWhen("usesAirMultiplier", false)]
    [SerializeField] private float airDefaultAcceleration = 0f;
    [ShowWhen("usesAirMultiplier", false)]
    [SerializeField] private float airDecelerationTowardDefaultSpeed = 0f;
    [ShowWhen("usesAirMultiplier", false)]
    [SerializeField] private float airDecelerationTowardZero = 0f;

    [Header("Flying")]
    [SerializeField] private bool flying = false;
    [SerializeField] private float gravity = 15f;
    [SerializeField] private float maxFallSpeed = 20f;

    //[Header("Other")]
    //[SerializeField] private Optional<LayerMask> customSolidGroundLayers;


    public float GroundDefaultAcceleration => groundDefaultAcceleration;
    public float GroundDecelerationTowardDefaultSpeed => groundDecelerationTowardDefaultSpeed;
    public float GroundDecelerationTowardZero => groundDecelerationTowardZero;

    // Computed properties for Air Physics based on the ground values and air multiplier
    public float AirDefaultAcceleration => usesAirMultiplier ? airMultiplier * groundDefaultAcceleration : airDefaultAcceleration;
    public float AirDecelerationTowardDefaultSpeed => usesAirMultiplier ? airMultiplier * groundDecelerationTowardDefaultSpeed : airDecelerationTowardDefaultSpeed;
    public float AirDecelerationTowardZero => usesAirMultiplier ? airMultiplier * groundDecelerationTowardZero : airDecelerationTowardZero;

    public bool Flying => flying;
    public float Gravity => gravity;
    public float MaxFallSpeed => maxFallSpeed;

    //public Optional<LayerMask> CustomSolidGroundLayers => customSolidGroundLayers;
}