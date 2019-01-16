using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RftpUI
{
    static class EventsManager
    {
        public static event EventHandler<MainPageArgs> ChangeMainPageEvent;

        public static void InvokeChangeMainPageEvent(object sender, string pageLocation, object extra)
        {
            MainPageArgs args = new MainPageArgs(pageLocation,extra);
            ChangeMainPageEvent?.Invoke(sender, args);
        }

        public static void AddChangeMainPageEventListener(EventHandler<MainPageArgs> eventListener)
        {
            ChangeMainPageEvent += eventListener;
        }
    }

    public class MainPageArgs
    {
        public string PageLocation;
        public object extra;

        public MainPageArgs(string pageLocation, object extra)
        {
            this.PageLocation = pageLocation;
            this.extra = extra;
        }
    }
}
