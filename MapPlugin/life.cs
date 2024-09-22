using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AtsEx.PluginHost.Native;

namespace MetroDrive
{
    internal class Life
    {
        
        //級数によって変更
        //減点
        public int overatc;
        public bool isOveratc;//速度超過時に音にする
        public int overtime;//時間超過時は警告表示を出さず、文字の色
        public bool isRestart;//再加速時
        public int restart;
        public int EBbrake;
        //public int EB;
        public bool isEB;
        public int EBstop;
        public bool isEBStop;//駅構内でEBを使用したとき
        //加点
        public int teitu;
        public bool isTeituu;//定通時
        public int good;
        public bool isGood;//good
        public int great;
        public bool isGreat;//grate
        public int bonus;
        public bool isBonus;//ボーナス（各死刑的）
        public int life;
        //その他
        //public int GoukakuHani;
        //以下同じ
        public int atc;
        public bool HideHorn;
        public bool isOverSound;

        public bool istaiken = false;//これは減点しないと設定するだけ(MainのisTaikenとは別物)
        public void OnStartFreeRun(int goukakuHani)//初期化
        {
            //難しさごとに変更（現在:初級）
            life = 999;
            //減点
            overatc = 0;//予告無視
            overtime = 0;//時間超過（一秒おき）
            restart = 0;//駅構内再加速
            EBbrake = 0;//非常ブレーキ
            EBstop = 0;//非常聖堂停車
            //加点
            teitu = 0;//定通
            good = 0;//Good停車
            great = 0;//Grate!停車
            bonus = 0;//ボーナス
            //その他
            goukakuHani = 200;//合格範囲
            //以下共通設定
            atc = 110;
            HideHorn = false;
        }
        public void OnStartElement(int goukakuHani)//初期化
        {
            //難しさごとに変更（現在:初級）
            life = 999;
            //減点
            overatc = 0;//予告無視
            overtime = 0;//時間超過（一秒おき）
            restart = 0;//駅構内再加速
            EBbrake = 0;//非常ブレーキ
            EBstop = 0;//非常聖堂停車
            //加点
            teitu = 0;//定通
            good = 0;//Good停車
            great = 0;//Grate!停車
            bonus = 0;//ボーナス
            //その他
            goukakuHani = 8;//合格範
            //以下共通設定
            atc = 110;
            HideHorn = false;
        }
        public void OnStartEasy(int goukakuHani)//初期化
        {
            //難しさごとに変更（現在:初級）
            life = 50;
            //減点
            overatc = 2;//予告無視
            overtime = 1;//時間超過（一秒おき）
            restart = 5;//駅構内再加速
            EBbrake = 5;//非常ブレーキ
            EBstop = 5;//非常聖堂停車
            //加点
            teitu = 3;//定通
            good = 3;//Good停車
            great = 5;//Grate!停車
            bonus = 2;//ボーナス
            //その他
            goukakuHani = 4;//合格範囲
            //以下共通設定
            atc = 110;
            HideHorn = false;
        }
        public void OnStartNormal(int GoukakuHani)//初期化
        {
            //難しさごとに変更（現在:初級）
            life = 40;
            //減点
            overatc = 4;//予告無視
            overtime = 1;//時間超過（一秒おき）
            restart = 5;//駅構内再加速
            EBbrake = 10;//非常ブレーキ
            EBstop = 10;//非常聖堂停車
            //加点
            teitu = 5;//定通
            good = 3;//Good停車
            great = 5;//Grate!停車
            bonus = 2;//ボーナス
            //その他
            GoukakuHani = 2;//合格範囲
            //以下共通設定
            atc = 110;
            HideHorn = false;
        }
        public void OnStartHard(int GoukakuHani)//初期化
        {
            //難しさごとに変更（現在:初級）
            life = 30;
            //減点
            overatc = 4;//予告無視
            overtime = 2;//時間超過（一秒おき）
            restart = 5;//駅構内再加速
            EBbrake = 5;//非常ブレーキ
            EBstop = 5;//非常聖堂停車
            //加点
            teitu = 3;//定通
            good = 3;//Good停車
            great = 5;//Grate!停車
            bonus = 2;//ボーナス
            //その他
            GoukakuHani = 4;//合格範囲
            //以下共通設定
            atc = 80;
            HideHorn = false;
        }
        public void OnStartVeryHard(int GoukakuHani)//初期化
        {
            //難しさごとに変更（現在:初級）
            life = 10;
            //減点
            overatc = 5;//予告無視
            overtime = 3;//時間超過（一秒おき）
            restart = 15;//駅構内再加速
            EBbrake = 10;//非常ブレーキ
            EBstop = 10;//非常聖堂停車
            //加点
            teitu = 2;//定通
            good = 2;//Good停車
            great = 3;//Grate!停車
            bonus = 1;//ボーナス
            //その他
            GoukakuHani = 1;//合格範囲
            //以下共通設定
            atc = 110;
            HideHorn = false;
        }

        public void NewUpdate(bool isDelay, bool isEB, bool isTeituu, bool isGreat, bool isEBStop, bool isOverRun, double nowLocation, double NeXTLocation, bool isRestart)
        {
            if (isDelay == true)
            {
                Decrease(overtime);
                isDelay = false;
            }
            if (isEB == true)
            {
                Decrease(EBbrake);
                isEB = false;
            }
            if (isTeituu == true)
            {
                life += teitu;
                isTeituu = false;
            }
            if (isGreat == true)
            {
                life += great;
                isGreat = false;
            }
            if (isEBStop == true)
            {
                Decrease(EBstop);
                isEBStop = false;
            }
            if (isRestart == true)
            {
                Decrease(restart);
                isRestart = false;
            }
        }
        public void Good()
        {
            life+=good;
        }
        public void Decrease(int minus)
        {
            if(minus < life&&!istaiken)
            {
                life -= minus;
            }
            else if(!istaiken)
            {
                life = 0;
                isOverSound = true;
            }
        }
        public void OnHorn(HornBlownEventArgs e)//警笛イベントのときに呼ばれる
        {
            if(HideHorn == true)//警笛ボーナス
            {
                life += bonus;
                isBonus = true;
            }
        }
    }
}
