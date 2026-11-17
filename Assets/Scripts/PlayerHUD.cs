using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerInteraction playerInteraction;

    [Header("Player Bars")]
    [SerializeField] private Image healthFill;
    [SerializeField] private Image staminaFill;

    [Header("Search UI")]
    [SerializeField] private GameObject searchPrompt;
    [SerializeField] private GameObject searchProgressBackground;
    [SerializeField] private Image searchProgressFill;


    private void Start()
    {
        UpdateHealthBar();

        if (staminaFill != null)
        {
            staminaFill.fillAmount = 1f;
        }

        if (searchPrompt != null)
        {
            searchPrompt.SetActive(false);
        }

        if (searchProgressBackground != null)
        {
            searchProgressBackground.SetActive(false);
        }
    }


    private void Update()
    {
        UpdateHealthBar();
        UpdateSearchUI();
    }


    private void UpdateHealthBar()
    {
        if (playerHealth == null ||
            healthFill == null)
        {
            return;
        }

        if (playerHealth.MaxHealth <= 0f)
        {
            healthFill.fillAmount = 0f;
            return;
        }

        healthFill.fillAmount =
            playerHealth.CurrentHealth /
            playerHealth.MaxHealth;
    }


    private void UpdateSearchUI()
    {
        if (playerInteraction == null)
        {
            return;
        }

        bool canSearch =
            playerInteraction.CanSearch;

        bool isSearching =
            playerInteraction.IsSearching;

        if (searchPrompt != null)
        {
            searchPrompt.SetActive(canSearch);
        }

        if (searchProgressBackground != null)
        {
            searchProgressBackground.SetActive(
                isSearching
            );
        }

        if (searchProgressFill != null)
        {
            if (isSearching)
            {
                searchProgressFill.fillAmount =
                    playerInteraction
                        .SearchProgressNormalized;
            }
            else
            {
                searchProgressFill.fillAmount = 0f;
            }
        }
    }
}