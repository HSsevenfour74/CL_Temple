using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ConceptGames.ConceptLineOrion.UI
{
	public class AudioVisualization2D : MonoBehaviour
	{
		public AudioSource _audio;

		[Range(64f, 256f)]
		public int _sampleLenght = 256;

		private float[] _samples;

		private List<Image> _uiList = new List<Image>();

		public RectTransform _uiParentRect;

		public GameObject _prefab;

		public float _uiDistance;

		[Range(1f, 30f)]
		public float UpLerp = 12f;

		private void Start()
		{
			CreatUI();
			_samples = new float[_sampleLenght];
		}

		private void CreatUI()
		{
			for (int i = 0; i < _sampleLenght; i++)
			{
				GameObject gameObject = Object.Instantiate(_prefab, _uiParentRect.transform);
				gameObject.name = $"Sample[{i + 1}]";
				_uiList.Add(gameObject.GetComponent<Image>());
				RectTransform component = gameObject.GetComponent<RectTransform>();
				component.localPosition = new Vector3(component.sizeDelta.x + _uiDistance * (float)i, 0f, 0f);
			}
		}

		private void Update()
		{
			_audio.GetSpectrumData(_samples, 0, FFTWindow.BlackmanHarris);
			for (int i = 0; i < _uiList.Count; i++)
			{
				Vector3 localScale = _uiList[i].transform.localScale;
				localScale = new Vector3(1f, Mathf.Clamp(_samples[i] * (50f + (float)(i * i) * 0.5f), 0f, 50f), 1f);
				_uiList[i].transform.localScale = Vector3.Lerp(_uiList[i].transform.localScale, localScale, Time.deltaTime * UpLerp);
			}
		}
	}
}