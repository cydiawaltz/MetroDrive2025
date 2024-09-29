using System;
using AtsEx.PluginHost.Plugins;
using BveTypes.ClassWrappers;
using FastMember;
using TypeWrapping;
using ObjectiveHarmonyPatch;
using AtsEx.PluginHost.Native;
//using AtsEx.PluginHost.MapStatements;
using AtsEx.Extensions.MapStatements;
using Mackoy.Bvets;
using System.Windows.Forms;
//using MetroDrive.Extension;
using AtsEx.Extensions.SoundFactory;
using AtsEx.PluginHost;

namespace MetroDrive.MapPlugin
{
    [Plugin(PluginType.MapPlugin)]
    public class MapPluginMain : AssemblyPluginBase
    {
        int index;
        double NeXTLocation;
        double nowLocation;
        int EB;
        int power;
        int brake;
        int nowMilli;
        double speed;
        string now;
        string arrive;
        int arriveMilli;
        int passMilli;
        bool pass;
        TimeDrawer timeDrawer;
        Life life;
        UIDrawer uIDrawer;
        Pause pause;
        Keikoku keikoku;
        SoundControll soundControll;
        int atc;
        bool hideHorn;
        //フラグ
        bool isDelay = false;
        bool isEB = false;
        bool isTeituu = false;
        bool isGreat = false;
        bool isEBStop = false;
        bool isOverRun = false;//「停止位置を修正します」みたいな感じのウィザード
        bool isRestart = false;
        int time;
        bool isBonus;//ボーナスがある時はこれで時刻表を追加する ->　これの時は条件付きでEndGameを茅場町で飛ばす
        bool isTaiken = false;//体験版
        int pointerIndex;
        bool isEnterPressed = false; 
        bool isExitScenario = false;
        bool isVoiceOn = true;
        bool isUIOff = false;
        //bool isGame;//ゲーム実行中意外はFormをオフに ->拡張機能の方に移植
        bool isPause = false;
        int timer;
        int nativepower;
        int nativebrake;
        int goukakuhani = 4;//初期設定（life.OnStartウンタラ()で上書き）
        /*string sharedMes
        {
            get => Extensions.GetExtension<PluginMain>().mapMes;
            set => Extensions.GetExtension<PluginMain>().mapMes = value;
        }*/
        string sharedMes;
        string stationName;
        string leaveStationName;
        TimeSpan totalElapsed = TimeSpan.Zero;
        TimeSpan taikenElapsed = TimeSpan.Zero;
        ISoundFactory soundFactory;
        Sound sound;//test
        HarmonyPatch drawPatch;
        bool isAuto = false;
        double oldSpeed;//1F前の速度
        bool isOverEnd = false;
        public MapPluginMain(PluginBuilder builder) : base(builder)
        {
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Launch();
            }
            sharedMes = "none";
            life = new Life();
            timeDrawer = new TimeDrawer();
            uIDrawer = new UIDrawer();
            pause = new Pause();
            keikoku = new Keikoku();
            soundControll = new SoundControll();
            //
            IStatementSet statementSet = Extensions.GetExtension<IStatementSet>();
            ClauseFilter metroFilter = new ClauseFilter("MetroDrive", ClauseType.Element);
            Statement levelStatement = statementSet.FindUserStatement("Wattz",metroFilter,new ClauseFilter("Level",ClauseType.Function));
            Statement bonusStatement = statementSet.FindUserStatement("Wattz", metroFilter, new ClauseFilter("isBonus", ClauseType.Function));
            Statement taikenStatement = statementSet.FindUserStatement("Wattz", metroFilter, new ClauseFilter("isTaiken", ClauseType.Function));
            //Statement locationStatement = statementSet.FindUserStatement("Wattz", metroFilter, new ClauseFilter("addedLocation", ClauseType.Function));
            //Statement arrivalStatement = statementSet.FindUserStatement("Wattz", metroFilter, new ClauseFilter("addedArrival", ClauseType.Function));
            Statement kayabaStatement = statementSet.FindUserStatement("Wattz", metroFilter, new ClauseFilter("isKayaba", ClauseType.Function));
            //keysはstru~[].putの[]のなか
            /*if (levelStatement.Source.Clauses[4].Args[0].ToString() == "Easy")
            {
                life.OnStartEasy();
            }*/
            life.OnStartEasy(goukakuhani);//仮
            //DirectX系処理
            ClassMemberSet assistantDrawerMembers = BveHacker.BveTypes.GetClassInfoOf<AssistantDrawer>();
            FastMethod drawMethod = assistantDrawerMembers.GetSourceMethodOf(nameof(AssistantDrawer.Draw));
            drawPatch = HarmonyPatch.Patch(Name, drawMethod.Source, PatchType.Prefix);
            timeDrawer.CreateModel(Location);
            uIDrawer.CreateModel(Location);
            pause.CreateModel(Location);
            keikoku.CreateModel(Location);
            drawPatch.Invoked += DrawPatch_Invoked;
            //計器照明を有効に
            InputEventArgs inputEventArgs = new InputEventArgs(-2, 15);
            //時刻表を無効に
            InputEventArgs inputEventArgs2 = new InputEventArgs(-3, 8);
            BveHacker.KeyProvider.KeyDown_Invoke(inputEventArgs);
            BveHacker.KeyProvider.KeyUp_Invoke(inputEventArgs);
            BveHacker.KeyProvider.KeyDown_Invoke(inputEventArgs2);
            BveHacker.KeyProvider.KeyUp_Invoke(inputEventArgs2);
            isPause= false;
            Native.BeaconPassed += new BeaconPassedEventHandler(BeaconPassed);
            Native.HornBlown += new HornBlownEventHandler(life.OnHorn);
            BveHacker.MainFormSource.KeyDown += OnKeyDown;
            BveHacker.ScenarioCreated += OnScenarioCreated;
            BveHacker.MainFormSource.Activate();
            BveHacker.MainFormSource.TopMost = true;
        }

