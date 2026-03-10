using System.Numerics;
using Content.Server.Shuttles.Systems;
using Content.Shared.Damage;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Content.Server.DeviceLinking.Events;

namespace Content.Server.Shuttles.Components
{
    [RegisterComponent, NetworkedComponent, AutoGenerateComponentPause]
    [Access(typeof(ThrusterSystem))]
    public sealed partial class ThrusterComponent : Component
    {
        /// <summary>
        /// Whether the thruster has been force to be enabled / disabled (e.g. VV, interaction, etc.)
        /// </summary>
        [DataField, ViewVariables(VVAccess.ReadWrite)]
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// This determines whether the thruster is actually enabled for the purposes of thrust
        /// </summary>
        public bool IsOn;

        // Need to serialize this because RefreshParts isn't called on Init and this will break post-mapinit maps!
        [ViewVariables(VVAccess.ReadWrite), DataField("thrust")]
        public float Thrust = 100f;

        [DataField("thrusterType")]
        public ThrusterType Type = ThrusterType.Linear;

        [DataField("burnShape")] public List<Vector2> BurnPoly = new()
        {
            new Vector2(-0.4f, 0.5f),
            new Vector2(-0.1f, 1.2f),
            new Vector2(0.1f, 1.2f),
            new Vector2(0.4f, 0.5f)
        };

        /// <summary>
        /// How much damage is done per second to anything colliding with our thrust.
        /// </summary>
        [DataField("damage")] public DamageSpecifier? Damage = new();

        [DataField("requireSpace")]
        public bool RequireSpace = true;

        // Used for burns

        public List<EntityUid> Colliding = new();

        public bool Firing = false;

        /// <summary>
        /// How often thruster deals damage.
        /// </summary>
        [DataField]
        public TimeSpan FireCooldown = TimeSpan.FromSeconds(2);

        /// <summary>
        /// Next time we tick damage for anyone colliding.
        /// </summary>
        [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), AutoPausedField]
        public TimeSpan NextFire = TimeSpan.Zero;

                [DataField("onPort", customTypeSerializer: typeof(PrototypeIdSerializer<SinkPortPrototype>))] // Frontier
        public string OnPort = "On"; // Frontier

        [DataField("offPort", customTypeSerializer: typeof(PrototypeIdSerializer<SinkPortPrototype>))] // Frontier
        public string OffPort = "Off"; // Frontier

        [DataField("togglePort", customTypeSerializer: typeof(PrototypeIdSerializer<SinkPortPrototype>))] // Frontier
        public string TogglePort = "Toggle"; // Frontier
    }

    public enum ThrusterType
    {
        Linear,
        // Angular meaning rotational.
        Angular,
    }
}
