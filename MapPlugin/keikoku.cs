using System;
using BveTypes.ClassWrappers;
using System.IO;
using System.Drawing;
using SlimDX;
using SlimDX.Direct3D9;
using System.Threading.Tasks;
using System.Threading;
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
        Model[] stationModel = new Model[10]; 
        Model pass;
        Model stop;
        Model gameover;
        Model good;
        Model great;
        Model bokutei;
        Model[] taikenDialog = new Model[31];
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
            for(int i= 0;i<10; i++)
            {
                stationModel[i] = CreateModels(@"picture\station\"+i+".png", -230, -60, 300, 120);
            }
            gameover = CreateModels(@"picture\station\over.png", -275, -75, 550, 150);
            //Goodとか
            good = CreateModels(@"picture\UI\good.png", -275, -150, 550, 300);
            great = CreateModels(@"picture\UI\great.png", -275, -150, 550, 300);
            //体験版
            int[] tempX = new int[31] { -250,-250,0,0,-380,-380,-450,-450,-450,-450,-250,-250,-250,-250,-400,-400,-250,-300,-300,-250,-350,-250,-300,-250,-250,-250,-250,-250,-250,-250,-250};
            int[] tempY = new int[31] {0,0,0,0,0,0,-150,-150,-150,-140,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
            int[] tempSizex = new int[31] {500,500,380,380,380,380,450,450,450,450,500,500,500,500,800,800,500,600,600,500,700,500,600,500,500,500,500,500,500,500,500};
            int[] tempSizey = new int[31] {170,170,130,130,130,130,150,150,150,140,450,170,220,170,310,310,170,350,280,170,240,85,280,85,170,130,150,170,170,170,85};
            for(int i = 0;i < 31; i++)
            {
                taikenDialog[i] = CreateModels(@"picture\dialog\"+i+".png", tempX[i], tempY[i], tempSizex[i], tempSizey[i]);
            }
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
                    case "北千住":
                        stationModel[0].Draw(Direct3DProvider.Instance, false);
                        break;
                    case "南千住":
                        stationModel[1].Draw(Direct3DProvider.Instance, false);
                        break;
                    case "三ノ輪":
                        stationModel[2].Draw(Direct3DProvider.Instance, false);
                        break;
                    case "入谷":
                        stationModel[3].Draw(Direct3DProvider.Instance, false);
                        break;
                    case "上野":
                        stationModel[4].Draw(Direct3DProvider.Instance, false);
                        break;
                    case "仲御徒町":
                        stationModel[5].Draw(Direct3DProvider.Instance, false);
                    break;
                    case "秋葉原":
                        stationModel[6].Draw(Direct3DProvider.Instance, false);
                        break;
                    case "小伝馬町":
                        stationModel[7].Draw(Direct3DProvider.Instance, false);
                        break;
                    case "人形町":
                        stationModel[8].Draw(Direct3DProvider.Instance, false);
                        break;
                    case "茅場町":
                        stationModel[9].Draw(Direct3DProvider.Instance, false);
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
                if(taikenindex >9||taikenindex == 0||taikenindex == 1)
                {
                    taikenDialog[taikenindex].Draw(Direct3DProvider.Instance, false);
                }
                device.SetTransform(TransformState.World, Matrix.Translation(-width/2+150, height / 2 - 200, 0));
                if (taikenindex == 2) { taikenDialog[2].Draw(Direct3DProvider.Instance, false); }
                if (taikenindex == 3) { taikenDialog[3].Draw(Direct3DProvider.Instance, false); }
                device.SetTransform(TransformState.World, Matrix.Translation(width / 2 - 150, height / 2 - 200, 0));
                if (taikenindex == 4) { taikenDialog[4].Draw(Direct3DProvider.Instance, false); }
                if (taikenindex == 5) { taikenDialog[5].Draw(Direct3DProvider.Instance, false); }
                //device.SetTransform(TransformState.World, Matrix.Translation(width / 2 - 400, -height / 2 +100, 0));
                device.SetTransform(TransformState.World, Matrix.Translation(0, 0, 0));
                if (taikenindex == 6) { taikenDialog[6].Draw(Direct3DProvider.Instance, false); }
                if (taikenindex == 7) { taikenDialog[7].Draw(Direct3DProvider.Instance, false); }
                if (taikenindex == 8) { taikenDialog[8].Draw(Direct3DProvider.Instance, false); }
                device.SetTransform(TransformState.World, Matrix.Translation(-width / 2 + 400, -height / 2 + 100, 0));
                if (taikenindex == 9) { taikenDialog[9].Draw(Direct3DProvider.Instance, false); }
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

