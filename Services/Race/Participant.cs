using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Threading.Tasks;
using System.Numerics;
using Newtonsoft.Json;

namespace NMSG2DiscordBot
{
    public partial class Participant : IComparable
    {
        public String name;                             // 우마무스메 이름
        public RunningStyle runningStyle;               // 우마무스메 각질

        public Derby derby;                             // 더비 정보
        public Racetrack racetrack;                     // 더비 경기장 정보

        public int rank;                                // 더비 중 현재 순위
        public int prevRank;                            // 더비 중 전 턴 순위

        public Vector2 currPosition;                    // 더비용 현재 턴 포지션
        public Vector2 prevPosition;                    // 더비용 이전 턴 포지션
        public CoursePhase coursePhase;                 // 더비용 현재 페이즈

        public Boolean isGoal;                          // 더비용 현재 골 여부 확인
        public double goalTiming;                       // 더비용 골 타이밍 (골인한 경우 갱신)

        private double calibratedSpeed;                 // 보정 스피드
        private double calibratedStamina;               // 보정 체력
        private double calibratedPower;                 // 보정 파워
        private double calibratedToughness;             // 보정 근성
        private double calibratedIntelligence;          // 보정 지능

        private double defaultTargetSpeed;              // 기본 목표 속도
        private double normalTargetSpeed;               // 일반 목표 속도
        private double spurtTargetSpeed;                // 스퍼트 목표 속도
        private double minimumTargetSpeed;              // 최소 목표 속도
        private double maximumTargetSpeed;              // 최대 목표 속도
        private double targetSpeed;                     // 목표 속도 (위의 목표 속도들을 이용한 목표 속도)

        private double lengthAptitudeSpeedValue;        // 거리 적성 스피드값
        private double lengthAptitudePowerValue;        // 거리 적성 파워값
        private double fieldTypeAccelCalibrateValue;    // 마정 적성 가속 보정값


        private Boolean isPositionKeep;                 // 포지션 킵 여부
        private Boolean isForceInMove;                  // 포지션 무브 여부
        private PositionKeep positionKeep;              // 포지션 킵 상태 (현재 포지션 유지하면서 달림)
        private ForceInMove forceInMove;                // 포지션 무브 상태 (다른 코스로 전환하면서 달림)

        private Boolean isBlocked;                      // 블록당함 여부
        private int[,] surroundCheck;                   // 포위 여부 확인을 위한 카운터
        private readonly (int, int)[] eyesight          // 시야 단계별 범위
            = { (1, 3), (3, 3), (4, 3), (5, 3), (5, 5), (6, 5), (7, 5) };
        private int eyesightLevel;

        private Boolean isFever;                        // 흥분 여부
        private Boolean isSpurt;                        // 스퍼트 여부
        private Boolean isStartAccel;                   // 출발 스퍼트 가속 여부
       
        public double maxStamina;                      // 더비용 최대 스테미나
        public double currStamina;                     // 더비용 현재 스테미나
        public double currSpeed;                        // 더비용 현재 속도
        public double currAccel;                       // 더비용 현재 가속도 


