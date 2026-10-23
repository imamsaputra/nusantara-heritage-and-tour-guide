using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigator : MonoBehaviour
{
    // Fungsi lama untuk Scene 1 ke Scene 2 (Jangan diubah!)
    public void GoToDetailScene()
    {
        Debug.Log("Tombol Lanjut Berhasil Diklik!");
        SceneManager.LoadScene("Scene_02_Onboarding");
    }

    // Fungsi baru untuk Scene 2 ke Scene 3
    public void GoToScene03()
    {
        Debug.Log("Pindah ke Scene 3 Berhasil!");
        SceneManager.LoadScene("Scene_03_ExploreTechScene");
    }

    // Fungsi baru untuk pindah ke Scene 4
    public void GoToScene04()
    {
        Debug.Log("Pindah ke Scene 4 Berhasil!");
        SceneManager.LoadScene("Scene_04_DashboardScene");
    }

    // Fungsi baru untuk pindah ke Scene 5
    public void GoToScene05()
    {
        Debug.Log("Pindah ke Scene 5 Berhasil!");
        SceneManager.LoadScene("Scene_05_DestinationListScene");
    }
}