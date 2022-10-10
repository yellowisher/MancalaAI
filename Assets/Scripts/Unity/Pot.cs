using Mancala.Common;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Mancala.Unity
{
    public class Pot : MonoBehaviour
    {
        [SerializeField] private int _index;
        [SerializeField] private GameObject _stonePrefab;
        [SerializeField] private Transform _stoneContainer;
        [SerializeField] private TextMeshProUGUI _countText;

        public int Index => _index;
        
        public static readonly UnityEvent<int> OnClickEvent = new();

        public void OnClick()
        {
            OnClickEvent.Invoke(_index);
        }

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
    }
}