        [JsonConstructor]
        public Participant(Umamusume u,
                           Derby d,
                           Racetrack r,
                           RunningStyle runningStyle,
                           float currPosition,
                           bool isUser = false)
        {
            name = u.name;
            this.runningStyle = runningStyle;

            derby = d;
            racetrack = r;

            rank = 0;
            prevRank = 0;

            this.currPosition = new Vector2(0, currPosition);
            this.prevPosition = this.currPosition;
            coursePhase = CoursePhase.First;

            isGoal = false;
            goalTiming = -1;

            // 보정 스탯
            calibratedSpeed = u.speed * GetCalibratingSpeedValue(d.statusType, u) + GetFieldTypeAptitudeSpeedValue();
            calibratedIntelligence = u.intelligence * GetRunningStyleAptitudeValue(u);
            calibratedPower = u.power + GetFieldTypeAptitudePowerValue();
            calibratedStamina = u.stamina;
            calibratedToughness = u.toughness;

            // 유저 우마무스메 스토리모드 보정
            //if(isUser)
            //{
            //    calibratedSpeed += 400;
            //    calibratedIntelligence += 400;
            //    calibratedPower += 400;
            //    calibratedStamina += 400;
            //    calibratedToughness += 400;
            //}

            lengthAptitudePowerValue = GetLengthAptitudePowerValue(u);
            lengthAptitudeSpeedValue = GetLengthAptitudeSpeedValue(u);

            // 목표 속도
            /// 기본 목표 속도
            defaultTargetSpeed = GetDefaultTargetSpeed();
            /// 일반 목표 속도
            normalTargetSpeed = GetNormalTargetSpeed();
            /// 스퍼트 목표 속도
            spurtTargetSpeed = GetSpurtTargetSpeed();
            /// 최대 목표 속도
            maximumTargetSpeed = 30;
            /// 최소 목표 속도
            minimumTargetSpeed = GetMinimumTargetSpeed();
            /// 실제 목표 속도
            targetSpeed = normalTargetSpeed;

            // 필드 적성 가속도 보정치
            fieldTypeAccelCalibrateValue = GetFieldTypeAccelCalibrateValue(u);

            // 포지션 킵/무브 여부 초기화
            isPositionKeep = false;
            isForceInMove = false;
            positionKeep = PositionKeep.non;
            forceInMove = ForceInMove.non;

            isBlocked = false;
            SetEyesightLevel();

            isFever = false;
            isSpurt = false;
            isStartAccel = true;

            
            // 최대 스태미너 초기화
            maxStamina = GetMaximumStamina();
            currStamina = maxStamina;
            currSpeed = 0;
            currAccel = 0;


        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            Participant otherParticipant = obj as Participant;
            if (otherParticipant != null)
            {
                if (this.isGoal && otherParticipant.isGoal) return this.rank.CompareTo(otherParticipant.rank);
                else if (this.isGoal) return -1;
                else if (otherParticipant.isGoal) return 1;
                else return this.currPosition.X.CompareTo(otherParticipant.currPosition.X) * -1;
            }
            else
                throw new ArgumentException("Object is not Participant");
        }

        public void TurnProcess(List<Participant> pList, int currTurn)
        {
            if(currTurn == 1000)
            {
                Console.WriteLine("{0} - 1000Turn", name);
            }
            if (isGoal) return;
            TurnActionDecide(pList, currTurn);
            TurnActionActivate();
            if (TurnSpecialSituationCheck(pList))
                TurnSpecialSituationProcess(pList);
            return;
        }
        public void TurnActionDecide(List<Participant> pList, int currTurn)
        {
            coursePhase = derby.GetCoursePhase(currPosition.X);

            CheckNear(pList);

            if(!isFever)
            {
                switch(derby.GetCoursePhase(currPosition.X))
                {
                    case CoursePhase.First:
                        FirstPhaseActionDecide(pList);
                        break;
                    case CoursePhase.Middle:
                        MiddlePhaseActionDecide(pList);
                        break;
                    case CoursePhase.Last:
                        LastPhaseActionDecide(pList);
                        break;
                    default:
                        NormalRun();
                        break;

                }
            }
            
            UpdateTargetSpeed();
            if (isStartAccel && ( targetSpeed <= currSpeed || currTurn >= 20))
                isStartAccel = false;
            return;
        }

