using UnityEngine;
using UnityEngine.EventSystems;
using DuloGames.UI;
namespace Player
{


    public class OpenShopScript : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private GameObject _Shop;
        
       

        public void OnPointerClick(PointerEventData eventData){
     _Shop.GetComponent<UIWindow>().Show();
            
          
    }
    
    }
}