using UnityEngine;
using UnityEngine.UI;

public class RandomImage : MonoBehaviour
{
    public Sprite[] sprite;
    public Image image;
    
    void Start()
    {
        image = GetComponent<Image>();
        int index = UnityEngine.Random.Range(0, sprite.Length);
        if(image)
        {
            image.sprite = sprite[index];
        }

    }
}
