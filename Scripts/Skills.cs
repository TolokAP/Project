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

             
                BoltLog.Warn("������� ������� � ��������� ������ {0}, �� ��� ��������� {1}", state.Skills[evnt.Skill],evnt.Value);
                int numberSkill = evnt.Skill;
                float total;
                total = state.Skills[numberSkill] + evnt.Value;
                BoltLog.Warn("�����  " + total.ToString());
                state.Skills[numberSkill] = (float)System.Math.Round(total,1);
            
                _ = LogEvent.Post(entity,EntityTargets.OnlySelf, string.Format("<color=#DAA520>������ {0} ����������� �� 0.1 � ������ ����� </color>{1} ", evnt.Skill,state.Skills[numberSkill]),false);
                _ = ChangeSkillAndStats.Post(GlobalTargets.OnlyServer,evnt.Skill,evnt.Value,state.Login,true);
              
               
            }



        }
        





    }
}
