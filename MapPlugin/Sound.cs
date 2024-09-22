using System.IO;
using AtsEx.Extensions.SoundFactory;
using BveTypes.ClassWrappers;

namespace MetroDrive.MapPlugin
{
    internal class SoundControll
    {
        Sound sound;
        public void OnStart(ISoundFactory soundFactory,string Location)
        {
            string path = Path.Combine(Path.GetDirectoryName(Location), @"sound\pon.wav");
            sound = soundFactory.LoadFrom(path, 1, Sound.SoundPosition.Cab, 1);
        }
        public void PlaySound(ISoundFactory soundFactory ,string Location,string filename)
        {
            string path = Path.Combine(Path.GetDirectoryName(Location),@"sound\"+ filename);
            sound = soundFactory.LoadFrom(path, 1, Sound.SoundPosition.Cab,1);
            sound.Play(1.5, 1, 0);
        }
        public void StationKanko(ISoundFactory soundFactory,string Location,string stationName)
        {
            string fileName = "B10.wav"; 
            switch(stationName)
            {
                case "北千住":
                    fileName = "B10.wav";
                    break;
                case "南千住":
                    fileName = "B11.wav";
                        break;
                case "三ノ輪":
                    fileName = "B12.wav";
                    break;
                case "入谷":
                    fileName = "B13.wav";
                    break;
                case "上野":
                    fileName = "B14.wav";
                    break;
                case "仲御徒町":
                    fileName = "B15.wav";
                    break;
                case "秋葉原":
                    fileName = "B16.wav";
                    break;
                case "小伝馬町":
                    fileName = "B17.wav";
                    break;
                case "人形町":
                    fileName = "B18.wav";
                    break;
                case "茅場町":
                    fileName = "B19.wav";
                    break;
            }
            string path = Path.Combine(Path.GetDirectoryName(Location), @"sound\" + fileName);
            sound = soundFactory.LoadFrom(path, 1, Sound.SoundPosition.Cab, 1);
            sound.Play(1.5, 1, 0);
        }
        public void OnDispose()
        {
            sound.Dispose();
        }
    }
}

