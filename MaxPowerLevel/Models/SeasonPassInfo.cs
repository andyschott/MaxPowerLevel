using System;

namespace MaxPowerLevel.Models
{
    public class SeasonPassInfo
    {
        public string SeasonName { get; }
        public DateTime EndDate { get; }
        public int Rank { get; }
        public int? RanksPerWeek { get; } = null;

        private const int ResetHour = 17;

        public SeasonPassInfo(string seasonName, DateTime endDate, int rank, int targetRank)
        {
            SeasonName = seasonName;
            EndDate = endDate;
            Rank = rank;

            var remainingWeeks = RemainingWeeks(EndDate);
            var remainingRanks = targetRank - Rank;
            
            if(remainingRanks > 0)
            {
                RanksPerWeek = (int)Math.Ceiling((double)remainingRanks / remainingWeeks);
            }
        }

        private static DateTime PreviousWeeklyReset()
        {
            var date = DateTime.UtcNow;
            while(date.DayOfWeek != DayOfWeek.Tuesday)
            {
                date = date.AddDays(-1);
            }

            return new DateTime(date.Year, date.Month, date.Day,
                ResetHour, 0, 0, DateTimeKind.Utc);
        }

        private static int RemainingWeeks(DateTime endDate)
        {
            var currentReset = PreviousWeeklyReset();
            var remaining = endDate - currentReset;

            return (int)Math.Ceiling(remaining.TotalDays / 7);
        }
    }
}