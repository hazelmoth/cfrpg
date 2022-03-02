using UnityEngine;

namespace IntroSequences
{
    /// A script for spawning the player and anything unique to this game setup when a new
    /// world is created.
    public interface IIntroSequence
    {
        /// Spawns the player and may do anything else unique to this game setup.
        public void Run(GameObject cameraRigPrefab, string playerActorId);
    }
}
