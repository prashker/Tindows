using NotificationsExtensions.Toasts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace Tindows.Toasts
{
    static class PassToast
    {
        public static void Do(string title, string b1, string b2)
        {
            ToastVisual visual = new ToastVisual()
            {
                TitleText = new ToastText()
                {
                    Text = title
                },
            };

            if (b1 != "")
            {
                visual.BodyTextLine1 = new ToastText()
                {
                    Text = b1
                };
            }

            if (b2 != "")
            {
                visual.BodyTextLine2 = new ToastText()
                {
                    Text = b2
                };
            }

            ToastContent toastContent = new ToastContent()
            {
                Visual = visual,
                Audio = null
                
            };

            var toast = new ToastNotification(toastContent.GetXml());

            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

    }
}
