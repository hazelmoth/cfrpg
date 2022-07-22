using System;
using ContentLibraries;
using UnityEngine;

namespace ActorTemplates
{
    /// A template for creating new actors. The returned actor will not be registered with
    /// the actor registry yet.
    public abstract class ActorTemplate : ScriptableObject, IContentItem
    {
        public abstract string Id { get; }

        /// Returns a newly-created ActorData instance, with an ID that is unique
        /// according to the provided function.
        public abstract ActorData CreateActor(Func<string, bool> isIdAvailable, out string id);

        /// Returns a unique ID based on the given name
        protected static string CreateUniqueId(string actorName, Func<string, bool> isIdAvailable)
        {
            string id = actorName.ToLower().Replace(" ", "")[..8];
            string baseId = id;
            int currentNum = 0;
            while (!isIdAvailable(id))
            {
                id = baseId + currentNum;
                currentNum++;

                if (currentNum > 1000)
                {
                    Debug.LogError("Apparent infinite loop detected generating ID for actor " + actorName);
                    break;
                }
            }
            return id;
        }
    }
}
