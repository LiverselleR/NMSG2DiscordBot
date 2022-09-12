using System;
using System.Collections.Generic;
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
                pList.Add(new Participant(e.uList[i], d, racetrack, e.rsList[i], i + 1));
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
            sl.Add(r.turn.GetResultRank());
            return sl;
        }
    }
}
