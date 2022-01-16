using System.Collections.Generic;

namespace Items
{
    /// An item that can be held and activated by an actor.
    public interface IActivatable
    {
        /// Invokes the effects for an instance of this item.
        void Activate(IDictionary<string, string> modifiers, Actor user);
    }
}
