namespace MaxPowerLevel.Models
{
    public class Engram
    {
        public Engram(string name, int lowestPower, int? highestPower = null)
        {
            Name = name;
            LowestPower = lowestPower;
            HighestPower = highestPower ?? LowestPower;
        }

        public string Name { get; }
        public int LowestPower { get; }
        public int HighestPower { get; }

        public string Description
        {
            get
            {
                if(LowestPower == HighestPower)
                {
                    return $"{Name}: {LowestPower}";
                }

                return $"{Name}: {LowestPower} - {HighestPower}";
            }
        }
    }
}
