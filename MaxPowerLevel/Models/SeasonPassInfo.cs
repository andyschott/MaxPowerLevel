using System;

namespace MaxPowerLevel.Models
{
    public class SeasonPassInfo
    {
        public DateTime EndDate { get; }
        public int Rank { get; }
        public int RanksPerWeek { get; }

        private const int ResetHour = 17;

        public SeasonPassInfo(DateTime endDate, int rank, int targetRank)
        {
            EndDate = endDate;
            Rank = rank;

            var remainingWeeks = RemainingWeeks(EndDate);
            var remainingRanks = targetRank - Rank;
            
            RanksPerWeek = (int)Math.Ceiling((double)remainingRanks / remainingWeeks);
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