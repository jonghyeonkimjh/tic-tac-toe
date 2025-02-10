using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SpriteRenderer))]
public class Block : MonoBehaviour
{
    [SerializeField] private Sprite oSprite;
    [SerializeField] private Sprite xSprite;
    [SerializeField] private SpriteRenderer markerSpriteRenderer;
    public enum MarkerType { None, O, X, }

    public delegate void OnBlockClickHandler(int index);

    private event OnBlockClickHandler _onBlockClicked;
    //row,col아닌 index 이유? Block과 Block controller는 index로 상호작용하는게 더 원할하기 때문, GameManager에서만 Col,Row
    private int _blockIndex;
    private SpriteRenderer _spriteRenderer;
    private Color _defaultColor;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _defaultColor = _spriteRenderer.color;
    }
    
    
    /// <summary>
    /// 블럭의 색상을 변경하는 함수
    /// </summary>
    /// <param name="color">색상</param>
    public void SetColor(Color color)
    {
        _spriteRenderer.color = color;
    }
    
    /// <summary>
    /// Block 초기화 함수
    /// </summary>
    /// <param name="blockIndex">Block 인덱스</param>
    /// <param name="onBlockClicked">Block 터치 이벤트</param>
    public void InitMarker(int blockIndex, OnBlockClickHandler onBlockClicked)
    {
        _blockIndex = blockIndex;
        SetMarker(MarkerType.None);
        _onBlockClicked = onBlockClicked;
    }


    /// <summary>
    /// 어떤 마커를 표시할지 전달하는 함수
    /// </summary>
    /// <param name="markerType">마커 타입</param>
    public void SetMarker(MarkerType markerType)
    {
        switch (markerType)
        {
            case MarkerType.O:
                markerSpriteRenderer.sprite = oSprite;
                break;
            case MarkerType.X:
                markerSpriteRenderer.sprite = xSprite;
                break;
            case MarkerType.None:
                markerSpriteRenderer.sprite = null;
                break;                
        }
    }
    
    private void OnMouseUpAsButton()
    {
        // UI가 overlay 되어있는 상태에서는 이벤트를 안받도록
        if (EventSystem.current.IsPointerOverGameObject()) return;
        // 이 블럭의 범위 안에서만 터치 이벤트를 감지하는 버튼 (버튼을 클릭시 취소한다거나 그런걸 할 수 있게함)
        _onBlockClicked?.Invoke(_blockIndex);
    }
}