using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMSG2DiscordBot
{
    public partial class Participant
    {
        // 레인 이동 속도
        float LaneMoveTargetSpeed = 0;
        float LaneMoveCurrentSpeed = 0;
        float LaneMoveAccel = 0;
        // 레인 거리 (최내레인으로부터의 거리). 1레인 = 1m
        float TargetLane = 0;
        float CurrentLane = 0;

        public string FindOvertakeTarget(List<Participant> pList)
        {
            List<Participant> TargetPList;
            foreach(Participant p in pList)
            {
                if(p.rank >= rank)
                {
                    continue;
                }
                float positionDifference = p.currPosition.X - currPosition.X;

            }
            return string.Empty;
        }

        private void SetLaneMoveTargetSpeed(List<Participant> pList)
        {
            RacePhase CurrentRacePhase = racetrack.GetCurrentRacePhase(currPosition.X);

            if(currStamina <= 0)
            {
                return;
            }
            else if (positionKeep == PositionKeep.paceDown)
            {
                TargetLane = 0.18f;
            }
            else if ((CurrentRacePhase == RacePhase.phase0 ||
                     CurrentRacePhase == RacePhase.phase1) &&
                     IsInsideBlocked(pList) == false)
            {
                TargetLane = CurrentLane - 0.05f;
            }
            else if (CurrentRacePhase == RacePhase.phase1 &&
                     IsInsideBlocked(pList) == true )
            {

            }
            else if (CurrentRacePhase == RacePhase.phase3 &&
                    IsOutsideBlocked(pList) == false)
            {

            }
        }

        private bool IsInsideBlocked(List<Participant> pList)
        {
            return false;
        }
        private bool IsOutsideBlocked(List<Participant> pList)
        {
            return false;
        }
    }
}
