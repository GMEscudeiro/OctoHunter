using UnityEngine;
using System.Collections;

public class BossAbilityVenomPool : MonoBehaviour
{
    [Header("Settings")]
    public GameObject venomPoolPrefab;
    public GameObject venomWarningPrefab;  // indicador visual antes da poça cair (círculo/marcação)
    public int        poolCount      = 3;
    public float      spreadRadius   = 3f;
    public float      warningDuration = 1.5f;  // tempo do aviso antes da poça aparecer

    public void Activate(Transform playerTransform)
    {
        StartCoroutine(SpawnWithWarning(playerTransform));
    }

    private IEnumerator SpawnWithWarning(Transform playerTransform)
    {
        Vector3[]    positions = new Vector3[poolCount];
        GameObject[] warnings  = new GameObject[poolCount];

        // Captura posições com base na posição atual do player e spawna indicadores
        for (int i = 0; i < poolCount; i++)
        {
            Vector2 offset = Random.insideUnitCircle * spreadRadius;
            positions[i]   = playerTransform.position + new Vector3(offset.x, offset.y, 0);

            if (venomWarningPrefab != null)
                warnings[i] = Instantiate(venomWarningPrefab, positions[i], Quaternion.identity);
        }

        yield return new WaitForSeconds(warningDuration);

        // Remove indicadores e spawna as poças reais
        for (int i = 0; i < poolCount; i++)
        {
            if (warnings[i] != null) Destroy(warnings[i]);
            Instantiate(venomPoolPrefab, positions[i], Quaternion.identity);
        }

        Debug.Log("[SnakeBoss] Habilidade: Poça de Veneno!");
    }
}
