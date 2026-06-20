using System.Collections.Generic;
using Unreal.Core;
using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models.Enums;

namespace ValorantReplayParser.Models;

public enum EAresAttributeIndex
{
    AllowFiringWhileUsing = 0,
    BonusDamage = 1,
    CanSeeOthersHealth = 2,
    DamageReduction = 3,
    DefensiveDetectionDuration = 4,
    DescendingGravityModifier = 5,
    DetectionDelay = 6,
    DetectionDisabled = 7,
    EquippableMovementModifier = 8,
    HealingEffectiveness = 9,
    ShieldRestoreEffectiveness = 10,
    FiringErrorModifier = 11,
    FiringRateModifier = 12,
    StabilityMovementModifier = 13,
    GravityModifier = 14,
    FootstepVolumeReduction = 15,
    ForceCrouch = 16,
    Frozen = 17,
    HeavyWeaponsDisabled = 18,
    Hide1P = 19,
    Hide3P = 20,
    HideCrosshair = 21,
    HudVisibleToAll = 22,
    HudVisibleToInstigator = 23,
    InDanger = 24,
    Intangible = 25,
    Invisible = 26,
    InvisibleEquippables = 27,
    InvisibleToEnemies = 28,
    InvisibleToEnemyNonPlayers = 29,
    InvisibleToOwner = 30,
    Invulnerable = 31,
    JumpForceModifier = 32,
    JumpMovementSlow = 33,
    MinimapBlinded = 34,
    MinimumAboveMaxSpeedDecayRateReduction = 35,
    FallDamageReduction = 36,
    LockMovement = 37,
    MagazineBonus = 38,
    MapVisibleToAll = 39,
    MaxHealthModifier = 40,
    MaxShieldModifier = 41,
    MaxTurnRate = 42,
    MinError = 43,
    MinimapDetectionDelay = 44,
    MinimapDisabled = 45,
    MinimapInvisible = 46,
    MinimapViewDistance = 47,
    MaxVisionDistance = 48,
    MoneyKillRewardModifier = 49,
    MovementBonus = 50,
    MovementBonusCrouch = 51,
    MovementBonusFlying = 52,
    MovementBonusJump = 53,
    MovementBonusRun = 54,
    MovementBonusWalk = 55,
    MovementErrorModifier = 56,
    MovementSlow = 57,
    OffensiveDetectionDuration = 58,
    PickupAmmoBonus = 59,
    PreventAbilities = 60,
    PreventCrouch = 61,
    PreventDroppingEquippables = 62,
    PreventJump = 63,
    PreventFiring = 64,
    PreventFiringPrimaryWeapon = 65,
    PreventFiringSecondaryWeapon = 66,
    PreventMeleeAttacking = 67,
    PreventMovementInput = 68,
    PreventReloading = 69,
    PreventSwitchingEquippables = 70,
    PreventUseCancelling = 71,
    PreventUsing = 72,
    PreventPickup = 73,
    PrimarySlotDisabled = 74,
    ReduceAbilityMovementPenalty = 75,
    ReduceTaggingMovementPenalty = 76,
    ReloadTimeModifier = 77,
    SpreadRecoveryModifier = 78,
    Stealthed = 79,
    Untrackable = 80,
    RecoilModifier = 81,
    ThirdPerson = 82,
    TurnRatePenalty = 83,
    UltimatePointsDeathModifier = 84,
    UltimatePointsDefuseModifier = 85,
    UltimatePointsKillModifier = 86,
    UltimatePointsPickUpModifier = 87,
    UltimatePointsPlantModifier = 88,
    UsingTimeModifier = 89,
    WallPenetrationDistanceModifier = 90,
    WeaponDrawTimeModifier = 91,
    FastEquipPrimaryWeapon = 92,
    FastEquipSecondaryWeapon = 93,
    WeaponsDisabled = 94,
    WeaponsLowered = 95,
    Disarmed = 96,
    GrenadeDisabled = 97,
    QDisabled = 98,
    EDisabled = 99,
    UltimateDisabled = 100,
    ZoomDisabled = 101,
    EquipmentDisabled = 102,
    TemporaryDamage = 103,
    IncomingDamageShieldPenetrationModifier = 104,
    IncomingSelfDamageModifier = 105,
    IncomingAllyDamageModifier = 106,
    PreventDeathFromDamage = 107,
    BlindImmune = 108,
    PreventUsingAbilities = 109,
    PreventUsingAscenders = 110,
    PreventUsingLoreItems = 111,
    OverrideEquippableBaseMovement = 112,
    DisableRegionalDamageMultipliers = 113,
    DisableIncomingDamageCombatTracking = 114,
    HealsFromAllyFlames = 115,
    Marked = 116,
    Suppressed = 117,
    PreventDowned = 118,
    PreventPlanting = 119,
    PreventDefusing = 120,
    PreventFollowing = 121,
    InPeril = 122,
    PreventMinimapFocusing = 123,
    SensitivityModifier = 124,
    ShowObserverKeybindsWhileHidden = 125,
    PreventSkinFinisherIfVictim = 126,
    PreventTeamWipeCondition = 127,
    GroundedFootstepMute = 128,
    DelayDeathUltPointReward = 129,
    PreventKillUltPointReward = 130,
    DashSpeedMultiplier = 131,
    DisablePrimaryWeaponFocusMode = 132,
    AbilityInvulnerable = 133,
    DisplacementImmunity = 134,
    ImpairmentImmunity = 135,
    BombPlantTime = 136,
    BombDefuseTime = 137,
    FootstepPlayTimeMultiplier = 138,
    HeadshotDamageMultiplier = 139,
    NormalDamageMultiplier = 140,
    LegshotDamageMultiplier = 141,
}

