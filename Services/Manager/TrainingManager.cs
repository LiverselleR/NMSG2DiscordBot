using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;

namespace NMSG2DiscordBot
{ 
    public class TrainingManager
    {
        public static void UmamusumeTraining(UInt64 ownerID, String stat) 
        {
            List<Training> tList = JSONManager.GetTrainingInfo();
            List<Umamusume> uList = JSONManager.GetUmamusumeList();
            StatusType status;

            try
            {
                status = ConvertStringToStatusType(stat);
            }
            catch (Exception e)
            {
                throw e;
            }


            Training? t = tList.Find(t => t.ownerID == ownerID);

            if (t == null)
            {
                Umamusume? u = uList.Find(u => u.ownerID == ownerID);
                if (u == null) throw new UserIDNotFoundException();
                else
                {
                    tList.Add(new Training(ownerID, 3, DateTime.Now));
                }
            }

            int ti = tList.FindIndex(t => t.ownerID == ownerID);
            int ui = uList.FindIndex(u => u.ownerID == ownerID);

            TimeSpan timeSpan = DateTime.Now.Date - tList[ti].date.Date;
            if (timeSpan.Days > 0)
                tList[ti].leftTraining = 3;
            if (tList[ti].leftTraining > 0)
            {
                tList[ti].leftTraining--;
                switch (status)
                {
                    case StatusType.speed:
                        uList[ui].speed += 40;
                        uList[ui].power += 10;
                        break;
                    case StatusType.stamina:
                        uList[ui].stamina += 40;
                        uList[ui].toughness += 10;
                        break;
                    case StatusType.power:
                        uList[ui].power += 40;
                        uList[ui].stamina += 10;
                        break;
                    case StatusType.toughness:
                        uList[ui].toughness += 40;
                        uList[ui].speed += 5;
                        uList[ui].power += 5;
                        break;
                    case StatusType.intelligence:
                        uList[ui].intelligence += 40;
                        uList[ui].speed += 10;
                        break;
                }
                tList[ti].date = DateTime.Now.Date;
            }
            else throw new NoLeftTrainingCountException();

            JSONManager.SetTrainingInfo(tList);
            JSONManager.SetUmamusumeList(uList);
        }

        public static String GetLeftTraining(UInt64 ownerID)
        {
            List<Training> tList = JSONManager.GetTrainingInfo();
            List<Umamusume> uList = JSONManager.GetUmamusumeList();

            Training? t = tList.Find(t => t.ownerID == ownerID);

            if (t == null)
            {
                Umamusume? u = uList.Find(u => u.ownerID == ownerID);
                if (u == null) throw new UserIDNotFoundException();
                else
                {
                    tList.Add(new Training(ownerID, 3, DateTime.Now));
                }
            }

            int ti = tList.FindIndex(t => t.ownerID == ownerID);
            int ui = uList.FindIndex(u => u.ownerID == ownerID);

            TimeSpan timeSpan = DateTime.Now.Date - tList[ti].date.Date;
            if (timeSpan.Days > 0)
                tList[ti].leftTraining = 3;

            return "■ " + uList[ui].name + " 의 남은 훈련 횟수 : " + tList[ti].leftTraining;

        }

        private static StatusType ConvertStringToStatusType(String str)
        {
            switch (str)
            {
                case "속도":
                    return StatusType.speed;
                case "체력":
                    return StatusType.stamina;
                case "근력":
                    return StatusType.power;
                case "근성":
                    return StatusType.toughness;
                case "지능":
                    return StatusType.intelligence;
                default:
                    throw new InvalidStatusTypeException();
            }
        }
    }

    public class Training
    {
        public UInt64 ownerID;
        public int leftTraining;
        public DateTime date;

        public Training()
        {
            this.ownerID = 0;
            this.leftTraining = 0;
            this.date = DateTime.Now.Date;
        }

        public Training(UInt64 ownerID, int leftTraining, DateTime date)
        {
            this.ownerID = ownerID;
            this.leftTraining = leftTraining;
            this.date = date;
        }
    }

    public class NoLeftTrainingCountException : Exception
    {
        public NoLeftTrainingCountException()
        {

        }
        public NoLeftTrainingCountException(string message)
            : base(message)
        {

        }
        public NoLeftTrainingCountException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
    public class UserIDNotFoundException : Exception
    {
        public UserIDNotFoundException()
        {
        }

        public UserIDNotFoundException(string message) : base(message)
        {
        }

        public UserIDNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserIDNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
    public class InvalidStatusTypeException : Exception
    {
        public InvalidStatusTypeException()
        {
        }

        public InvalidStatusTypeException(string message) : base(message)
        {
        }

        public InvalidStatusTypeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidStatusTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }


}
