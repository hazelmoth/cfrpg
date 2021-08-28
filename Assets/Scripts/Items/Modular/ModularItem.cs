using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Items.Modular.Components;
using UnityEngine;

namespace Items.Modular
{
    /// An item whose functionality is divided into modular components.
    public class ModularItem : ItemData
    {
        [SerializeField] protected List<ItemComponent> components;

        /// Returns all components of the specified type.
        public ImmutableList<T> GetComponents<T>() => components.OfType<T>().ToImmutableList();

        /// Returns true if there exist any components of the given type.
        /// Outputs all such components.
        public bool TryGetComponents<T>(out ImmutableList<T> result)
        {
            result = GetComponents<T>();
            return result.Any();
        }
    }
}
