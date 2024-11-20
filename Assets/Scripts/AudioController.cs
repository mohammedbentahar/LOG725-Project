#pragma warning disable 0649 // Désactive les avertissements sur les champs non assignés dans l'éditeur
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Contrôleur audio pour gérer la musique de fond, les effets sonores et la fonction "Mute".
/// </summary>
public class AudioController : MonoBehaviour
{
    // Singleton pour garantir qu'il n'y a qu'une seule instance du contrôleur audio
    public static AudioController instance { get; private set; }

    private void Awake()
    {
        // Configure le Singleton avec persistance entre les scènes
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Garde cet objet en vie entre les scènes
        }
        else if (instance != this)
        {
            Destroy(gameObject); // Détruit les doublons
        }
    }

    // Références aux sources audio
    [SerializeField] private AudioSource _musicSource; // Source pour la musique de fond

    // Références pour la gestion des icônes
    [SerializeField] private GameObject _muteIcon;    // GameObject pour l'icône "Mute"
    [SerializeField] private GameObject _unmuteIcon; // GameObject pour l'icône "Unmute"
    private bool _mute; // État actuel du son (activé ou coupé)

    /// <summary>
    /// Basculer l'état "Mute" de la musique et mettre à jour les icônes.
    /// </summary>
    public void Toggle()
    {
        _mute = !_mute; // Inverse l'état "Mute"

        // Régle le volume de la musique (0 si coupé, 1 si activé)
        _musicSource.volume = _mute ? 0f : 1f;

        // Met à jour la visibilité des icônes
        _muteIcon.SetActive(_mute);       // Affiche l'icône "Mute" si le son est coupé
        _unmuteIcon.SetActive(!_mute);    // Affiche l'icône "Unmute" si le son est activé

        Debug.Log("État audio : " + (_mute ? "Coupé" : "Activé")); // Affiche l'état dans la console
    }
}
