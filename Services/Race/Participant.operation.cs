using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMSG2DiscordBot
{
    public partial class Participant
    {
        public int overtakePointer = 0;

        public void NormalRun()
        {
            isPositionKeep = false;
            isForceInMove = false;
            positionKeep = PositionKeep.non;
            forceInMove = ForceInMove.non;
        }
        public void InsideOvertake()
        {
            isPositionKeep = false;
            isForceInMove = true;
            positionKeep = PositionKeep.non;
            forceInMove = ForceInMove.InsideOvertaking;
        }
        public void FrontOvertake()
        {
            isPositionKeep = true;
            isForceInMove = false;
            positionKeep = PositionKeep.Overtaking;
            forceInMove = ForceInMove.non;
        }
        public void OutsideOvertake()
        {
            isPositionKeep = false;
            isForceInMove = true;
            positionKeep = PositionKeep.non;
            forceInMove = ForceInMove.OutsideOvertaking;
        }
        public void PaceUp()
        {
            isPositionKeep = true;
            isForceInMove = false;
            positionKeep = PositionKeep.paceUp;
            forceInMove = ForceInMove.non;
        }

        public void PaceDown()
        {
            isPositionKeep = true;
            isForceInMove = false;
            positionKeep = PositionKeep.paceDown;
            forceInMove = ForceInMove.non;
        }
        public void SpeedUp()
        {
            isPositionKeep = true;
            isForceInMove = false;
            positionKeep = PositionKeep.SpeedUp;
            forceInMove = ForceInMove.non;
        }
        public void InsideMove()
        {
            isPositionKeep = false;
            isForceInMove = true;
            positionKeep = PositionKeep.non;
            forceInMove = ForceInMove.InsideMove;
        }
        public void OutsideMove()
        {
            isPositionKeep = false;
            isForceInMove = true;
            positionKeep = PositionKeep.non;
            forceInMove = ForceInMove.OutsideMove;
        }

        public void Overtake()
        {
            OvertakeType[] overtakeTypes = new OvertakeType[5];
            List<int> route = new List<int>();
            int startX = 0;
            int startY = eyesight[eyesightLevel].Item2 / 2;
            int endX = eyesight[eyesightLevel].Item1 + 1;
            int endY = eyesight[eyesightLevel].Item2;

            int x = startX;
            int y = startY;
            int routePointer = 0;

            switch (runningStyle)
            {
                case RunningStyle.Runaway:
                case RunningStyle.Front:
                    overtakeTypes[0] = OvertakeType.inside;
                    overtakeTypes[1] = OvertakeType.front;
                    overtakeTypes[2] = OvertakeType.inMove;
                    overtakeTypes[3] = OvertakeType.outside;
                    overtakeTypes[4] = OvertakeType.outMove;
                    break;
                case RunningStyle.FI:
                case RunningStyle.Stretch:
                    overtakeTypes[0] = OvertakeType.front;
                    overtakeTypes[1] = OvertakeType.inside;
                    overtakeTypes[2] = OvertakeType.outside;
                    overtakeTypes[3] = OvertakeType.inMove;
                    overtakeTypes[4] = OvertakeType.outMove;
                    break;
                default:
                    break;
            }
           
            route.Add(-1);
            while (x < endX - 1 && x >= 0)
            {
                route[routePointer]++;
                
                if (route[routePointer] > 4)
                {
                    break;
                    routePointer--;
                    if (routePointer < 0)
                    {
                        routePointer = 0;
                        route.Clear();
                        route.Add(-1);
                        break;
                    }
                    OvertakeType currentOvertakeType = overtakeTypes[route[routePointer]];
                    switch (currentOvertakeType)
                    {
                        case OvertakeType.inside:
                            x = x - 1;
                            y = y + 1;
                            break;
                        case OvertakeType.front:
                            x = x - 1;
                            break;
                        case OvertakeType.outside:
                            x = x - 1;
                            y = y - 1;
                            break;
                        case OvertakeType.inMove:
                            y = y + 1;
                            break;
                        case OvertakeType.outMove:
                            y = y - 1;
                            break;
                        default:
                            break;
                    }
                    route.RemoveAt(routePointer+1);
                    continue;
                }
                int plusX = x;
                int plusY = y;
                OvertakeType type = overtakeTypes[route[routePointer]];
                if(routePointer > 0)
                {
                    OvertakeType previousOvertakeType = overtakeTypes[route[routePointer - 1]];
                    if ((int) type + (int) previousOvertakeType == 7)
                    {
                        continue;
                    }
                }
                List<(int, int)> checklist = new List<(int, int)>();
                switch (type)
                {
                    case OvertakeType.inside:
                        plusX = x + 1;
                        plusY = y - 1;
                        checklist.Add((x, y - 1));
                        checklist.Add((x + 1, y));
                        break;
                    case OvertakeType.front:
                        plusX = x + 1;
                        checklist.Add((x + 1, y));
                        break;
                    case OvertakeType.outside:
                        plusX = x + 1;
                        plusY = y + 1;
                        checklist.Add((x, y + 1));
                        checklist.Add((x + 1, y));
                        break;
                    case OvertakeType.inMove:
                        plusY = y - 1;
                        checklist.Add((x, y - 1));
                        break;
                    case OvertakeType.outMove:
                        plusY = y + 1;
                        checklist.Add((x, y + 1));
                        break;
                    default:
                        break;
                }

                if (plusX < 0 || plusY < 0 || plusY >= endY)
                    continue;
                Boolean isWayBlocked = false;
                foreach((int, int) i in checklist)
                {
                    isWayBlocked = isWayBlocked || (surroundCheck[i.Item1, i.Item2] != 0);
                }
                if (!isWayBlocked)
                {
                    x = plusX;
                    y = plusY;
                    routePointer++;
                    route.Add(-1);
                }                
            }

            if (endX == 0 || route[0] == -1)
            {
                if (surroundCheck[0, startY + 1] == 0)
                    OutsideMove();
                else if (surroundCheck[0, startY - 1] == 0)
                    InsideMove();
                else
                    NormalRun();
            }
            else
            {
                switch(overtakeTypes[route[0]])
                {
                    case OvertakeType.inside:
                        InsideOvertake();
                        break;
                    case OvertakeType.front:
                        FrontOvertake();
                        break;
                    case OvertakeType.outside:
                        OutsideOvertake();
                        break;
                    case OvertakeType.inMove:
                        InsideMove();
                        break;
                    case OvertakeType.outMove:
                        OutsideMove();
                        break;
                    default:
                        NormalRun();
                        break;
                }
            }

            overtakePointer = routePointer;
            return;
            

        }
        private double GetStaminaExhaustionSpeed(double speed)
        {
            return GetUmamusumeStatusCalibrateValue() * (Math.Pow((speed - GetRaceReferenceSpeed() + 12), 2) / 144)
                * GetTurfConditionStaminaCalibrateValue(derby.turfCondition);
        }
        private double GetSpurtStaminaExhaustionSpeed(double speed)
        {
            return GetUmamusumeStatusCalibrateValue() * (Math.Pow((speed - GetRaceReferenceSpeed() + 12), 2) / 144)
                * GetTurfConditionStaminaCalibrateValue(derby.turfCondition) * (1 + 200 / Math.Sqrt(600 * calibratedToughness));
        }
        private double GetSpurtStaminaExhaustionSpeed()
        {
            return GetUmamusumeStatusCalibrateValue() * (Math.Pow((spurtTargetSpeed - GetRaceReferenceSpeed() + 12), 2) / 144)
                * GetTurfConditionStaminaCalibrateValue(derby.turfCondition) * (1 + 200 / Math.Sqrt(600 * calibratedToughness));
        }

        private double GetSpurtLength()
        {
            return spurtTargetSpeed * (currStamina / GetSpurtStaminaExhaustionSpeed());
        }
        private double GetSpurtLength(double stamina)
        {
            return spurtTargetSpeed * stamina / GetSpurtStaminaExhaustionSpeed();
        }

        public void RunawayKeepPace()
        {
            double leftDistance = racetrack.GetTrackLength() - currPosition.X;
            double expectedStaminaExhaust = GetStaminaExhaustionSpeed(normalTargetSpeed);
            int expectedTurn = (int)leftDistance / (int)normalTargetSpeed;
            if (expectedStaminaExhaust * expectedTurn > currStamina) PaceDown();
            else NormalRun();
        }

        public double ExpectSpurtLength()
        {
            double leftMiddleLength = racetrack.GetMiddleEndLength() - currPosition.X;
            int leftMiddleTurn = (int) (leftMiddleLength / targetSpeed);

            double spurtLength = GetSpurtLength(currStamina - GetStaminaExhaustionSpeed(targetSpeed) * leftMiddleTurn);
            return spurtLength;
        }

        public void PaceAdjust()
        {
            double expectedSpurtLength = ExpectSpurtLength();
            double spurtLength;

            switch (runningStyle)
            {
                case RunningStyle.Stretch:
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
            if (spurtLength < expectedSpurtLength)
            {
                if (surroundCheck[1, eyesight[eyesightLevel].Item2 / 2 - 1] == 0)
                    InsideMove();
                else
                    PaceUp();
            }
            else
                NormalRun();
        }

        public void SetMinorSpurtTargetSpeed()
        {
            while (racetrack.GetLastSpurt() > GetSpurtLength() && spurtTargetSpeed > minimumTargetSpeed)
            {
                spurtTargetSpeed -= 0.01;
            }
                
        }


    }

    public enum OvertakeType
    {
        inside = 0,
        front = 1,
        outside = 2,
        inMove = 3,
        outMove = 4
    }
}
