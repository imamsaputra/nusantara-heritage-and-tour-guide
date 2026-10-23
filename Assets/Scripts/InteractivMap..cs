
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Gree.UnityWebView;

public class InteractiveMap : MonoBehaviour
{
    [Header("Area Peta")]
    public RectTransform mapArea;
    public Canvas canvas;

    private WebViewObject webView;

    IEnumerator Start()
    {
        if (mapArea == null || canvas == null)
        {
            Debug.LogError(
                "MapArea atau Canvas belum dihubungkan di Inspector."
            );
            yield break;
        }

        // Tunggu sampai posisi UI selesai dihitung
        yield return new WaitForEndOfFrame();

        string path = System.IO.Path.Combine(
            Application.streamingAssetsPath,
            "map.html"
        );

        using (UnityWebRequest request = UnityWebRequest.Get(path))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(
                    "Gagal membaca map.html: " + request.error
                );
                yield break;
            }

            webView = gameObject.AddComponent<WebViewObject>();
            webView.Init();

            // Hitung batas area peta berdasarkan RectTransform
            Vector3[] corners = new Vector3[4];
            mapArea.GetWorldCorners(corners);

            Camera uiCamera = null;

            if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                uiCamera = canvas.worldCamera;
            }

            float minX = Screen.width;
            float maxX = 0;
            float minY = Screen.height;
            float maxY = 0;

            for (int i = 0; i < 4; i++)
            {
                Vector2 point =
                    RectTransformUtility.WorldToScreenPoint(
                        uiCamera, corners[i]
                    );

                minX = Mathf.Min(minX, point.x);
                maxX = Mathf.Max(maxX, point.x);
                minY = Mathf.Min(minY, point.y);
                maxY = Mathf.Max(maxY, point.y);
            }

            int left = Mathf.RoundToInt(minX);
            int right = Mathf.RoundToInt(Screen.width - maxX);
            int bottom = Mathf.RoundToInt(minY);
            int top = Mathf.RoundToInt(Screen.height - maxY);

            webView.SetMargins(left, top, right, bottom);
            webView.SetVisibility(true);

            webView.LoadHTML(
                request.downloadHandler.text,
                "https://localhost/"
            );
        }
    }

    void OnDestroy()
    {
        if (webView != null)
        {
            webView.SetVisibility(false);
            Destroy(webView);
        }
    }
}
