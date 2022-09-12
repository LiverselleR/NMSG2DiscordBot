using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMSG2DiscordBot
{
    public class Team
    {
        public string name;
        public UInt64 trainerOwnerID;
        public List<UInt64> umamusumeOwnerIDList;

        public Team()
        {
            this.name = "테스트_팀";
            trainerOwnerID = 0;
            umamusumeOwnerIDList = new List<UInt64>();
        }
        public Team(
           string name)
        {
            this.name = name;
            trainerOwnerID = 0;
            umamusumeOwnerIDList = new List<UInt64>();
        }
        public Team(
            string name, UInt64 trainerOwnerID)
        {
            this.name = name;
            this.trainerOwnerID = trainerOwnerID;
            umamusumeOwnerIDList = new List<UInt64>();
        }

        public Team(
            string name, UInt64 trainerOwnerID, List<UInt64> umamusumeOwnerIDList)
        {
            this.name = name;
            this.trainerOwnerID = trainerOwnerID;
            this.umamusumeOwnerIDList = umamusumeOwnerIDList;
        }

        public Boolean FindUmamusume(UInt64 ownerID)
        {
            return umamusumeOwnerIDList.Contains(ownerID);
        }

    }


}
