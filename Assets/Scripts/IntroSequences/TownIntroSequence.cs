using System.Collections;
using System.Collections.Immutable;
using ContentLibraries;
using Items;
using MyBox;
using UnityEngine;
using WorldState;

namespace IntroSequences
{
    /// Handles the sequence of events at the very beginning of the game.
    [CreateAssetMenu(fileName = "IntroSequence", menuName = "IntroSequence/TownIntroSequence")]
    public class TownIntroSequence : IntroSequence
    {
        private const string MortgageLetterText =
            "Her Majesty's Year 760.\n" +
            "\n" +
            "Dear Sir or Madam,\n" +
            "\n" +
            "I am pleased to inform you that your mortgage deed concerning 1.77 octopares of land in the Crescian " +
            "Central Valley has been completed. Your mortgage totals Eight Thousand, Eight Hundred Dollars to be " +
            "paid in the next year.\n" +
            "\n" +
            "Yours faithfully,\n" +
            "\n" +
            "E.J.\n" +
            "The Second Imperial Bank of Lyndon.";

        private static readonly ImmutableList<ItemStack> StartingInv =
            ImmutableList.Create(
                new ItemStack("wheat_seeds", 12),
                new ItemStack("letter", 1)
                    .WithModifier(ItemData.ItemNameModifier,   "Mortgage Letter")
                    .WithModifier(Document.SenderNameModifier, "the Bank of Crescia")
                    .WithModifier(Document.MessageModifier,    MortgageLetterText));

        /// After how many meters the train will stop
        private const float TrainStopPosition = 90f;
        private const float TrainStopTime = 15f;
        private const float BlackScreenTime = 6f;
        private const float FadeInTime = 4f;
        private static readonly Vector2 PlayerSpawnRelativeToTrain = new(15.5f, -0.5f);
        private static readonly Vector2 IntroGuySpawnRelativeToPlayer = new(2f, 0f);
        private const Direction IntroGuySpawnDirection = Direction.Left;
        private const string IntroGuyTemplate = "intro_guy";

        private WorldStateManager worldStateManager;
        private TrainSpawner trainSpawner;
        private GameObject cameraRigPrefab;
        private string playerActorId;

        public override void Run(GameObject cameraRigPrefab, string playerActorId)
        {
            this.cameraRigPrefab = cameraRigPrefab;
            this.playerActorId = playerActorId;

            worldStateManager = GameObject.FindObjectOfType<WorldStateManager>();
            Debug.Assert(worldStateManager != null, "WorldStateManager not found");

            worldStateManager.SetString("intro_state", "awaiting_train");

            // Set inventory
            ActorData playerData = ActorRegistry.Get(playerActorId).data;
            StartingInv.ForEach(
                stack => playerData.Inventory.AttemptAddItem(stack));

            GlobalCoroutineObject.Instance.StartCoroutine(IntroSequenceCoroutine());
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

            // Spawn the intro guy
            Vector2 introGuySpawn =
                TilemapInterface.WorldPosToScenePos(
                    trainSpawner.transform.position.ToVector2(),
                    SceneObjectManager.WorldSceneId)
                + Vector2.left * TrainStopPosition
                + PlayerSpawnRelativeToTrain
                + IntroGuySpawnRelativeToPlayer;

            ActorData introGuyData =
                ActorGenerator.Generate(ContentLibrary.Instance.ActorTemplates.Get(IntroGuyTemplate));
            ActorRegistry.Register(introGuyData);
            ActorSpawner.Spawn(
                introGuyData.ActorId,
                introGuySpawn,
                SceneObjectManager.WorldSceneId,
                IntroGuySpawnDirection);

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

            worldStateManager.SetString("intro_state", "finished");

            // Save the game so we don't have a save in an invalid state
            GameSaver.SaveGame(SaveInfo.SaveFileId);
        }
    }
}
