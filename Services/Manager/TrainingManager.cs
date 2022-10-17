using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;

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
                    tList.Add(new Training(ownerID));
                }
            }

            int ti = tList.FindIndex(t => t.ownerID == ownerID);
            int ui = uList.FindIndex(u => u.ownerID == ownerID);

            TimeSpan timeSpan = DateTime.Now.Date - tList[ti].date.Date;
            if (timeSpan.Days == 0)
            {
                throw new NoLeftTrainingCountException();
            }

            switch (status)
            {
                case StatusType.speed:
                    {
                        int SpeedIncrease = 12;
                        int PowerIncrease = 5;
                        if (DateTime.Now.Date.Subtract(new DateTime(2022, 10, 15)).Days == 0
                            || DateTime.Now.Date.Subtract(new DateTime(2022, 10, 16)).Days == 0)
                        {
                            SpeedIncrease = 16;
                            PowerIncrease = 7;
                        }
                        else if (tList[ti].SpeedTrainingCount >= 0)
                        {
                            SpeedIncrease = 12;
                            PowerIncrease = 5;
                        }
                        uList[ui].speed += SpeedIncrease;
                        uList[ui].power += PowerIncrease;
                        tList[ti].SpeedTrainingCount++;
                        break;
                    }
                case StatusType.stamina:
                    {
                        int StaminaIncrease = 10;
                        int ToughnessIncrease = 4;
                        if (DateTime.Now.Date.Subtract(new DateTime(2022, 10, 15)).Days == 0
                            || DateTime.Now.Date.Subtract(new DateTime(2022, 10, 16)).Days == 0)
                        {
                            StaminaIncrease = 15;
                            ToughnessIncrease = 6;
                        }
                        else if (tList[ti].SpeedTrainingCount >= 0)
                        {
                            StaminaIncrease = 10;
                            ToughnessIncrease = 4;
                        }
                        uList[ui].stamina += StaminaIncrease;
                        uList[ui].toughness += ToughnessIncrease;
                        tList[ti].StaminaTrainingCount++;
                        break;
                    }
                case StatusType.power:
                    {
                        int PowerIncrease = 9;
                        int StaminaIncrease = 5;
                        if (DateTime.Now.Date.Subtract(new DateTime(2022, 10, 15)).Days == 0
                            || DateTime.Now.Date.Subtract(new DateTime(2022, 10, 16)).Days == 0)
                        {
                            PowerIncrease = 14;
                            StaminaIncrease = 7;
                        }
                        else if (tList[ti].SpeedTrainingCount >= 0)
                        {
                            PowerIncrease = 9;
                            StaminaIncrease = 5;
                        }
                        uList[ui].power += PowerIncrease;
                        uList[ui].stamina += StaminaIncrease;
                        tList[ti].PowerTrainingCount++;
                        break;
                    }
                case StatusType.toughness:
                    {
                        int ToughnessIncrease = 9;
                        int SpeedIncrease = 4;
                        int PowerIncrease = 4;
                        if (DateTime.Now.Date.Subtract(new DateTime(2022, 10, 15)).Days == 0
                            || DateTime.Now.Date.Subtract(new DateTime(2022, 10, 16)).Days == 0)
                        {
                            ToughnessIncrease = 14;
                            SpeedIncrease = 5;
                            PowerIncrease = 5;
                        }
                        else if (tList[ti].SpeedTrainingCount >= 0)
                        {
                            ToughnessIncrease = 9;
                            SpeedIncrease = 4;
                            PowerIncrease = 4;
                        }
                        uList[ui].toughness += ToughnessIncrease;
                        uList[ui].speed += SpeedIncrease;
                        uList[ui].power += PowerIncrease;
                        tList[ti].ToughnessTrainingCount++;
                        break;
                    }
                case StatusType.intelligence:
                    {
                        int IntelligenceIncrease = 10;
                        int SpeedIncrease = 2;

                        if (DateTime.Now.Date.Subtract(new DateTime(2022, 10, 15)).Days == 0
                            || DateTime.Now.Date.Subtract(new DateTime(2022, 10, 16)).Days == 0)
                        {
                            IntelligenceIncrease = 15;
                            SpeedIncrease = 4;
                        }
                        else if (tList[ti].SpeedTrainingCount >= 0)
                        {
                            IntelligenceIncrease = 10;
                            SpeedIncrease = 2;
                        }
                        uList[ui].intelligence += IntelligenceIncrease;
                        uList[ui].speed += SpeedIncrease;
                        tList[ti].IntelligenceTrainingCount++;
                        break;
                    }
            }

            tList[ti].date = DateTime.Now.Date;
            JSONManager.SetTrainingInfo(tList);
            JSONManager.SetUmamusumeList(uList);
        }

        public static String DidTraining(UInt64 ownerID)
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
                    tList.Add(new Training(ownerID));
                }
            }

            int ti = tList.FindIndex(t => t.ownerID == ownerID);
            int ui = uList.FindIndex(u => u.ownerID == ownerID);

            TimeSpan timeSpan = DateTime.Now.Date - tList[ti].date.Date;
            if (timeSpan.Days > 0)
                return "■ 오늘의 트레이닝을 아직 진행하지 않았습니다.";
            else
                return "■ 오늘의 트레이닝을 이미 진행했습니다. 내일 트레이닝을 진행해 주세요.";

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
        public DateTime date;

        public int SpeedTrainingCount;
        public int StaminaTrainingCount;
        public int PowerTrainingCount;
        public int ToughnessTrainingCount;
        public int IntelligenceTrainingCount;

        public Training()
        {
            this.ownerID = 0;
            this.date = DateTime.MinValue;

            SpeedTrainingCount = 0;
            StaminaTrainingCount = 0;
            PowerTrainingCount = 0;
            ToughnessTrainingCount = 0;
            IntelligenceTrainingCount = 0;
        }

        public Training(UInt64 ownerID)
        {
            this.ownerID = ownerID;
            this.date = DateTime.MinValue;

            SpeedTrainingCount = 0;
            StaminaTrainingCount = 0;
            PowerTrainingCount = 0;
            ToughnessTrainingCount = 0;
            IntelligenceTrainingCount = 0;
        }

        [JsonConstructor]
        public Training(UInt64 ownerID, DateTime date, int SpeedTrainingCount, int StaminaTrainingCount, int PowerTrainingCount, int ToughnessTrainingCount, int IntelligenceTrainingCount)
        {
            this.ownerID = ownerID;
            this.date = date;

            this.SpeedTrainingCount = SpeedTrainingCount;
            this.StaminaTrainingCount = StaminaTrainingCount;
            this.PowerTrainingCount = PowerTrainingCount;
            this.ToughnessTrainingCount = ToughnessTrainingCount;
            this.IntelligenceTrainingCount = IntelligenceTrainingCount;
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
