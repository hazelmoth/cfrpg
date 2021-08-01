using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ContentLibraries;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FeatureGenerators
{
    /// A RegionFeatureGenerator which places entities in a predefined arrangement, at a
    /// random location in the region.
    [CreateAssetMenu(menuName = "Feature Generator/Predefined Entity Arrangement")]
    public class PredefinedEntityArrangement : RegionFeatureGenerator
    {
        [SerializeField] private List<EntityPlacement> entities;

        public override bool AttemptApply(RegionMap region, RegionInfo info, int seed)
        {
            // determine bounds of arrangement
            // randomly pick origin such that bounds fit in region bounds
            // place entities at origin + offset

            (Vector2Int min, Vector2Int max) = CalculateBounds(entities.Select(entity => entity.position));
            int x = Random.Range(min.x, SaveInfo.RegionSize.x - 1 - max.x);
            int y = Random.Range(min.y, SaveInfo.RegionSize.y - 1 - max.y);

            entities.ForEach(
                entity => region.AttemptPlaceEntity(
                    ContentLibrary.Instance.Entities.Get(entity.id),
                    1, 
                    entity.position + new Vector2Int(x, y),
                    RegionMapUtil.PlacementSettings.PlaceOverAnything));

            return true;
        }

        /// Returns two vectors representing the bounds of the given collection of vectors,
        /// where the result vectors are (min x, min y) and (max x, max y).
        private static Tuple<Vector2Int, Vector2Int> CalculateBounds(IEnumerable<Vector2Int> vectors)
        {
            IEnumerable<Vector2Int> vector2Ints = vectors.ToList();
            return Tuple.Create(
                new Vector2Int(
                    vector2Ints.Select(v => v.x).Min(),
                    vector2Ints.Select(v => v.y).Min()),
                new Vector2Int(
                    vector2Ints.Select(v => v.x).Max(),
                    vector2Ints.Select(v => v.y).Max())
            );
        }

        [Serializable]
        private class EntityPlacement
        {
            public string id;
            public Vector2Int position;
        }
    }
}