[NetFieldExportGroup("/Script/ShooterGame.AresAttributeSet", minimalParseMode: ParseMode.Normal)]
public class AresAttributeSet : INetFieldExportGroup, IHandleNetFieldExportGroup
{
    public List<AttributeValue> Attributes { get; set; } = [];
    public IEnumerable<AttributeValue> ChangedAttributes => Attributes.Where(a => a.BaseValue.HasValue && a.CurrentValue.HasValue && Math.Abs(a.BaseValue.Value - a.CurrentValue.Value) > 1e-5);
    public float? Healing { get; set; }
    public float? Damage { get; set; }
    public float? Shield { get; set; }

    public bool ReadFieldHandle(uint handle, NetBitReader reader)
    {
        var totalAttributePairs = (int)EAresAttributeIndex.LegshotDamageMultiplier + 1;
        if (handle < totalAttributePairs * 2u)
        {
            var index = (int)(handle / 2u);
            var isCurrent = (handle % 2) != 0;

            while (Attributes.Count <= index)
            {
                Attributes.Add(new AttributeValue());
            }

            if (isCurrent)
            {
                Attributes[index].Handle = handle;
                Attributes[index].AttributeName = ((EAresAttributeIndex)index).ToString();
                Attributes[index].IsBoolean = _booleanAttributes.Contains((EAresAttributeIndex)index);
                Attributes[index].CurrentValue = reader.SerializePropertyFloat();
            }
            else
            {
                Attributes[index].Handle = handle;
                Attributes[index].AttributeName = ((EAresAttributeIndex)index).ToString();
                Attributes[index].IsBoolean = _booleanAttributes.Contains((EAresAttributeIndex)index);
                Attributes[index].BaseValue = reader.SerializePropertyFloat();
            }

            return true;
        }

        switch (handle)
        {
            case HealingHandle:
                Healing = reader.SerializePropertyFloat();
                return true;
            case DamageHandle:
                Damage = reader.SerializePropertyFloat();
                return true;
            case ShieldHandle:
                Shield = reader.SerializePropertyFloat();
                return true;
        }

        return false;
    }

    private const uint HealingHandle = (uint)EAresAttributeIndex.LegshotDamageMultiplier * 2 + 2;
    private const uint DamageHandle = HealingHandle + 1;
    private const uint ShieldHandle = HealingHandle + 2;

