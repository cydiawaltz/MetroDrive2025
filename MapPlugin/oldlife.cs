using System;
using AtsEx.PluginHost;
using BveTypes.ClassWrappers;

namespace AtsExCsTemplate.MapPlugin
{
    
    internal class oldlife
    {
        //級数によって変更
        //減点
        int overatc;
        public bool overatcset;//速度超過時に音にする
        int overtime;//時間超過時は警告表示を出さず、文字の色のみ
        int restart;
        public bool restartset;//再加速時
        int EBbrake;
        public bool EBbrakeset;
        int EBstop;
        public bool EBstopset;//駅構内でEBを使用したとき
        //加点
        int teitu;
        public bool teituset;//定通時
        int good;
        public bool goodset;//good
        int grate;
        public bool grateset;//grate
        int bonus;
        public bool bonusset;//ボーナス（各死刑的）
        //その他
        int GoukakuHani;
        public int lifetime;
        //以下同じ
        int atc;
        bool HideHorn;
        public double NowLocation;
        public double NeXTLocation;
        public int Power;
        public int Brake;
        public int now;//ミリ秒でいくので表示側でTimeSpanに変換してくれ
        double speed;
        public int arrival;//以上同文
        public int pass;//以上同文
        public bool passset; 
        int index;
        public void PluginMain(PluginBuilder builder) : base(builder)
        {
            //難しさごとに変更（現在:初級）
            //減点
            overatc = 2;//予告無視
            overtime = 1;//時間超過（一秒おき）
            restart = 5;//駅構内再加速
            EBbrake = 5;//非常ブレーキ
            EBstop = 5;//非常聖堂停車
            overatcset = false;
            restartset = false;
            EBbrakeset = false;
            EBstopset = false;
            teituset = false;
            goodset = false;
            grateset = false;
            //加点
            teitu = 3;//定通
            good = 3;//Good停車
            grate = 5;//Grate!停車
            bonus = 2;//ボーナス
            //その他
            GoukakuHani = 4;//合格範囲
            lifetime = 30;//初期持ち時間
            //以下共通設定
            atc = 80;
            HideHorn = false;
            //Beaconイベント
            Native.BeaconPassed += new BeaconPassedEventHandler(BeaconPassed);

        }

        public override void Dispose()
        {
            Native.BeaconPassed -= BeaconPassed;
        }

        public override TickResult Tick(TimeSpan elapsed)
        {
            index = BveHacker.Scenario.Route.Stations.CurrentIndex + 1;//Index
            var station = BveHacker.Scenario.Route.Stations[index] as Station;
            if (station == null)
            {
                arrival = station.ArrivalTimeMilliseconds;
                past = station.DepartureTimeMilliseconds;
                pass = station.Pass;
            }
            NowLocation = Native.VehicleState.Location;//現在位置を設定
            NeXTLocation = BveHacker.Scenario.Route.Stations[index].Location;//次駅位置
            Power = Native.Handles.Power.Notch;//PowerNotch
            Brake = Native.Handles.Brake.Notch;//BrakeNotch
            now = BveHacker.Scenario.TimeManager.TimeMilliseconds;//Now
            speed = Native.VehicleState.Speed;//speed
            return new MapPluginTickResult();
        }
        public void BeaconPassed(BeaconPassedEventArgs e)
        {
            switch (e.type)
            {
                case 10://信号0
                    atc = 0;
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
                case 900://隠し警笛開始
                    HideHorn = true;
                    break;
                case 901://隠し警笛終了
                    HideHorn = false;
                    break;
            }
        }
    }
}
