using UnityEngine;
using UnityEngine.UI;

public class ARViewer : MonoBehaviour
{
    // Variabel Publik & Privat (Lihat Detail)

    // Objek Plane yang akan menampilkan feed kamera
    public GameObject targetPlane;

    // Objek kamera yang akan menangkap gambar
    private WebCamTexture webcamTexture;

    // void Start()
    void Start()
    {
        // PENTING: Untuk zoom yang benar, harap setel Pivot RectTransform MapContent ke (0.5, 0.5).

        // Memeriksa apakah ada perangkat webcam
        if (WebCamTexture.devices.Length == 0)
        {
            Debug.LogError("Tidak ada perangkat webcam yang ditemukan!");
            return;
        }

        // // Membuat dan Memulai WebCamTexture
        webcamTexture = new WebCamTexture();
        webcamTexture.Play();
    }

    // void Update()
    void Update()
    {
        // 3. Menampilkan pada Objek (Update)

        // // Pasang Tekstur ke Material Plane
        if (webcamTexture != null && targetPlane != null)
        {
            // Umpan Kamera Langsung pada Material Objek
            targetPlane.GetComponent<Renderer>().material.mainTexture = webcamTexture;
        }
    }

    // void OnDisable()
    void OnDisable()
    {
        // // Menghentikan Kamera Saat Nonaktif
        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            webcamTexture.Stop();
        }
    }
}