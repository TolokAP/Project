
using UnityEngine;
using Photon.Bolt;

namespace Player {
    public class PlayerState : EntityEventListener<IPlayer>
    {
        public BoltEntity GO;
        private UIPlayer _UIPlayer;
        [SerializeField]
        private GameObject _characterCamera;


        public int localHealth;
        public int _maxHealth;

       
        public override void Attached()
        {
            if (entity.IsOwner)
            {
                _UIPlayer = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIPlayer>();
                Instantiate(_characterCamera, gameObject.transform);
                GO = GetComponent<BoltEntity>();
                state.AddCallback("CurrentHealth", CurrentHealthCallBack);//Обратный вызов Текущее здоровье
                state.AddCallback("TotalHealth", MaxHealthCallBack);//Обратный вызов максимальное здоровье
                state.AddCallback("Skills[]", SkillPlayerCallbacks);//Обратный вызов изменения скилла
                state.AddCallback("Stats[]", StatsPlayerCallbacks); //Обратный вызов изменения характеристик
                state.AddCallback("SpecialAttack[]", UpdateSpecialAttack);//Обратный вызов изменение специальных аттак
            }

        }


        private void UpdateSpecialAttack(IState state2, string propertyPath, ArrayIndices arrayIndices)
        {
            int index = arrayIndices[0];
            if (state.SpecialAttack[index].AttackNumber == 0) { _UIPlayer.UpdateIconSpecialAttack(index, state.SpecialAttack[index].AttackNumber, false); }
            else { _UIPlayer.UpdateIconSpecialAttack(index, state.SpecialAttack[index].AttackNumber, true); }

        }


        private void StatsPlayerCallbacks(IState state, string propertyPath, ArrayIndices arrayIndices)
        {
            var index = arrayIndices[0];
            _UIPlayer.UpdateUI(index, GO.GetState<IPlayer>().Stats[index]);




        }




        #region Bolt Callbacks
        private void MaxHealthCallBack()//CallBack изменение максимального здоровья.
        {
            if (entity.IsOwner)
            {

                if (state.TotalHealth != state.CurrentHealth)
                {
                    int value = state.TotalHealth - _maxHealth;
                    state.CurrentHealth += value;
                    _maxHealth = state.TotalHealth;
                    _UIPlayer.UpdateHealthUI(state.CurrentHealth, state.TotalHealth);
                 
                }

            }

        }
        private void CurrentHealthCallBack()//CallBack изменение текущего здоровья.
        {

            if (entity.IsOwner)
            {
          
                localHealth = state.CurrentHealth;
                _UIPlayer.UpdateHealthUI(state.CurrentHealth, state.TotalHealth);
             
            }
          
        }

        private void SkillPlayerCallbacks(IState state, string propertyPath, ArrayIndices arrayIndices)
        {
            var index = arrayIndices[0];
            _UIPlayer.UpdateUI(index, GO.GetState<IPlayer>().Skills[index]);


        }


        #endregion





    }
}