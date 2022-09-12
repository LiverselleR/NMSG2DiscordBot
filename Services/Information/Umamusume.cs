using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Discord;

namespace NMSG2DiscordBot
{
    public class Umamusume
    {
        public string name;         // 우마무스메 이름
        public UInt64 ownerID;      // 오너 아이디 (디스코드 아이디)

        // 기본 스탯
        public int speed;                   // 속도 - 최고 속도
        public int stamina;                 // 최대 체력
        public int power;                   // 근력 - 가속력, 코스 잡기, 돌파력
        public int toughness;               // 근성 - 종반 라스트 스퍼트, 달라붙기, 스태미너 보조
        public int intelligence;            // 지능 - 지구력 관리, 스킬 사용 빈도, 디버프 회피

        // 각질 적성
        public Aptitude runawayAptitude;    // 도주
        public Aptitude frontAptitude;      // 선행
        public Aptitude fiAptitude;         // 선출
        public Aptitude stretchAptitude;    // 추입

        // 경기장 적성
        public Aptitude grassAptitude;      // 잔디
        public Aptitude durtAptitude;       // 더트

        // 거리 적성
        public Aptitude shortAptitude;      // 단거리 적성
        public Aptitude mileAptitude;       // 마일 적성
        public Aptitude middleAptitude;     // 중거리 적성
        public Aptitude longAptitude;       // 장거리 적성



        public Umamusume()
        {
            this.name = "테스트";
            this.ownerID = 0;

            this.speed = 600;
            this.stamina = 600;
            this.power = 600;
            this.toughness = 600;
            this.intelligence = 600;

            this.runawayAptitude = Aptitude.A;
            this.frontAptitude = Aptitude.A;
            this.fiAptitude = Aptitude.A;
            this.stretchAptitude = Aptitude.A;

            this.grassAptitude = Aptitude.A;
            this.durtAptitude = Aptitude.A;

            this.shortAptitude = Aptitude.A;
            this.mileAptitude = Aptitude.A;
            this.middleAptitude = Aptitude.A;
            this.longAptitude = Aptitude.A;

        }

        public Umamusume(string name, UInt64 ownerId,
                        int speed, int stamina, int power, int toughness, int intelligence,
                        Aptitude runawayAptitude, Aptitude frontAptitude, Aptitude fiAptitude, Aptitude stretchAptitude,
                        Aptitude grassAptitude, Aptitude durtAptitude,
                        Aptitude shortAptitude, Aptitude mileAptitude, Aptitude middleAptitude, Aptitude longAptitude)
        {
            this.name = name;
            this.ownerID = ownerId;

            this.speed = speed;
            this.stamina = stamina;
            this.power = power;
            this.toughness = toughness;
            this.intelligence = intelligence;

            this.runawayAptitude = runawayAptitude;
            this.frontAptitude = frontAptitude;
            this.fiAptitude = fiAptitude;
            this.stretchAptitude = stretchAptitude;

            this.grassAptitude = grassAptitude;
            this.durtAptitude = durtAptitude;

            this.shortAptitude = shortAptitude;
            this.mileAptitude = mileAptitude;
            this.middleAptitude = middleAptitude;
            this.longAptitude = longAptitude;
        }


        public static Umamusume FindeUmamusume(UInt64 ownerID, List<Umamusume> uList)
        {
            Umamusume u = uList.Find(u => u.ownerID == ownerID);
            return u;
        }

        public override string ToString()
        {
            List<Trainer> trainerList = JSONManager.GetTrainerList();
            List<Team> teamList = JSONManager.GetTeamList();
            String teamName;
            String teamTrainerName;

            Team team = teamList.Find(t => t.FindUmamusume(this.ownerID));
            if (team == null)
            {
                teamName = "없음";
                teamTrainerName = "없음";
            }
            else
            {
                teamName = team.name;
                if (team.trainerOwnerID == 0) teamTrainerName = "없음";
                else teamTrainerName = trainerList.Find(t => t.ownerID == team.trainerOwnerID).name;
            }

            StringBuilder sb = new StringBuilder("◎이름 - " + name + " ( 소속 팀 : " + teamName + " / 전담 트레이너 : " + teamTrainerName + " ) \n"
                + "■ 스테이터스 - 스피드 : " + speed + " / 스태미나 : " + stamina + " / 근력 : " + power + " / 근성 : " + toughness + " / 지능 : " + intelligence + "\n"
                + "■ 각질적성 - 도주 : " + runawayAptitude.ToString() + " / 선행 : " + frontAptitude.ToString() + " / 선입 : " + fiAptitude.ToString() + " / 추입 : " + stretchAptitude.ToString() + "\n"
                + "■ 마장적성 - 잔디 : " + grassAptitude.ToString() + " / 더트 : " + durtAptitude.ToString() + "\n"
                + "■ 거리적성 - 단거리 : " + shortAptitude.ToString() + " / 마일 : " + mileAptitude.ToString() + " / 중거리 : " + middleAptitude.ToString() + " / 장거리 : " + longAptitude.ToString());
            sb.Replace('_', ' ');
            return sb.ToString();
        }

