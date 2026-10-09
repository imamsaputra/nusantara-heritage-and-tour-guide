using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Nusantara
{
    /// Animasi intro + navigasi tombol untuk Splash Screen Nusantara Heritage & Tour Guide.
    public class SplashController : MonoBehaviour
    {
        [Header("Layer (diisi otomatis oleh builder)")]
        public RectTransform sunGlow, clouds, palmLeft, palmRight, logo, title, btnStart, btnGuide;
        public CanvasGroup fade;

        [Header("Navigasi")]
        public string startScene = "MainMenu";
        public string guideScene = "GuideMenu";

        [Header("Animasi")]
        public float cloudDrift = 45f, cloudSpeed = 0.25f;
        public float palmSwayAngle = 1.6f, palmSwaySpeed = 0.9f;
        public float sunPulse = 0.04f, buttonPulse = 0.03f;

        CanvasGroup cgLogo, cgTitle, cgStart, cgGuide;
        Vector2 cloudBase, titleBase;
        bool ready, loading;

        void Awake()
        {
            cgLogo = Group(logo); cgTitle = Group(title); cgStart = Group(btnStart); cgGuide = Group(btnGuide);
            if (clouds) cloudBase = clouds.anchoredPosition;
            if (title) titleBase = title.anchoredPosition;
            Wire(btnStart, () => Go(startScene));
            Wire(btnGuide, () => Go(guideScene));
        }

        void Start() { StartCoroutine(Intro()); }

        CanvasGroup Group(RectTransform r)
        {
            if (!r) return null;
            var g = r.GetComponent<CanvasGroup>();
            if (!g) g = r.gameObject.AddComponent<CanvasGroup>();
            g.alpha = 0f; return g;
        }

        void Wire(RectTransform r, UnityEngine.Events.UnityAction a)
        {
            if (!r) return;
            var b = r.GetComponent<Button>();
            if (b) b.onClick.AddListener(a);
        }

        IEnumerator Intro()
        {
            if (fade) { fade.alpha = 1f; yield return Tween(1.0f, t => fade.alpha = 1f - t); }
            yield return Tween(0.8f, t => { logo.localScale = Vector3.one * EaseOutBack(t); cgLogo.alpha = Mathf.Clamp01(t * 2f); });
            yield return Tween(0.7f, t =>
            {
                cgTitle.alpha = t;
                title.anchoredPosition = titleBase + new Vector2(0, -60f * (1f - EaseOut(t)));
            });
            yield return Tween(0.5f, t => cgStart.alpha = t);
            yield return Tween(0.5f, t => cgGuide.alpha = t);
            ready = true;
        }

        void Update()
        {
            float t = Time.time;
            if (clouds) clouds.anchoredPosition = cloudBase + new Vector2(Mathf.Sin(t * cloudSpeed) * cloudDrift, 0);
            if (palmLeft) palmLeft.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(t * palmSwaySpeed) * palmSwayAngle);
            if (palmRight) palmRight.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(t * palmSwaySpeed + 1.7f) * -palmSwayAngle);
            if (sunGlow) sunGlow.localScale = Vector3.one * (1f + Mathf.Sin(t * 1.2f) * sunPulse);
            if (ready && btnStart) btnStart.localScale = Vector3.one * (1f + Mathf.Sin(t * 3f) * buttonPulse);
        }

        void Go(string scene)
        {
            if (loading) return;
            if (!Application.CanStreamedLevelBeLoaded(scene))
            { Debug.LogWarning("[Nusantara] Scene '" + scene + "' belum ada di Build Settings."); return; }
            loading = true; StartCoroutine(Outro(scene));
        }

        IEnumerator Outro(string scene)
        {
            if (fade) yield return Tween(0.5f, t => fade.alpha = t);
            SceneManager.LoadScene(scene);
        }

        static IEnumerator Tween(float dur, System.Action<float> step)
        {
            for (float e = 0; e < dur; e += Time.unscaledDeltaTime) { step(e / dur); yield return null; }
            step(1f);
        }
        static float EaseOut(float t) { return 1f - (1f - t) * (1f - t); }
        static float EaseOutBack(float t) { const float c = 1.70158f; t -= 1f; return 1f + (c + 1f) * t * t * t + c * t * t; }
    }
}
