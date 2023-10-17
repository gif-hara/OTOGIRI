using OTOGIRI.GameSystems;
using UnityEngine;

namespace OTOGIRI
{
    public class BootSystem
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            HK.Framework.BootSystems.BootSystem.AdditionalSetupContainerBuilderAsync += (builder) =>
            {
                GameEvents.RegisterEvents(builder);
            };
        }
    }
}
