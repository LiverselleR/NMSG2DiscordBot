using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NMSG2DiscordBot
{
    public class JSONManager
    {
        public static void Initialize()
        {
            String path = AppDomain.CurrentDomain.BaseDirectory + @"/Data";

            DirectoryInfo dl = new DirectoryInfo(path);
            if (dl.Exists == false) dl.Create();

            String uPath = path + @"/Umamusume.json";
            String trainingPath = path + @"/Training.json";
            String trainerPath = path + @"/Trainer.json";
            String teamPath = path + @"/Team.json";
            String derbyPath = path + @"/Derby.json";
            String entryPath = path + @"/Entry.json";
            String racetrackPath = path + @"/Racetrack.json";

            if (!File.Exists(uPath))
            {
                using (FileStream fs = File.Create(uPath))
                {
                    JArray dataset = new JArray();
                    Umamusume u = new Umamusume();
                    dataset.Add(JObject.FromObject(u));
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine(dataset.ToString());
                    sw.Close();
                }
            }
            if (!File.Exists(trainingPath))
            {
                using (FileStream fs = File.Create(trainingPath))
                {
                    JArray dataset = new JArray();
                    Training training = new Training();
                    dataset.Add(JObject.FromObject(training));
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine(dataset.ToString());
                    sw.Close();
                }
            }
            if (!File.Exists(trainerPath))
            {
                using (FileStream fs = File.Create(trainerPath))
                {
                    JArray dataset = new JArray();
                    Trainer trainer = new Trainer();
                    dataset.Add(JObject.FromObject(trainer));
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine(dataset.ToString());
                    sw.Close();
                }
            }
            if (!File.Exists(teamPath))
            {
                using (FileStream fs = File.Create(teamPath))
                {
                    JArray dataset = new JArray();
                    Team team = new Team();
                    dataset.Add(JObject.FromObject(team));
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine(dataset.ToString());
                    sw.Close();
                }
            }
            if (!File.Exists(derbyPath))
            {
                using (FileStream fs = File.Create(derbyPath))
                {
                    JArray dataset = new JArray();
                    Derby d = new Derby();
                    dataset.Add(JObject.FromObject(d));
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine(dataset.ToString());
                    sw.Close();
                }
            }
            if (!File.Exists(entryPath))
            {
                using (FileStream fs = File.Create(entryPath))
                {
                    JArray dataset = new JArray();
                    Entry entry = new Entry();
                    dataset.Add(JObject.FromObject(entry));
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine(dataset.ToString());
                    sw.Close();
                }
            }
            if (!File.Exists(racetrackPath))
            {
                using (FileStream fs = File.Create(racetrackPath))
                {
                    JArray dataset = new JArray();
                    Racetrack r = new Racetrack(0, new List<int>(), new List<CourseType>(), new List<double>(), 30, 110, FieldType.durt);
                    dataset.Add(JObject.FromObject(r));
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine(dataset.ToString());
                    sw.Close();
                }
            }
        }
        public static List<Umamusume> GetUmamusumeList()
        {
            List<Umamusume> uList = new List<Umamusume>();
            String path = AppDomain.CurrentDomain.BaseDirectory + @"/Data";
            JArray dataset = new JArray();

            DirectoryInfo dl = new DirectoryInfo(path);
            if (dl.Exists == false) dl.Create();

            path = path + @"/Umamusume.json";
            using (FileStream fs = File.OpenRead(path))
            {
                StreamReader sr = new StreamReader(fs);
                dataset = JArray.Parse(sr.ReadToEnd());
                uList = dataset.ToObject<List<Umamusume>>();
            }

            return uList;
        }
        public static void SetUmamusumeList(List<Umamusume> uList)
        {
            String path = AppDomain.CurrentDomain.BaseDirectory + @"/Data";
            JArray dataset = new JArray();

            DirectoryInfo dl = new DirectoryInfo(path);
            if (dl.Exists == false) dl.Create();

            path = path + @"/Umamusume.json";

            dataset = JArray.FromObject(uList);
            File.WriteAllText(path, dataset.ToString());

        }

        public static List<Training> GetTrainingInfo()
        {
            List<Training> trainList = new List<Training>();
            List<Umamusume> uList = GetUmamusumeList();
            String path = AppDomain.CurrentDomain.BaseDirectory + @"/Data";
            JArray dataset = new JArray();

            DirectoryInfo dl = new DirectoryInfo(path);
            if (dl.Exists == false) dl.Create();

            path = path + @"/Training.json";

            if (!File.Exists(path))
            {
                File.Create(path);
            }
            else
            {
                using (FileStream fs = File.OpenRead(path))
                {
                    StreamReader sr = new StreamReader(fs);
                    dataset = JArray.Parse(sr.ReadToEnd());
                    trainList = dataset.ToObject<List<Training>>();
                }
            }

            return trainList;
        }
        public static void SetTrainingInfo(List<Training> tList)
        {
            String path = AppDomain.CurrentDomain.BaseDirectory + @"/Data";
            JArray dataset = new JArray();
            FileStream fs;

            DirectoryInfo dl = new DirectoryInfo(path);
            if (dl.Exists == false) dl.Create();

            path = path + @"/Training.json";

            dataset = JArray.FromObject(tList);
            File.WriteAllText(path, dataset.ToString());
        }

        public static List<Trainer> GetTrainerList()
        {
            List<Trainer> tList = new List<Trainer>();
            String path = AppDomain.CurrentDomain.BaseDirectory + @"/Data";
            JArray dataset = new JArray();

            DirectoryInfo dl = new DirectoryInfo(path);
            if (dl.Exists == false) dl.Create();

            path = path + @"/Trainer.json";

            using (FileStream fs = File.OpenRead(path))
            {
                StreamReader sr = new StreamReader(fs);
                dataset = JArray.Parse(sr.ReadToEnd());
                tList = dataset.ToObject<List<Trainer>>();
            }

            return tList;
        }
        public static void SetTrainerList(List<Trainer> tList)
        {
            String path = AppDomain.CurrentDomain.BaseDirectory + @"/Data";
            JArray dataset = new JArray();

            DirectoryInfo dl = new DirectoryInfo(path);
            if (dl.Exists == false) dl.Create();

            path = path + @"/Trainer.json";

            dataset = JArray.FromObject(tList);
            File.WriteAllText(path, dataset.ToString());

            return;
        }

        public static List<Team> GetTeamList()
        {
            List<Team> tList = new List<Team>();
            String path = AppDomain.CurrentDomain.BaseDirectory + @"/Data";
            JArray dataset = new JArray();

            DirectoryInfo dl = new DirectoryInfo(path);
            if (dl.Exists == false) dl.Create();

            path = path + @"/Team.json";

            using (FileStream fs = File.OpenRead(path))
            {
                StreamReader sr = new StreamReader(fs);
                dataset = JArray.Parse(sr.ReadToEnd());
                tList = dataset.ToObject<List<Team>>();
            }

            return tList;
        }
        public static void SetTeamList(List<Team> tList)
        {
            String path = AppDomain.CurrentDomain.BaseDirectory + @"/Data";
            JArray dataset = new JArray();

            DirectoryInfo dl = new DirectoryInfo(path);
            if (dl.Exists == false) dl.Create();

            path = path + @"/Team.json";

            dataset = JArray.FromObject(tList);
            File.WriteAllText(path, dataset.ToString());
        
            return;
        }
        public static List<Derby> GetDerbyList()
        {
            List<Derby> dList = new List<Derby>();
            String path = AppDomain.CurrentDomain.BaseDirectory + @"/Data";
            JArray dataset = new JArray();

            DirectoryInfo dl = new DirectoryInfo(path);
            if (dl.Exists == false) dl.Create();

            path = path + @"/Derby.json";

            using (FileStream fs = File.OpenRead(path))
            {
                StreamReader sr = new StreamReader(fs);
                dataset = JArray.Parse(sr.ReadToEnd());
                dList = dataset.ToObject<List<Derby>>();
            }

            return dList;
        }
        public static void SetDerbyList(List<Derby> dList)
        {
            String path = AppDomain.CurrentDomain.BaseDirectory + @"/Data";
            JArray dataset = new JArray();

            DirectoryInfo dl = new DirectoryInfo(path);
            if (dl.Exists == false) dl.Create();

            path = path + @"/Derby.json";

            dataset = JArray.FromObject(dList);
            File.WriteAllText(path, dataset.ToString());
        
            return;
        }
        public static List<Entry> GetEntryList()
        {
            List<Entry> eList = new List<Entry>();
            String path = AppDomain.CurrentDomain.BaseDirectory + @"/Data";
            JArray dataset = new JArray();

            DirectoryInfo dl = new DirectoryInfo(path);
            if (dl.Exists == false) dl.Create();

            path = path + @"/Entry.json";

            using (FileStream fs = File.OpenRead(path))
            {
                StreamReader sr = new StreamReader(fs);
                dataset = JArray.Parse(sr.ReadToEnd());
                eList = dataset.ToObject<List<Entry>>();
            }

            return eList;
        }
        public static void SetEntryList(List<Entry> eList)
        {
            String path = AppDomain.CurrentDomain.BaseDirectory + @"/Data";
            JArray dataset = new JArray();

            DirectoryInfo dl = new DirectoryInfo(path);
            if (dl.Exists == false) dl.Create();

            path = path + @"/Entry.json";

            dataset = JArray.FromObject(eList);
            File.WriteAllText(path, dataset.ToString());
        
            return;
        }
        public static List<Racetrack> GetRacetrackList()
        {
            List<Racetrack> rList = new List<Racetrack>();
            String path = AppDomain.CurrentDomain.BaseDirectory + @"/Data";
            JArray dataset = new JArray();

            DirectoryInfo dl = new DirectoryInfo(path);
            if (dl.Exists == false) dl.Create();

            path = path + @"/Racetrack.json";

            using (FileStream fs = File.OpenRead(path))
            {
                StreamReader sr = new StreamReader(fs);
                dataset = JArray.Parse(sr.ReadToEnd());
                rList = dataset.ToObject<List<Racetrack>>();
            }

            return rList;
        }
        public static void SetRacetrackList(List<Racetrack> rList)
        {
            String path = AppDomain.CurrentDomain.BaseDirectory + @"/Data";
            JArray dataset = new JArray();

            DirectoryInfo dl = new DirectoryInfo(path);
            if (dl.Exists == false) dl.Create();

            path = path + @"/Racetrack.json";

            dataset = JArray.FromObject(rList);
            File.WriteAllText(path, dataset.ToString());
        
            return;
        }
    }
}
