using System;
using System.Collections.Generic;
using System.Linq;
using ContinentMaps;
using UnityEngine;

namespace SettlementSystem
{
    /// Maintains data for all settlements in the game.
    public class SettlementManager : MonoBehaviour
    {
        /// Maps region ID to settlement
        private Dictionary<string, SettlementInfo> settlements;

        /// A Settlement is a group of buildings and people living in the same region.
        [System.Serializable]
        public class SettlementInfo
        {
            /// Map of scene IDs to building info
            public Dictionary<string, BuildingInfo> buildings;
            /// Everyone living in the settlement
            public List<ResidentInfo> residents;

            public SettlementInfo()
            {
                buildings = new Dictionary<string, BuildingInfo>();
                residents = new List<ResidentInfo>();
            }
        }

        [System.Serializable]
        public class ResidentInfo
        {
            public string actorId;
            public string homeScene;
            public string workplaceScene;
        }

        private void Start()
        {
            SceneObjectManager.OnAnySceneLoaded += UpdateBuildingInfo;
        }

        /// Returns the full settlement data for each region.
        public Dictionary<string, SettlementInfo> GetSettlements()
        {
            return new Dictionary<string, SettlementInfo>(settlements);
        }

        /// Initializes the SettlementManager with the given data.
        public void Initialize(Dictionary<string, SettlementInfo> settlements)
        {
            this.settlements = new Dictionary<string, SettlementInfo>(settlements);
            RemoveDeadResidents();
        }

        /// Returns the home scene for the given actor ID in the given region.
        /// Returns null if the actor is not a resident.
        public string GetHomeScene(string actorId, string regionId)
        {
            if (settlements == null) Initialize(new Dictionary<string, SettlementInfo>());
            if (!settlements.ContainsKey(regionId)) return null;

            return (from resident in settlements[regionId].residents
                where resident.actorId == actorId
                select resident.homeScene).FirstOrDefault();
        }

        /// Returns the workplace scene for the given actor ID in the given region.
        /// Returns null if the actor is not a resident or has no workplace.
        public string GetWorkplaceScene(string actorId, string regionId)
        {
            if (settlements == null) Initialize(new Dictionary<string, SettlementInfo>());
            if (!settlements.ContainsKey(regionId)) return null;

            return (from resident in settlements[regionId].residents
                where resident.actorId == actorId
                select resident.workplaceScene).FirstOrDefault();
        }

        /// Returns the building info for the building with the given scene ID in the
        /// given region. Returns null if the building does not exist.
        public BuildingInfo GetBuildingInfo(string sceneId, string regionId)
        {
            if (settlements == null) Initialize(new Dictionary<string, SettlementInfo>());
            return settlements[regionId].buildings.GetValueOrDefault(sceneId);
        }

        /// Registers a resident in the settlement.
        public void AddResident(string actorId, string regionId, string homeScene, string workplaceScene)
        {
            if (settlements == null) Initialize(new Dictionary<string, SettlementInfo>());
            if (!settlements.ContainsKey(regionId))
            {
                settlements.Add(regionId, new SettlementInfo());
            }

            settlements[regionId].residents.Add(new ResidentInfo()
            {
                actorId = actorId,
                homeScene = homeScene,
                workplaceScene = workplaceScene
            });
        }

        /// Returns scene ID for all unoccupied buildings in the given region.
        public List<string> GetUnoccupiedBuildings(string regionId)
        {
            if (settlements == null) Initialize(new Dictionary<string, SettlementInfo>());
            if (!settlements.ContainsKey(regionId)) return new List<string>();

            return (from scene in settlements[regionId].buildings.Keys
                let isOccupied = settlements[regionId].residents.Any(resident => resident.homeScene == scene)
                where !isOccupied
                select scene).ToList();
        }

        /// Checks for any dead residents and removes them from their settlements.
        public void RemoveDeadResidents()
        {
            if (settlements == null) Initialize(new Dictionary<string, SettlementInfo>());

            foreach (SettlementInfo region in settlements.Values)
                (from resident in region.residents
                        where ActorRegistry.Get(resident.actorId).data.Health.IsDead
                        select resident).ToList()
                    .ForEach(resident => region.residents.Remove(resident));
        }

        /// Registers a building as part of the settlement in the given region.
        /// Creates a new settlement if one does not already exist. If the building is
        /// already registered, updates its info.
        private void RegisterBuilding(string regionId, string sceneId, BuildingInfo building)
        {
            if (settlements == null) Initialize(new Dictionary<string, SettlementInfo>());

            if (!settlements.ContainsKey(regionId))
                settlements.Add(regionId, new SettlementInfo());

            settlements[regionId].buildings[sceneId] = building;
        }

        /// Searches for all Buildings in the current scene that have interiors, and
        /// adds them to the current region's settlement or updates their info if they
        /// are already registered.
        private void UpdateBuildingInfo()
        {
            if (settlements == null) Initialize(new Dictionary<string, SettlementInfo>());

            string loadedRegion = ContinentManager.CurrentRegionId;

            foreach (Building building in GameObject.FindObjectsOfType<Building>())
            {
                InteriorSceneCoordinator interiorCoordinator = building.GetComponent<InteriorSceneCoordinator>();
                if (interiorCoordinator == null) continue;

                string interiorSceneId = interiorCoordinator.GetEntrancePortal().DestinationSceneObjectId;
                if (interiorSceneId == null) continue;
                if (!SceneObjectManager.SceneExists(interiorSceneId)) continue;

                RegisterBuilding(loadedRegion, interiorSceneId, building.info);
            }
        }

        public void DebugSettlements()
        {
            Debug.Log("SettlementManager Debug:");
            foreach (string region in settlements.Keys)
            {
                Debug.Log("Region: " + region);
                foreach (ResidentInfo resident in settlements[region].residents)
                {
                    Debug.Log("  Resident: " + resident.actorId);
                    Debug.Log("    Home: " + resident.homeScene);
                    Debug.Log("    Work: " + resident.workplaceScene);
                }
                foreach (string building in settlements[region].buildings.Keys)
                {
                    Debug.Log("  Building: " + building);
                    Debug.Log("    Type: " + settlements[region].buildings[building].type);
                    Debug.Log("    Profession: " + settlements[region].buildings[building].RequiredProfession);
                    Debug.Log("    MaxResidents: " + settlements[region].buildings[building].maxResidents);
                }
            }
        }
    }
}
