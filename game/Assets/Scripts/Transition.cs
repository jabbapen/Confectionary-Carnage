using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Animates menus.
/// </summary>
public class Transition : MonoBehaviour
{
    public static Transition Instance = null;

    [SerializeField]
    private GameObject backgroundPanel; // The background panel to animate.

    [SerializeField]
    private float transitionDuration = 1f; // Time it takes to show/hide the screen.

    private RectTransform panelRectTransform;
    private Vector2 screenSize;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Ensure the backgroundPanel has a RectTransform component
        if (backgroundPanel != null)
        {
            panelRectTransform = backgroundPanel.GetComponent<RectTransform>();
        }

        if (panelRectTransform == null)
        {
            Debug.LogError("BackgroundPanel must have a RectTransform component.");
            enabled = false;
            return;
        }

        // Get the screen size based on the Canvas scaler (assuming 1920x1080 reference)
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null || canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            Debug.LogError("Transition script requires a Canvas set to ScreenSpaceOverlay.");
            enabled = false;
            return;
        }

        screenSize = new Vector2(Screen.width, Screen.height);
        DontDestroyOnLoad(gameObject);
    }

    public void ShowScreen()
    {
        StopAllCoroutines();
        StartCoroutine(SlidePanel(Vector2.zero));
    }


    public void HideScreen()
    {
        StopAllCoroutines();
        StartCoroutine(SlidePanel(new Vector2(0, 1200)));
    }

    /// <summary>
    /// Slides the panel to the target position over the transition duration.
    /// </summary>
    private IEnumerator SlidePanel(Vector2 targetPosition)
    {
        Vector2 startPosition = panelRectTransform.anchoredPosition;
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / transitionDuration);

            // Smooth transition
            panelRectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);

            yield return null;
        }

        // Ensure the final position is set
        panelRectTransform.anchoredPosition = targetPosition;
    }
}
