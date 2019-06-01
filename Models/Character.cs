using Destiny2.Entities;

namespace MaxPowerLevel.Models
{
    public class Character
    {
        public Character(long characterId, DestinyCharacterComponent character)
        {
            Id = characterId;
            Level = character.Level;
            PowerLevel = character.Light;
        }

        public long Id { get; }
        public int Level { get; }
        public int PowerLevel { get; }

        public override string ToString()
        {
            return PowerLevel.ToString();
        }
    }
}