        public string ToNameString()
        {
            return name;
        }

        public static List<Umamusume> GetTestUList()
        {
            List<Umamusume> result = new List<Umamusume>();

            result.Add(
                new Umamusume("파란 천둥 (도주)", 0, 150, 150, 150, 90, 90,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.C,
                Aptitude.C, Aptitude.D,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.D));
            result.Add(
                new Umamusume("블랙갈릭 캐슬 (도주)", 0, 150, 150, 150, 90, 90,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.C,
                Aptitude.C, Aptitude.D,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.D));
            result.Add(
                new Umamusume("호크 레드아이 (선행)", 0, 150, 150, 150, 90, 90,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.C,
                Aptitude.C, Aptitude.D,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.D));
            result.Add(
                new Umamusume("블루오션 씨걸 (선행)", 0, 150, 150, 150, 90, 90,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.C,
                Aptitude.C, Aptitude.D,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.D));
            result.Add(
                new Umamusume("웨이브 서퍼 (선입)", 0, 150, 150, 150, 90, 90,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.C,
                Aptitude.C, Aptitude.D,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.D));
            result.Add(
                new Umamusume("레인보우 서퍼 (선입)", 0, 150, 150, 150, 90, 90,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.C,
                Aptitude.C, Aptitude.D,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.D));
            result.Add(
                new Umamusume("후유마츠리 (추입)", 0, 150, 150, 150, 90, 90,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.C,
                Aptitude.C, Aptitude.D,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.D));
            result.Add(
                new Umamusume("썬더 샤베트 (추입)", 0, 150, 150, 150, 90, 90,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.C,
                Aptitude.C, Aptitude.D,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.D));

            return result;
        }
        public static List<Umamusume> GetTempUList()
        {
            List<Umamusume> result = new List<Umamusume>();

            result.Add(
                new Umamusume("스타라이트 허니", 0, 270, 270, 270, 150, 150,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.C,
                Aptitude.C, Aptitude.D,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.D));
            result.Add(
                new Umamusume("다프네 더블", 0, 270, 270, 270, 150, 150,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.C,
                Aptitude.C, Aptitude.D,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.D));
            result.Add(
                new Umamusume("시후 내추럴", 0, 270, 270, 270, 150, 150,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.C,
                Aptitude.C, Aptitude.D,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.D));
            result.Add(
                new Umamusume("블랙 모카 번", 0, 270, 270, 270, 150, 150,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.C,
                Aptitude.C, Aptitude.D,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.D));
            result.Add(
                new Umamusume("섬머 아이스", 0, 270, 270, 270, 150, 150,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.C,
                Aptitude.C, Aptitude.D,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.D));
            result.Add(
                new Umamusume("윈터 신기루", 0, 270, 270, 270, 150, 150,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.C,
                Aptitude.C, Aptitude.D,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.D));
            result.Add(
                new Umamusume("미드나잇 노크", 0, 270, 270, 270, 150, 150,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.C,
                Aptitude.C, Aptitude.D,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.D));
            result.Add(
                new Umamusume("화이트 이사벨라", 0, 270, 270, 270, 150, 150,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.C,
                Aptitude.C, Aptitude.D,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.D));
            result.Add(
                new Umamusume("레드 얼럿", 0, 270, 270, 270, 150, 150,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.C,
                Aptitude.C, Aptitude.D,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.D));
            result.Add(
                new Umamusume("로마노바 시니", 0, 270, 270, 270, 150, 150,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.C,
                Aptitude.C, Aptitude.D,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.D));
            result.Add(
                new Umamusume("바유시키 바유", 0, 270, 270, 270, 150, 150,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.C,
                Aptitude.C, Aptitude.D,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.D));

            return result;
        }


    }

    public enum RunningStyle
    {
        Runaway = 1,        // 도주
        Front = 2,          // 선행  
        FI = 3,             // 선출
        Stretch = 4         // 추입
    }

    public enum Aptitude
    {
        S = 1,
        A = 2,
        B = 3,
        C = 4,
        D = 5,
        E = 6,
        F = 7,
        G = 8
    }



}
