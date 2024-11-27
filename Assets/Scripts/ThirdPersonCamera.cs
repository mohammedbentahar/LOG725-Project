using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform player; // R�f�rence au joueur
    public Vector3 offset; // D�calage de la cam�ra par rapport au joueur
    public float rotationSpeed = 5f; // Vitesse de rotation de la cam�ra

    void Start()
    {
        // D�finir l'offset initial de la cam�ra
        offset = new Vector3(0, 1, -2); // Exemple d'offset : 5 unit�s en hauteur et 10 unit�s derri�re
    }

    void Update()
    {
        // Rotation de la cam�ra
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        transform.RotateAround(player.position, Vector3.up, horizontalInput * rotationSpeed * Time.deltaTime);

        // Maintenir la position de la cam�ra relative au joueur
        transform.position = player.position + offset;
        transform.LookAt(player); // Regarder le joueur en tout temps
    }
}
