
using UnityEngine;
namespace Player
{
    public struct ScriptMethodsSkill
    {
        public float Choose(float[] probs)
        {

            float total = 0;
            foreach (float elem in probs)
            {
                total += elem;
            }
            float randomPoint = Random.value * total;
            for (int i = 0; i < probs.Length; i++)
            {
                if (randomPoint < probs[i]) return i;
                else randomPoint -= probs[i];

            }
            return probs.Length - 1;
        }

        public bool SetChanceUpSkill(float skill)
        {
            bool up;
            float[] dropChance = new float[2]; 
       
            dropChance[0] = skill;
            dropChance[1] = 1f - dropChance[0];

            float chance = Choose(dropChance);
      
            if (chance == 1)
            {
                up = true;

            }
            else
            {
                up = false;
            }
            return up;
        }

    }
}
