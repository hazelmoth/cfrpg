using FeatureGenerators;
using UnityEditor;

namespace ContentLibraries
{
    public static class LibraryBuilder
    {
        [MenuItem("Assets/Build Feature Generator Library")]
        public static void BuildFeatureGeneratorLibrary()
        {
            GenericLibraryBuilder.Build<RegionFeatureGenerator, RegionFeatureLibraryAsset>(
                "Content/Features",
                "Resources/RegionFeatureLibrary.asset");
        }
        
        [MenuItem("Assets/Build Biome Library")]
        public static void BuildBiomeLibrary()
        {
            GenericLibraryBuilder.Build<Biome, BiomeLibraryAsset>(
                "Content/Biomes",
                "Resources/BiomeLibrary.asset");
        }
        
        [MenuItem("Assets/Build Biotope Library")]
        public static void BuildBiotopeLibrary()
        {
            GenericLibraryBuilder.Build<Biotope, BiotopeLibraryAsset>(
                "Content/Biotopes",
                "Resources/BiotopeLibrary.asset");
        }
        
        [MenuItem("Assets/Build Actor Template Library")]
        public static void BuildActorTemplateLibrary()
        {
            GenericLibraryBuilder.Build<AdvancedRandomizedActorTemplate, ActorTemplateLibraryAsset>(
                "Content/Actor Templates",
                "Resources/ActorTemplateLibrary.asset");
        }
        
        [MenuItem("Assets/Build Personality Library")]
        public static void BuildPersonalityLibrary()
        {
            GenericLibraryBuilder.Build<PersonalityData, PersonalityLibraryAsset>(
                "Content/Personalities",
                "Resources/PersonalityLibrary.asset");
        }
    }
}