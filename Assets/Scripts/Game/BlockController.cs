using UnityEngine;

public class BlockController : MonoBehaviour
{
    [SerializeField] private Block[] blocks;
    public delegate void OnBlockClickHandler(int row, int column);
    public OnBlockClickHandler OnBlockClicked;
    
    public void InitBlocks()
    {
        for (int i = 0; i < blocks.Length; i++)
        {
            blocks[i].InitMarker(i, (blockIndex) =>
            {
                var clickedRow = blockIndex / 3;
                var clickedColumn = blockIndex % 3;
                OnBlockClicked?.Invoke(clickedRow, clickedColumn);
            });
        }
    }
    
    
    /// <summary>
    /// 특정 Block에 마커 표시하는 함수
    /// </summary>
    /// <param name="markerType">마커 타입</param>
    /// <param name="row">Row</param>
    /// <param name="column">Col</param>
    public void PlaceMarker(Block.MarkerType markerType, int row,int column)
    {
        // row, col를 index로 변환
        var markerIndex = row * 3 + column;
        
        blocks[markerIndex].SetMarker(markerType);
    }

    public void SetBlockColor(GameManager.PlayerType playerType,
        (int row,int column)[] blockPositions)
    {
        if (playerType == GameManager.PlayerType.None) return;
        
        foreach (var blockPosition in blockPositions)
        {
            var blockIndex = blockPosition.row * 3 + blockPosition.column;
            Color32 markerColor;
            if (playerType == GameManager.PlayerType.PlayerA)
            {
                markerColor = new Color32(0,166,255,255); 
            } else if (playerType == GameManager.PlayerType.PlayerB)
            {
                markerColor = new Color32(255,0,94,255); 
            }
            else
            {
                markerColor = Color.black;
            }
            blocks[blockIndex].SetColor(markerColor);
        }
    }
}
