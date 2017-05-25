using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace ofo.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SplashPage : Page
    {

        StatusBar statusbar;
      

        public SplashPage()
        {
            this.InitializeComponent();
        }
        int n = 0;
        ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
        //获取本地应用设置容器

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!settings.Values.ContainsKey("First"))
            { //应用首次启动，必定不会含"First"这个键，让应用导航到GuidePage这个页面，GuidePage这个页面就是对应用的介绍啦

            }
            else
            {
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(0, 0, 5);
                timer.Tick += (sender, args) =>
                {

                    if (n == 0)
                    //  FullAD.Visibility = Visibility.Collapsed;
                    {
                        Frame.Navigate(typeof(MainPage));
                        n = 1;
                    }
                };
                timer.Start();
            }

        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {

            //判断settings容器里面有没有"First"这个键
            if (!settings.Values.ContainsKey("First"))
            { //应用首次启动，必定不会含"First"这个键，让应用导航到GuidePage这个页面，GuidePage这个页面就是对应用的介绍啦
                Frame.Navigate(typeof(GuidePage));
                //在settings容器里面写入"First"这个键值对，应用再次启动时，就不会在导航到介绍页面了。
                settings.Values["First"] = "yes";
            }
            else
            {
            }
            // 判断是否存在 StatusBar
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                //静态方法取得当前 StatusBar 实例
                statusbar = StatusBar.GetForCurrentView();

                //显示状态栏
                await statusbar.HideAsync();
            }

        }
    }
}
