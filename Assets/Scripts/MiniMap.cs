using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    public Camera miniMapCamera; // Référence à la caméra de la mini-carte
    public RawImage miniMapImage; // Image pour afficher la mini-carte
    public RectTransform miniMapRect; // Pour le positionnement de la mini-carte à l'écran

    void Start()
    {
        // Assurez-vous que la caméra de la mini-carte est bien définie
        miniMapCamera.targetTexture = new RenderTexture(256, 256, 16); // Créer une texture pour la mini-carte
        miniMapImage.texture = miniMapCamera.targetTexture; // Assigner la texture à l'image de la mini-carte

        // Ajuster la position et la taille de la mini-carte
        miniMapRect.anchorMin = new Vector2(1, 0); // Ancrer en bas à droite
        miniMapRect.anchorMax = new Vector2(1, 0);
        miniMapRect.pivot = new Vector2(1, 0);
        miniMapRect.sizeDelta = new Vector2(200, 200); // Taille de la mini-carte
    }
}
