using UnityEngine;

namespace Game.Core
{

    [RequireComponent(typeof(RectTransform))]
    [ExecuteAlways]
    public class SafeAreaFitter : MonoBehaviour
    {
        [SerializeField] private Vector2 offsetMin = Vector2.zero;
        [SerializeField] private Vector2 offsetMax = Vector2.zero;

        [SerializeField] private RectTransform rectTransform;
        private Rect lastSafeArea = new Rect(0, 0, 0, 0);
        private Vector2Int lastScreenSize = new Vector2Int(0, 0);

        private void Awake()
        {
            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }
            ApplySafeArea();
        }

        private void OnEnable()
        {
            if (rectTransform == null)
            {

                rectTransform = GetComponent<RectTransform>();
            }

            ApplySafeArea();
        }

        private void Update()
        {
            if (Screen.safeArea != lastSafeArea || Screen.width != lastScreenSize.x || Screen.height != lastScreenSize.y)
            {
                ApplySafeArea();
            }
        }
        
        private void ApplySafeArea()
        {
            Rect safeArea = Screen.safeArea;

            float screenWidth = Mathf.Max(Screen.width, 1f);
            float screenHeight = Mathf.Max(Screen.height, 1f);

            if (safeArea.width <= 0 || safeArea.height <= 0)
            {
                safeArea = new Rect(0, 0, screenWidth, screenHeight);
            }

            Vector2 anchorMin = new Vector2(
                Mathf.Clamp01(safeArea.xMin / screenWidth),
                Mathf.Clamp01(safeArea.yMin / screenHeight)
            );
            Vector2 anchorMax = new Vector2(
                Mathf.Clamp01(safeArea.xMax / screenWidth),
                Mathf.Clamp01(safeArea.yMax / screenHeight)
            );

            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.offsetMin = offsetMin;
            rectTransform.offsetMax = offsetMax;

            lastSafeArea = safeArea;
            lastScreenSize = new Vector2Int(Screen.width, Screen.height);

            Debug.Log("SafeAreaFitter: Safe area applied.");
        }



    }
}

