using System.Collections.Generic;
using OTOGIRI.ActorControllers;
using OTOGIRI.ActorControllers.Behaviours;

namespace OTOGIRI
{
    public class DungeonModel
    {
        public ActorModel PlayerModel { get; private set; }

        private readonly List<ActorModel> allModels = new();
        
        private readonly List<ActorModel> otherModels = new();
        
        public IReadOnlyList<ActorModel> AllModels => allModels;
        
        public IReadOnlyList<ActorModel> OtherModels => otherModels;
        
        public Define.CellType[,] Map { get; set; }

        public void SetPlayerModel(ActorModel model)
        {
            this.PlayerModel = model;
            this.allModels.Add(model);
        }
        
        public void AddOtherModel(ActorModel model)
        {
            this.otherModels.Add(model);
            this.allModels.Add(model);
        }
    }
}