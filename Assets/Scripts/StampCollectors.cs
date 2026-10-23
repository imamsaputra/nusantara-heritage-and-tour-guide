using UnityEngine;
using UnityEngine.SceneManagement;

public class StampCollector : MonoBehaviour
{
    // Fungsi pindah scene berdasarkan nama scene
    public void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Fungsi keluar dari aplikasi
    public void QuitApp()
    {
        Application.Quit();
        Debug.Log("Aplikasi Ditutup");
    }
}