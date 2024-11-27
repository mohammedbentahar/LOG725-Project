using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform player; // Référence au joueur
    public Vector3 offset; // Décalage de la caméra par rapport au joueur
    public float rotationSpeed = 5f; // Vitesse de rotation de la caméra

    void Start()
    {
        // Définir l'offset initial de la caméra
        offset = new Vector3(0, 1, -2); // Exemple d'offset : 5 unités en hauteur et 10 unités derrière
    }

    void Update()
    {
        // Rotation de la caméra
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        transform.RotateAround(player.position, Vector3.up, horizontalInput * rotationSpeed * Time.deltaTime);

        // Maintenir la position de la caméra relative au joueur
        transform.position = player.position + offset;
        transform.LookAt(player); // Regarder le joueur en tout temps
    }
}
