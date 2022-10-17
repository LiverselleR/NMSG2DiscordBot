using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NMSG2DiscordBot
{
    public static class Racemanager
    {
        public static List<String> RaceStart(String derbyName)
        {
            List<Derby> dList = JSONManager.GetDerbyList();
            List<Entry> eList = JSONManager.GetEntryList();
            List<Participant> pList = new List<Participant>();

            Derby d = dList.Find(d => d.derbyName == derbyName);
            if (d == null)
                throw new DerbyNameNotFoundException();
            Entry e = eList.Find(e => e.derbyID == d.id);

            Racetrack racetrack = JSONManager.GetRacetrackList().Find(rt => rt.id == d.id);

            for(int i = 0; i < e.uList.Count; i++)
            {
                pList.Add(new Participant(e.uList[i], d, racetrack, e.rsList[i], i + 1, true));
            }
            if(e.uList.Count < d.numberParticipants)
            {
                List<Umamusume> uList = Umamusume.GetTempUList();
                for(int i = e.uList.Count; i < d.numberParticipants; i++)
                {
                    RunningStyle temprs;
                    if (i - e.uList.Count <= 1) temprs = RunningStyle.Runaway;
                    else if (i - e.uList.Count <= 3) temprs = RunningStyle.Front;
                    else if (i - e.uList.Count <= 5) temprs = RunningStyle.FI;
                    else /* if (i - e.uList.Count <= 7) */ temprs = RunningStyle.Stretch;
                    
                    pList.Add(new Participant(uList[i - e.uList.Count], d, racetrack, temprs, i + 1));
                }
            }
            Race r = new Race(d, pList);
            r.Proceed();
            List<String> sl = r.turn.GetLog();
            List<String> raceDetailLog = r.turn.GetDetailLog();
            sl.Add(r.turn.GetResultRank());

            String path = string.Format("{0}/RaceLog/{1}", AppDomain.CurrentDomain.BaseDirectory, derbyName);

            DirectoryInfo dl = new DirectoryInfo(path);
            if (dl.Exists == false) dl.Create();

            String LogPath = path + @"/Log_" + derbyName + "_" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".txt";

            if (!File.Exists(LogPath))
            {
                using (FileStream fs = File.Create(LogPath))
                {
                    StreamWriter sw = new StreamWriter(fs);
                    foreach (String raceDetailLogLine in raceDetailLog)
                    {
                        sw.WriteLine(raceDetailLogLine);
                    }
                    sw.Close();
                }
            }

            String CSVLogDirectoryPath = string.Format("{0}/Participant", path);
            DirectoryInfo csvdl = new DirectoryInfo(path);
            if (csvdl.Exists == false) dl.Create();

            Dictionary<String, List<String>> participantsLog = r.turn.GetCSVLog();

            foreach(KeyValuePair<String, List<String>> participantLog in participantsLog)
            {
                String CSVLogPath = string.Format("{0}/Log_{1}_{2}_{3}.csv", path, derbyName,
                    DateTime.Now.ToString("yyyyMMdd-HHmmss"), participantLog.Key);

                if (!File.Exists(CSVLogPath))
                {
                    using (FileStream fs = File.Create(CSVLogPath))
                    {
                        StreamWriter sw = new StreamWriter(fs);
                        foreach (String csvLogLine in participantLog.Value)
                        {
                            sw.WriteLine(csvLogLine);
                        }
                        sw.Close();
                    }
                }
            }

            return sl;
        }
    }
}