        public void TurnActionActivate()
        {
            currAccel = GetAccel();
            currSpeed += currAccel / 20;
            prevPosition = currPosition;

            double currSpeedPerTurn = currSpeed / 20;
            
            if (isForceInMove)
            {
                switch (forceInMove)
                {
                    case ForceInMove.InsideCatchUp:
                    case ForceInMove.InsideOvertaking:
                    case ForceInMove.InsideMove:
                        {
                            currSpeedPerTurn = Math.Sqrt(Math.Pow(currSpeedPerTurn, 2) - 0.0025f);
                            currPosition.Y -= 0.05f;
                        }
                        break;
                    case ForceInMove.OutsideOvertaking:
                    case ForceInMove.OutsideMove:
                    case ForceInMove.OutsideCatchUp:
                        {
                            currSpeedPerTurn = Math.Sqrt(Math.Pow(currSpeedPerTurn, 2) - 0.0025f);
                            currPosition.Y += 0.05f;
                        }
                        break;
                    default:
                        break;
                }
            }


            CourseType courseType = racetrack.GetCourseType(currPosition.X);
            if (courseType == CourseType.curve)
                currSpeedPerTurn = racetrack.GetCurveMoveLength(currSpeedPerTurn, currPosition.Y);

            currPosition.X += (float) currSpeedPerTurn;

            if (currStamina > 0) 
                currStamina -= GetStaminaExhaustionSpeed(derby.turfCondition);

            return;
        }

        public Boolean TurnSpecialSituationCheck(List<Participant> pList)
        {
            return false;
        }
        public void TurnSpecialSituationProcess(List<Participant> pList)
        {
            return;
        }

        public void GoalCheck()
        {
            if (isGoal) return;
            else if(currPosition.X >= racetrack.GetTrackLength())
            {
                isGoal = true;
                goalTiming = (racetrack.GetTrackLength() - prevPosition.X) / (currPosition.X - prevPosition.X);
            }
        }

        public void RankRenewal(int currRank)
        {
             prevRank = this.rank;
             this.rank = currRank;
             GoalCheck();    
        }

        // 기본 속도 보정 - 더비에서 정해진 스탯 값에 따라 기본 보정
        private double GetCalibratingSpeedValue(StatusType statusType, Umamusume u)
        {
            double result;
            int standard;

            switch (statusType)
            {
                case StatusType.intelligence:
                    standard = u.intelligence;
                    break;
                case StatusType.power:
                    standard = u.power;
                    break;
                case StatusType.speed:
                    standard = u.speed;
                    break;
                case StatusType.stamina:
                    standard = u.stamina;
                    break;
                case StatusType.toughness:
                    standard = u.toughness;
                    break;
                default:
                    standard = 0;
                    break;
            }

            switch (standard / 300)
            {
                case 0:
                    result = 1.00;
                    break;
                case 1:
                    result = 1.05;
                    break;
                case 2:
                    result = 1.10;
                    break;
                case 3:
                    if (standard < 1000) result = 1.15;
                    else result = 1.20;
                    break;
                default:
                    result = 1.00;
                    break;

            }

            return result;
        }

        // 경마장 상태 보정 - 스피드
        private double GetFieldTypeAptitudeSpeedValue()
        {
            switch (derby.turfCondition)
            {
                case TurfCondition.bad:
                    return -50.0;
                default:
                    return 0;
            }
        }
        // 경마장 상태 보정 - 파워
        private double GetFieldTypeAptitudePowerValue()
        {
            switch (derby.turfCondition)
            {
                case TurfCondition.normal:
                    return 0;
                default:
                    return -50.0;
            }
        }
        // 각질 적성 보정
        private double GetRunningStyleAptitudeValue(Umamusume u)
        {
            Aptitude aptitude;

            switch (runningStyle)
            {
                case RunningStyle.Runaway:
                    aptitude = u.runawayAptitude;
                    break;
                case RunningStyle.Front:
                    aptitude = u.frontAptitude;
                    break;
                case RunningStyle.FI:
                    aptitude = u.fiAptitude;
                    break;
                case RunningStyle.Stretch:
                    aptitude = u.stretchAptitude;
                    break;
                default:
                    aptitude = u.runawayAptitude;
                    break;
            }

            switch (aptitude)
            {
                case Aptitude.S:
                    return 1.10;
                case Aptitude.A:
                    return 1.00;
                case Aptitude.B:
                    return 0.85;
                case Aptitude.C:
                    return 0.75;
                case Aptitude.D:
                    return 0.60;
                case Aptitude.E:
                    return 0.40;
                case Aptitude.F:
                    return 0.20;
                case Aptitude.G:
                    return 0.10;
                default:
                    return 0.10;

            }
        }

