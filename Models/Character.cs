using Destiny2;
using Destiny2.Definitions;
using Destiny2.Entities;

namespace MaxPowerLevel.Models
{
    public class Character
    {
        public Character(long characterId, DestinyCharacterComponent character, DestinyClassDefinition classDefinition)
        {
            Id = characterId;
            Level = character.Level;
            PowerLevel = character.Light;
            ClassName = classDefinition.DisplayProperties.Name;
            ClassType = classDefinition.ClassType;
        }

        public long Id { get; }
        public int Level { get; }
        public int PowerLevel { get; }
        public string ClassName { get; }
        public DestinyClass ClassType { get; }

        public override string ToString()
        {
            return $"{ClassName} ({PowerLevel})";
        }
    }
}