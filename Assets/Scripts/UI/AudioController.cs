#pragma warning disable 0649
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Manages audio playback for music and sound effects, with a toggleable mute feature.
    /// </summary>
    public class AudioController : MonoBehaviour
    {
        #region Singleton

        public static AudioController Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        #endregion

        #region Serialized Fields

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("Mute Controls")]
        [SerializeField] private Image muteImage;
        [SerializeField] private Sprite onSprite;
        [SerializeField] private Sprite offSprite;

        #endregion

        #region Private Fields

        private bool isMuted;

        #endregion

        #region Public Methods

        /// <summary>
        /// Plays a sound effect with optional volume and pitch randomization.
        /// </summary>
        /// <param name="clip">The audio clip to play.</param>
        /// <param name="volume">The volume to play the clip at. Default is 1.</param>
        public void Play(AudioClip clip, float volume = 1f)
        {
            if (clip == null || sfxSource == null) return;

            sfxSource.pitch = Random.Range(0.95f, 1.05f);
            sfxSource.volume = volume;
            sfxSource.PlayOneShot(clip);
        }

        /// <summary>
        /// Toggles the mute state for music and updates the mute button sprite.
        /// </summary>
        public void ToggleMute()
        {
            if (muteImage == null) return;

            isMuted = !isMuted;
            UpdateMuteState();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the mute state for music and the mute button UI.
        /// </summary>
        private void UpdateMuteState()
        {
            if (musicSource != null)
            {
                musicSource.volume = isMuted ? 0f : 1f;
            }

            if (muteImage != null)
            {
                muteImage.sprite = isMuted ? offSprite : onSprite;
            }
        }

        #endregion
    }
}
