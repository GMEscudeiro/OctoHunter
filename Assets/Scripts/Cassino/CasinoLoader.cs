using UnityEngine;
using UnityEngine.SceneManagement;

// Adicione este script em qualquer GameObject da cena do jogo.
// Chame OpenCasino() pelo botão na UI.
public class CasinoLoader : MonoBehaviour
{
    [Header("Scene Name")]
    public string casinoSceneName = "CassinoScene";  // nome exato da cena no Build Settings

    public void OpenCasino()
    {
        Time.timeScale = 0f;  // pausa o jogo
        SceneManager.LoadScene(casinoSceneName);
    }

    // Chamado pelo CasinoManager quando o player sair do cassino
    public static void CloseCasino(string casinoSceneName)
    {
        Time.timeScale = 1f;  // retoma o jogo
        SceneManager.UnloadSceneAsync(casinoSceneName);
    }
}
