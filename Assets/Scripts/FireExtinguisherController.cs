using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;  // для работы с InputAction

public class FireExtinguisherController : MonoBehaviour
{
    [Header("Components")]
    public ParticleSystem extinguisherParticles;
    public AudioSource extinguisherSound;
    
    [Header("Input")]
    [SerializeField] private float activationThreshold = 0.5f;
    [Range(0, 1)] public float hapticIntensity = 0.3f;
    [Range(0, 1)] public float hapticDuration = 0.1f;

    // Добавляем InputAction для получения направления джойстика
    [Header("Joystick Input")]
    public InputActionProperty joystickDirection;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private ActionBasedController actionController;
    private bool isHeld = false;

    void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    void Update()
    {
        if (!isHeld) return;

        HandleInput();
        HandleJoystickRotation();
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        // Получаем компонент ActionBasedController, который содержит привязки Input Action
        actionController = args.interactorObject.transform.GetComponent<ActionBasedController>();
        isHeld = true;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isHeld = false;
        actionController = null;
        StopExtinguisher();
    }

    private void HandleInput()
    {
        if (actionController == null) return;

        // Читаем значение привязанного действия активации
        float activateValue = actionController.activateAction.action.ReadValue<float>();

        if (activateValue > activationThreshold)
        {
            StartExtinguisher();
        }
        else
        {
            StopExtinguisher();
        }
    }

    // Новый метод для поворота огнетушителя по направлению джойстика
    private void HandleJoystickRotation()
    {
        // Проверяем, назначено ли действие и включено ли оно
        if (joystickDirection == null || joystickDirection.action == null)
            return;

        // Считываем значение Vector2 с джойстика
        Vector2 input = joystickDirection.action.ReadValue<Vector2>();

        // Если джойстик немного отклонён (можно настроить порог)
        if (input.magnitude >= 0.1f)
        {
            // Вычисляем угол в градусах.
            // Mathf.Atan2 принимает сначала значение по оси X, затем по оси Y.
            float angle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg;

            // Применяем поворот вокруг оси Y (можно скорректировать, если нужна другая ось)
            transform.localRotation = Quaternion.Euler(0, angle, 0);
        }
    }

    private void StartExtinguisher()
    {
        if (!extinguisherParticles.isPlaying)
        {
            extinguisherParticles.Play();
            extinguisherSound.Play();
            TriggerHaptic();
        }
    }

    private void StopExtinguisher()
    {
        if (extinguisherParticles.isPlaying)
        {
            extinguisherParticles.Stop();
            extinguisherSound.Stop();
        }
    }

    private void TriggerHaptic()
    {
        if (actionController != null)
        {
            actionController.SendHapticImpulse(hapticIntensity, hapticDuration);
        }
    }

    void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }
}
