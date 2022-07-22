using System;

namespace ActorComponents
{
    [Serializable]
    public class ActorName : IActorComponent
    {
        public string value;

        public ActorName(string value)
        {
            this.value = value;
        }
    }
}
