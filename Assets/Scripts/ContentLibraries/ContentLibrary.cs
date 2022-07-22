using Crafting;

namespace ContentLibraries
{
	/// Loads and stores references to all the content library assets in the game.
	public class ContentLibrary
	{
		private static ContentLibrary instance;
		public static ContentLibrary Instance
		{
			get { return instance ??= new ContentLibrary(); }
		}

		public static bool Loaded { get; private set; }

		public GenericContentLibrary<AdvancedRandomizedActorTemplate> ActorTemplates { get; private set; }
		public GenericContentLibrary<Biome> Biomes { get; private set; }
		public GenericContentLibrary<Biotope> Biotopes { get; private set; }
		public ContainerLayoutElementPrefabLibrary ContainerLayoutElementPrefabs { get; private set; }
		public GenericContentLibrary<RecipeList> CraftingRecipes { get; private set; }
		public GenericContentLibrary<EntityData> Entities { get; private set; }
		public GenericContentLibrary<GroundMaterial> GroundMaterials { get; private set; }
		public GenericContentLibrary<Hair> Hairs { get; private set; }
		public ItemLibrary Items { get; private set; }
		public NameLibrary Names { get; private set; }
		public PersonalityLibrary Personalities { get; private set; }
		public RaceLibrary Races { get; private set; }
		public RegionFeatureLibrary RegionFeatures { get; private set; }

		public void LoadAllLibraries ()
		{
			ActorTemplates = new GenericContentLibrary<AdvancedRandomizedActorTemplate>("ActorTemplateLibrary");
			Biomes = new GenericContentLibrary<Biome>("BiomeLibrary");
			Biotopes = new GenericContentLibrary<Biotope>("BiotopeLibrary");
			CraftingRecipes = new GenericContentLibrary<RecipeList>("CraftingRecipeLibrary");
			ContainerLayoutElementPrefabs = new ContainerLayoutElementPrefabLibrary();
			Entities = new GenericContentLibrary<EntityData>("EntityLibrary");
			GroundMaterials = new GenericContentLibrary<GroundMaterial>("GroundMaterialLibrary");
			Hairs = new GenericContentLibrary<Hair>("HairLibrary");
			Items = new ItemLibrary();
			Names = new NameLibrary();
			Personalities = new PersonalityLibrary();
			Races = new RaceLibrary();
			RegionFeatures = new RegionFeatureLibrary();

			ActorTemplates.LoadLibrary();
			Biomes.LoadLibrary();
			Biotopes.LoadLibrary();
			CraftingRecipes.LoadLibrary();
			ContainerLayoutElementPrefabs.LoadLibrary();
			Entities.LoadLibrary();
			GroundMaterials.LoadLibrary();
			Hairs.LoadLibrary();
			Items.LoadLibrary();
			Names.LoadLibrary();
			Personalities.LoadLibrary();
			Races.LoadLibrary();
			RegionFeatures.LoadLibrary();

			Loaded = true;
		}
	}
}