        // 기본 시야 단계
        public void SetEyesightLevel()
        {
            int quotient = (int)calibratedIntelligence / 150;
            if (isFever) quotient = 0;
            switch (quotient)
            {
                case 0:
                    eyesightLevel = 1;
                    break;
                case 1:
                    eyesightLevel = 2;
                    break;
                case 2:
                    eyesightLevel = 3;
                    break;
                case 3:
                    eyesightLevel = 4;
                    break;
                case 4:
                case 5:
                    eyesightLevel = 5;
                    break;
                case 6:
                case 7:
                    eyesightLevel = 6;
                    break;
                default:
                    eyesightLevel = 0;
                    break;

            }
            surroundCheck = new int[eyesight[eyesightLevel].Item1 + 1, eyesight[eyesightLevel].Item2];

            return;
        }

        // 기본 목표 속도 계산
        private double GetDefaultTargetSpeed()
        {
            return (Math.Sqrt(500 * calibratedSpeed)) * lengthAptitudeSpeedValue * 0.002
                + GetRaceReferenceSpeed() * (GetRunningStyleCalibratingValue() + GetIntelligenceRandomCalibratingValue());
        }
        /// 레이스 타입 확인
        private Aptitude GetLengthAptitude(Umamusume u)
        {
            Aptitude aptitude;
            if (racetrack.GetTrackLength() < 1600)
            {
                aptitude = u.shortAptitude;
            }
            else if (racetrack.GetTrackLength() < 2000)
            {
                aptitude = u.mileAptitude;
            }
            else if (racetrack.GetTrackLength() <= 2400)
            {
                aptitude = u.middleAptitude;
            }
            else // length > 2400
            {
                aptitude = u.longAptitude;
            }

            return aptitude;
        }
        /// 거리 적성 보정 - 스피드  
        private double GetLengthAptitudeSpeedValue(Umamusume u)
        {
            Aptitude aptitude;

            aptitude = GetLengthAptitude(u);

            switch (aptitude)
            {
                case Aptitude.S:
                    return 1.05;
                case Aptitude.A:
                    return 1.00;
                case Aptitude.B:
                    return 0.90;
                case Aptitude.C:
                    return 0.80;
                case Aptitude.D:
                    return 0.60;
                case Aptitude.E:
                    return 0.40;
                case Aptitude.F:
                    return 0.20;
                case Aptitude.G:
                    return 0.10;
                default:
                    return 0.10;

            }
        }
        /// 거리 적성 보정 - 파워
        private double GetLengthAptitudePowerValue(Umamusume u)
        {
            Aptitude aptitude;

            aptitude = GetLengthAptitude(u);

            switch (aptitude)
            {
                case Aptitude.E:
                    return 0.60;
                case Aptitude.F:
                    return 0.50;
                case Aptitude.G:
                    return 0.40;
                default:
                    return 1.00;
            }
        }
        /// 레이스 기준속도 (레이스 길이에 따라 달라짐)
        private double GetRaceReferenceSpeed()
        {
            double result = 20;

            result = result + (double) (2000 - racetrack.GetTrackLength()) / 1000;

            return result;
        }
        /// 각질 속도 보정
        private double GetRunningStyleCalibratingValue()
        {
            switch (runningStyle)
            {
                case RunningStyle.Runaway:
                    {
                        if (isSpurt) 
                            return 0.962;
                        else if (coursePhase == CoursePhase.First)
                            return 1.000;
                        else if (coursePhase == CoursePhase.Middle)
                            return 0.980;
                        else  /* coursePhase == CoursePhase.Last */
                            return 0.962;
                    }
                case RunningStyle.Front:
                    {
                        if (isSpurt)
                            return 0.975;
                        else if (coursePhase == CoursePhase.First)
                            return 0.978;
                        else if (coursePhase == CoursePhase.First) 
                            return 0.991;
                        else /* coursePhase == CoursePhase.First */ 
                            return 0.975;
                    }
                case RunningStyle.FI:
                    {
                        if (isSpurt)
                            return 0.994;
                        else if (coursePhase == CoursePhase.First)
                            return 0.938;
                        else if (coursePhase == CoursePhase.Middle)
                            return 0.998;
                        else /*coursePhase == CoursePhase.Last */
                            return 0.994;
                    }
                case RunningStyle.Stretch:
                    {
                        if (!isSpurt && coursePhase == CoursePhase.First) return 0.931;
                        else return 1.000;
                    }
                default:
                    return 1.000;
            }
        }
        /// 지능 랜덤 보정
        private double GetIntelligenceRandomCalibratingValue()
        {
            Random random = new Random();
            double maximum = ((double) calibratedIntelligence / 5500) * Math.Log(calibratedIntelligence * 0.1) * 0.01;
            double minimum = maximum - 0.0065;
            return minimum + random.NextDouble() * 0.0065;
        }

