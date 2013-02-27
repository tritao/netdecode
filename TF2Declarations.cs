using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace netdecode
{
    // Adapted from sourcemod tf2.inc
    [Flags]
    enum TFStunType
    {
        SLOWDOWN        = (1 << 0), // activates slowdown modifier
        BONKSTUCK       = (1 << 1), // bonk sound, stuck
        LIMITMOVEMENT   = (1 << 2), // disable forward/backward movement
        CHEERSOUND      = (1 << 3), // cheering sound
        NOSOUNDOREFFECT = (1 << 5), // no sound or particle
        THIRDPERSON     = (1 << 6), // panic animation
        GHOSTEFFECT     = (1 << 7), // ghost particles

        LOSERSTATE     = SLOWDOWN|NOSOUNDOREFFECT|THIRDPERSON,
        GHOSTSCARE     = GHOSTEFFECT|THIRDPERSON,
        SMALLBONK      = THIRDPERSON|SLOWDOWN,
        NORMALBONK     = BONKSTUCK,
        BIGBONK        = CHEERSOUND|BONKSTUCK
    }

    enum TFClassType
    {
        Unknown = 0,
        Scout,
        Sniper,
        Soldier,
        Demoman,
        Medic,
        Heavy,
        Pyro,
        Spy,
        Engineer
    }

    enum TFTeam
    {
        Unassigned = 0,
        Spectator = 1,
        Red = 2,
        Blue = 3
    }

    enum TFCond
    {
        Slowed = 0,
        Zoomed,
        Disguising,
        Disguised,
        Cloaked,
        Ubercharged,
        TeleportedGlow,
        Taunting,
        UberchargeFading,
        Unknown1, //9
        CloakFlicker = 9,
        Teleporting,
        Kritzkrieged,
        Unknown2, //12
        TmpDamageBonus = 12,
        DeadRingered,
        Bonked,
        Dazed,
        Buffed,
        Charging,
        DemoBuff,
        CritCola,
        InHealRadius,
        Healing,
        OnFire,
        Overhealed,
        Jarated,
        Bleeding,
        DefenseBuffed,
        Milked,
        MegaHeal,
        RegenBuffed,
        MarkedForDeath,
        NoHealingDamageBuff,
        SpeedBuffAlly,
        HalloweenCritCandy,
    
        CritHype = 36,
        CritOnFirstBlood,
        CritOnWin,
        CritOnFlagCapture,
        CritOnKill,
        RestrictToMelee,
    
        CritMmmph = 44,
        DefenseBuffMmmph
    }

    enum TFHoliday
    {
        Birthday = 1,
        Halloween,
        Christmas,
        MeetThePyro,
        MannVsMachine,
        FullMoon,
        HalloweenOrFullMoon,
    }

    enum TFObjectType
    {
        CartDispenser = 0,
        Dispenser = 0,
        Teleporter = 1,
        Sentry = 2,
        Sapper = 3
    }

    enum TFObjectMode
    {
        None = 0,
        Entrance = 0,
        Exit = 1
    }

    // Adapted from tf2_stocks.inc
    [Flags]
    enum TFConditionType
    {
        NONE            = 0,
        SLOWED          = (1 << 0),
        ZOOMED          = (1 << 1),
        DISGUISING      = (1 << 2),
        DISGUISED       = (1 << 3),
        CLOAKED         = (1 << 4),
        UBERCHARGED     = (1 << 5),
        TELEPORTGLOW    = (1 << 6),
        TAUNTING        = (1 << 7),
        UBERCHARGEFADE  = (1 << 8),
        CLOAKFLICKER    = (1 << 9),
        TELEPORTING     = (1 << 10),
        KRITZKRIEGED    = (1 << 11),
        DEADRINGERED    = (1 << 13),
        BONKED          = (1 << 14),
        DAZED           = (1 << 15),
        BUFFED          = (1 << 16),
        CHARGING        = (1 << 17),
        DEMOBUFF        = (1 << 18),
        CRITCOLA        = (1 << 19),
        INHEALRADIUS    = (1 << 20),
        HEALING         = (1 << 21),
        ONFIRE          = (1 << 22),
        OVERHEALED      = (1 << 23),
        JARATED         = (1 << 24),
        BLEEDING        = (1 << 25),
        DEFENSEBUFFED   = (1 << 26),
        MILKED          = (1 << 27),
        MEGAHEAL        = (1 << 28),
        REGENBUFFED     = (1 << 29),
        MARKEDFORDEATH  = (1 << 30)
    }

    [Flags]
    enum TFDeathType
    {
        KILLERDOMINATION   = (1 << 0),
        ASSISTERDOMINATION = (1 << 1),
        KILLERREVENGE      = (1 << 2),
        ASSISTERREVENGE    = (1 << 3),
        FIRSTBLOOD         = (1 << 4),
        DEADRINGER         = (1 << 5),
        INTERRUPTED        = (1 << 6),
        GIBBED             = (1 << 7),
        PURGATORY          = (1 << 8)
    }

    // Custom kill identifiers for the customkill property on the player_death event
    enum TFCustomKill
    {
        HEADSHOT = 1,
        BACKSTAB,
        BURNING,
        WRENCH_FIX,
        MINIGUN,
        SUICIDE,
        TAUNT_HADOUKEN,
        BURNING_FLARE,
        TAUNT_HIGH_NOON,
        TAUNT_GRAND_SLAM,
        PENETRATE_MY_TEAM,
        PENETRATE_ALL_PLAYERS,
        TAUNT_FENCING,
        PENETRATE_HEADSHOT,
        TAUNT_ARROW_STAB,
        TELEFRAG,
        BURNING_ARROW,
        FLYINGBURN,
        PUMPKIN_BOMB,
        DECAPITATION,
        TAUNT_GRENADE,
        BASEBALL,
        CHARGE_IMPACT,
        TAUNT_BARBARIAN_SWING,
        AIR_STICKY_BURST,
        DEFENSIVE_STICKY,
        PICKAXE,
        ROCKET_DIRECTHIT,
        TAUNT_UBERSLICE,
        PLAYER_SENTRY,
        STANDARD_STICKY,
        SHOTGUN_REVENGE_CRIT,
        TAUNT_ENGINEER_SMASH,
        BLEEDING,
        GOLD_WRENCH,
        CARRIED_BUILDING,
        COMBO_PUNCH,
        TAUNT_ENGINEER_ARM,
        FISH_KILL,
        TRIGGER_HURT,
        DECAPITATION_BOSS,
        STICKBOMB_EXPLOSION,
        AEGIS_ROUND,
        FLARE_EXPLOSION,
        BOOTS_STOMP,
        PLASMA,
        PLASMA_CHARGED,
        PLASMA_GIB,
        PRACTICE_STICKY,
        EYEBALL_ROCKET,
        HEADSHOT_DECAPITATION,
        TAUNT_ARMAGEDDON,
        FLARE_PELLET,
        CLEAVER,
        CLEAVER_CRIT,
        SAPPER_RECORDER_DEATH,
        MERASMUS_PLAYER_BOMB,
        MERASMUS_GRENADE,
        MERASMUS_ZAP,
        MERASMUS_DECAPITATION,
    }

    // Weapon codes as used in some events, such as player_death
    // (not to be confused with Item Definition Indexes)
    enum TFWeapon
    {
        NONE = 0,
        BAT,
        BAT_WOOD,
        BOTTLE,
        FIREAXE,
        CLUB,
        CROWBAR,
        KNIFE,
        FISTS,
        SHOVEL,
        WRENCH,
        BONESAW,
        SHOTGUN_PRIMARY,
        SHOTGUN_SOLDIER,
        SHOTGUN_HWG,
        SHOTGUN_PYRO,
        SCATTERGUN,
        SNIPERRIFLE,
        MINIGUN,
        SMG,
        SYRINGEGUN_MEDIC,
        TRANQ,
        ROCKETLAUNCHER,
        GRENADELAUNCHER,
        PIPEBOMBLAUNCHER,
        FLAMETHROWER,
        GRENADE_NORMAL,
        GRENADE_CONCUSSION,
        GRENADE_NAIL,
        GRENADE_MIRV,
        GRENADE_MIRV_DEMOMAN,
        GRENADE_NAPALM,
        GRENADE_GAS,
        GRENADE_EMP,
        GRENADE_CALTROP,
        GRENADE_PIPEBOMB,
        GRENADE_SMOKE_BOMB,
        GRENADE_HEAL,
        GRENADE_STUNBALL,
        GRENADE_JAR,
        GRENADE_JAR_MILK,
        PISTOL,
        PISTOL_SCOUT,
        REVOLVER,
        NAILGUN,
        PDA,
        PDA_ENGINEER_BUILD,
        PDA_ENGINEER_DESTROY,
        PDA_SPY,
        BUILDER,
        MEDIGUN,
        GRENADE_MIRVBOMB,
        FLAMETHROWER_ROCKET,
        GRENADE_DEMOMAN,
        SENTRY_BULLET,
        SENTRY_ROCKET,
        DISPENSER,
        INVIS,
        FLAREGUN,
        LUNCHBOX,
        JAR,
        COMPOUND_BOW,
        BUFF_ITEM,
        PUMPKIN_BOMB,
        SWORD,
        DIRECTHIT,
        LIFELINE,
        LASER_POINTER,
        DISPENSER_GUN,
        SENTRY_REVENGE,
        JAR_MILK,
        HANDGUN_SCOUT_PRIMARY,
        BAT_FISH,
        CROSSBOW,
        STICKBOMB,
        HANDGUN_SCOUT_SEC,
        SODA_POPPER,
        SNIPERRIFLE_DECAP,
        RAYGUN,
        PARTICLE_CANNON,
        MECHANICAL_ARM,
        DRG_POMSON,
        BAT_GIFTWRAP,
        GRENADE_ORNAMENT,
        RAYGUN_REVENGE,
        PEP_BRAWLER_BLASTER,
        CLEAVER,
        GRENADE_CLEAVER,
    }

    // TF2 Weapon Loadout Slots
    enum TFWeaponSlot
    {
        Primary,
        Secondary,
        Melee,
        Grenade,
        Building,
        PDA,
        Item1,
        Item2
    }

    // Identifiers for the eventtype property on the teamplay_flag_event event
    enum TFFlagEvent
    {
        PICKEDUP = 1,
        CAPTURED,
        DEFENDED,
        DROPPED,
        RETURNED
    }

    enum TFResourceType
    {
        Ping,
        Score,
        Deaths,
        TotalScore, 
        Captures,
        Defenses,
        Dominations,
        Revenge,
        BuildingsDestroyed, 
        Headshots,
        Backstabs,
        HealPoints,
        Invulns,
        Teleports,
        ResupplyPoints,
        KillAssists,
        MaxHealth,
        PlayerClass
    }
}
