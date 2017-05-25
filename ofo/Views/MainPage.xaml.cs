using System;
using System.IO;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.Foundation.Metadata;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace ofo.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {

        StatusBar statusbar;

        public MainPage()
        {
            this.InitializeComponent();

            WebSite.PermissionRequested += WebSite_PermissionRequested;
           
        }
        int i = 0;

        private void OnNavCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)

        {
            loading.Visibility = Visibility.Collapsed;

            LoadingText.Visibility = Visibility.Collapsed;

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;

            SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;

        }

        private void App_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (WebSite.CanGoBack)
            {
                WebSite.GoBack();
            }

            Frame rootFrame = Window.Current.Content as Frame;

            if (i == 1)
            {
                rootFrame.GoBack();
                i = 0;
            }
        }

        int n = 0;
        private void OnContentLoading(WebView sender, WebViewContentLoadingEventArgs args)
        {
        
            loading.Visibility = Visibility.Visible;
            LoadingText.Text = "拼命加载中...";
            LoadingPic.Visibility = Visibility.Collapsed;
            if (n == 1&&("Windows.Mobile" == Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily))
            {
                Back.Visibility = Visibility.Visible;
            }
            n = 1;

        }


        //位置请求弹框
        private void WebSite_PermissionRequested(WebView sender, WebViewPermissionRequestedEventArgs args)
        {
            if (args.PermissionRequest.PermissionType == WebViewPermissionType.Geolocation)
            {
                args.PermissionRequest.Allow();
            }
         
        }

        // private string PayUrl = "http://www.ip138.com/useragent/";

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
              WebSite.Navigate(new Uri("https://common.ofo.so/newdist/?Login"));

               // 判断是否存在 StatusBar
               if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
               {
                   //静态方法取得当前 StatusBar 实例
                   statusbar = StatusBar.GetForCurrentView();

                   //显示状态栏
                   await statusbar.HideAsync();
               }
               
        }
        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            //把Split的打开状态调整为相反
            MenuView.IsPaneOpen = !MenuView.IsPaneOpen;
        }

        private void Setting_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //把Split的打开状态调整为相反
            MenuView.IsPaneOpen = !MenuView.IsPaneOpen;
            SettingView.IsPaneOpen = !SettingView.IsPaneOpen;
        }

        private async void Assess_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store://pdp/?productid=9nc2p9fkkdsc"));
        }

        private void Exit_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        private async void More_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store://publisher/?name=MEP Studio"));
        }
       
        private async void Login_Tapped(object sender, TappedRoutedEventArgs e)
        {
             await WebView.ClearTemporaryWebDataAsync();
             WebSite.Navigate(new Uri("https://common.ofo.so/newdist/?Login"));
        }

        private void About_Tapped(object sender, TappedRoutedEventArgs e)
        {
            i = 1;
            Frame.Navigate(typeof(AboutPage));           
        }
              
        private void Home_Tapped(object sender, TappedRoutedEventArgs e)
        {
            WebSite.Navigate(new Uri("https://common.ofo.so/newdist/?Login"));
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (WebSite.CanGoBack)
            {
                WebSite.GoBack();
            }
        }
        private void Recharge_Click(object sender, RoutedEventArgs e)
        {           
            WebSite.Navigate(new Uri("https://common.ofo.so/newdist/?Purchase"));        
        }

        private void QR_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(QRPage));
        }

        private void Cash_Click(object sender, RoutedEventArgs e)
        {
            WebSite.Navigate(new Uri("https://common.ofo.so/newdist/?Identification&~result=%7B%22jump%22%3A1%2C%22rule%22%3A3%2C%22bond%22%3A0%2C%22auth%22%3A0%2C%22zhiMaEnable%22%3A0%7D&~lat=34.27250009152079&~lng=108.94566740783335"));
        }

        private void Use_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(UsePage));
          
        }
    }
}
