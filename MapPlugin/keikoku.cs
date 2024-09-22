using System;
using AtsEx.PluginHost.Plugins;
using BveTypes.ClassWrappers;
using FastMember;
using TypeWrapping;
using ObjectiveHarmonyPatch;
using System.IO;
using System.Drawing;
using SlimDX;
using SlimDX.Direct3D9;
using AtsEx.PluginHost;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;
using static System.Collections.Specialized.BitVector32;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Windows.Forms;
using System.Diagnostics;

namespace MetroDrive.MapPlugin
{
    internal class Keikoku
    {
        Device device;
        int width;
        int height;
        Model eB;
        Model eBStop;
        Model over;
        Model restart;
        Model stopSoon;
        Model leave;
        Model h16;
        Model h17;
        Model h18;
        Model h19;
        Model pass;
        Model stop;
        Model gameover;
        Model good;
        Model great;
        Model bokutei;
        Model taikenDialog0;
        Model taikenDialog1;
        Model taikenDialog2;
        Model taikenDialog3;
        Model taikenDialog4;
        Model taikenDialog5;
        Model taikenDialog6;
        Model taikenDialog7;
        Model taikenDialog8;
        Model taikenDialog9;
        Model taikenDialog10;
        Model taikenDialog11;
        Model taikenDialog12;
        Model taikenDialog13;
        Model taikenDialog14;
        Model taikenDialog15;
        Model taikenDialog16;
        Model taikenDialog17;
        Model taikenDialog18;
        Model taikenDialog19;
        Model taikenDialog20;
        Model taikenDialog21;
        Model taikenDialog22;
        Model taikenDialog23;
        Model taikenDialog24;
        Model taikenDialog25;
        Model taikenDialog26;
        Model taikenDialog27;
        Model taikenDialog28;
        Model taikenDialog29;
        Model taikenDialog30;
        Thread thread;
        bool isDrawing;
        string drawingName;
        string stationName;
        bool isGameOver;
        int endTimer = -1000;
        bool isGood;
        bool isGreat;
        bool taikenDrawing;
        public int taikenindex;
        bool istaikenDrawing = false;
        public bool isTaikenStart = false;//
        public bool isAtcIntro = false;
        public bool isCheckAtc = false;
        public bool next = false;
        public bool clear = false;//Mainでしか使わない
        public bool isTaikenLife = false;
        public void CreateModel(string Location)
        {
            eB = CreateModels(@"picture\keikoku\EB.png",-150,-60,300,120);
            eBStop = CreateModels(@"picture\keikoku\EBStop.png", -210, -60, 420, 120);
            over = CreateModels(@"picture\keikoku\over.png", -150, -60, 300, 120);
            restart = CreateModels(@"picture\keikoku\restart.png", -210, -60, 420, 120);
            stopSoon= CreateModels(@"picture\keikoku\stopsoon.png", -250, -30, 500, 60);
            //ダイアログ
            bokutei = CreateModels(@"picture\station\bokutei.png", -175, -30, 350, 60);
            //駅系
            leave = CreateModels(@"picture\station\leave.png", 0, -60, 240, 120);
            pass = CreateModels(@"picture\station\pass.png", 0, -60, 240, 120);
            stop = CreateModels(@"picture\station\stop.png", 0, -60, 240, 120);
            h16 = CreateModels(@"picture\station\h16.png", -230, -60, 300, 120);
            h17 = CreateModels(@"picture\station\h17.png", -230, -60, 300, 120);
            h18 = CreateModels(@"picture\station\h18.png", -230, -60, 300, 120);
            h19 = CreateModels(@"picture\station\h19.png", -230, -60, 300, 120);
            gameover = CreateModels(@"picture\station\over.png", -275, -75, 550, 150);
            //Goodとか
            good = CreateModels(@"picture\UI\good.png", -275, -150, 550, 300);
            great = CreateModels(@"picture\UI\great.png", -275, -150, 550, 300);
            //体験版
            taikenDialog0 = CreateModels(@"picture\dialog\0.png", -250, 0, 500, 170);
            taikenDialog1 = CreateModels(@"picture\dialog\1.png", -250, 0, 500, 170);
            taikenDialog2 = CreateModels(@"picture\dialog\2.png", 0, 0, 380, 130);
            taikenDialog3 = CreateModels(@"picture\dialog\3.png", 0, 0, 380, 130);
            taikenDialog4 = CreateModels(@"picture\dialog\4.png", -380, 0, 380, 130);
            taikenDialog5 = CreateModels(@"picture\dialog\5.png", -380, 0, 380, 130);
            taikenDialog6 = CreateModels(@"picture\dialog\6.png", -450, -150, 450, 150);
            taikenDialog7 = CreateModels(@"picture\dialog\7.png", -450, -150, 450, 150);
            taikenDialog8 = CreateModels(@"picture\dialog\8.png", -450, -150, 450, 150);
            taikenDialog9 = CreateModels(@"picture\dialog\9.png", -450, -140, 450, 140);
            taikenDialog10 = CreateModels(@"picture\dialog\10.png", -250, 0, 500, 450);
            taikenDialog11 = CreateModels(@"picture\dialog\11.png", -250, 0, 500, 170);
            taikenDialog12 = CreateModels(@"picture\dialog\12.png", -250, 0, 500, 220);
            taikenDialog13 = CreateModels(@"picture\dialog\13.png", -250, 0, 500, 170);
            taikenDialog14 = CreateModels(@"picture\dialog\14.png", -400, 0, 800, 310);
            taikenDialog15 = CreateModels(@"picture\dialog\15.png", -400, 0, 800, 310);
            taikenDialog16 = CreateModels(@"picture\dialog\16.png", -250, 0, 500, 170);
            taikenDialog17 = CreateModels(@"picture\dialog\17.png", -300, 0, 600, 350);
            taikenDialog18 = CreateModels(@"picture\dialog\18.png", -300, 0, 600, 280);
            taikenDialog19 = CreateModels(@"picture\dialog\19.png", -250, 0, 500, 170);
            taikenDialog20 = CreateModels(@"picture\dialog\20.png", -350, 0, 700, 240);
            taikenDialog21 = CreateModels(@"picture\dialog\21.png", -250, 0, 500, 85);
            taikenDialog22 = CreateModels(@"picture\dialog\22.png", -300, 0, 600, 280);
            taikenDialog23 = CreateModels(@"picture\dialog\23.png", -250, 0, 500, 85);
            taikenDialog24 = CreateModels(@"picture\dialog\24.png", -250, 0, 500, 170);
            taikenDialog25 = CreateModels(@"picture\dialog\25.png", -250, 0, 500, 130);
            taikenDialog26 = CreateModels(@"picture\dialog\26.png", -250, 0, 500, 150);
            taikenDialog27 = CreateModels(@"picture\dialog\27.png", -250, 0, 500, 170);
            taikenDialog28 = CreateModels(@"picture\dialog\28.png", -250, 0, 500, 170);
            taikenDialog29 = CreateModels(@"picture\dialog\29.png", -250, 0, 500, 170);
            taikenDialog30 = CreateModels(@"picture\dialog\30.png", -250, 0, 500, 85);
            Model CreateModels(string path, float x, float y, float sizex, float sizey)
            {
                string texFilePath = Path.Combine(Path.GetDirectoryName(Location), path);
                RectangleF rectangleF = new RectangleF(x, y, sizex, -sizey);
                Model brakeNotch = Model.CreateRectangleWithTexture(rectangleF, 0, 0, texFilePath);//四角形の3Dモデル
                return brakeNotch;
            }
        }
        public void Patch(bool isEB,bool isEBStop,bool isOver,bool isRestart,double speed,bool doorClosed)
        {
            device = Direct3DProvider.Instance.Device;
            width = Direct3DProvider.Instance.PresentParameters.BackBufferWidth;
            height = Direct3DProvider.Instance.PresentParameters.BackBufferHeight;
            if (isGameOver)
            {
                if (!(endTimer == 0))
                {
                    endTimer = endTimer + 10;
                }
                device.SetTransform(TransformState.World, Matrix.Translation(endTimer, 300, 0));
                gameover.Draw(Direct3DProvider.Instance, false);
                isEB = false;
                isEBStop = false;
                isOver = false;
                isRestart = false;
                isDrawing = false;
            }
            device.SetTransform(TransformState.World, Matrix.Translation(0 , 300 , 0));
            if(isEB)
            {
                eB.Draw(Direct3DProvider.Instance, false);
            }
            if(isEBStop)
            {
                eBStop.Draw(Direct3DProvider.Instance, false);
            }
            if(isOver)
            {
                over.Draw(Direct3DProvider.Instance, false);
                if(speed > 0)
                {
                    device.SetTransform(TransformState.World, Matrix.Translation(0, 200, 0));
                    stopSoon.Draw(Direct3DProvider.Instance, false);
                }
            }
            if(isRestart)
            {
                device.SetTransform(TransformState.World, Matrix.Translation(0, 300, 0));
                restart.Draw(Direct3DProvider.Instance, false);
            }
            if(isDrawing)
            {
                device.SetTransform(TransformState.World, Matrix.Translation(0, 300, 0));
                switch (drawingName)
                {
                    case "bokutei":
                        bokutei.Draw(Direct3DProvider.Instance, false);
                    break;
                    case "leave":
                        leave.Draw(Direct3DProvider.Instance, false);
                    break;
                    case "pass":
                        pass.Draw(Direct3DProvider.Instance, false);
                    break;
                    case "stop":
                        stop.Draw(Direct3DProvider.Instance, false);
                    break;
                }
                device.SetTransform(TransformState.World, Matrix.Translation(0, 300, 0));
                switch (stationName)
                {
                    case "入谷":
                        h19.Draw(Direct3DProvider.Instance, false);
                        break;
                    case "上野":
                        h18.Draw(Direct3DProvider.Instance, false);
                    break;
                    case "仲御徒町":
                        h17.Draw(Direct3DProvider.Instance, false);
                    break;
                    case "秋葉原":
                        h16.Draw(Direct3DProvider.Instance, false);
                    break;
                }
            }
            if(isGood)
            {
                device.SetTransform(TransformState.World, Matrix.Translation(0, 300, 0));
                good.Draw(Direct3DProvider.Instance, false);
            }
            if (taikenDrawing)
            {//上族
                device.SetTransform(TransformState.World, Matrix.Translation(0, height/2, 0));
                if (taikenindex == 0) { taikenDialog0.Draw(Direct3DProvider.Instance, false); }
                if (taikenindex == 1) { taikenDialog1.Draw(Direct3DProvider.Instance, false); }
                if (taikenindex == 10) { taikenDialog10.Draw(Direct3DProvider.Instance, false); }
                if (taikenindex == 11) { taikenDialog11.Draw(Direct3DProvider.Instance, false); }
                if (taikenindex == 12) { taikenDialog12.Draw(Direct3DProvider.Instance, false); }
                if (taikenindex == 13) { taikenDialog13.Draw(Direct3DProvider.Instance, false); }
                if (taikenindex == 14) { taikenDialog14.Draw(Direct3DProvider.Instance, false); }
                if (taikenindex == 15) { taikenDialog15.Draw(Direct3DProvider.Instance, false); }
                if (taikenindex == 16) { taikenDialog16.Draw(Direct3DProvider.Instance, false); }
                if (taikenindex == 17) { taikenDialog17.Draw(Direct3DProvider.Instance, false); }
                if (taikenindex == 18) { taikenDialog18.Draw(Direct3DProvider.Instance, false); }
                if (taikenindex == 19) { 
                    taikenDialog19.Draw(Direct3DProvider.Instance, false); }
                if (taikenindex == 20) { taikenDialog20.Draw(Direct3DProvider.Instance, false); }
                if (taikenindex == 21) { taikenDialog21.Draw(Direct3DProvider.Instance, false); }
                if (taikenindex == 22) { taikenDialog22.Draw(Direct3DProvider.Instance, false); }
                if (taikenindex == 24) { taikenDialog24.Draw(Direct3DProvider.Instance, false); }
                if (taikenindex == 25) { taikenDialog25.Draw(Direct3DProvider.Instance, false); }
                if (taikenindex == 26) { taikenDialog26.Draw(Direct3DProvider.Instance, false); }
                if (taikenindex == 27) { taikenDialog27.Draw(Direct3DProvider.Instance, false); }
                if (taikenindex == 28) { taikenDialog28.Draw(Direct3DProvider.Instance, false); }
                if (taikenindex == 29) { taikenDialog29.Draw(Direct3DProvider.Instance, false); }
                if (taikenindex == 30) { taikenDialog30.Draw(Direct3DProvider.Instance, false); }
                device.SetTransform(TransformState.World, Matrix.Translation(-width/2+150, height / 2 - 200, 0));
                if (taikenindex == 2) { taikenDialog2.Draw(Direct3DProvider.Instance, false); }
                if (taikenindex == 3) { taikenDialog3.Draw(Direct3DProvider.Instance, false); }
                device.SetTransform(TransformState.World, Matrix.Translation(width / 2 - 150, height / 2 - 200, 0));
                if (taikenindex == 4) { taikenDialog4.Draw(Direct3DProvider.Instance, false); }
                if (taikenindex == 5) { taikenDialog5.Draw(Direct3DProvider.Instance, false); }
                //device.SetTransform(TransformState.World, Matrix.Translation(width / 2 - 400, -height / 2 +100, 0));
                device.SetTransform(TransformState.World, Matrix.Translation(0, 0, 0));
                if (taikenindex == 6) { 
                    taikenDialog6.Draw(Direct3DProvider.Instance, false); }
                if (taikenindex == 7) { taikenDialog7.Draw(Direct3DProvider.Instance, false); }
                if (taikenindex == 8) { taikenDialog8.Draw(Direct3DProvider.Instance, false); }
                device.SetTransform(TransformState.World, Matrix.Translation(-width / 2 + 400, -height / 2 + 100, 0));
                if (taikenindex == 9) { taikenDialog9.Draw(Direct3DProvider.Instance, false); }
            }
        }
        public void DrawDialog(string model,string station,int stoptime)
        {
            thread = new Thread(() => DrawingMain(model,station,stoptime));
            thread.Start();
        }
        public void GameOver(string sharedmes)
        {
            Thread thread2 = new Thread(() => GameOverDraw(sharedmes));
            thread2.Start();
        }
        public async void GoodGreat(string type)
        {
            if(type == "good")
            {
                isGood = true;
            }
            if(type == "great")
            {
                isGreat = true;
            }
            await Task.Delay(2000);
            isGood = false;
            isGreat = false;
        }
        async void GameOverDraw(string sharedmes)
        {
            isGameOver = true;
            await Task.Delay(5000);
            sharedmes = "end";
        }
        async void DrawingMain(string model, string station,int dialogTimer)
        {
            isDrawing = true;
            drawingName = model;
            stationName = station;
            await Task.Delay(dialogTimer);
            isDrawing = false;
        }
        
