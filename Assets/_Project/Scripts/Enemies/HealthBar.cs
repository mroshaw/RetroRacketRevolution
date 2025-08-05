using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;

namespace DaftAppleGames.RetroRacketRevolution.Enemies
{
    public class HealthBar : MonoBehaviour
    {
        // Public serializable properties
        [BoxGroup("UI Settings")] public Slider healthSlider;

        [BoxGroup("Health")] public int normalHealth;
        [BoxGroup("Health")] public Color normalHealthColor = Color.green;
        [BoxGroup("Health")] public int warningHealth;
        [BoxGroup("Health")] public Color warningHealthColor = Color.yellow;
        [BoxGroup("Health")] public int critialHealth;
        [BoxGroup("Health")] public Color criticalHealthColor = Color.red;

        private Image _healthImage;

        /// <summary>
        /// Initialise this component
        /// </summary>   
        private void Awake()
        {
            _healthImage = healthSlider.fillRect.gameObject.GetComponent<Image>();
        }

        /// <summary>
        /// Initialise the health bar
        /// </summary>
        /// <param name="health"></param>
        public void InitHealthBar(int health)
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = health;
            UpdateHealthBar(health);
        }

        /// <summary>
        /// Update the health bar
        /// </summary>
        /// <param name="health"></param>
        public void UpdateHealthBar(int health)
        {
            healthSlider.value = health;
            if (health >= normalHealth)
            {
                _healthImage.color = normalHealthColor;
            }
            else if (health < normalHealth && health > warningHealth)
            {
                _healthImage.color = warningHealthColor;
            }
            else
            {
                _healthImage.color = criticalHealthColor;
            }
        }
    }
}