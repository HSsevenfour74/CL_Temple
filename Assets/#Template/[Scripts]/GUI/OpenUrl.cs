using UnityEngine;

namespace ConceptGames.ConceptLineOrion.UI
{
	[DisallowMultipleComponent]
	public class OpenUrl : MonoBehaviour
	{
		public void OnClick(string url)
		{
			Application.OpenURL(url);
		}
	}
}
