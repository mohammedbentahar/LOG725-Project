using UnityEngine;
using TMPro;

public class DifficultyManager : MonoBehaviour
{
    public TMP_Dropdown DifficultyDropdown;

    // Variable statique pour conserver le niveau de difficulté
    public static int SelectedDifficulty = 0;

    private void Start()
    {
        // Charge la valeur sauvegardée (utile si vous revenez au menu)
        DifficultyDropdown.value = SelectedDifficulty;

        // Ajoute un listener pour sauvegarder le niveau sélectionné
        DifficultyDropdown.onValueChanged.AddListener(OnDifficultyChanged);
    }

    private void OnDifficultyChanged(int difficultyIndex)
    {
        SelectedDifficulty = difficultyIndex;
        Debug.Log("Difficulté sélectionnée : " + difficultyIndex);
    }
}
