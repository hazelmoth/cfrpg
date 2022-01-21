using MyBox;
using UnityEngine;

namespace SettlementSystem
{
    [System.Serializable]
    public class BuildingInfo
    {
        /// Whether this building is a dwelling, workplace, or both.
        [SerializeField]
        public Type type;

        /// How many actors can live in this building.
        [ConditionalField(nameof(type), false, Type.Dwelling, Type.Hybrid), SerializeField]
        public int maxResidents = 1;

        /// The required profession for working in this building, if it is a workplace.
        [ConditionalField(nameof(type), false, Type.Workplace, Type.Hybrid), SerializeField]
        private string requiredProfession;

        public enum Type
        {
            Dwelling,
            Workplace,
            Hybrid
        }

        /// The required profession for working in this building, if it is a workplace.
        public string RequiredProfession
        {
            get => requiredProfession?.Trim() == "" ? null : requiredProfession;
            set => requiredProfession = value;
        }
    }
}
