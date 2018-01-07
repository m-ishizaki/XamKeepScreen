using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace RKSoftware.XamKeepScreen
{
    public static class KeepScreen
    {
        public static void Off() => Set(false);
        public static void On() => Set(true);

        public static void Set(bool isOn)
        {
            switch (Device.RuntimePlatform)
            {
                case Device.Android: SetForAndroid(isOn); break;
                case Device.iOS: SetForIOS(isOn); break;
                default: throw new NotSupportedException();
            }
        }

        private static bool SetForAndroid(bool isOn)
        {
            var formsType = Type.GetType("Xamarin.Forms.Forms,Xamarin.Forms.Platform.Android");

            var activityType = Type.GetType("Android.App.Activity,Mono.Android");
            var windowType = Type.GetType("Android.Views.Window,Mono.Android");
            var windowManagerFlagsType = Type.GetType("Android.Views.WindowManagerFlags,Mono.Android");

            var method = windowType.GetMethod(isOn ? "AddFlags" : "ClearFlags");
            var activityInstance = formsType.GetProperty("Context").GetValue(null);
            var windowInstance = activityType.GetProperty("Window").GetValue(activityInstance);
            var keepScreenOnValue = Enum.Parse(windowManagerFlagsType, "KeepScreenOn");

            Device.BeginInvokeOnMainThread(() =>
                method.Invoke(windowInstance, new[] { keepScreenOnValue })
            );

            return true;
        }

        private static bool SetForIOS(bool isOn)
        {
            var uiapplicationType = Type.GetType("UIKit.UIApplication,Xamarin.iOS");

            var property = uiapplicationType.GetProperty("IdleTimerDisabled");
            var uiapplicationInstance = uiapplicationType.GetProperty("SharedApplication").GetValue(null);

            Device.BeginInvokeOnMainThread(() =>
                property.SetValue(uiapplicationInstance, isOn)
            );

            return true;
        }
    }
}
