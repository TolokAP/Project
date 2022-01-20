
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Bolt;
using Photon.Bolt.Utils;
namespace Player {
    public class UIPlayer : MonoBehaviour
    {
        public ItemDatabase itemDatabase;
        public Slider SliderPlayer;
        public Text _HealthPlayer;
        public Transform spawnPositionStatsUI;
        [SerializeField]
        private GameObject prefabStats;
        [SerializeField]
        private GameObject prefabSeparator;
        [SerializeField]
        private GameObject prefabLabel;

        public List<Sprite> spritesIconSpecialAttack;
        public List<Image> iconSpecialAttack;



        public List<GameObject> gameObjectsUI;
        public List<GameObject> gameObjectsSkill;



        public List<string> stats = new List<string>()
    {
      {"Характеристики" },  {"Сила" }, {"Ловкость" }, {"Выносливость" }, {"Мощь" }, {"Умения" }, {"Луки" }, {"Мечи" }, {"Мощь" }

    };
        public List<string> listSkill = new List<string>()
    {
      {"Луки" }, {"Мечи" }, {"Ножи" }

    };


        public void UpdateHealthUI(float currentHealth, float maxHealth)
        {
            SliderPlayer.maxValue = maxHealth;
            SliderPlayer.value = currentHealth;
            _HealthPlayer.text = currentHealth.ToString() + " / " + maxHealth.ToString();
        }


        private void Start()
        {

            if (BoltNetwork.IsClient)
            {
                CreateUICharacter();
            }



        }

        public void UpdateIconSpecialAttack(int index, int icon, bool active)//Обновление иконок на кнопках и включение кнопок специальных умений
        {
            iconSpecialAttack[index].sprite = spritesIconSpecialAttack[icon];

            iconSpecialAttack[index].GetComponentInParent<Button>().interactable = active;


        }

        private void InstantiateUI(GameObject prefab, string value)
        {
            GameObject obj = Instantiate(prefab, spawnPositionStatsUI);
            if (!string.IsNullOrEmpty(value)) obj.GetComponentInChildren<Text>().text = value;

        }

        private void CreateUICharacter()
        {
            InstantiateUI(prefabLabel, stats[0]);
            InstantiateUI(prefabSeparator, "");
            for (int i = 1; i < 5; i++)
            {
                GameObject go = Instantiate(prefabStats, spawnPositionStatsUI);
                gameObjectsUI.Add(go);
                go.GetComponentInChildren<Text>().text = stats[i];
            }
            InstantiateUI(prefabSeparator, "");
            InstantiateUI(prefabLabel, stats[5]);
            InstantiateUI(prefabSeparator, "");
            for (int i = 0; i < 3; i++)
            {
                GameObject go = Instantiate(prefabStats, spawnPositionStatsUI);
                gameObjectsSkill.Add(go);
                go.GetComponentInChildren<Text>().text = listSkill[i];
            }

        }

        public void UpdateUI(int index, int value) //Обновление в окне персонажа UI эллементов(int) эллементы
        {
            Text[] texts = gameObjectsUI[index].GetComponentsInChildren<Text>();
            texts[1].text = value.ToString();

        }
        public void UpdateUI(int index, float value) //Обновление в окне персонажа UI эллементов (float) эллементы
        {
            BoltLog.Warn(index.ToString() + "Номер скилла");
            Text[] texts = gameObjectsSkill[index].GetComponentsInChildren<Text>();
            texts[1].text = value.ToString();

        }

        public void UpdateUIHealth(Slider slider, float currentHealth, float maxHealth)
        {
            slider.maxValue = maxHealth;
            slider.value = currentHealth;

        }



        public void ChangeIconSpecialButton(int numberIcon)
        {
            BoltLog.Warn("Смена иконки " + numberIcon);




        }







    }
    
}
