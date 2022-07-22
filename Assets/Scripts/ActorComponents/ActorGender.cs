using System;

namespace ActorComponents
{
    [Serializable]
    public class ActorGender : IActorComponent
    {
        public readonly Gender value;

        public ActorGender(Gender value)
        {
            this.value = value;
        }
    }
}