        void OnScenarioCreated(ScenarioCreatedEventArgs e)
        {
            soundControll.OnStart(Extensions.GetExtension<ISoundFactory>(),Location);
        }
        private PatchInvokationResult DrawPatch_Invoked(object sender, PatchInvokedEventArgs e)
        {
            timeDrawer.Patch(NeXTLocation, nowLocation, isUIOff, goukakuhani);
            uIDrawer.UIDraw(power, nativebrake, EB, life.isTeituu, life.life, isUIOff,isAuto);
            uIDrawer.LifeDraw(life.life, isUIOff);
            pause.PauseMenuDrawer(pointerIndex, isEnterPressed, isExitScenario, isVoiceOn, isUIOff);
            keikoku.Patch(isEB,isEBStop,isOverRun,isRestart,speed,BveHacker.Scenario.Vehicle.Doors.AreAllClosed);
            return PatchInvokationResult.DoNothing(e);
        }

        public override void Dispose()
        {
            Native.BeaconPassed -= BeaconPassed;
            Native.HornBlown -= life.OnHorn;
            drawPatch.Invoked -= DrawPatch_Invoked;
            soundControll.OnDispose();
        }
        public override TickResult Tick(TimeSpan elapsed)
        {
            totalElapsed += elapsed;
            if(totalElapsed.TotalSeconds >= 1)
            {
                FlagBuilder();
                totalElapsed -= TimeSpan.FromSeconds(1);
            }
            OnStop();
            var station = BveHacker.Scenario.Route.Stations[index] as Station;
            if (station == null)
            {
                arriveMilli = station.ArrivalTimeMilliseconds;
                passMilli = station.DepartureTimeMilliseconds;
                pass = station.Pass;
            }
            stationName = station.Name;
            if(index-1>=0)
            {
                var leavestation = BveHacker.Scenario.Route.Stations[index - 1] as Station;
                leaveStationName = leavestation.Name;
            }
            index = BveHacker.Scenario.Route.Stations.CurrentIndex + 1;//Index
            nowLocation = Native.VehicleState.Location;//現在位置を設定
            NeXTLocation = BveHacker.Scenario.Route.Stations[index].Location;//次駅位置
            EB = 9;//EB
            //power = BveHacker.Handles.Power.Notch;//PowerNotch
            //brake = BveHacker.Handles.Brake.Notch;//BrakeNotch
            nativepower = Native.Handles.Power.Notch;//ATCの方
            nativebrake = Native.Handles.Brake.Notch;
            power = BveHacker.Scenario.Vehicle.Instruments.Cab.Handles.PowerNotch;//入力の方
            brake = BveHacker.Scenario.Vehicle.Instruments.Cab.Handles.BrakeNotch;
            nowMilli = BveHacker.Scenario.TimeManager.TimeMilliseconds;//Now
            oldSpeed = speed;
            speed = Native.VehicleState.Speed;//speed
            now = BveHacker.Scenario.TimeManager.Time.ToString("hhmmss");
            arrive = station.DepartureTime.ToString("hhmmss");
            if(speed<0||(speed>0&&!BveHacker.Scenario.Vehicle.Doors.AreAllClosed))
            {
                BveHacker.Scenario.LocationManager.SetSpeed(0);
            }
            if(brake > 0||nativebrake > 0)
            {
                BveHacker.Scenario.Vehicle.Instruments.Cab.Handles.PowerNotch = 0;
            }
            if(!(brake == nativebrake))
            {
                isAuto = true;
            }
            else
            {
                isAuto= false;
            }
            if (pass == true && NeXTLocation == nowLocation)
            {
                OnPass();
            }
            timeDrawer.Tick(index, now, NeXTLocation, nowLocation, arrive);
            uIDrawer.tick(life.life);
            if (isExitScenario&&speed == 0)
            {
                sharedMes = "Exit";
            }
            if (timer < 3) { timer++; }
            if (timer == 2)//1だと動かない（うさぷら側の仕様？？）
            {
                InputEventArgs inputEventArgs2 = new InputEventArgs(2, 8);
                BveHacker.KeyProvider.LeverMoved_Invoke(inputEventArgs2);
            }
            if (!pass && nowLocation > goukakuhani + NeXTLocation)
            {
                isOverRun = true;
            }
            else
            {
                isOverRun = false;
            }
            if(life.life == 0)
            {
                keikoku.GameOver(sharedMes);
                isExitScenario = true;
                BveHacker.Scenario.Vehicle.Instruments.Cab.Handles.BrakeNotch = 8;
                BveHacker.Scenario.Vehicle.Instruments.Cab.Handles.PowerNotch = 0;
                double speedMS = BveHacker.Scenario.LocationManager.SpeedMeterPerSecond;
                BveHacker.Scenario.LocationManager.SetSpeed(speedMS-0.08);
                if(life.isOverSound)
                {
                    soundControll.PlaySound(Extensions.GetExtension<ISoundFactory>(), Location, "tin.wav");
                    life.isOverSound = false;
                }
            }
            if (isTaiken)
            {
                if (taikenElapsed.TotalSeconds < 35){taikenElapsed += elapsed;}
                if(taikenElapsed.TotalSeconds > 33 && keikoku.isTaikenStart){keikoku.TaikenStart(power, brake);}
                if (taikenElapsed.TotalSeconds > 33 && keikoku.isAtcIntro){keikoku.TaikenATC(); }
                if (taikenElapsed.TotalSeconds > 33 && keikoku.isCheckAtc) {keikoku.CheckAtc(speed);}
                if (taikenElapsed.TotalSeconds < 33){keikoku.TaikenUpdate(taikenElapsed);}
                if(life.life == 0) { 
                    keikoku.TaikenEnd(false,Location); }
                if (isOverEnd)
                {
                    double speedMS = BveHacker.Scenario.LocationManager.SpeedMeterPerSecond;
                    if(speed > 10)
                    {
                        BveHacker.Scenario.LocationManager.SetSpeed(speedMS - 0.1);
                    }
                    BveHacker.Scenario.Vehicle.Instruments.Cab.Handles.BrakeNotch = 8;
                }
                life.istaiken = keikoku.isTaikenLife;
                //if(keikoku.clear&&speed == 0) { keikoku.TaikenEnd(true); }
            }
            return new MapPluginTickResult();
        }
        public void BeaconPassed(BeaconPassedEventArgs e)
        {
            switch (e.Type)
            {
                case 10://信号0
                    atc = 0;
                    //if (isVoiceOn) { /*音を再生する処理*/}
                    break;
                case 18://ATC信号40
                    atc = 40;
                    break;
                case 19://ATC45
                    atc = 45;
                    break;
                case 21://ATC55
                    atc = 55;
                    break;
                case 23: //ATC65
                    atc = 65;
                    break;
                case 25://ATC75
                    atc = 75;
                    break;
                case 26://Atc80
                    atc = 80;
                    break;
                case 800://ダイアログ開始
                    isTaiken = true;
                    life.istaiken = true;
                    keikoku.isTaikenStart = true;
                    keikoku.isTaikenLife = true;
                    break;
                case 801:
                    keikoku.isTaikenStart = false;
                    keikoku.isAtcIntro = true;//ATCについての紹介
                    break;
                case 802:
                    keikoku.isAtcIntro = false;
                    keikoku.isCheckAtc = true;
                    break;
                case 803:
                    keikoku.isCheckAtc = false;
                    keikoku.TaikenStop();
                    keikoku.next = true;
                    break;
                case 804://906番と同じように設置
                    if(!isOverRun)
                    {
                        life.istaiken = false;
                    }
                    break;
                case 805:
                    keikoku.next = false;
                    break;
                case 806://終着地点の位置番手前
                    keikoku.clear = true;
                    break;
                case 807:
                    isOverEnd = true;
                    break;
                case 900://隠し警笛開始
                    hideHorn = true;
                    break;
                case 901://隠し警笛終了
                    hideHorn = false;
                    break;
                case 902://「墨堤通り」ダイアログ
                    
                    break;
                case 903://発車判定
                    if (!isOverRun) { keikoku.DrawDialog("leave", leaveStationName, 3000); }
                    break;
                case 904://停車/通過判定
                    if (pass) { keikoku.DrawDialog("pass", stationName, 3000); }
                    else { keikoku.DrawDialog("stop", stationName, 3000); }
                    break;
                case 905:
                    //「次は停車駅です」とか
                    break;
                case 906://過走時に
                    if (isOverRun&&isVoiceOn) { soundControll.PlaySound(Extensions.GetExtension<ISoundFactory>(), Location,"bi.wav"); }
                break;
            }
        }
        void FlagBuilder()
        {
            isRestart = false;
            isEB = false;
            isEBStop = false;
            isTeituu = false;
            if (nowMilli - arriveMilli < 0)
            {
                if (pass == true) {
                    if (Math.Abs(nowLocation - NeXTLocation) > goukakuhani)
                    { isDelay = true;}
                    if (Math.Abs(nowLocation - NeXTLocation) < goukakuhani && !(speed == 0))
                    { isDelay = true; }
                }
                else 
                {
                    if (nowLocation < NeXTLocation)
                    {
                        isDelay = true;
                    }
                }
            }
            if(!pass&&NeXTLocation<nowLocation-goukakuhani&&speed == 0&&BveHacker.Scenario.Vehicle.Doors.AreAllClosed)
            {
                FixOverRun();
            }
            if (Math.Abs(arriveMilli - nowMilli) < 1000 && NeXTLocation == nowLocation)
            { isTeituu = true; }
            if (brake == EB && speed > 0 && NeXTLocation - nowLocation < 150)
            { isEBStop = true; }
            if (brake == EB && speed > 0 && NeXTLocation - nowLocation >= 150)
            { isEB = true; }
            life.NewUpdate(isDelay, isEB, isTeituu, isGreat, isEBStop, isOverRun, nowLocation, NeXTLocation, isRestart);
        }
        void OnStop()
        {
            if (Math.Abs(nowLocation - NeXTLocation) < 1 && Math.Abs(nowMilli - arriveMilli) >= 2000 && speed == 0 && oldSpeed > 0 && BveHacker.Scenario.Vehicle.Doors.AreAllClosed)
            { life.Good(); keikoku.GoodGreat("good"); if (isTaiken) { keikoku.TaikenStoped(true, false, stationName,Location); } }
            if (Math.Abs(nowLocation - NeXTLocation) < goukakuhani && speed == 0 && oldSpeed > 0 && BveHacker.Scenario.Vehicle.Doors.AreAllClosed)//事実上のOnStop??
            { if (isTaiken) { keikoku.TaikenStoped(false, false, stationName, Location); } }
            if (Math.Abs(nowLocation - NeXTLocation) < 1 && Math.Abs(nowMilli - arriveMilli) < 2000 && speed == 0 && oldSpeed > 0)
            { isGreat = true; }
        }

        void OnPass()
        {
            if (Math.Abs(arriveMilli - nowMilli) < 1000 && Math.Abs(NeXTLocation - nowLocation) < 5)
            { isTeituu = true; }
        }
        void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.P) 
            { 
                pause.OnInputP();
                soundControll.PlaySound(Extensions.GetExtension<ISoundFactory>(),Location, "pon.wav");
            }
            if (e.KeyCode == Keys.Return) { isEnterPressed = true; }
            if (e.KeyCode == Keys.R)
            {
                isUIOff = !isUIOff;
            }
        }
        void FixOverRun()//オーバーランを直す
        {
            int overrun = Convert.ToInt32(nowLocation - NeXTLocation);
            if(life.istaiken)
            {
                life.Decrease(overrun);
            }
            if(life.life>0)
            {
                BveHacker.Scenario.LocationManager.SetLocation(NeXTLocation, false);//距離呈の変更
            }
            isOverRun = false;
            if(isTaiken)
            {
                keikoku.TaikenStoped(false, true, stationName, Location);
            }
        }
        
        /*
         *メモ
         *終了コード（EndBVEのwritething）
         *0→
         *1→茅場町追加ミス
         */
    }
 }
