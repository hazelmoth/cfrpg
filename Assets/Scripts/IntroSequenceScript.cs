using System.Collections;
using MyBox;
using UnityEngine;

/// Handles the sequence of events at the very beginning of the game.
public class IntroSequenceScript : MonoBehaviour
{
    /// After how many meters the train will stop
    private const float TrainStopPosition = 90f;
    private const float TrainStopTime = 15f;
    private const float BlackScreenTime = 6f;
    private const float FadeInTime = 4f;
    private static readonly Vector2 PlayerSpawnRelativeToTrain = new Vector2(15.5f, -0.5f);

    private TrainSpawner trainSpawner;
    private GameObject cameraRigPrefab;
    private string playerActorId;

    public void RunIntroSequence(GameObject cameraRigPrefab, string playerActorId)
    {
        this.cameraRigPrefab = cameraRigPrefab;
        this.playerActorId = playerActorId;
        StartCoroutine(IntroSequenceCoroutine());
    }

    /// A Coroutine that waits for the train to stop and then spawns the player.
    private IEnumerator IntroSequenceCoroutine()
    {
        trainSpawner = FindObjectOfType<TrainSpawner>();
        if (trainSpawner == null)
        {
            Debug.LogError("No train spawner found");
        }

        ScreenFadeAnimator.FadeOut(0);
        float blackScreenStartTime = Time.time;
        bool fadedIn = false;

        // Get the train
        if (!trainSpawner.CurrentTrain)
            trainSpawner.SpawnStoppingTrain(TrainStopPosition, TrainStopTime);
        else trainSpawner.CurrentTrain.GetComponent<Train>().Stop(TrainStopPosition, TrainStopTime);

        Train train = trainSpawner.CurrentTrain.GetComponent<Train>();

        // Wait a frame for train to start moving
        yield return null;

        // Instantiate camera rig
        GameObject cameraRig = Instantiate(cameraRigPrefab);

        // Wait for train to stop
        while (!train.Stopped)
        {
            // Set the camera to follow the train
            cameraRig.transform.position = train.transform.position + PlayerSpawnRelativeToTrain.ToVector3();
            if (Time.time - blackScreenStartTime > BlackScreenTime)
            {
                ScreenFadeAnimator.FadeIn(FadeInTime);
                fadedIn = true;
            }
            yield return null;
        }
        if (!fadedIn) ScreenFadeAnimator.FadeIn(FadeInTime);

        // Spawn the player
        Vector2 spawnPoint = train.transform.position.ToVector2() + PlayerSpawnRelativeToTrain;
        spawnPoint = TilemapInterface.WorldPosToScenePos(spawnPoint, SceneObjectManager.WorldSceneId);
        Actor player = ActorSpawner.Spawn(playerActorId, spawnPoint, SceneObjectManager.WorldSceneId);
        PlayerController.SetPlayerActor(playerActorId);

        yield return null;
        Destroy(cameraRig);
    }
}
