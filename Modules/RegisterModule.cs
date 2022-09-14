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
    [Group("등록")]
    public class RegisterModule : ModuleBase<SocketCommandContext>
    {
        [Command("우마무스메")]
        [Summary("우마무스메 등록")]
        public async Task UmamusumeRegisterAsync(String uName)
        {

            if (Context.Channel.Id != 8)
            {
                await ReplyAsync("오류 : 등록은 관리실에서 진행됩니다. 관리실로 와 주세요.");
                return;
            }

            List<Umamusume> uList = JSONManager.GetUmamusumeList();
            Console.WriteLine(uName);
            try
            {
                UmamusumeManager.Register(Context.User.Id, uName, ref uList);
                JSONManager.SetUmamusumeList(uList);
                await ReplyAsync("등록 완료! 호르트 아카데미에 오신 것을 환영합니다!");
            }
            catch (UmamusumeNameNotFoundException e)
            {
                await ReplyAsync("오류 : 해당 우마무스메를 찾을 수 없습니다.");
            }
            catch (UmamusumeAlreadyRegisteredException e)
            {
                await ReplyAsync("오류 : 해당 우마무스메는 이미 등록되었습니다.");
            }
            catch (DiscordIdAlreadyRegisteredException e)
            {
                await ReplyAsync("오류 : 이미 등록이 완료된 유저(우마무스메) 입니다.");
            }

        }

        [Command("트레이너")]
        [Summary("트레이너 등록")]
        public async Task TrainerRegisterAsync(String tName)
        {
            if (Context.Channel.Id != 1010831944667381790)
            {
                await ReplyAsync("오류 : 등록은 관리실에서 진행됩니다. 관리실로 와 주세요.");
                return;
            }
            List<Trainer> tList = JSONManager.GetTrainerList();
            try
            {
                TrainerManager.Register(Context.User.Id, tName, ref tList);
                JSONManager.SetTrainerList(tList);
                await ReplyAsync("등록 완료! 호르트 아카데미에 오신 것을 환영합니다!");
            }
            catch (TrainerNameNotFoundException e)
            {
                await ReplyAsync("오류 : 해당 트레이너를 찾을 수 없습니다.");
            }
            catch (TrainerAlreadyRegisteredException e)
            {
                await ReplyAsync("오류 : 해당 트레이너는 이미 등록되었습니다.");
            }
            catch (DiscordIdAlreadyRegisteredException e)
            {
                await ReplyAsync("오류 : 이미 등록이 완료된 유저(트레이너) 입니다.");
            }
        }

        [Command("팀")]
        [Summary("팀 등록")]
        public async Task TeamRegister(String tName)
        {
            if (Context.Channel.Id != 8)
            {
                await ReplyAsync("오류 : 등록은 관리실에서 진행됩니다. 관리실로 와 주세요.");
                return;
            }
            List<Team> teamList = JSONManager.GetTeamList();
            List<Trainer> trainerList = JSONManager.GetTrainerList();
            List<Umamusume> uList = JSONManager.GetUmamusumeList();
            try
            {
                TeamManager.Register(Context.User.Id, tName, ref teamList, trainerList, uList);
                JSONManager.SetTeamList(teamList);
                await ReplyAsync("팀 등록 완료!");
            }
            catch (DiscordIDNotRegisteredException e)
            {
                await ReplyAsync("오류 : 해당 디스코드 아이디는 등록되지 않았습니다. 우마무스메나 트레이너 등록 이후 팀 등록을 진행해 주세요.");
            }
            catch (UmamusumeAlreadyRegisteredException e)
            {
                await ReplyAsync("오류 : 해당 우마무스메는 이미 등록되었습니다.");
            }
            catch (TrainerAlreadyRegisteredException e)
            {
                await ReplyAsync("오류 : 해당 트레이너는 이미 등록되었습니다.");
            }
            catch (TeamNameNotFoundException e)
            {
                await ReplyAsync("오류 : 해당 팀을 찾을 수 없습니다.");
            }
        }
    }
}