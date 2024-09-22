using System.Drawing;
using System.IO;
using BveTypes.ClassWrappers;
using SlimDX.Direct3D9;
using SlimDX;

namespace MetroDrive
{
    internal class Pause
    {
        Model pauseMenu;
        Model pointer;
        Model back;//背景の若干暗い所
        //Model fixOverRun;
        public bool isPause;
        public void OnInputP()
        {
            isPause = !isPause;
        }
        public void CreateModel(string location)
        {
            Device device = Direct3DProvider.Instance.Device;
            int width = Direct3DProvider.Instance.PresentParameters.BackBufferWidth;
            int height = Direct3DProvider.Instance.PresentParameters.BackBufferHeight;
            pauseMenu = CreateAnyModel(@"picture\UI\PauseMenu.png",-width/6, height/20, width/3, height/10);
            pointer = CreateAnyModel(@"picture\UI\PausePointer.png", 0, 0, 30, 30);
            back = CreateAnyModel(@"picture\UI\PauseBack.png", -width/2, height/2, width, height);
            //fixOverRun = CreateAnyModel();
            Model CreateAnyModel(string path, float x, float y, float sizex, float sizey)
            {
                string texFilePath = Path.Combine(Path.GetDirectoryName(location), path);
                RectangleF rectangleF = new RectangleF(x, y, sizex, -sizey);
                Model brakeNotch = Model.CreateRectangleWithTexture(rectangleF, 0, 0, texFilePath);//四角形の3Dモデル
                return brakeNotch;
            }
        }
        public void PauseMenuDrawer(int pointerIndex,bool isEnterPressed,bool isEndScenario,bool isVoiceOn,bool isUIOff)
        {
            if(isPause == true)
            {
                Device device = Direct3DProvider.Instance.Device;
                int width = Direct3DProvider.Instance.PresentParameters.BackBufferWidth;
                int height = Direct3DProvider.Instance.PresentParameters.BackBufferHeight;
                device.SetTransform(TransformState.World, Matrix.Translation(0, 0, 0));
                back.Draw(Direct3DProvider.Instance, false);
                if (pointerIndex ==1)//補助表示（UI）オフ
                {
                    device.SetTransform(TransformState.World, Matrix.Translation(width / 2-10, height / 2+50, 0));
                    pointer.Draw(Direct3DProvider.Instance, false);
                    if(isEnterPressed == true){ if (isUIOff) { isUIOff = false; } else { isUIOff = true; } isEnterPressed = false; }
                }
                if(pointerIndex ==2)//運転終了
                {
                    device.SetTransform(TransformState.World, Matrix.Translation(width / 2 - 10, height / 2 , 0));
                    pointer.Draw(Direct3DProvider.Instance, false);
                    if (isEnterPressed == true) { isEndScenario = true; isEnterPressed = false; }
                }
                if(pointerIndex == 3)//運転士歓呼on/off
                {
                    device.SetTransform(TransformState.World, Matrix.Translation(width / 2 - 10, height / 2, 0));
                    pointer.Draw(Direct3DProvider.Instance, false);
                    if (isEnterPressed == true) { if (isVoiceOn == true) { isVoiceOn = false; }if (isVoiceOn == false){ isVoiceOn = true; } isEnterPressed = false; }
                }
                device.SetTransform(TransformState.World, Matrix.Translation(0, 0, 0));
                pauseMenu.Draw(Direct3DProvider.Instance, false);
            }
        }
    }
}
