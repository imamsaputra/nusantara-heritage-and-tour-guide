using UnityEngine;
using UnityEngine.EventSystems;

// Skrip ini harus dipasang pada GameObject Peta (MapContent)
public class MapContent : MonoBehaviour, IDragHandler, IScrollHandler
{
    private RectTransform rectTransform;

    [Header("Pengaturan Zoom")]
    public float zoomSpeed = 0.1f;
    public float minScale = 1f;  // Zoom minimum
    public float maxScale = 5f;  // Zoom maksimum

    void Start()
    {
        // Mendapatkan komponen RectTransform dari objek tempat skrip dipasang
        rectTransform = GetComponent<RectTransform>();

        // Pastikan pivot RectTransform disetel ke tengah (0.5, 0.5) agar zoom berfungsi dengan benar
        if (rectTransform.pivot != new Vector2(0.5f, 0.5f))
        {
            Debug.LogWarning("Untuk zoom yang benar, harap setel Pivot RectTransform MapContent ke (0.5, 0.5).");
        }
    }

    // --- MENANGANI ZOOM (SCROLL MOUSE) ---
    public void OnScroll(PointerEventData eventData)
    {
        // Ambil perubahan scroll wheel (eventData.scrollDelta.y)
        float zoomDelta = eventData.scrollDelta.y * zoomSpeed;

        // Terapkan zoom
        ApplyZoom(zoomDelta);
    }

    // --- MENANGANI ZOOM (PINCH DUA JARI DI HP) ---
    void Update()
    {
        // Periksa input dua jari (pinch gesture) untuk zoom di layar sentuh
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Ambil posisi sentuhan sebelumnya
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Hitung magnitudo (jarak) antar sentuhan
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Hitung perbedaan magnitudo (seberapa jauh pinch terjadi)
            float deltaMagnitudeDiff = touchDeltaMag - prevTouchDeltaMag;

            // Terapkan zoom berdasarkan perbedaan magnitudo
            ApplyZoom(deltaMagnitudeDiff * zoomSpeed * 0.01f); // Sesuaikan kecepatan pinch
        }
    }

    // Fungsi inti untuk menerapkan zoom (mengubah LocalScale)
    void ApplyZoom(float increment)
    {
        // Ambil skala saat ini (local scale x atau y sama)
        float currentScale = rectTransform.localScale.x;

        // Hitung skala baru
        float newScale = currentScale + increment;

        // Batasi skala agar tidak keluar dari minScale dan maxScale
        newScale = Mathf.Clamp(newScale, minScale, maxScale);

        // Terapkan skala baru ke RectTransform
        rectTransform.localScale = new Vector3(newScale, newScale, 1f);
    }

    // --- MENANGANI GESER (DRAG) ---
    // (Fungsi ini kosong karena Scroll Rect induk menangani geser, 
    // tetapi interface IDragHandler diperlukan untuk OnScroll berfungsi di Scroll Rect)
    public void OnDrag(PointerEventData eventData)
    {
        // Jika Anda ingin logika geser kustom di skrip ini, Anda bisa menambahkannya di sini.
        // Tapi Scroll Rect pada MapViewport sudah menangani geser secara default.
    }
}