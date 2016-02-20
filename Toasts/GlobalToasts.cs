using NotificationsExtensions.Toasts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tindows.Externals.Tinder_Objects;
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

    static class NewMessageToast
    {
        public static void Do(Match fromWho, Message message_obj)
        {
            ToastVisual visual = new ToastVisual()
            {
                TitleText = new ToastText()
                {
                    Text = fromWho.person.name + " has sent you a message"
                },
            };

            string textForToastTruncated = "";

            if (message_obj.type == "gif")
            {
                textForToastTruncated = "Sent you a Giphy";
            }
            else
            {
                var t = message_obj.message;
                textForToastTruncated = t.Substring(0, Math.Min(25, t.Length));
            }

            visual.BodyTextLine1 = new ToastText()
            {
                Text = textForToastTruncated + "..."
            };

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
