using SimpleJSON;

public static class JSONHelper {

	public static int GetElementCount (JSONNode json) {
		int count = 0;
		JSONNode.ValueEnumerator valEnum = json.Values;
		while (valEnum.MoveNext())
			count++;
		return count;
	}
	
}
