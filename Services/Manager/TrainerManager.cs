using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace NMSG2DiscordBot
{
    public class TrainerManager
    {
        public static void Register(UInt64 discordID, string tName, ref List<Trainer> tList)
        {
            int i = tList.FindIndex(x => x.name.Contains(tName));
            if (i == -1) throw new TrainerNameNotFoundException();
            else if (tList.FindIndex(x => x.ownerID.CompareTo(discordID) == 0) != -1) throw new DiscordIdAlreadyRegisteredException();
            else
            {
                if (tList[i].ownerID != 0) throw new TrainerAlreadyRegisteredException();
                tList[i].ownerID = discordID;
                return;
            }
        }

        public static Boolean Lookup(UInt64 discordID, List<Trainer> tList)
        {
            int i = tList.FindIndex(t => t.ownerID == discordID);
            if (i == -1) return false;
            else return true;
        }
    }
    public class TrainerNameNotFoundException : Exception
    {
        public TrainerNameNotFoundException()
        {
        }

        public TrainerNameNotFoundException(string message) : base(message)
        {
        }

        public TrainerNameNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TrainerNameNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
    public class TrainerAlreadyRegisteredException : Exception
    {
        public TrainerAlreadyRegisteredException()
        {
        }

        public TrainerAlreadyRegisteredException(string message) : base(message)
        {
        }

        public TrainerAlreadyRegisteredException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TrainerAlreadyRegisteredException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
