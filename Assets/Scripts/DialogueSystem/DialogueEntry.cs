using UnityEngine;

[System.Serializable]
public class DialogueEntry
{
    [TextArea(3, 10)]
    public string text;
    public Sprite displayImage;

    [Tooltip("Sprite da borda da fala para este personagem. Se vazio, usa o padrão do DialogueData.style.")]
    public Sprite borderSprite;
}
