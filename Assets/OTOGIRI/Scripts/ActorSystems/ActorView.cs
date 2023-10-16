using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UnityEngine;

namespace OTOGIRI.ActorControllers
{
    public class ActorView
    {
        public ActorView(ActorModel actorModel, CancellationToken cancellationToken)
        {
            actorModel.PositionAsReactiveProperty()
                .Subscribe(position =>
                {
                    Debug.Log($"{actorModel.Name}: {position}");
                })
                .AddTo(cancellationToken);
        }
    }
}
