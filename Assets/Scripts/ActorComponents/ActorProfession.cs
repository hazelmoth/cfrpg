using System;

namespace ActorComponents
{
    [Serializable]
    public class ActorProfession : IActorComponent
    {
        public readonly string Id;

        public ActorProfession(string id)
        {
            Id = id;
        }
    }
}