        // 일반 목표 속도 계산
        private double GetNormalTargetSpeed()
        {
            if (isPositionKeep)
            {
                switch (positionKeep)
                {
                    case PositionKeep.SpeedUp:
                        return defaultTargetSpeed * 1.04;
                    case PositionKeep.Overtaking:
                        return defaultTargetSpeed * 1.05;
                    case PositionKeep.paceDown:
                        return defaultTargetSpeed * 0.915;
                    case PositionKeep.paceUp:
                        return defaultTargetSpeed * 1.04;
                    default:
                        return defaultTargetSpeed;
                }
            }
            else if (isForceInMove)
            {
                double result;
                Random rand = new Random();
                switch (forceInMove)
                {
                    case ForceInMove.OutsideOvertaking:
                    case ForceInMove.InsideOvertaking:
                        result = 1.01;
                        break;
                    case ForceInMove.InsideMove:
                    case ForceInMove.OutsideMove:
                        result = 1.01;
                        break;
                    case ForceInMove.InsideCatchUp:
                    case ForceInMove.OutsideCatchUp:
                        result = 1.03;
                        break;
                    default:
                        result = 1;
                        break;
                }
                return defaultTargetSpeed * (result + rand.NextDouble() * 0.10);
            }
            else return defaultTargetSpeed;
        }

        // 스퍼트 목표 속도 계산
        private double GetMaximumSpurtTargetSpeed()
        {
            return 1.05 * defaultTargetSpeed + 0.01 * GetRaceReferenceSpeed() + Math.Sqrt(500 * calibratedSpeed) * lengthAptitudeSpeedValue * 0.002;
        }

        // 최소 목표 속도 계산
        private double GetMinimumTargetSpeed()
        {
            return GetRaceReferenceSpeed() * 0.85 + Math.Sqrt(200 * calibratedToughness) * 0.001;
        }

        // 블록 당하고 있을 때 최대 목표 속도 계산
        private double GetMaximumTargetSpeed(Participant p)
        {
            Random rand = new Random();
            return (0.988 + rand.NextDouble() * 0.012) * p.currSpeed;
        }


        // 현재 타겟 속도 계산
        private void UpdateTargetSpeed()
        {
            defaultTargetSpeed = GetDefaultTargetSpeed();
            normalTargetSpeed = GetNormalTargetSpeed();
            if(isSpurt)
                spurtTargetSpeed = GetSpurtTargetSpeed();
            // maximumTargetSpeed : CheckNear 에서 확인
            if (currStamina <= 0)
                targetSpeed = minimumTargetSpeed;
            else if (isSpurt)
                targetSpeed = spurtTargetSpeed;
            else
            {
                if (normalTargetSpeed > maximumTargetSpeed && !isForceInMove && !isPositionKeep)
                    targetSpeed = maximumTargetSpeed;
                else
                    targetSpeed = normalTargetSpeed;
            }
            if (targetSpeed < minimumTargetSpeed)
                targetSpeed = minimumTargetSpeed;
                
            
        }


