using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ContinentMaps;
using UnityEngine;

namespace SettlementSystem
{
    // Stores data for the settlement, such as home ownership
    public class SettlementManager : MonoBehaviour
    {
        private HashSet<House> houses;
        public IEnumerable<House> Houses => houses;

        private void Start()
        {
            RegionMapManager.regionLoaded += Clear;
            houses ??= new HashSet<House>();
        }

        public void RegisterHouse(House house)
        {
            houses ??= new HashSet<House>();
            houses.Add(house);
        }

        public void UnregisterHouse(House house)
        {
            houses ??= new HashSet<House>();
            if (houses.Contains(house)) houses.Remove(house);
        }

        public House GetHouse (string owner)
        {
            houses ??= new HashSet<House>();
            return houses.FirstOrDefault(house => house.Owner == owner);
        }

        private void Clear()
        {
            houses = new HashSet<House>();
        }
    }
}