        public async void TaikenWait(int waitMilliMinutes,int index)
        {
            taikenindex = index;
            if(!istaikenDrawing)
            {
                istaikenDrawing = true;
                taikenDrawing = true;
                await Task.Delay(waitMilliMinutes);
                taikenDrawing = false;
                istaikenDrawing = false;
            }
        }
        public void TaikenStart(int power,int brake)
        {
            taikenDrawing = true;
            if(brake > 0)
            {
                taikenindex = 14;
            }
            else if(power < 4&&brake == 0)
            {
                taikenindex = 15;
            }
            else
            {
                taikenindex = 16;
            }
        }
        public void TaikenATC()
        {
            taikenDrawing = true;
            taikenindex = 17;
        }
        public void CheckAtc(double speed)
        {
            taikenDrawing = false;
            if (speed > 65)
            {
                taikenDrawing = true;
                taikenindex = 18;
            }
        }
        public async void TaikenStop()//ブレーキを調整みたいなダイアログ
        {
            taikenDrawing = true;
            taikenindex = 19;
            await Task.Delay(2000);
            taikenindex = 20;
            await Task.Delay(3000);
            taikenDrawing = false;
        }
        public async void TaikenStoped(bool isGood,bool isOver,string stationName,string Location)//停車時
        {
            taikenDrawing = true;
            if (isGood) { taikenindex = 21; }
            else if (isOver) { taikenindex = 22; }
            else { taikenDrawing = false; }
            await Task.Delay(3000);
            if(stationName == "仲御徒町")
            {
                isTaikenLife = false;
                taikenDrawing = true;
                taikenindex = 24;
                await Task.Delay(6000);
                taikenDrawing = false;
            }
            else
            {
                TaikenEnd(true,Location);
            }
        }
        bool isCleared = false;
        public async void TaikenEnd(bool isClear,string Location)
        {
            if (!isCleared)
            {
                isCleared = true;
                taikenDrawing = true;
                if (isClear)
                {
                    taikenindex = 25;
                }
                else
                {
                    taikenindex = 26;
                }
                await Task.Delay(3000);
                taikenindex = 27;
                await Task.Delay(3000);
                taikenindex = 28;
                await Task.Delay(4000);
                taikenindex = 29;
                await Task.Delay(5000);
                taikenindex = 30;
                await Task.Delay(3000);
                string a = Path.Combine(Location, "../../../../../../../../HongoMCCGame2024.exe");
                if (File.Exists(Path.Combine(Location,"../demo.txt")))
                {
                    try
                    {
                        Process.Start(Path.Combine(Location, "../../../../../../../../HongoMCCGame2024.exe"));
                    }
                    finally
                    {
                        Application.Exit();
                    }
                }
                Application.Exit();
            }

        }
        public void TaikenUpdate(TimeSpan taikenElapsed)
        {
            if (taikenElapsed.TotalSeconds > 2 && taikenElapsed.TotalSeconds < 3)
            {
                TaikenWait(3000, 0);//「メトロドライブ日比谷編へようこそ」
            }
            if (taikenElapsed.TotalSeconds > 5 && taikenElapsed.TotalSeconds < 6)
            {
                TaikenWait(3000, 1);//「はじめに~~」
            }
            if (taikenElapsed.TotalSeconds > 8 && taikenElapsed.TotalSeconds < 9)
            {
                TaikenWait(2000, 2);//「これがマスコンです」
            }
            if (taikenElapsed.TotalSeconds > 10 && taikenElapsed.TotalSeconds < 11)
            {
                TaikenWait(3000, 3);//
            }
            if (taikenElapsed.TotalSeconds > 13 && taikenElapsed.TotalSeconds < 15)
            {
                TaikenWait(2000, 4);
            }
            if (taikenElapsed.TotalSeconds > 15 && taikenElapsed.TotalSeconds < 16)
            {
                TaikenWait(3000, 5);
            }
            if (taikenElapsed.TotalSeconds > 18 && taikenElapsed.TotalSeconds < 19)
            {
                TaikenWait(2000, 6);
            }
            if (taikenElapsed.TotalSeconds > 20 && taikenElapsed.TotalSeconds < 21)
            {
                TaikenWait(2000, 7);
            }
            if (taikenElapsed.TotalSeconds > 22 && taikenElapsed.TotalSeconds < 23)
            {
                TaikenWait(2000, 8);
            }
            if (taikenElapsed.TotalSeconds > 24 && taikenElapsed.TotalSeconds < 25)
            {
                TaikenWait(2000, 9);
            }
            if (taikenElapsed.TotalSeconds > 26 && taikenElapsed.TotalSeconds < 27)
            {
                TaikenWait(3000, 10);
            }
            if (taikenElapsed.TotalSeconds > 29 && taikenElapsed.TotalSeconds < 30)
            {
                TaikenWait(2000, 11);
            }
            if (taikenElapsed.TotalSeconds > 31 && taikenElapsed.TotalSeconds < 32)
            {
                TaikenWait(4000, 12);
            }
            if (taikenElapsed.TotalSeconds > 33 && taikenElapsed.TotalSeconds < 34)
            {
                TaikenWait(2000, 13);
            }
        }
    }
}