        // 가속도 계산
        private double GetAccel()
        {
            double result;

            if (currSpeed > targetSpeed)
                result = GetDeceleration() * -1;
            else if (currSpeed < targetSpeed)
            {
                result = GetRunningStyleAccelCalibrateValue() * 0.0006 * Math.Sqrt(500 * calibratedPower) * fieldTypeAccelCalibrateValue;
            }
            else result = 0;

            if (isStartAccel) return result + 24;
            else return result;
        }

        // 각질 가속도 보정
        private double GetRunningStyleAccelCalibrateValue()
        {
            switch (runningStyle)
            {
                case RunningStyle.Runaway:
                    {
                        if (isSpurt)
                            return 0.996;
                        else if (coursePhase == CoursePhase.First) 
                            return 1.000;
                        else if (coursePhase == CoursePhase.Middle)
                            return 1.000;
                        else /* coursePhase == CoursePhase.Last */ 
                            return 0.996;
                    }
                case RunningStyle.Front:
                    {
                        if (isSpurt)
                            return 0.996;
                        else if (coursePhase == CoursePhase.First)
                            return 0.985;
                        else if (coursePhase == CoursePhase.Middle)
                            return 1.000;
                        else /* coursePhase == CoursePhase.Last */
                            return 0.996;
                    }
                case RunningStyle.FI:
                    {
                        if (isSpurt)
                            return 1.000;
                        else if (coursePhase == CoursePhase.First)
                            return 0.975;
                        else if (coursePhase == CoursePhase.Middle)
                            return 1.000;
                        else /* coursePhase == CoursePhase.Last */
                            return 1.000;
                    }
                case RunningStyle.Stretch:
                    {
                        if (isSpurt)
                            return 0.997;
                        else if (coursePhase == CoursePhase.First)
                            return 0.945;
                        else if (coursePhase == CoursePhase.Middle)
                            return 1.000;
                        else /* coursePhase == CoursePhase.Last */
                            return 0.997;
                    }
                default:
                    return 1.000;
            }
        }
        // 마장 적성 보정
        private double GetFieldTypeAccelCalibrateValue(Umamusume u)
        {
            Aptitude aptitude;
            if (racetrack.fieldType == FieldType.grass) aptitude = u.grassAptitude;
            else aptitude = u.durtAptitude;

            switch (aptitude)
            {
                case Aptitude.S:
                    return 1.05;
                case Aptitude.A:
                    return 1.00;
                case Aptitude.B:
                    return 0.90;
                case Aptitude.C:
                    return 0.80;
                case Aptitude.D:
                    return 0.70;
                case Aptitude.E:
                    return 0.50;
                case Aptitude.F:
                    return 0.30;
                case Aptitude.G:
                    return 0.10;
                default:
                    return 0.10;

            }
        }
        // 감속 계산
        private double GetDeceleration()
        {
            if (currStamina <= 0) return 1.2;
            else if (coursePhase == CoursePhase.First) return 1.2;
            else if (coursePhase == CoursePhase.Middle) return 0.8;
            else return 1.0;
        }

        // 최대 체력 계산
        private double GetMaximumStamina()
        {
            return racetrack.GetTrackLength() + 0.8 * GetRunningStyleStaminaCalibrateValue() * calibratedStamina;
        }
        // 각질 체력 보정
        private double GetRunningStyleStaminaCalibrateValue()
        {
            switch (runningStyle)
            {
                case RunningStyle.Runaway:
                    return 0.95;
                case RunningStyle.Front:
                    return 0.89;
                case RunningStyle.FI:
                    return 1.00;
                case RunningStyle.Stretch:
                    return 0.995;
                default:
                    return 1.00;
            }
        }


