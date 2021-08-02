using System;
using System.Collections.Generic;
using ContentLibraries;
using MyBox;
using UnityEngine;

// Describes any object that can be placed on a tile (buildings, plants, boxes, etc.)
[Serializable]
public class EntityData : IContentItem
{
    [SerializeField] private string entityId;
    [SerializeField] private string entityName;
    [SerializeField] private EntityCategory category;
    [SerializeField] private GameObject entityPrefab;
    [SerializeField] private bool pivotAtCenterOfTile;
    [SerializeField] private bool isConstructable;
    [SerializeField] [ConditionalField("isConstructable")]
    private int workToBuild;
    [SerializeField] [ConditionalField("isConstructable")]
    private List<CraftingIngredient> constructionIngredients = new List<CraftingIngredient>();
    [SerializeField] private bool canBeBuiltOver;
    [SerializeField] private bool canBeWalkedThrough;
    [SerializeField] private float extraTraversalCost;
    [SerializeField] private List<Vector2Int> baseShape = new List<Vector2Int> {new Vector2Int(0, 0)};
    public string EntityName => entityName;
    public EntityCategory Category => category;
    public GameObject EntityPrefab => entityPrefab;
    /// This should be true of non-blocky objects like plants, fences, etc. (necessary for proper sprite sorting).
    /// Take note that this assumes the pivot will always be in the origin tile of multi-tile objects.
    public bool PivotAtCenterOfTile => pivotAtCenterOfTile;
    /// Whether colonists can build this entity
    public bool IsConstructable => isConstructable;
    public int WorkToBuild => workToBuild;
    /// Resources required to initially place this entity
    public List<CraftingIngredient> ConstructionIngredients => constructionIngredients;
    /// Whether you can just build something over this (should be true of weeds, etc.)
    public bool CanBeBuiltOver => canBeBuiltOver;
    /// Determines whether Actors will view the occupied tile as navigable
    public bool CanBeWalkedThrough => canBeWalkedThrough;
    /// How undesirable this tile is to walk through (if it's a bush for example)
    public float ExtraTraversalCost => extraTraversalCost;
    /// Defines what tiles the entity covers
    public List<Vector2Int> BaseShape => baseShape;

    public string Id => entityId;

    [Serializable]
    public struct CraftingIngredient
    {
        public string itemId;
        public int quantity;
    }
}