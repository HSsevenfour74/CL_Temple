using ConceptGames.ConceptLineOrion.Level;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SoundEffectSettting : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IPointerUpHandler
{
    private float value = 2;
    public Slider Slider;
    public Text text;
    public AudioClip stopClip;
    private void Start()
    {
        value = PlayerPrefs.GetFloat("Game_Sound_Effect");
        text.text = value.ToString("F2");
        Slider.value = value;   
    }

    // ÍÏ×§¿ªÊ¼
    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DoSliderEnd();
        EventSystem.current.SetSelectedGameObject(null);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        DoSliderEnd();
        EventSystem.current.SetSelectedGameObject(null);
    }

    void DoSliderEnd()
    {
        float val = Slider.value;
        text.text = val.ToString("F2");
        PlayerPrefs.SetFloat("Game_Sound_Effect", val);
        AudioManager.PlayClip(stopClip);
    }

}
