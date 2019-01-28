using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CellsData", menuName = "Map/CellsData", order = 0)]
public class CellsData : ScriptableObject
{
    public DataItem[] dataItems;

    [System.Serializable]
    public class DataItem
    {
#if UNITY_EDITOR
        /// <summary>
        /// EDITOR ONLY!
        /// </summary>
        [HideInInspector] public string name;
#endif
        public Sprite sprite;
        public Color color = new Color(1, 1, 1, 1);

        public void SetupCell(Cell cell, float size)
        {
            cell.spriteRenderer.sprite = sprite;
            cell.spriteRenderer.color = color;

            float scale = size * sprite.pixelsPerUnit / Mathf.Max(sprite.texture.width, sprite.texture.height);
            cell.transform.localScale = new Vector3(scale, scale, 1);
        }
    }

    private void OnValidate()
    {
        if(dataItems != null)
        {
            for (int i = 0; i < dataItems.Length; i++)
            {
                dataItems[i].name = dataItems[i].sprite == null ? $"Item {i}" : dataItems[i].sprite.name;
            }
        }
    }
}
