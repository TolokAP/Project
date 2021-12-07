
using UnityEngine;
using UnityEngine.UI;

public class UITarget : MonoBehaviour
{
    public Text targetHealth;
    public Slider SlyderTarget;
    public Text targetName;

  

    public void UpdateUI(float currentHealth,float maxHealth,string name)
    {

        SlyderTarget.maxValue = maxHealth;
        SlyderTarget.value = currentHealth;
        targetHealth.text = currentHealth.ToString() + " / " + maxHealth.ToString();
        targetName.text = name;

    }

    
}
