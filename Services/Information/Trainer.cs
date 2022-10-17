using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace NMSG2DiscordBot
{
    public class Trainer
    {
        public UInt64 ownerID;
        public String name;

        public Trainer()
        {
            this.ownerID = 0;
            this.name = "테스트";
        }

        public Trainer(UInt64 ownerID, String name)
        {
            this.ownerID = ownerID;
            this.name = name;
        }

        public override string ToString()
        {
            List<Team> teamList = JSONManager.GetTeamList();
            List<Umamusume> uList = JSONManager.GetUmamusumeList();
            List<Umamusume> teamUList = new List<Umamusume>();
            StringBuilder sb = new StringBuilder();
            String teamName;

            Team team = teamList.Find(t => t.trainerOwnerID == ownerID);
            if (team == null || ownerID == 0)
            {
                teamName = "없음";
                sb.Append("없음");
            }
            else
            {
                teamName = team.name;
                foreach (UInt64 uId in team.umamusumeOwnerIDList)
                {
                    teamUList.Add(uList.Find(u => u.ownerID == uId));
                }
                if (teamUList.Count == 0) sb.Append("없음");
                else
                {
                    foreach (Umamusume u in teamUList)
                    {
                        sb.Append(u.name + " / ");
                    }
                    sb.Remove(sb.Length - 3, 2);
                }
            }
            
            
            return "◆ 이름 - " + name + " ( 담당 팀 : " + teamName + " ) \n"
                + "■ 담당 우마무스메 : " + sb.ToString();
        }
    }
}