    private static readonly HashSet<EAresAttributeIndex> _booleanAttributes =
    [
        EAresAttributeIndex.AllowFiringWhileUsing,
        EAresAttributeIndex.CanSeeOthersHealth,
        EAresAttributeIndex.DetectionDisabled,
        EAresAttributeIndex.ForceCrouch,
        EAresAttributeIndex.Frozen,
        EAresAttributeIndex.HeavyWeaponsDisabled,
        EAresAttributeIndex.Hide1P,
        EAresAttributeIndex.Hide3P,
        EAresAttributeIndex.HideCrosshair,
        EAresAttributeIndex.HudVisibleToAll,
        EAresAttributeIndex.HudVisibleToInstigator,
        EAresAttributeIndex.InDanger,
        EAresAttributeIndex.Intangible,
        EAresAttributeIndex.Invisible,
        EAresAttributeIndex.InvisibleEquippables,
        EAresAttributeIndex.InvisibleToEnemies,
        EAresAttributeIndex.InvisibleToEnemyNonPlayers,
        EAresAttributeIndex.InvisibleToOwner,
        EAresAttributeIndex.Invulnerable,
        EAresAttributeIndex.MinimapBlinded,
        EAresAttributeIndex.LockMovement,
        EAresAttributeIndex.MapVisibleToAll,
        EAresAttributeIndex.MinimapDisabled,
        EAresAttributeIndex.MinimapInvisible,
        EAresAttributeIndex.PreventAbilities,
        EAresAttributeIndex.PreventCrouch,
        EAresAttributeIndex.PreventDroppingEquippables,
        EAresAttributeIndex.PreventJump,
        EAresAttributeIndex.PreventFiring,
        EAresAttributeIndex.PreventFiringPrimaryWeapon,
        EAresAttributeIndex.PreventFiringSecondaryWeapon,
        EAresAttributeIndex.PreventMeleeAttacking,
        EAresAttributeIndex.PreventMovementInput,
        EAresAttributeIndex.PreventReloading,
        EAresAttributeIndex.PreventSwitchingEquippables,
        EAresAttributeIndex.PreventUseCancelling,
        EAresAttributeIndex.PreventUsing,
        EAresAttributeIndex.PreventPickup,
        EAresAttributeIndex.PrimarySlotDisabled,
        EAresAttributeIndex.Stealthed,
        EAresAttributeIndex.Untrackable,
        EAresAttributeIndex.ThirdPerson,
        EAresAttributeIndex.WeaponsDisabled,
        EAresAttributeIndex.WeaponsLowered,
        EAresAttributeIndex.Disarmed,
        EAresAttributeIndex.GrenadeDisabled,
        EAresAttributeIndex.QDisabled,
        EAresAttributeIndex.EDisabled,
        EAresAttributeIndex.UltimateDisabled,
        EAresAttributeIndex.ZoomDisabled,
        EAresAttributeIndex.EquipmentDisabled,
        EAresAttributeIndex.PreventDeathFromDamage,
        EAresAttributeIndex.BlindImmune,
        EAresAttributeIndex.PreventUsingAbilities,
        EAresAttributeIndex.PreventUsingAscenders,
        EAresAttributeIndex.PreventUsingLoreItems,
        EAresAttributeIndex.DisableRegionalDamageMultipliers,
        EAresAttributeIndex.DisableIncomingDamageCombatTracking,
        EAresAttributeIndex.HealsFromAllyFlames,
        EAresAttributeIndex.Marked,
        EAresAttributeIndex.Suppressed,
        EAresAttributeIndex.PreventDowned,
        EAresAttributeIndex.PreventPlanting,
        EAresAttributeIndex.PreventDefusing,
        EAresAttributeIndex.PreventFollowing,
        EAresAttributeIndex.InPeril,
        EAresAttributeIndex.PreventMinimapFocusing,
        EAresAttributeIndex.ShowObserverKeybindsWhileHidden,
        EAresAttributeIndex.PreventSkinFinisherIfVictim,
        EAresAttributeIndex.PreventTeamWipeCondition,
        EAresAttributeIndex.GroundedFootstepMute,
        EAresAttributeIndex.DelayDeathUltPointReward,
        EAresAttributeIndex.PreventKillUltPointReward,
        EAresAttributeIndex.DisablePrimaryWeaponFocusMode,
        EAresAttributeIndex.AbilityInvulnerable,
        EAresAttributeIndex.DisplacementImmunity,
        EAresAttributeIndex.ImpairmentImmunity,
        EAresAttributeIndex.FastEquipPrimaryWeapon,
        EAresAttributeIndex.FastEquipSecondaryWeapon,
    ];
}

public class AttributeValue
{
    public uint Handle { get; set; }
    public string AttributeName { get; set; } = string.Empty;
    public bool IsBoolean { get; set; }
    public float? BaseValue { get; set; }
    public float? CurrentValue { get; set; }
    public bool? BoolValue => IsBoolean ? (CurrentValue == 1.0f) : null;
}
