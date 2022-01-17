using ContentLibraries;
using MyBox;
using UnityEngine;

/// A class responsible for making moist farmland eventually become dry farmland.
public class FarmlandMoistureUpdater : MonoBehaviour
{
    private const string DryFarmlandGroundMaterialId = "farmland";
    private const string MoistFarmlandGroundMaterialId = "farmland_moist";

    private void Update()
    {
        if (PlayerController.GetPlayerActor() == null) return;

        // For every tile in the player's current scene, check if it's a moist farmland
        // tile; if it is, check if its moisture has expired, and if so, make it a normal
        // farmland tile.
        RegionMapManager.GetMapUnits(PlayerController.GetPlayerActor().CurrentScene)
            .ForEach(
                pair =>
                {
                    (Vector2Int key, MapUnit value) = pair;

                    if (value.groundCover is not { Id: MoistFarmlandGroundMaterialId }) return;
                    if (!value.IsMoist)
                        RegionMapManager.ChangeGroundMaterial(
                            key,
                            PlayerController.GetPlayerActor().CurrentScene,
                            TilemapLayer.GroundCover,
                            ContentLibrary.Instance.GroundMaterials.Get(DryFarmlandGroundMaterialId));
                });
    }
}
