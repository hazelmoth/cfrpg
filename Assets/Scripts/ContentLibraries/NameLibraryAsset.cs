using UnityEngine;

namespace ContentLibraries
{
	[CreateAssetMenu(menuName = "Content Libraries/Name Library")]
	public class NameLibraryAsset : ScriptableObject
	{
		public TextAsset maleFirstJson;
		public TextAsset femaleFirstJson;
		public TextAsset lastJson;
	}
}
