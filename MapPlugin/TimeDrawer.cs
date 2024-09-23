using System;
using System.Drawing;
using System.IO;
using AtsEx.PluginHost.Plugins;
using BveTypes.ClassWrappers;
using SlimDX;
using SlimDX.Direct3D9;

namespace MetroDrive
{
    internal class TimeDrawer : IDisposable//このクラスでは、
    {
        //public string Location;
        public float width;
        public float height;
        int[] arriveMes = new int[10];//一桁目(前から数えるので)
        int[] nowMes = new int[10];
        int[] nextMes = new int[10];
        int next;
        Model[] arriveModel = new Model[10];
        Model[] nowModel = new Model[10];
        Model[] remainingModel = new Model[10];//次駅距離
        Model[] nextModel = new Model[10];
        public Model arrivePicture;//"Arv."
        public Model nowPicture;//"Now."
        public Model arriveColon;
        public Model nowColon;
        public Model ato;//「あと」
        public Model meter;//「m」
        Model over;
        Model teisiiti;
        public void CreateModel(string Location)//Mainの中で呼び出すやつ
        {
            //時間系(arrive)
            for(int i = 0; i < 10; i++)
            {
                arriveModel[i] = CreateTimeModel("arrive", i, 0, 0);//SetTransformで横の位置は変更するゾ
            }
            //(now)
            for(int i = 0; i<10; i++)
            {
                nowModel[i] = CreateTimeModel("now", i, 0, 0);
            }
            //残り距離
            /*for (int i = 0; i < 10; i++)
            {
                remainingModel[i] = CreateAnyModel(@"picture\remain\" + i + ".png", 0, 0, 40, 80);
            }*/
            for (int i = 0;i < 10; i++)
            {
                nextModel[i] = CreateAnyModel(@"picture\remain\" + i + ".png", 0, 0, 40, 80);
            }
            //固定UI
            arrivePicture = CreateArvModel();
            nowPicture = CreateNowModel();
            arriveColon = CreateAnyModel(@"picture\arrive\colon.png", 0, 0, 30, 60);
            nowColon = CreateAnyModel(@"picture\now\colon.png", 0, 0, 30, 60);
            ato = CreateAnyModel(@"picture\remain\ato.png", 0, 0, 150, 80);
            over = CreateAnyModel(@"picture\remain\over.png", 0, 0, 200, 80);
            teisiiti = CreateAnyModel(@"picture\remain\teisiiti.png", 0, 0, 250, 80);
            meter = CreateAnyModel(@"picture\remain\m.png", 0, 0, 50, 60);
            
            Model CreateArvModel()
            {
                string texFilePath = Path.Combine(Path.GetDirectoryName(Location), @"picture\arrive\arv.png");
                RectangleF rectangleF = new RectangleF(0 / 4, 0 / 2, 150, -80);
                Model brakeNotch = Model.CreateRectangleWithTexture(rectangleF, 0, 0, texFilePath);//四角形の3Dモデル
                return brakeNotch;
            }
            Model CreateTimeModel(string type, int number, float x, float y)//第一引数にはarrive/nowで入れる
            {
                string texFilePath = Path.Combine(Path.GetDirectoryName(Location), @"picture\" + type + @"\" + number + ".png");
                RectangleF rectangleF = new RectangleF(0 - x / 4, 0 - y / 2, 40, -60);
                Model timeModel = Model.CreateRectangleWithTexture(rectangleF, 0, 0, texFilePath);//四角形の3Dモデル
                return timeModel;
            }
            Model CreateNowModel()
            {
                string texFilePath = Path.Combine(Path.GetDirectoryName(Location), @"picture\now\now.png");
                RectangleF rectangleF = new RectangleF(0 / 4, 0 / 2, 150, -80);
                Model brakeNotch = Model.CreateRectangleWithTexture(rectangleF, 0, 0, texFilePath);//四角形の3Dモデル
                return brakeNotch;
            }
            Model CreateAnyModel(string path, float x, float y, float sizex, float sizey)
            {
                string texFilePath = Path.Combine(Path.GetDirectoryName(Location), path);
                RectangleF rectangleF = new RectangleF(x, y, sizex, -sizey);
                Model brakeNotch = Model.CreateRectangleWithTexture(rectangleF, 0, 0, texFilePath);//四角形の3Dモデル
                return brakeNotch;
            }
        }

        public void Dispose()
        {
            //ここはいつか配列になおしてdisposeする  検索 => Todo
            
        }

        public void Patch(double NeXTLocation,double nowLocation,bool isUIOff,double Goukakuhani)//まずこれを呼ぶ
        {
            if(isUIOff == false)
            {
                width = Direct3DProvider.Instance.PresentParameters.BackBufferWidth;
                height = Direct3DProvider.Instance.PresentParameters.BackBufferHeight;
                //3D modelをどこに配置するのか指定するかんじ device.settransform(文字略) 頂点位置の変換
                Device device = Direct3DProvider.Instance.Device;
                device.SetTransform(TransformState.View, Matrix.Identity);
                device.SetTransform(TransformState.Projection, Matrix.OrthoOffCenterLH(-width / 2, width / 2, -height / 2, height / 2, 0, 1));
                device.SetTransform(TransformState.World, Matrix.Translation(width / 2 - 450, -height / 2+90, 0));
                arrivePicture.Draw(Direct3DProvider.Instance, false);
                device.SetTransform(TransformState.World, Matrix.Translation(width / 2 - 450, -height / 2+170, 0));
                nowPicture.Draw(Direct3DProvider.Instance, false);
                device.SetTransform(TransformState.World, Matrix.Translation(width / 2 - 300, -height / 2 + 80 , 0));//arrive１個目
                arriveModel[0].Draw(Direct3DProvider.Instance, false);
                device.SetTransform(TransformState.World, Matrix.Translation(width / 2 - 260, -height / 2 + 80, 0));//arrive２個め
                arriveModel[1].Draw(Direct3DProvider.Instance, false);
                device.SetTransform(TransformState.World, Matrix.Translation(width / 2 - 230, -height / 2 + 80, 0));
                arriveColon.Draw(Direct3DProvider.Instance, false);
                device.SetTransform(TransformState.World, Matrix.Translation(width / 2 - 200, -height / 2 + 80, 0));//arrive３個目
                arriveModel[2].Draw(Direct3DProvider.Instance, false);
                device.SetTransform(TransformState.World, Matrix.Translation(width / 2 - 160, -height / 2 + 80, 0));//now
                arriveModel[3].Draw(Direct3DProvider.Instance, false);
                device.SetTransform(TransformState.World, Matrix.Translation(width / 2 - 130, -height /2+80, 0));//now
                arriveColon.Draw(Direct3DProvider.Instance, false);
                device.SetTransform(TransformState.World, Matrix.Translation(width / 2 - 100, -height / 2 + 80, 0));//now
                arriveModel[4].Draw(Direct3DProvider.Instance, false);
                device.SetTransform(TransformState.World, Matrix.Translation(width / 2 - 60, -height / 2 + 80, 0));//now
                arriveModel[5].Draw(Direct3DProvider.Instance, false);
                device.SetTransform(TransformState.World, Matrix.Translation(width / 2 - 300, -height / 2+160, 0));//now
                nowModel[0].Draw(Direct3DProvider.Instance, false);
                device.SetTransform(TransformState.World, Matrix.Translation(width / 2 - 260, -height / 2+160, 0));//now
                nowModel[1].Draw(Direct3DProvider.Instance, false);
                device.SetTransform(TransformState.World, Matrix.Translation(width / 2 - 230, -height / 2+160, 0));
                nowColon.Draw(Direct3DProvider.Instance, false);
                device.SetTransform(TransformState.World, Matrix.Translation(width / 2 - 200, -height / 2+160, 0));//now
                nowModel[2].Draw(Direct3DProvider.Instance, false);
                device.SetTransform(TransformState.World, Matrix.Translation(width / 2 - 160, -height / 2+160, 0));//now
                nowModel[3].Draw(Direct3DProvider.Instance, false);
                device.SetTransform(TransformState.World, Matrix.Translation(width / 2 - 130, -height / 2+160, 0));
                nowColon.Draw(Direct3DProvider.Instance, false);
                device.SetTransform(TransformState.World, Matrix.Translation(width / 2 - 100, -height / 2+160, 0));//now
                nowModel[4].Draw(Direct3DProvider.Instance, false);
                device.SetTransform(TransformState.World, Matrix.Translation(width / 2 - 60, -height / 2+160, 0));//now
                nowModel[5].Draw(Direct3DProvider.Instance, false);
                device.SetTransform(TransformState.World, Matrix.Translation(-width / 2 + 10, -height / 2 + 180, 0));
                if(Math.Abs(nowLocation - NeXTLocation)<Goukakuhani)
                {
                    teisiiti.Draw(Direct3DProvider.Instance, false);
                }
                else
                {
                    if (NeXTLocation < nowLocation)
                    {
                        over.Draw(Direct3DProvider.Instance, false);
                    }
                    else
                    {
                        ato.Draw(Direct3DProvider.Instance, false);
                    }
                }
                //残距離
                device.SetTransform(TransformState.World, Matrix.Translation(-width / 2 + 200, -height / 2 +60, 0));
                meter.Draw(Direct3DProvider.Instance, false);
                if (Math.Abs(NeXTLocation - nowLocation) >= 1000)
                {
                    for(int i = 0;i<4; i++)
                    {
                        device.SetTransform(TransformState.World, Matrix.Translation(-width / 2 + 50*i, -height / 2 + 100, 0));
                        nextModel[nextMes[i]].Draw(Direct3DProvider.Instance, false);
                    }
                }
                if (Math.Abs(NeXTLocation - nowLocation) < 1000 && Math.Abs(NeXTLocation - nowLocation) >= 100)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        device.SetTransform(TransformState.World, Matrix.Translation(-width / 2 + 50 * (i+1), -height / 2 + 100, 0));
                        nextModel[nextMes[i]].Draw(Direct3DProvider.Instance, false);
                    }
                }
                if (Math.Abs(NeXTLocation - nowLocation) < 100 && Math.Abs(NeXTLocation - nowLocation) >= 10)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        device.SetTransform(TransformState.World, Matrix.Translation(-width / 2 + 50 * (i + 2), -height / 2 + 100, 0));
                        nextModel[nextMes[i]].Draw(Direct3DProvider.Instance, false);
                    }
                }
                if (Math.Abs(NeXTLocation - nowLocation) < 10)
                {
                    device.SetTransform(TransformState.World, Matrix.Translation(-width / 2 + 150, -height / 2 + 100, 0));
                    nextModel[nextMes[0]].Draw(Direct3DProvider.Instance, false);
                }
                //device.SetTransform(TransformState.World, Matrix.Translation(-width / 2 + 150, -height / 2 + 100, 0));

            }
        }
        //500行超えるのでLife２関しての記述はUIDrawerにて
        //Native.VehicleState.Timeから
        public TickResult Tick(int index,string now,double NeXTLocation,double nowLocation,string arrive)
        {
            for(int  i = 0; i < 6;i++)
            {
                arriveMes[i] = int.Parse(arrive.Substring(i, 1));
                nowMes[i] = int.Parse(now.Substring(i, 1));
            }
            next = (int)Math.Abs(NeXTLocation - nowLocation);
            nextMes[0] = int.Parse(next.ToString().Substring(0, 1));
            if (NeXTLocation - nowLocation >= 10) { nextMes[1] = int.Parse(next.ToString().Substring(1, 1)); }
            if (NeXTLocation - nowLocation >= 100) { nextMes[2] = int.Parse(next.ToString().Substring(2, 1)); }
            if (NeXTLocation - nowLocation >= 1000) { nextMes[3] = int.Parse(next.ToString().Substring(3, 1)); }
            return new MapPluginTickResult();
        }
    }
}