        // 턴당 체력 소모 속도 (20턴 == 1초)
        private double GetStaminaExhaustionSpeed(TurfCondition turfCondition)
        {
            return GetUmamusumeStatusCalibrateValue() * (Math.Pow((currSpeed - GetRaceReferenceSpeed() + 12), 2) / 144)
                * GetTurfConditionStaminaCalibrateValue(turfCondition) * GetSpurtStaminaCalibrateValue();
        }
        // 체력 소모 속도 말 상태 보정
        private double GetUmamusumeStatusCalibrateValue()
        {
            if (isFever) return 1.6;
            else if (isPositionKeep || positionKeep == PositionKeep.paceDown) return 0.6;
            else return 1;
        }
        // 마장 상태 체력 보정
        private double GetTurfConditionStaminaCalibrateValue(TurfCondition turfCondition)
        {
            if(racetrack.fieldType == FieldType.grass)
            {
                switch (turfCondition)
                {
                    case TurfCondition.heavy:
                    case TurfCondition.bad:
                        return 1.02;
                    default:
                        return 1.00;
                }
            }
            else // fieldType == FieldType.durt
            {
                switch (turfCondition)
                {
                    case TurfCondition.heavy:
                        return 1.01;
                    case TurfCondition.bad:
                        return 1.02;
                    default:
                        return 1.00;
                }
            }
        }
        // 스퍼트 체력 보정
        private double GetSpurtStaminaCalibrateValue()
        {
            if (isSpurt) return (1 + 200 / Math.Sqrt(600 * calibratedToughness));
            else return 1;
        }

        // 주변 확인
        private void CheckNear(List<Participant> pList)
        {
            SetEyesightLevel();

            int startX = 0;
            int startY = eyesight[eyesightLevel].Item2 / 2;
            int endX = eyesight[eyesightLevel].Item1 + 1;
            int endY = eyesight[eyesightLevel].Item2;
            
            if(currPosition.Y <= 0.5)
            {
                for(int i = 0; i < endX; i++)
                {
                    for(int j = 0; j < startY; j++)
                    {
                        surroundCheck[i, j]++;
                    }
                }
            }
            else if(currPosition.Y >= racetrack.width - 0.5)
            {
                for (int i = 0; i < endX; i++)
                {
                    for(int j = startY + 1; j < endY; j++)
                    {
                        surroundCheck[i, j]++;
                    }
                }
            }

            for (int i = rank - 1; i > 0; i--)
            {
                if (pList[i].isGoal) continue;

                isBlocked = false;
                int x = 0;
                int y = 0;
                double diffX = pList[i].currPosition.X - currPosition.X;
                double diffY = pList[i].currPosition.Y - currPosition.Y;
                for(y = 0; y < endY; y++ )                
                    if (diffY >= -0.25 + (y - startY) * 0.5 && diffY < 0.25 + (y - startY) * 0.5)
                        break;
                for (x = 0; x < endX; x++)
                    if (diffX >= -0.25 + (x - startX) * 0.5 && diffX < 0.25 + (x - startX) * 0.5)
                        break;
                if (y == endY || x == endX) continue;
                else
                {
                    surroundCheck[x, y]++;
                    if(y == startY && (x == startX || x == startX + 1) && !isBlocked)
                    {
                        isBlocked = true;
                        GetMaximumTargetSpeed(pList[i]);
                    }
                }
            }
            if (!isBlocked) maximumTargetSpeed = 30;

            return;
        }

