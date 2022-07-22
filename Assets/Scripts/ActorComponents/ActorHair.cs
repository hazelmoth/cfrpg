using System;

namespace ActorComponents
{
    [Serializable]
    public class ActorHair : IActorComponent
    {
        public readonly string id;

        public ActorHair(string id)
        {
            this.id = id;
        }
    }
}
