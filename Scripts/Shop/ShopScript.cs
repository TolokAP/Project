using UnityEngine;
using Photon.Bolt.Utils;



namespace Player
{
    public class ShopScript : MonoBehaviour
    {
   
        public ItemDatabase itemDatabase;
        
        [SerializeField] private GameObject content;

        [SerializeField] private GameObject _itemInfo;

       
        private void Start() {


           

            int size = itemDatabase._itemData.Count;
            BoltLog.Warn(size);
   
            for (int i = 1; i <= size; i++)
            {
                GameObject item = Instantiate(_itemInfo,content.transform);
                item.GetComponent<ShopInfoItem>().SetID(i);
            
            }
        }

        
       
       
    }
}