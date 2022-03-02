using UnityEngine;

namespace IntroSequences
{
    public abstract class IntroSequence : ScriptableObject, IIntroSequence
    {
        public abstract void Run(GameObject cameraRigPrefab, string playerActorId);
    }
}
