//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using System.IO;
//using Discord;
//using Discord.Interactions;
//using Discord.WebSocket;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using NMSG2DiscordBot.Services;

//namespace NMSG2DiscordBot.Modules
//{
//    [Group("test", "슬래시 커맨드 테스트 그룹")]
//    public class TestModule : InteractionModuleBase<SocketInteractionContext>
//    {
//        public InteractionService Commands { get; set; }
//        private CommandHandler _handler;
//        public TestModule(CommandHandler handler)
//        {
//            _handler = handler;
//        }

//        [SlashCommand("유저정보", "유저 정보 테스트")]
//        public async Task UserInfoAsync(SocketUser user = null)
//        {
//            var userInfo = user ?? Context.User;
//            await RespondAsync($"{userInfo.Username}#{userInfo.Id}");
//        }

//        [SlashCommand("jsontest", "유저 정보 테스트")]
//        public async Task JsonTestAsync()
//        {
//            List<Umamusume> uList = JSONManager.GetUmamusumeList();

//            Console.WriteLine(uList[0].ToString());
//            await RespondAsync(uList[0].ToString());
//            await RespondAsync("JObject Test Complete");
//        }

//        [SlashCommand("등록", "등록 테스트")]
//        public async Task RegisterTestAsync(String uName = null)
//        {
//            List<Umamusume> uList = JSONManager.GetUmamusumeList();

//            try
//            {
//                UmamusumeManager.Register(Context.User.Id, uName, ref uList);
//                JSONManager.SetUmamusumeList(uList);
//                await RespondAsync("Register Comlete");
//            }
//            catch (UmamusumeNameNotFoundException e)
//            {
//                await RespondAsync("Register Resigned : Umamusume name Incorrect");
//            }
//            catch (UmamusumeAlreadyRegisteredException e)
//            {
//                await RespondAsync("Register Resigned : Umamusume Already Registered");
//            }
//            catch (DiscordIdAlreadyRegisteredException e)
//            {
//                await RespondAsync("Register Resigned : User Already Registered");
//            }

//        }

//        [SlashCommand("더비", "더비 테스트")]
//        public async Task DerbyTestAsync()
//        {
//            await RespondAsync("테스트 더비 시작");

//            List<String> derbyCast = Derby.TestDerby_Process();

//            if (derbyCast.Count <= 0)
//            {
//                await RespondAsync("derbyCast is Empty. Test End.");
//                return;
//            }

//            //await ReplyAsync(derbyCast.Last<String>());


//            foreach (String str in derbyCast)
//            {
//                await ReplyAsync(str);
//                await Task.Delay(2000);
//            }

//            await ReplyAsync("Test Derby Complete");
//        }
//    }
//}