        private void FirstPhaseActionDecide(List<Participant> pList)
        {
            if (runningStyle == RunningStyle.Runaway)
            {
                SpeedUp();
            }
            else
            {
                if (rank == 0)
                    NormalRun();
                else if (pList[rank - 1].runningStyle == runningStyle)
                    Overtake();
                else
                {
                    switch (runningStyle)
                    {
                        case RunningStyle.Front:
                            if (pList[rank - 1].runningStyle == RunningStyle.Runaway)
                                NormalRun();
                            else
                                Overtake();
                            break;
                        case RunningStyle.FI:
                            if (pList[rank - 1].runningStyle == RunningStyle.Stretch)
                                Overtake();
                            else
                                NormalRun();
                            break;
                        default:
                            NormalRun();
                            break;
                    }
                }
            }
        }
        private void MiddlePhaseActionDecide(List<Participant> pList)
        {
            isSpurt = false;
            if (rank == 0)
                PaceAdjust();
            else if (pList[rank - 1].runningStyle == runningStyle)
                Overtake();
            else if (runningStyle == RunningStyle.Stretch)
                PaceUp();
            else
                PaceAdjust();
        }
        private void LastPhaseActionDecide(List<Participant> pList)
        {
            double spurtLength;

            switch (runningStyle)
            {
                case RunningStyle.Stretch:
                    spurtLength = racetrack.GetLastLength();
                    break;
                case RunningStyle.FI:
                    spurtLength = racetrack.GetLastLength();
                    break;
                case RunningStyle.Front:
                    spurtLength = racetrack.GetLastStraight();
                    break;
                default:
                    spurtLength = racetrack.GetLastSpurt();
                    break;
            }

            if (GetLeftDistance() < GetSpurtLength() 
                || GetLeftDistance() < spurtLength)
                isSpurt = true;
            if (currStamina <= 0)
                isSpurt = false;

            Overtake();

            return;
            
        }

        private double GetSpurtTargetSpeed()
        {
            double max = GetMaximumSpurtTargetSpeed();
            if (derby.GetCoursePhase(currPosition.X) == CoursePhase.First) return max;
            else return max;
        }

        public override string ToString()
        {
            return (rank + 1).ToString().PadLeft(2) + " : " + name.PadLeft(15, ' ')
                + " - targetSpeed(m/s) : " + targetSpeed.ToString("F")
                + " / defaultTarget(m/s) : " + defaultTargetSpeed.ToString("F")
                + " / spurtTarget(m/s) : " + spurtTargetSpeed.ToString("F")
                + " / currPosition : " + currPosition.ToString("F")
                + " / Stamina : " + (currStamina * 100 / maxStamina).ToString("F")
                + "% / Phase : " + derby.GetCoursePhase(currPosition.X).ToString()
                + " / PositionKeep : " + positionKeep.ToString()
                + " / ForceInMove : " + forceInMove.ToString()
                + " / isSpurt : " + isSpurt;
        }
        public string ToCSVString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}, {9}", targetSpeed, defaultTargetSpeed, spurtTargetSpeed,
                currPosition.X, currPosition.Y, (float)currStamina * 100 / maxStamina, derby.GetCoursePhase(currPosition.X), positionKeep.ToString(),
                forceInMove.ToString(), isSpurt);
        }
        static public string CVSFieldString()
        {
            return "turn, targetSpeed, defaultTargetSpeed, spurtTargetSpeed, currPosition.x, currPosition.Y, currStamina(%), coursePhase, positionKeep, forceInMove, isSpurt";
        }
        public string ToSimpleString()
        {
            return rank.ToString().PadLeft(2) + " : " + name.PadLeft(15, ' ')
                + " : " + currPosition.X.ToString("F2") + "m / 현재 속도 : "
                + (currSpeed * 3.6).ToString("F2") + "km/h";
        }

        public float GetLeftDistance()
        {
            return racetrack.GetTrackLength() - currPosition.X;
        }
    }

    public enum PositionKeep
    {
        non = 0,
        SpeedUp = 1,
        Overtaking = 2,
        paceDown = 3,
        paceUp = 4
    }

    public enum ForceInMove
    {
        non = 0,
        InsideOvertaking = 1,
        OutsideOvertaking = 2,
        InsideMove = 3,
        OutsideMove = 4,
        InsideCatchUp = 5,
        OutsideCatchUp = 6
    }


}
