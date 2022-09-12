using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace NMSG2DiscordBot
{
    public static class RaceEntryManager
    {
        public static void Entry(UInt64 ownerID, String derbyName, String runningStyleString)
        {
            List<Umamusume> uList = JSONManager.GetUmamusumeList();
            List<Derby> dList = JSONManager.GetDerbyList();
            List<Entry> eList = JSONManager.GetEntryList();

            Umamusume u = uList.Find(u => u.ownerID == ownerID);
            Derby d = dList.Find(d => d.derbyName == derbyName);
            RunningStyle runningStyle;
            switch(runningStyleString)
            {
                case "도주":
                    runningStyle = RunningStyle.Runaway;
                    break;
                case "선행":
                    runningStyle = RunningStyle.Front;
                    break;
                case "선입":
                    runningStyle = RunningStyle.FI;
                    break;
                case "추입":
                    runningStyle = RunningStyle.Stretch;
                    break;
                default:
                    throw new ArgumentException();
            }

            if (u == null) throw new DiscordIDNotRegisteredException();
            if (d == null) throw new DerbyNameNotFoundException();

            int eIndex = eList.FindIndex(e => e.derbyID == d.id);
            Entry e;
            if(eIndex == -1)
            {
                e = new Entry(d.id, new List<Umamusume>(), new List<RunningStyle>());
                eList.Add(e);
                eIndex = eList.FindIndex(e => e.derbyID == d.id);
            }
            else
            {
                e = eList[eIndex];
                if (e.uList.Find(temp => temp.ownerID == u.ownerID) != null)
                    throw new UmamusumeAlreadyRegisteredException();
            }

            eList[eIndex].uList.Add(u);
            eList[eIndex].rsList.Add(runningStyle);

            JSONManager.SetEntryList(eList);

            return;
        }
    }

    public class Entry
    {
        public int derbyID;
        public List<Umamusume> uList;
        public List<RunningStyle> rsList;

        public Entry()
        {
            this.derbyID = -1;
            this.uList = new List<Umamusume>();
            this.rsList = new List<RunningStyle>();
        }

        public Entry(int derbyID, List<Umamusume> uList, List<RunningStyle> rsList)
        {
            this.derbyID = derbyID;
            this.uList = uList;
            this.rsList = rsList;
        }
    }


    public class DerbyNameNotFoundException : Exception
    {
        public DerbyNameNotFoundException()
        {
        }

        public DerbyNameNotFoundException(string message) : base(message)
        {
        }

        public DerbyNameNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DerbyNameNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
