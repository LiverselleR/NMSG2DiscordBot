using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Newtonsoft.Json;

namespace NMSG2DiscordBot
{
    public class Derby
    {
        public int id; 
        public String derbyName;
        public int numberParticipants;                  // 참가자 수
        public StatusType statusType;                   // 경기 주요 스테이터스
        public TurfCondition turfCondition;             // 마장 상태

        public Derby()
        {
            id = -1;
            this.derbyName = "테스트더비";
            this.numberParticipants = 8;
            this.statusType = StatusType.speed;
            turfCondition = TurfCondition.normal;

        }

        [JsonConstructor]
        public Derby(int id, String derbyName, int numberParticipants, StatusType statusType)
        {
            this.id = id;
            this.derbyName = derbyName;
            this.numberParticipants = numberParticipants;
            this.statusType = statusType;
            turfCondition = TurfCondition.normal;
        }

        
        public static List<RunningStyle> TestDerby_RunningStyle()
        {
            Derby test = new Derby();
            Racetrack racetrack = JSONManager.GetRacetrackList()[0];

            Console.WriteLine(racetrack.partLength.Count);
            
            List<Umamusume> entryList = Umamusume.GetTestUList();
            List<Participant> entry = new List<Participant>();

            entry.Add(new Participant(entryList[0], test, racetrack, RunningStyle.Runaway, 3));
            entry.Add(new Participant(entryList[1], test, racetrack, RunningStyle.Runaway, 4));
            entry.Add(new Participant(entryList[2], test, racetrack, RunningStyle.Front, 5));
            entry.Add(new Participant(entryList[3], test, racetrack, RunningStyle.Front, 6));
            entry.Add(new Participant(entryList[4], test, racetrack, RunningStyle.FI, 7));
            entry.Add(new Participant(entryList[5], test, racetrack, RunningStyle.FI, 8));
            entry.Add(new Participant(entryList[6], test, racetrack, RunningStyle.Stretch, 9));
            entry.Add(new Participant(entryList[7], test, racetrack, RunningStyle.Stretch, 10));

            Race r = new Race(test, entry);
            r.Proceed();
            return r.turn.GetRunningStyles();
        }
        public static List<String> TestDerby_Process()
        {
            Derby test = new Derby();
            Racetrack racetrack = JSONManager.GetRacetrackList()[0];
            List<Umamusume> entryList = Umamusume.GetTestUList();
            List<Participant> entry = new List<Participant>();

            entry.Add(new Participant(entryList[0], test, racetrack, RunningStyle.Runaway, 3));
            entry.Add(new Participant(entryList[1], test, racetrack, RunningStyle.Runaway, 4));
            entry.Add(new Participant(entryList[2], test, racetrack, RunningStyle.Front, 5));
            entry.Add(new Participant(entryList[3], test, racetrack, RunningStyle.Front, 6));
            entry.Add(new Participant(entryList[4], test, racetrack, RunningStyle.FI, 7));
            entry.Add(new Participant(entryList[5], test, racetrack, RunningStyle.FI, 8));
            entry.Add(new Participant(entryList[6], test, racetrack, RunningStyle.Stretch, 9));
            entry.Add(new Participant(entryList[7], test, racetrack, RunningStyle.Stretch, 10));

            Race r = new Race(test, entry);
            r.Proceed();
            List<String> raceDetailLog = r.turn.GetDetailLog();

            String path = AppDomain.CurrentDomain.BaseDirectory + @"/RaceLog";

            DirectoryInfo dl = new DirectoryInfo(path);
            if (dl.Exists == false) dl.Create();


            String LogPath = path + @"/Log_TestDerby" + "_" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".txt";

            if (!File.Exists(LogPath))
            {
                using (FileStream fs = File.Create(LogPath))
                {
                    StreamWriter sw = new StreamWriter(fs);
                    foreach(String raceDetailLogLine in raceDetailLog)
                    {
                        sw.WriteLine(raceDetailLogLine);
                    }
                    sw.Close();
                }
            }

            return r.turn.GetLog();
        }
    
        public CoursePhase GetCoursePhase(double currPosition)
        {
            /// 초반 : 스타트 후 직선 코스에서 경쟁
            /// 중반 : 후반 가기 전 컨디션 조절
            /// 후반 : 최후 코너 + 최후 직선

            int prev = 0;
            int curr = 0;
            int currIndex;

            List<Racetrack> racetracks = JSONManager.GetRacetrackList();
            Racetrack racetrack = racetracks.Find(rt => rt.id == id);

            int racetrackLength = racetrack.GetTrackLength();

            for (currIndex = 0; currIndex < racetrack.partLength.Count; currIndex++)
            {
                prev = curr;
                curr = curr + racetrack.partLength[currIndex];
                if (currPosition >= prev && currPosition < curr)
                    break;
            }

            if (currPosition < racetrackLength / 6) return CoursePhase.First;
            else if (currPosition < racetrackLength * 2 / 3) return CoursePhase.Middle;
            else return CoursePhase.Last;
        }
        
            
    }

    public enum CourseType
    {
        curve = 1,
        straight = 2
    }
    public enum StatusType
    {
        speed = 1,
        stamina = 2, 
        power = 3,
        toughness = 4,
        intelligence = 5
    }
    public enum CoursePhase
    {
        First = 1,
        Middle = 2,
        Last = 3
    }
}
