using Photon.Bolt;
using Photon.Bolt.Utils;

namespace Player
{
    public class Skills : EntityEventListener<IPlayer>
    {

      

      
        public override void OnEvent(ChangeSkill evnt)
        {
            if (evnt.FromSelf)
            {

             
                BoltLog.Warn("Получил событие о изменение скилла {0}, на что увеличить {1}", state.Skills[evnt.Skill],evnt.Value);
                int numberSkill = evnt.Skill;
                float total;
                total = state.Skills[numberSkill] + evnt.Value;
                BoltLog.Warn("Тотал  " + total.ToString());
                state.Skills[numberSkill] = (float)System.Math.Round(total,1);
            
                _ = LogEvent.Post(entity,EntityTargets.OnlySelf, string.Format("<color=#DAA520>Умение {0} увеличелось на 0.1 и сейчас равно </color>{1} ", evnt.Skill,state.Skills[numberSkill]),false);
                _ = ChangeSkillAndStats.Post(GlobalTargets.OnlyServer,evnt.Skill,evnt.Value,state.Login,true);
              
               
            }



        }
        





    }
}
