using System;

namespace ActorComponents
{
    [Serializable]
    public class ActorPersonality : IActorComponent
    {
        public readonly string id;

        public ActorPersonality(string id)
        {
            this.id = id;
        }
    }
}
