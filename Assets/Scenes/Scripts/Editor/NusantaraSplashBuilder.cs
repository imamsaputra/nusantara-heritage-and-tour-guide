using System;
using System.IO;
using System.Linq;
using Nusantara;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// Menu: Nusantara > Build Splash Scene -> membuat scene lengkap (Canvas, layer, logo, judul, tombol, animasi).
public static class NusantaraSplashBuilder
{
    const string Dir = "Assets/NusantaraSplash/";
    static readonly Vector2 Ref = new Vector2(1080, 1920);

    [MenuItem("Nusantara/Build Splash Scene")]
    public static void Build()
    {
        AssetDatabase.Refresh();
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        var camGo = new GameObject("Main Camera") { tag = "MainCamera" };
        var cam = camGo.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor; cam.backgroundColor = Color.black;
        camGo.AddComponent<AudioListener>();

        var es = new GameObject("EventSystem", typeof(EventSystem));
        var newInput = Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
        es.AddComponent(newInput ?? typeof(StandaloneInputModule));

        var cvGo = new GameObject("SplashCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        cvGo.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        var sc = cvGo.GetComponent<CanvasScaler>();
        sc.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        sc.referenceResolution = Ref; sc.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight; sc.matchWidthOrHeight = 0.5f;

        // Stage: menjaga rasio 9:16 & menutupi layar (envelope) agar tidak gepeng di semua perangkat.
        var stage = new GameObject("Stage", typeof(RectTransform), typeof(AspectRatioFitter)).GetComponent<RectTransform>();
        stage.SetParent(cvGo.transform, false); Stretch(stage);
        var arf = stage.GetComponent<AspectRatioFitter>();
        arf.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent; arf.aspectRatio = Ref.x / Ref.y;

        Full(stage, "Sky", "01_bg_sky");
        var sun = Full(stage, "SunGlow", "02_sun_glow", new Vector2(540, 1010));
        var clouds = Full(stage, "Clouds", "03_clouds");
        Full(stage, "MountainsFar", "04_mountains_far");
        Full(stage, "Borobudur", "05_borobudur");
        Full(stage, "HillsMid", "06_hills_mid");
        Full(stage, "RiceTerrace", "07_rice_terrace_fore");
        var pl = Full(stage, "PalmLeft", "08_palm_left", new Vector2(110, 1560));
        var pr = Full(stage, "PalmRight", "09_palm_right", new Vector2(970, 1560));

        var top = Anchored(cvGo.transform, "BatikTop", "10_batik_border_top", new Vector2(.5f, 1), new Vector2(.5f, 1), Vector2.zero, new Vector2(1080, 100));
        top.anchorMin = new Vector2(0, 1); top.anchorMax = new Vector2(1, 1); top.pivot = new Vector2(.5f, 1);
        top.sizeDelta = new Vector2(0, 100); top.anchoredPosition = Vector2.zero; top.GetComponent<Image>().preserveAspect = false;
        var bot = Anchored(cvGo.transform, "BatikBottom", "11_batik_border_bottom", new Vector2(.5f, 0), new Vector2(.5f, 0), Vector2.zero, new Vector2(1080, 100));
        bot.anchorMin = new Vector2(0, 0); bot.anchorMax = new Vector2(1, 0); bot.pivot = new Vector2(.5f, 0);
        bot.sizeDelta = new Vector2(0, 100); bot.anchoredPosition = Vector2.zero; bot.GetComponent<Image>().preserveAspect = false;

        var logo = Anchored(cvGo.transform, "Logo", "12_logo_emblem", new Vector2(.5f, 1), new Vector2(.5f, 1), new Vector2(0, -370), new Vector2(560, 560));
        var title = Anchored(cvGo.transform, "Title", "15_title_text", new Vector2(.5f, 1), new Vector2(.5f, 1), new Vector2(0, -850), new Vector2(980, 420));
        var bStart = MakeButton(cvGo.transform, "BtnMulai", "13_btn_mulai", 370);
        var bGuide = MakeButton(cvGo.transform, "BtnPemandu", "14_btn_secondary", 210);

        var fadeGo = new GameObject("FadeOverlay", typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
        fadeGo.transform.SetParent(cvGo.transform, false); Stretch(fadeGo.GetComponent<RectTransform>());
        fadeGo.GetComponent<Image>().color = Color.black;
        var cg = fadeGo.GetComponent<CanvasGroup>(); cg.blocksRaycasts = false; cg.interactable = false;

        var ctrl = cvGo.AddComponent<SplashController>();
        ctrl.sunGlow = sun; ctrl.clouds = clouds; ctrl.palmLeft = pl; ctrl.palmRight = pr;
        ctrl.logo = logo; ctrl.title = title; ctrl.btnStart = bStart; ctrl.btnGuide = bGuide; ctrl.fade = cg;
        logo.localScale = Vector3.zero;

        Directory.CreateDirectory(Dir + "Scenes");
        string path = Dir + "Scenes/SplashScreen.unity";
        EditorSceneManager.SaveScene(scene, path);
        var list = EditorBuildSettings.scenes.ToList();
        if (!list.Any(s => s.path == path)) { list.Insert(0, new EditorBuildSettingsScene(path, true)); EditorBuildSettings.scenes = list.ToArray(); }
        Debug.Log("[Nusantara] Splash scene dibuat: " + path + " (masuk Build Settings index 0). Tekan Play!");
    }

    static Sprite Spr(string n)
    {
        var s = AssetDatabase.LoadAssetAtPath<Sprite>(Dir + "Sprites/" + n + ".png");
        if (!s) Debug.LogWarning("[Nusantara] Sprite tidak ditemukan / belum jadi Sprite: " + n);
        return s;
    }
    static void Stretch(RectTransform r) { r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one; r.offsetMin = r.offsetMax = Vector2.zero; }
    static RectTransform Mk(Transform p, string n, string sprite)
    {
        var go = new GameObject(n, typeof(RectTransform), typeof(Image)); go.transform.SetParent(p, false);
        var im = go.GetComponent<Image>(); im.sprite = Spr(sprite); im.raycastTarget = false; return go.GetComponent<RectTransform>();
    }
    // Layer penuh 1080x1920; pivotPx (px dari kiri-atas) dipakai untuk rotasi/skala animasi.
    static RectTransform Full(Transform p, string n, string sprite, Vector2? pivotPx = null)
    {
        var r = Mk(p, n, sprite); Stretch(r);
        if (pivotPx.HasValue) r.pivot = new Vector2(pivotPx.Value.x / Ref.x, 1f - pivotPx.Value.y / Ref.y);
        return r;
    }
    static RectTransform Anchored(Transform p, string n, string sprite, Vector2 aMin, Vector2 aMax, Vector2 pos, Vector2 size)
    {
        var r = Mk(p, n, sprite); r.anchorMin = aMin; r.anchorMax = aMax; r.pivot = new Vector2(.5f, .5f);
        r.anchoredPosition = pos; r.sizeDelta = size; r.GetComponent<Image>().preserveAspect = true; return r;
    }
    static RectTransform MakeButton(Transform p, string n, string sprite, float fromBottom)
    {
        var r = Anchored(p, n, sprite, new Vector2(.5f, 0), new Vector2(.5f, 0), new Vector2(0, fromBottom), new Vector2(620, 140));
        var img = r.GetComponent<Image>(); img.raycastTarget = true;
        var b = r.gameObject.AddComponent<Button>(); b.targetGraphic = img;
        var c = b.colors; c.pressedColor = new Color(.8f, .8f, .8f); b.colors = c; return r;
    }
}
