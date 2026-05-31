using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Game.Runtime.Background
{

    public class BackgroundView : MonoBehaviour
    {
        [SerializeField] private RectMask2D mask;
        [SerializeField] private float duration = 0.5f;

        private Tween tween;


        public void Show()
        {
            if (tween != null && tween.IsActive())
                tween.Kill();

            mask.softness = new Vector2Int(10000, 10000);

            tween = DOTween.To(
                () => new Vector2(mask.softness.x, mask.softness.y),
                x => mask.softness = new Vector2Int((int)x.x, (int)x.y),
                new Vector2(0, 0),
                duration
            ).SetEase(Ease.OutQuad);
        }


        public void Hide()
        {
            if (tween != null && tween.IsActive())
                tween.Kill();
            tween = DOTween.To(
                () => new Vector2(mask.softness.x, mask.softness.y),
                x => mask.softness = new Vector2Int((int)x.x, (int)x.y),
                new Vector2(10000, 10000),
                duration
            ).SetEase(Ease.InQuad);
        }

    }
}