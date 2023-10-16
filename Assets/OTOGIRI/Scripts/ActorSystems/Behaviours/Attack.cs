using UnityEngine;

namespace OTOGIRI.ActorControllers.Behaviours
{
    public class Attack : ActorBehaviour
    {
        public Attack(int consumeActionPoint)
            : base(Define.ActorBehaviourType.Action, consumeActionPoint)
        {
        }
        
        public override void Execute(ActorModel actorModel, DungeonModel dungeonModel)
        {
            // TODO: Implement attack logic
            Debug.Log($"{actorModel.Name}: Attack");
        }
    }
}