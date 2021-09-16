using System;
using System.Collections.Generic;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Services
{
    public interface ISeason
    {
        int SoftCap { get; }
        int PowerfulCap { get; }
        int HardCap { get; }
        uint SeasonHash { get; }
        int TargetRankPlus20Power { get; }
        DateTime? EndDateOverride { get; }

        IEnumerable<PinnacleActivity> CreatePinnacleActivities(bool includeTrials);
        IEnumerable<PinnacleActivity> CreateWeakPinnacleActivities();
    }
}