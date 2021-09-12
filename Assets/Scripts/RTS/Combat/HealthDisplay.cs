using UnityEngine;
using UnityEngine.UI;

namespace RTS.Combat
{
    public class HealthDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject healthBarParent;
        [SerializeField] private Image healthBarImage;
        
        private Health _health;

        private void Awake()
        {
            _health = GetComponent<Health>();
            _health.ClientOnHealthUpdated += HandleHealthUpdated;
        }

        private void OnDestroy()
        {
            _health.ClientOnHealthUpdated -= HandleHealthUpdated;
        }

        private void OnMouseEnter()
        {
            healthBarParent.SetActive(true);
        }

        private void OnMouseExit()
        {
            healthBarParent.SetActive(false);
        }

        private void HandleHealthUpdated(int currentHealth, int maxHealth)
        {
            healthBarImage.fillAmount = (float)currentHealth / maxHealth;
            healthBarParent.SetActive(true);
        }
    }
}