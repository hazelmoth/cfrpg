using UnityEngine;

public static class MousePositionHelper
{
	public static float AngleToMouse(Vector2 startPos)
	{
		Vector2 endPos = GetMouseWorldPos();
		Vector2 diff = endPos - startPos;
		return Mathf.Rad2Deg * Mathf.Atan2(diff.y, diff.x);
	}

	public static Vector2 GetMouseWorldPos()
	{
		Vector2 inputPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
		return inputPos;
	}
}
