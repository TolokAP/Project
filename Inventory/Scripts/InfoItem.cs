
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Player {
    public class InfoItem : MonoBehaviour, IPointerClickHandler
    {
     
        [SerializeField]
        private TMP_Text _nameItem;
        [SerializeField]
        private TMP_Text _damage;
        [SerializeField]
        private TMP_Text _descriptionAbility;
   

      
      

        public void OnPointerClick(PointerEventData eventData)
        {
            Destroy(gameObject);
        }


        public void SetStringInfo(string itemName, string itenDamage, string itemSpecialAttack)
        {
          
        
            _nameItem.text = itemName;
            _damage.text = itenDamage;
            _descriptionAbility.text = itemSpecialAttack;

        }
    }
}