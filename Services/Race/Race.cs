using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMSG2DiscordBot
{
    public class Race
    {
        public Derby derby;
        public List<Participant> entry;
        public List<String> turnLog;
        public Turn turn;

        [JsonConstructor]
        public Race(Derby derby, List<Participant> entry)
        {
            this.derby = derby;
            this.entry = entry;
            turnLog = new List<string>();

            List<Racetrack> racetracks = JSONManager.GetRacetrackList();
            Racetrack racetrack = racetracks.Find(rt => rt.id == derby.id);
            Console.WriteLine(racetrack.partLength.Count);

            turn = new Turn(entry, racetrack.partType);
        }

        public void Proceed()
        {
            while(turn.IsRaceOver())
            {
                turn.Process();
            }

            turnLog = turn.GetLog();
            turnLog.Add(turn.GetResultRank());
            return;
        }
               
    }

    public enum TurfCondition
    {
        normal = 1,
        littleHeavy = 2,
        heavy = 3,
        bad = 4
    }
}
