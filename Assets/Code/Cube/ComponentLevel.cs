using Data.DataBase;
using TMPro;
using UnityEngine;


namespace Code.Cube
{
    public class ComponentLevel : MonoBehaviour
    {
        [SerializeField] private DataUp _dataUp;
        [SerializeField] private int _weight;

        private TrailRenderer _trailRenderer;
        private TextMeshProUGUI _infoWeight;

        private Renderer _renderer;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _trailRenderer = GetComponentInChildren<TrailRenderer>();
            _infoWeight = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void Start()
        {
            if (_infoWeight != null)
            {
                _infoWeight.text = (_weight + 1).ToString();
            }

            _dataUp.NextLevel(_weight);
            _renderer.material = _dataUp.GetCurrentProperty().DBMaterials;
        }

        public void UpLevel()
        {
            _weight++;
            _dataUp.NextLevel(_weight);
          
            _renderer.material = _dataUp.GetCurrentProperty().DBMaterials;
            if (_trailRenderer != null)
                _trailRenderer.material = _dataUp.GetCurrentProperty().DBMaterials;

           

            if (_infoWeight != null)
                _infoWeight.text =  (_weight + 1).ToString();
        }

        public int GetWeight() => _weight;
    }
}