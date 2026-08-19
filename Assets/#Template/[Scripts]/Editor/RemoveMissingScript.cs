using UnityEditor;
using UnityEngine;

public class RemoveMissingScripts : EditorWindow
{

	[MenuItem("Tools/Remove Missing Scripts")]
	static void Apply()
	{
		GameObject[] gameObjects = FindObjectsOfType<GameObject>(true);
		foreach (GameObject go in gameObjects)
			GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
	}

}