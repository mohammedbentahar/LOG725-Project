#pragma warning disable 0649 // D�sactive les avertissements sur les champs non assign�s dans l'�diteur
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Contr�leur audio pour g�rer la musique de fond, les effets sonores et la fonction "Mute".
/// </summary>
public class AudioController : MonoBehaviour
{
    // Singleton pour garantir qu'il n'y a qu'une seule instance du contr�leur audio
    public static AudioController instance { get; private set; }

    private void Awake()
    {
        // Configure le Singleton avec persistance entre les sc�nes
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Garde cet objet en vie entre les sc�nes
        }
        else if (instance != this)
        {
            Destroy(gameObject); // D�truit les doublons
        }
    }

    // R�f�rences aux sources audio
    [SerializeField] private AudioSource _musicSource; // Source pour la musique de fond

    // R�f�rences pour la gestion des ic�nes
    [SerializeField] private GameObject _muteIcon;    // GameObject pour l'ic�ne "Mute"
    [SerializeField] private GameObject _unmuteIcon; // GameObject pour l'ic�ne "Unmute"
    private bool _mute; // �tat actuel du son (activ� ou coup�)

    /// <summary>
    /// Basculer l'�tat "Mute" de la musique et mettre � jour les ic�nes.
    /// </summary>
    public void Toggle()
    {
        _mute = !_mute; // Inverse l'�tat "Mute"

        // R�gle le volume de la musique (0 si coup�, 1 si activ�)
        _musicSource.volume = _mute ? 0f : 1f;

        // Met � jour la visibilit� des ic�nes
        _muteIcon.SetActive(_mute);       // Affiche l'ic�ne "Mute" si le son est coup�
        _unmuteIcon.SetActive(!_mute);    // Affiche l'ic�ne "Unmute" si le son est activ�

        Debug.Log("�tat audio : " + (_mute ? "Coup�" : "Activ�")); // Affiche l'�tat dans la console
    }
}
