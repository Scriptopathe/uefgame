using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
namespace Geex.Edit.XnaControl
{
    public class XnaFrameworkDispatcherService
    {
        static private Timer timer;
        public static void Init()
        {
            timer = new Timer(new TimerCallback(OnTick), "", Timeout.Infinite, Timeout.Infinite);
            FrameworkDispatcher.Update();
        }
        static void OnTick(object o)
        {
            FrameworkDispatcher.Update();
        }
        static public void StartService()
        {
            timer.Change(0, 80);
        }
        static public void StopService()
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
        }
    }
}
