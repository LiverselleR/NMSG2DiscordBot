using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NMSG2DiscordBot
{
    public class TrainingModule : ModuleBase<SocketCommandContext>
    {
        [Command("훈련")]
        [Summary("우마무스메 훈련")]
        public async Task UmamusumeTrainingAsync(String keyword = null, String stat = null)
        {
            if ((Context.Channel as ITextChannel).CategoryId != 8)
            {
                await ReplyAsync("오류 : 훈련은 훈련 시설( 체력단련실, 운동장, 연습용 트랙( 잔디, 더트), 도서관 ) 에서만 가능합니다.");
                return;
            }
            else if (keyword == "횟수")
            {
                try
                {
                    string result = TrainingManager.GetLeftTraining(Context.User.Id);
                    await ReplyAsync(result);
                }
                catch (UserIDNotFoundException e)
                {
                    await ReplyAsync("오류 : 디스코드 ID에 해당하는 우마무스메를 찾을 수 없습니다. 등록을 먼저 진행해 주세요.");
                }
            }
            else if (keyword == "진행")
            {
                try
                {
                    TrainingManager.UmamusumeTraining(Context.User.Id, stat);
                    await ReplyAsync("훈련 완료!");
                }
                catch (NoLeftTrainingCountException e)
                {
                    await ReplyAsync("오류 : 오늘의 훈련 가능 횟수를 모두 소진했습니다. 한국 시간 기준 자정에 초기화 됩니다.");
                }
                catch (UserIDNotFoundException e)
                {
                    await ReplyAsync("오류 : 디스코드 ID에 해당하는 우마무스메를 찾을 수 없습니다. 등록을 먼저 진행해 주세요.");
                }
                catch (InvalidStatusTypeException e)
                {
                    await ReplyAsync("오류 : 훈련 스탯 종류가 잘못 입력되었습니다. (훈련 가능 스탯 : 속도 / 체력 / 근력 / 근성 / 지능)");
                }
            }
            else if (keyword == "예시")
            {
                switch (stat)
                {
                    case "속도":
                        await ReplyAsync("속도 트레이닝 예시 : 잔디 / 런닝머신 / 피트니스 바이크 / 걸레질 / 샷 건 터치 (버튼을 누른 순간 먼 거리에서 낙하를 시작하는 공을 다이빙 캐치하는 트레이닝.) \n" +
                            "< 속도가 상승합니다. 추가로 파워가 소폭 상승합니다. >");
                        break;
                    case "체력":
                        await ReplyAsync("체력 트레이닝 예시 : 평영 / 자유형 / 배영 / 접영 / 고속 개헤엄 \n" +
                            "(체력단련실에 수영장이 포함되어 있다는 설정입니다.) \n" +
                            "< 체력이 상승합니다. 추가로 근성이 소폭 상승합니다. >");
                        break;
                    case "근력":
                        await ReplyAsync("파워 트레이닝 예시 : 더트 / 스쿼트 / 윗몸 일으키기 / 복싱 / 기왓장 깨기 \n" +
                            "< 파워가 상승합니다. 추가로 체력이 소폭 상승합니다. >");
                        break;
                    case "근성":
                        await ReplyAsync("근성 트레이닝 예시 : 더트 언덕길 오르기 / 더트 언덕길 토끼뜀 / 댄스 연습 / 높은 계단 대쉬 / 대형 타이어 끌기 \n" +
                            "< 근성이 상승합니다. 추가로 스피드와 파워가 소폭 상승합니다. >");
                        break;
                    case "지능":
                        await ReplyAsync("지능 트레이닝 : 공부 / 독서 / 퀴즈(스피드 퀴즈) / 비디오 연구 / 일본 장기 \n" +
                            "< 지능이 상승합니다. 추가로 스피드가 소폭 상승합니다. >");
                        break;
                    default:
                        break;
                }
            }
            else
            {
                await ReplyAsync("훈련 커맨드 입력 방식 : [ !훈련 ] (커맨드 입력 방식) / [ !훈련 횟수 ] (남은 횟수 조회) \n" +
                    " / [ !훈련 예시 (속도/체력/근력/근성/지능)] (훈련 예시. 실제 우마무스메 게임판에서의 훈련 내용) \n" +
                    " / [ !훈련 진행 (속도/체력/근력/근성/지능)] (훈련 진행, 정해진 스탯 중 택 1) \n");
            }
        }
    }
}