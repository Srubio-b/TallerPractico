using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public GameObject weaponPrefab; // El modelo del arma
    public Transform weaponHolder; // Donde se mostrará el arma equipada
    public float pickupRange = 3f;
    public KeyCode pickupKey = KeyCode.E;

    [Header("Weapon Position")]
    public Vector3 weaponPosition = new Vector3(0.5f, -0.3f, 0.5f);
    public Vector3 weaponRotation = new Vector3(0f, 0f, 0f);
    public Vector3 weaponScale = new Vector3(1f, 1f, 1f);

    [Header("UI")]
    public GameObject pickupUI; // Texto "Presiona E para recoger"

    private Camera playerCamera;
    private GameObject currentWeaponObject;
    private bool hasWeapon = false;

    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();

        if (pickupUI != null)
        {
            pickupUI.SetActive(false);
        }
    }

    void Update()
    {
        CheckForWeapon();

        if (Input.GetKeyDown(pickupKey) && !hasWeapon)
        {
            TryPickupWeapon();
        }

        // Soltar arma (opcional)
        if (Input.GetKeyDown(KeyCode.G) && hasWeapon)
        {
            DropWeapon();
        }
    }

    void CheckForWeapon()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange))
        {
            // Verificar si el objeto tiene el tag "Weapon"
            if (hit.collider.CompareTag("Weapon") && !hasWeapon)
            {
                if (pickupUI != null)
                {
                    pickupUI.SetActive(true);
                }
            }
            else
            {
                if (pickupUI != null)
                {
                    pickupUI.SetActive(false);
                }
            }
        }
        else
        {
            if (pickupUI != null)
            {
                pickupUI.SetActive(false);
            }
        }
    }

    void TryPickupWeapon()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange))
        {
            if (hit.collider.CompareTag("Weapon"))
            {
                // Destruir el arma del suelo
                Destroy(hit.collider.gameObject);

                // Crear el arma en la posición del jugador
                EquipWeapon();
            }
        }
    }

    void EquipWeapon()
    {
        if (weaponPrefab != null && weaponHolder != null)
        {
            // Instanciar el arma
            currentWeaponObject = Instantiate(weaponPrefab, weaponHolder);

            // Posicionar el arma
            currentWeaponObject.transform.localPosition = weaponPosition;
            currentWeaponObject.transform.localRotation = Quaternion.Euler(weaponRotation);
            currentWeaponObject.transform.localScale = weaponScale;

            // Desactivar colisiones del arma equipada
            Collider weaponCollider = currentWeaponObject.GetComponent<Collider>();
            if (weaponCollider != null)
            {
                weaponCollider.enabled = false;
            }

            // Desactivar Rigidbody si tiene
            Rigidbody weaponRb = currentWeaponObject.GetComponent<Rigidbody>();
            if (weaponRb != null)
            {
                weaponRb.isKinematic = true;
            }

            hasWeapon = true;

            if (pickupUI != null)
            {
                pickupUI.SetActive(false);
            }

            Debug.Log("Arma equipada!");
        }
    }

    void DropWeapon()
    {
        if (currentWeaponObject != null)
        {
            // Crear el arma en el mundo
            Vector3 dropPosition = playerCamera.transform.position + playerCamera.transform.forward * 2f;
            GameObject droppedWeapon = Instantiate(weaponPrefab, dropPosition, Quaternion.identity);

            // Añadir tag
            droppedWeapon.tag = "Weapon";

            // Asegurar que tenga collider
            if (droppedWeapon.GetComponent<Collider>() == null)
            {
                droppedWeapon.AddComponent<BoxCollider>();
            }

            // Destruir el arma equipada
            Destroy(currentWeaponObject);
            hasWeapon = false;

            Debug.Log("Arma soltada!");
        }
    }
}