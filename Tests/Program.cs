using LumenRush;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
int checks=0;
void Check(bool condition,string message){if(!condition)throw new Exception(message);checks++;}
string root=Path.GetFullPath(Path.Combine(AppContext.BaseDirectory,"../../../../"));
foreach(string file in Directory.GetFiles(Path.Combine(root,"Assets"),"*.cs",SearchOption.AllDirectories)){
 var tree=CSharpSyntaxTree.ParseText(File.ReadAllText(file));
 var errors=tree.GetDiagnostics().Where(d=>d.Severity==DiagnosticSeverity.Error).ToArray();
 Check(errors.Length==0,file+": "+string.Join("; ",errors.Select(e=>e.ToString())));
 if(args.Contains("--format"))File.WriteAllText(file,tree.GetRoot().NormalizeWhitespace(indentation:"    ",eol:"\n").ToFullString()+"\n");
}
for(int seed=1;seed<=100;seed++)for(int row=0;row<1000;row++){
 int lane=RunRules.SafeLane(seed,row);Check(lane>=-1&&lane<=1,"Safe lane out of bounds");
 Check(lane==RunRules.SafeLane(seed,row),"Generation must be deterministic");
}
Check(RunRules.ClampLane(-10)==-1&&RunRules.ClampLane(10)==1,"Lane clamp");
Check(RunRules.Speed(0)==12&&RunRules.Speed(100000)==30,"Speed curve bounds");
Check(RunRules.Crosses(5,-5,.8f),"Swept hit must survive low frame rate");
Check(!RunRules.Crosses(-5,-6,.8f)&&!RunRules.Crosses(5,4,.8f),"No false crossings");
var score=new ScoreManager();score.Advance(100,3);int boosted=score.Total(10);score.Advance(1,1);
Check(score.Total(10)>boosted,"Score cannot fall after booster expires");score.Reset();Check(score.Total(0)==0,"Score reset");
Check(RunRules.Level(0)==1&&RunRules.Level(1000)==3,"XP levels");
var now=new DateTime(2026,9,14,12,0,0,DateTimeKind.Utc);
Check(RunRules.CanClaimDaily("",now),"First daily claim");Check(!RunRules.CanClaimDaily("2026-09-14",now),"No duplicate daily claim");
Check(!RunRules.CanClaimDaily("2026-09-15",now),"Clock rollback cannot reclaim");Check(RunRules.CanClaimDaily("2026-09-13",now),"Next-day reward");
Directory.CreateDirectory(UnityEngine.Application.persistentDataPath);
try {
 var save=new SaveManager(new LocalSaveStore());var shop=new ShopManager(save);var missions=new MissionManager(save);
 Check(!shop.BuyCharacter(1)&&save.Data.coins==0,"Insufficient funds cannot spend");
 Check(!shop.BuyCharacter(-1)&&!shop.BuyCharacter(3),"Reject invalid character IDs");
 save.Data.coins=2000;Check(shop.BuyCharacter(1)&&save.Data.coins==1600,"Character purchase");
 Check(shop.BuyCharacter(1)&&save.Data.coins==1600,"Owned characters equip for free");
 Check(shop.BuyUpgrade()&&save.Data.upgrade==1&&save.Data.coins==1450,"Upgrade purchase");
 Check(shop.BuyCosmetic(true)&&save.Data.board==1&&save.Data.coins==1150,"Cosmetic purchase");
 shop.BuyCosmetic(true);Check(save.Data.coins==1150,"No duplicate cosmetic charge");
 missions.Record(500,3000,25,5);save.Data.bestDistance=3000;int wallet=save.Data.coins;
 missions.ClaimMissions();Check(save.Data.coins==wallet+150+500+800,"All mission and achievement payouts");
 wallet=save.Data.coins;missions.ClaimMissions();Check(save.Data.coins==wallet,"Rewards are idempotent");
 Check(missions.ClaimDailyReward(),"Daily reward delivery");Check(!missions.ClaimDailyReward(),"Daily reward is idempotent");
 var leaderboard=new LocalLeaderboard(save);leaderboard.Submit(900);leaderboard.Submit(500);Check(leaderboard.PersonalBest==900,"High score monotonicity");
 save.Flush();var loaded=new SaveManager(new LocalSaveStore());Check(loaded.Data.coins==save.Data.coins&&loaded.Data.character==1,"Save round trip");
 File.WriteAllText(Path.Combine(UnityEngine.Application.persistentDataPath,"lumen-save.json"),"invalid");
 loaded=new SaveManager(new LocalSaveStore());Check(loaded.Data.coins==save.Data.coins,"Corrupt primary recovers backup");
 save.Data.dailyPeriod="2000-01-01";save.Data.weeklyPeriod="2000-01-01";missions.Refresh();
 Check(save.Data.dailyCoins==0&&!save.Data.dailyMissionClaimed&&save.Data.weeklyMeters==0,"Mission period reset");
}finally{Directory.Delete(UnityEngine.Application.persistentDataPath,true);}
Console.WriteLine($"PASS: {checks:N0} checks. Domain code compiled; all Unity C# files syntax parsed. Unity runtime/build still requires editor validation.");
