using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private Image _healthbarSprite;
    [SerializeField] private float reduceSpeed = 2;

    private float target = 1;
    private Camera cam;

    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        _healthbarSprite.fillAmount = currentHealth / maxHealth;
    }

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
        //_healthbarSprite.fillAmount = Mathf.MoveTowards(_healthbarSprite.fillAmount, target, reduceSpeed * Time.deltaTime);
    }
}
