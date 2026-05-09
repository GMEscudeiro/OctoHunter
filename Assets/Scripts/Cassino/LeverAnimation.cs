using UnityEngine;
using System.Collections;

public class LeverAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    public float startAngle  =  30f;   // posição inicial (para cima/esquerda)
    public float endAngle    = -30f;   // posição final (para baixo/direita)
    public float duration    =  0.3f;  // duração de cada movimento

    // Chame este método no OnClick do botão de Reroll
    public void PullLever()
    {
        StartCoroutine(LeverRoutine());
    }

    private IEnumerator LeverRoutine()
    {
        // Vai para a posição final
        yield return StartCoroutine(RotateTo(endAngle));

        // Pequena pausa no fim do movimento
        yield return new WaitForSeconds(0.1f);

        // Volta para a posição inicial
        yield return StartCoroutine(RotateTo(startAngle));
    }

    private IEnumerator RotateTo(float targetAngle)
    {
        float startAngle = transform.localEulerAngles.z;

        // Normaliza o ângulo para evitar saltos de 0 para 360
        if (startAngle > 180f) startAngle -= 360f;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            float angle = Mathf.Lerp(startAngle, targetAngle, t);
            transform.localEulerAngles = new Vector3(0f, 0f, angle);
            yield return null;
        }

        transform.localEulerAngles = new Vector3(0f, 0f, targetAngle);
    }
}
