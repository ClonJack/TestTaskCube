using Unity.VisualScripting;
using UnityEngine;

namespace Data.DataBase
{
    [System.Serializable]
    public struct UpProperty
    {
        public Material DBMaterials;
        public int Weight;
    }

    [CreateAssetMenu(fileName = "DataUp", menuName = "SO DATA DATA UP", order = 0)]
    public class DataUp : ScriptableObject
    {
        [SerializeField] private UpProperty[] DBUpProperties;
        [SerializeField] private UpProperty CurrentProperty;

        public UpProperty GetCurrentProperty() => CurrentProperty;

        public void NextLevel(int Weight)
        {
            foreach (var property in DBUpProperties)
            {
                if (property.Weight != Weight) continue;
                CurrentProperty = property;
                return;
            }
        }
    }
}