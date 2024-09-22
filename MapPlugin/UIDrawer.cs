using System;
using System.Drawing;
using System.IO;
using AtsEx.PluginHost.Plugins;
using BveTypes.ClassWrappers;
using SlimDX;
using SlimDX.Direct3D9;

namespace MetroDrive
{
    internal class UIDrawer
    {
        Model[] powerModel = new Model[5];
        Model[] brakeModel = new Model[9];
        Model teituu;
        Model lifePicture;//「Life.」
        Model[] lifeModel = new Model[10];
        Model autoB;
        Model autoP;
        int[] lifeMes = new int[3];
        public void CreateModel(string Location)
        {
            for(int i = 0; i < 4; i++)
            {
                powerModel[i] = CreateModel(@"picture\P" + i + ".png",0,0,150,-225);
            }
            for (int i = 0; i < 9; i++)
            {
                brakeModel[i] = CreateModel(@"picture\B" + i + ".png", 0, 0, -150, 225);
            }
            teituu = CreateModel(@"picture\UI\teituu.png", 0, 0, 400, 150);
            lifePicture = CreateModel(@"picture\life\life.png", 0, 0, 150, 80);
            for (int i = 0; i < 9; i++)
            {
                lifeModel[i] = CreateModel(@"picture\life\" + i + ".png", -150, 0, 40, -80);
            }
            autoB = CreateModel(@"picture\autoB.png", -50, 0, 50, -225);
            autoP = CreateModel(@"picture\autop.png", 0, 0, 50, -225);
            Model CreateModel(string path, float x, float y, float sizex, float sizey)
            {
                string texFilePath = Path.Combine(Path.GetDirectoryName(Location), path);
                RectangleF rectangleF = new RectangleF(x, y, sizex, -sizey);
                Model brakeNotch = Model.CreateRectangleWithTexture(rectangleF, 0, 0, texFilePath);//四角形の3Dモデル
                return brakeNotch;
            }
        }
        public void UIDraw(int power,int brake,int EB,bool isTeituu,int life,bool isUIOff,bool isAuto)
        {
            if(isUIOff == false)
            {
                int width = Direct3DProvider.Instance.PresentParameters.BackBufferWidth;
                int height = Direct3DProvider.Instance.PresentParameters.BackBufferHeight;
                Device device = Direct3DProvider.Instance.Device;
                device.SetTransform(TransformState.World, Matrix.Translation(-width / 2, height / 2, 0));
                powerModel[power].Draw(Direct3DProvider.Instance, false);
                if (isAuto) { autoP.Draw(Direct3DProvider.Instance, false); }
                device.SetTransform(TransformState.World, Matrix.Translation(width / 2, height / 2, 0));//(float)Math.Sin(Native.VehicleState.Time.TotalSeconds)*100
                brakeModel[brake].Draw(Direct3DProvider.Instance, false);
                if (isAuto) { autoB.Draw(Direct3DProvider.Instance, false); }
                device.SetTransform(TransformState.World, Matrix.Translation(0 / 2 - 200, 0 / 2 + 150, 0));
                if (isTeituu == true) { teituu.Draw(Direct3DProvider.Instance, false); }
            }
        }
        public void LifeDraw(int life,bool isUIOff)
        {
            if(isUIOff == false)
            {
                Device device = Direct3DProvider.Instance.Device;
                int width = Direct3DProvider.Instance.PresentParameters.BackBufferWidth;
                int height = Direct3DProvider.Instance.PresentParameters.BackBufferHeight;
                if (life >= 100)
                {
                    device.SetTransform(TransformState.World, Matrix.Translation(width / 2-150, -height / 2+250, 0));
                    lifeModel[lifeMes[0]].Draw(Direct3DProvider.Instance, false);//1の位
                    device.SetTransform(TransformState.World, Matrix.Translation(width / 2-110, -height / 2+250, 0));
                    lifeModel[lifeMes[1]].Draw(Direct3DProvider.Instance, false);//10
                    device.SetTransform(TransformState.World, Matrix.Translation(width / 2-70, -height / 2+250, 0));
                    lifeModel[lifeMes[2]].Draw(Direct3DProvider.Instance, false);//100
                }
                if (life > 10 && life < 100)
                {
                    device.SetTransform(TransformState.World, Matrix.Translation(width / 2 - 110, -height / 2+250, 0));
                    lifeModel[lifeMes[0]].Draw(Direct3DProvider.Instance, false);
                    device.SetTransform(TransformState.World, Matrix.Translation(width / 2 -70, -height / 2 + 250, 0));
                    lifeModel[lifeMes[1]].Draw(Direct3DProvider.Instance, false);
                }
                if(life == 10)
                {
                    device.SetTransform(TransformState.World, Matrix.Translation(width / 2 - 110, -height / 2 + 250, 0));
                    lifeModel[1].Draw(Direct3DProvider.Instance, false);
                    device.SetTransform(TransformState.World, Matrix.Translation(width / 2 - 70, -height / 2 + 250, 0));
                    lifeModel[0].Draw(Direct3DProvider.Instance, false);
                }
                if (life < 10)
                {
                    device.SetTransform(TransformState.World, Matrix.Translation(width / 2-110, -height / 2+250, 0));
                    lifeModel[0].Draw(Direct3DProvider.Instance, false);
                    device.SetTransform(TransformState.World, Matrix.Translation(width / 2 - 70, -height / 2+250, 0));
                    lifeModel[lifeMes[0]].Draw(Direct3DProvider.Instance, false);
                }
                if(life<=0)
                {
                    device.SetTransform(TransformState.World, Matrix.Translation(width / 2 - 110, -height / 2 + 250, 0));
                    lifeModel[0].Draw(Direct3DProvider.Instance, false);
                    device.SetTransform(TransformState.World, Matrix.Translation(width / 2 - 70, -height / 2 + 250, 0));
                    lifeModel[0].Draw(Direct3DProvider.Instance, false);
                }
                device.SetTransform(TransformState.World, Matrix.Translation(width / 2 -450, -height / 2 + 250, 0));
                lifePicture.Draw(Direct3DProvider.Instance, false);
            }
        }
        public TickResult tick(int life)
        {
            if (life > 0)
            { lifeMes[0] = int.Parse(life.ToString().Substring(0, 1)); }//1の位(3桁)
            if (life > 10)
            { lifeMes[1] = int.Parse(life.ToString().Substring(1, 1)); }//10の位(ry)
            if (life > 100)
            { lifeMes[2] = int.Parse(life.ToString().Substring(1, 1)); }//100の(ry)
            return new MapPluginTickResult();
        }
    }
}