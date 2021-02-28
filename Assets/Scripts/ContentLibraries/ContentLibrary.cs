
// Loads and stores references to all the content libraries in the game.
public class ContentLibrary
{
	private static ContentLibrary instance;
	public static ContentLibrary Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new ContentLibrary();
			}
			return instance;
		}
	}

	public static bool Loaded { get; private set; }

	public BiotopeLibrary Biotopes { get; private set; }
	public CharacterGenTemplateLibrary CharacterGenTemplates { get; private set; }
	public ContainerLayoutElementPrefabLibrary ContainerLayoutElementPrefabs { get; private set; }
	public EntityLibrary Entities { get; private set; }
	public GroundMaterialLibrary GroundMaterials { get; private set; }
	public HairLibrary Hairs { get; private set; }
	public ItemLibrary Items { get; private set; }
	public NameLibrary Names { get; private set; }
	public PersonalityLibrary Personalities { get; private set; }
	public RaceLibrary Races { get; private set; }

	public void LoadAllLibraries ()
	{
		Biotopes = new BiotopeLibrary();
		CharacterGenTemplates = new CharacterGenTemplateLibrary();
		ContainerLayoutElementPrefabs = new ContainerLayoutElementPrefabLibrary();
		Entities = new EntityLibrary();
		GroundMaterials = new GroundMaterialLibrary();
		Hairs = new HairLibrary();
		Items = new ItemLibrary();
		Names = new NameLibrary();
		Personalities = new PersonalityLibrary();
		Races = new RaceLibrary();

		Biotopes.LoadLibrary();
		CharacterGenTemplates.LoadLibrary();
		ContainerLayoutElementPrefabs.LoadLibrary();
		Entities.LoadLibrary();
		GroundMaterials.LoadLibrary();
		Hairs.LoadLibrary();
		Items.LoadLibrary();
		Names.LoadLibrary();
		Personalities.LoadLibrary();
		Races.LoadLibrary();

		Loaded = true;
	}
}