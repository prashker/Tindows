using Newtonsoft.Json.Linq;
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

            string textForToast = "";

            if (message_obj.type == "gif")
            {
                textForToast = "Sent you a Giphy";
            }
            else
            {
                var t = message_obj.message;
                textForToast = t;
            }

            visual.BodyTextLine1 = new ToastText()
            {
                Text = textForToast
            };

            // Pass a payload as JSON to the Toast
            dynamic payload = new JObject();
            payload.source = typeof(NewMessageToast).ToString();
            payload.args = fromWho._id;

            string payload_json = payload.ToString();
        
            ToastContent toastContent = new ToastContent()
            {
                Visual = visual,
                Audio = null,
                Actions = new ToastActionsCustom()
                {
                    Buttons =
                    {
                        new ToastButton("Go to Message", payload_json)
                        {
                            ActivationType = ToastActivationType.Foreground,
                        },
                        new ToastButtonDismiss()
                    }
                },
                Launch = payload_json
            };

            var toast = new ToastNotification(toastContent.GetXml());

            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

    }

    static class NewMatchToast
    {
        public static void Do(Match fromWho)
        {
            ToastVisual visual = new ToastVisual()
            {
                TitleText = new ToastText()
                {
                    Text = "Matched!"
                },
            };

            visual.BodyTextLine1 = new ToastText()
            {
                Text = "You have matched " + fromWho.person.name
            };

            visual.BodyTextLine2 = new ToastText()
            {
                Text = "Chat em up!"
            };

            // Pass a payload as JSON to the Toast
            dynamic payload = new JObject();
            payload.source = typeof(NewMatchToast).ToString();
            payload.args = fromWho._id;

            string payload_json = payload.ToString();

            ToastContent toastContent = new ToastContent()
            {
                Visual = visual,
                Audio = null,
                Actions = new ToastActionsCustom()
                {
                    Buttons =
                    {
                        new ToastButton("Go to Match", payload_json)
                        {
                            ActivationType = ToastActivationType.Foreground,
                        },
                        new ToastButtonDismiss()
                    }
                },
                Launch = payload_json
            };

            var toast = new ToastNotification(toastContent.GetXml());

            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

    }
}
