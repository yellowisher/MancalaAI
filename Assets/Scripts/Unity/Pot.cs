using DG.Tweening;
using Mancala.Common;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Mancala.Unity
{
    public class Pot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private int _index;
        [SerializeField] private Image _image;
        [SerializeField] private GameObject _stonePrefab;
        [SerializeField] private Transform _stoneContainer;
        [SerializeField] private TextMeshProUGUI _countText;

        public int Index => _index;
        
        public static readonly UnityEvent<int> OnClickEvent = new();

        public void SetStoneCount(int count)
        {
            _countText.text = count.ToString();
            
            _stoneContainer.DestroyAllActiveChildren();
            count = Mathf.Min(count, 12);
            
            for (int i = 0; i < count; i++)
            {
                var stone = Instantiate(_stonePrefab, _stoneContainer);
                stone.SetActive(true);
            }
        }

        public void SetInteractable(bool isInteractable)
        {
            _image.raycastTarget = isInteractable;
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            transform.DOBlendableScaleBy(Vector3.one * 0.2f, 0.1f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            transform.DOBlendableScaleBy(Vector3.one * -0.2f, 0.1f);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClickEvent.Invoke(_index);
        }

        public void SetOutlineColor(Color color)
        {
            _image.color = color;
        }
    }
}