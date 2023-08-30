using UnityEngine;

namespace OTOGIRI.DungeonSystems
{
    [CreateAssetMenu(fileName = "DungeonBuilder", menuName = "OTOGIRI/DungeonBuilder")]
    public class DungeonBuilderScriptableObject : ScriptableObject
    {
        [SerializeReference]
        private IDungeonBuilder dungeonBuilder;
        
        public Dungeon Build()
        {
            return this.dungeonBuilder.Build();
        }
    }
}
