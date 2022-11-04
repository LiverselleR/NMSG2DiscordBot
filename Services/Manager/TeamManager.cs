using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace NMSG2DiscordBot
{ 
    public class TeamManager
    {
        public static void Register(UInt64 ownerID, String tName, ref List<Team> tList, List<Trainer> trainerList, List<Umamusume> uList)
        {
            Boolean isTrainerRegistered = TrainerManager.Lookup(ownerID, trainerList);
            Boolean isUmamusumeRegistered = UmamusumeManager.Lookup(ownerID, uList);

            try
            {
                if (isTrainerRegistered) RegisterTrainer(ownerID, tName, ref tList);
                else if (isUmamusumeRegistered) RegisterUmamusume(ownerID, tName, ref tList);
                else throw new DiscordIDNotRegisteredException();
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        public static void RegisterTrainer(UInt64 trainerOwnerID, String tName, ref List<Team> tList)
        {
            Boolean isTrainerRegistered = false;
            foreach (Team t in tList)
            {
                isTrainerRegistered = isTrainerRegistered || (t.trainerOwnerID == trainerOwnerID);
            }
            if (isTrainerRegistered) throw new TrainerAlreadyRegisteredException();

            int tIndex = tList.FindIndex(t => t.name == tName);
            if (tIndex == -1)
            {
                tList.Add(new Team(tName, trainerOwnerID));
            }
            else tList[tIndex].trainerOwnerID = trainerOwnerID;
        }

        public static void RegisterUmamusume(UInt64 umamusumeOwnerID, String tName, ref List<Team> tList)
        {
            Boolean isUmamusumeRegistered = false;
            foreach (Team t in tList)
            {
                isUmamusumeRegistered = isUmamusumeRegistered || t.FindUmamusume(umamusumeOwnerID);
            }
            if (isUmamusumeRegistered) throw new UmamusumeAlreadyRegisteredException();

            int tIndex = tList.FindIndex(t => t.name == tName);
            if (tIndex == -1) throw new TeamNameNotFoundException();
            else tList[tIndex].umamusumeOwnerIDList.Add(umamusumeOwnerID);
        }


    }

    public class DiscordIDNotRegisteredException : Exception
    {
        public DiscordIDNotRegisteredException()
        {
        }

        public DiscordIDNotRegisteredException(string message) : base(message)
        {
        }

        public DiscordIDNotRegisteredException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DiscordIDNotRegisteredException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
    public class TeamNameNotFoundException : Exception
    {
        public TeamNameNotFoundException()
        {
        }

        public TeamNameNotFoundException(string message) : base(message)
        {
        }

        public TeamNameNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TeamNameNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
