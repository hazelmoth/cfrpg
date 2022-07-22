﻿using ActorComponents;

namespace Items
{
    public interface IEdible
    {
        public abstract void ApplyEffects(ActorHealth actorHealth);
    }
}
