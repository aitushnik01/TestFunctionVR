using UnityEngine;

public class FireController : MonoBehaviour
{
    [Header("Fire Settings")]
    public float extinguishSpeed = 0.5f;
    public float minIntensity = 0.1f;

    [Header("Fire Components")]
    public ParticleSystem fireParticles;
    public Light fireLight;
    
    // Отдельные переменные для начальных значений
    private float initialParticleSize;
    private float initialLightIntensity;
    private Vector3 initialScale;
    
    private float currentIntensity = 1f;
    private bool isExtinguished = false;

    void Start()
    {
        // Сохраняем начальные значения для каждого компонента отдельно
        if (fireParticles != null)
        {
            initialParticleSize = fireParticles.main.startSize.constant;
        }
        
        if (fireLight != null)
        {
            initialLightIntensity = fireLight.intensity;
        }
        
        initialScale = transform.localScale;
    }

    void OnParticleCollision(GameObject other)
    {
        if (!isExtinguished && other.CompareTag("FireExtinguisher"))
        {
            ReduceFireIntensity();
        }
    }

    void ReduceFireIntensity()
    {
        currentIntensity = Mathf.Clamp(
            currentIntensity - extinguishSpeed * Time.deltaTime, 
            minIntensity, 
            1f
        );

        UpdateFireProperties(currentIntensity);

        if (currentIntensity <= minIntensity)
        {
            isExtinguished = true;
            fireParticles?.Stop();
        }
    }
// Добавьте в существующий FireController
public void Extinguish(float amount)
{
    currentIntensity = Mathf.Clamp(currentIntensity - amount, minIntensity, 1f);
    UpdateFireProperties(currentIntensity);
    
    if (currentIntensity <= minIntensity)
    {
        isExtinguished = true;
        fireParticles?.Stop();
    }
}
    void UpdateFireProperties(float intensity)
    {
        // Обновляем частицы
        if (fireParticles != null)
        {
            var main = fireParticles.main;
            main.startSize = initialParticleSize * intensity;
        }

        // Обновляем свет
        if (fireLight != null)
        {
            fireLight.intensity = initialLightIntensity * intensity;
        }

        // Обновляем масштаб
        transform.localScale = initialScale * intensity;
    }
}