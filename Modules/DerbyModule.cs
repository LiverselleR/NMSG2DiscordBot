using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Discord;
using Discord.WebSocket;
using Discord.Interactions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NMSG2DiscordBot
{
    [Group("더비", "더비 관련 커맨드 그룹입니다.")]
    public class DerbyModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("참가", "더비에 참가합니다. 더비 이름과 등록하려는 각질을 입력해 주세요.")]
        public async Task DerbyEntryAsync(String derbyName, String runningStyleString)
        {
            try
            {
                RaceEntryManager.Entry(Context.User.Id, derbyName, runningStyleString);
                await RespondAsync("더비 등록 완료! 귀하의 건승을 기원합니다!");
            }
            catch (DiscordIDNotRegisteredException)
            {
                await RespondAsync("오류 : 우마무스메로 등록되지 않은 유저입니다. 트레이너님은 직접 뛰고 싶으셔도 조금만 참아 주세요.");
            }
            catch (UmamusumeAlreadyRegisteredException)
            {
                await RespondAsync("오류 : 해당 우마무스메는 이 더비에 이미 등록되어 있습니다. (각질 변경 기능은 추후 업데이트 예정입니다.");
            }
            catch (DerbyNameNotFoundException)
            {
                await RespondAsync("오류 : 해당 더비를 찾을 수 없습니다. ");
            }
            catch (ArgumentException)
            {
                await RespondAsync("오류 : 각질 이름이 잘못되었습니다. 각질은 [도주 / 선행 / 선입 / 추입] 4종류 입니다.");
            }
        }

        [SlashCommand("확인", "더비 참가 여부를 확인합니다. 더비에 참여했을 경우, 등록한 각질을 확인할 수 있습니다.")]
        public async Task DerbyCheckAsync(String derbyName, String? umamusumeName = null)
        {
            ulong ownerID = 0;
            if(umamusumeName == null)
            {
                ownerID = Context.User.Id;
            }
            else
            {
                List<Umamusume> uList = JSONManager.GetUmamusumeList();
                Umamusume umamusume = uList.Find(u => u.name == umamusumeName);
                if(umamusume != null)
                {
                    ownerID = umamusume.ownerID;
                }
                else
                {
                    await RespondAsync("오류 : 해당 이름의 우마무스메를 찾을 수 없습니다.");
                    return;
                }
            }

            try
            {
                RunningStyle? runningStyle = RaceEntryManager.Check(ownerID, derbyName);
                if (runningStyle != null)
                {
                    switch (runningStyle)
                    {
                        case RunningStyle.Runaway:
                            {
                                await RespondAsync("레이스에 등록된 우마무스메입니다. 각질 : 도주");
                                return;
                            }
                        case RunningStyle.Front:
                            {
                                await RespondAsync("레이스에 등록된 우마무스메입니다. 각질 : 선행");
                                return;
                            }
                        case RunningStyle.FI:
                            {
                                await RespondAsync("레이스에 등록된 우마무스메입니다. 각질 : 선입");
                                return;
                            }
                        case RunningStyle.Stretch:
                            {
                                await RespondAsync("레이스에 등록된 우마무스메입니다. 각질 : 추입");
                                return;
                            }
                    }
                }
            }
            catch (UmamusumeNameNotFoundException)
            {
                await RespondAsync("오류 : 우마무스메로 등록되지 않은 유저입니다.");
            }
            catch (DerbyNameNotFoundException)
            {
                await RespondAsync("오류 : 해당 더비를 찾을 수 없습니다. ");
            }
            catch (UmamusumeNotRegisteredException)
            {
                await RespondAsync("해당 레이스에 등록되지 않은 우마무스메 입니다.");
            }            
        }

        [SlashCommand("시작", "더비를 시작합니다. 관리자만 시작할 수 있습니다.")]
        public async Task DerbyStartAsync(String derbyName)
        {
            if (Context.User.Id != 0 && Context.User.Id != 0)
            {
                await RespondAsync("오류 : 더비 시작 권한이 없습니다.");
                return;
            }
            try
            {
                await RespondAsync("잠시 후 더비가 시작됩니다.");

                List<String> sList = Racemanager.RaceStart(derbyName);

                await ReplyAsync("레이스 시작 15초 전");
                await Task.Delay(10000);
                await ReplyAsync("레이스 시작 5초 전");
                await Task.Delay(2000);
                await ReplyAsync("3");
                await Task.Delay(1000);
                await ReplyAsync("2");
                await Task.Delay(1000);
                await ReplyAsync("1");
                await Task.Delay(1000);

                foreach (String str in sList)
                {
                    await ReplyAsync(str);
                    await Task.Delay(2000);
                }
            }
            catch (DerbyNameNotFoundException)
            {
                await ReplyAsync("오류 : 해당 더비를 찾을 수 없습니다.");
            }
        }
        [SlashCommand("테스트", "더비 테스트 커맨드입니다.")]
        public async Task DerbyTestAsync(String derbyName)
        {
            if (Context.User.Id != 0 && Context.User.Id != 0)
            {
                await RespondAsync("오류 : 테스트 권한이 없습니다.");
                return;
            }

            try
            {
                await RespondAsync("테스트 더비 시작");
                List<String> sList = Racemanager.RaceStart(derbyName);
            }
            catch (DerbyNameNotFoundException)
            {
                await ReplyAsync("오류 : 해당 더비를 찾을 수 없습니다.");
            }

            //List<String> derbyCast = Derby.TestDerby_Process();
            //if (derbyCast.Count <= 0)
            //{
            //    await RespondAsync("derbyCast is Empty. Test End.");
            //    return;
            //}

            //await ReplyAsync(derbyCast.Last<String>());


            //foreach (String str in derbyCast)
            //{
            //    await ReplyAsync(str);
            //    await Task.Delay(2000);
            //}

            await ReplyAsync("테스트 더비 완료");
        }
    }
}