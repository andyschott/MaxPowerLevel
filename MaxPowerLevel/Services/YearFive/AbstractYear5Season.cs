using System.Collections.Generic;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Services.YearFive
{
    public abstract class AbstractYear5Season : AbstractSeason
    {
        protected static readonly PinnacleActivity _graspOfAvarice = new PinnacleActivity("Grasp of Avarice", new[] { AllSlots });
        protected static readonly PinnacleActivity _daresOfEternity = new PinnacleActivity("Dares of Eternity Pinnacle Challenge", new[] { AllSlots });
        protected static readonly PinnacleActivity _voxObscura = new PinnacleActivity("Vox Obscura", new[] { AllSlots });
        protected static readonly PinnacleActivity _wellspring = new PinnacleActivity("Wellspring Pinnacle Challenge", new[] { AllSlots });
        protected static readonly PinnacleActivity _missionHighScore = new PinnacleActivity("Weekly Mission High Score", new[] { AllSlots });
        protected static readonly PinnacleActivity _vowOfTheDisciple = new PinnacleActivity("Vow of the Disciple", new[]
        {
            new[]
            {
                ItemSlot.SlotHashes.Kinetic, // SMG, Fusion
                ItemSlot.SlotHashes.Power, // LFR
                ItemSlot.SlotHashes.Helmet,
                ItemSlot.SlotHashes.ChestArmor,
                ItemSlot.SlotHashes.LegArmor
            },
            new[]
            {
                ItemSlot.SlotHashes.Kinetic, // SMG
                ItemSlot.SlotHashes.Energy, // Pulse, GL
                ItemSlot.SlotHashes.Power, // LFR
                ItemSlot.SlotHashes.Helmet,
                ItemSlot.SlotHashes.Gauntlet,
                ItemSlot.SlotHashes.ClassArmor
            },
            new[]
            {
                ItemSlot.SlotHashes.Kinetic, // SMG, Fusion
                ItemSlot.SlotHashes.Helmet,
                ItemSlot.SlotHashes.LegArmor
            },
            new[]
            {
                ItemSlot.SlotHashes.Energy, // Glaive, GL
                ItemSlot.SlotHashes.Gauntlet,
                ItemSlot.SlotHashes.ClassArmor,
                ItemSlot.SlotHashes.ChestArmor
            },
        });
        protected static readonly PinnacleActivity _preservation = new PinnacleActivity("Preservation", new[] { AllSlots });

        protected override IEnumerable<PinnacleActivity> CreatePinnacleActivities()
        {
            return new[]
            {
                NightfallScore,
                _graspOfAvarice,
                _daresOfEternity,
                _voxObscura,
                _wellspring,
                _missionHighScore  ,
                _preservation 
            };
        }
    }
}