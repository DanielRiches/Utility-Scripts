using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private Image[] images;
    [SerializeField] private Color32 onColor = Colors.red;
    [SerializeField] private Color32 offColor = Colors.white;
    public void UIMouseOver()
    {
        if (images != null)
        {
            foreach (Image image in images)
            {
                if (image)
                {
                    Utils.TintUI(image, onColor);
                }
            }
        }       
    }
    public void UIMouseExit()
    {
        if (images != null)
        {
            foreach (Image image in images)
            {
                if (image)
                {
                    Utils.TintUI(image, offColor);
                }
            }
        }      
    }
}
