using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace ConceptGames.ConceptLineOrion.UI
{
	public class CanvasFade : MonoBehaviour
	{
		[SerializeField]
		private List<CanvasGroup> fadeIn;

		[SerializeField]
		private List<CanvasGroup> fadeOut;

		[SerializeField]
		private float fadeDuration;

		[SerializeField]
		private UnityEvent onFadeIn = new UnityEvent();

		[SerializeField]
		private UnityEvent onFadeOut = new UnityEvent();

		private List<Tween> fadeInTweens = new List<Tween>();

		private List<Tween> fadeOutTweens = new List<Tween>();

		private void Start()
		{
			fadeInTweens.Clear();
			fadeOutTweens.Clear();
		}

		public void Fade()
		{
			if (fadeInTweens != null)
			{
				foreach (Tween fadeInTween in fadeInTweens)
				{
					fadeInTween.Kill();
				}
				fadeInTweens.Clear();
			}
			if (fadeOutTweens != null)
			{
				foreach (Tween fadeOutTween in fadeOutTweens)
				{
					fadeOutTween.Kill();
				}
			}
			onFadeIn.Invoke();
			foreach (CanvasGroup item in fadeIn)
			{
				item.alpha = 0f;
				item.gameObject.SetActive(value: true);
				fadeInTweens.Add(item.DOFade(1f, fadeDuration).SetAutoKill(autoKillOnCompletion: true));
			}
			foreach (CanvasGroup c in fadeOut)
			{
				c.alpha = 1f;
				fadeOutTweens.Add(c.DOFade(0f, fadeDuration).SetAutoKill(autoKillOnCompletion: true).OnComplete(delegate
				{
					c.gameObject.SetActive(value: false);
					onFadeOut.Invoke();
				}));
			}
		}
	}
}
