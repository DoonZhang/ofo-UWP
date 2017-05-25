using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Media.Devices;
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
    public sealed partial class QR : Page
    {
        public QR()
        {
            this.InitializeComponent();
        }
        /*
        private async void InitMediaCaptureAsync()
        {
            //寻找后置摄像头
            var allVideoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            var cameraDevice = allVideoDevices.FirstOrDefault(x => x.EnclosureLocation != null && x.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Back);

            if (cameraDevice == null)
            {
                Debug.WriteLine("No camera device found!");

                return;
            }

            var settings = new MediaCaptureInitializationSettings
            {
                StreamingCaptureMode = StreamingCaptureMode.Video,
                //必须，否则截图的时候会很卡很慢
                PhotoCaptureSource = PhotoCaptureSource.VideoPreview,
                VideoDeviceId = cameraDevice.Id
            };

            _mediaCapture = new MediaCapture();

            try
            {
                await _mediaCapture.InitializeAsync(settings);
                _initialized = true;//初始化成功
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("The app was denied access to the camera");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception when initializing MediaCapture with {0}: {1}", cameraDevice.Id, ex.ToString());
            }

            if (_initialized)
            {
                var focusControl = _mediaCapture.VideoDeviceController.FocusControl;

                if (focusControl.Supported)
                {
                    var focusSettings = new FocusSettings()
                    {
                        Mode = focusControl.SupportedFocusModes.FirstOrDefault(f => f == FocusMode.Continuous),
                        DisableDriverFallback = true,
                        AutoFocusRange = focusControl.SupportedFocusRanges.FirstOrDefault(f => f == AutoFocusRange.FullRange),
                        Distance = focusControl.SupportedFocusDistances.FirstOrDefault(f => f == ManualFocusDistance.Nearest)
                    };

                    //设置聚焦，最好使用FocusMode.Continuous，否则影响截图会很模糊，不利于识别
                    focusControl.Configure(focusSettings);
                }

                captureElement.Source = _mediaCapture;
                captureElement.FlowDirection = FlowDirection.LeftToRight;

                try
                {
                    await _mediaCapture.StartPreviewAsync();
                    _previewing = true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception when starting the preview: {0}", ex.ToString());
                }

                if (_previewing)
                {
                    try
                    {
                        if (_mediaCapture.VideoDeviceController.FlashControl.Supported)
                        {
                            //关闭闪光灯
                            _mediaCapture.VideoDeviceController.FlashControl.Enabled = false;
                        }
                    }
                    catch
                    {
                    }

                    if (focusControl.Supported)
                    {
                        //开始聚焦
                        await focusControl.FocusAsync();
                    }
                }
            }

            private void InitTimer()
            {
                _timer = new DispatcherTimer();
                //每50毫秒截一次图
                _timer.Interval = TimeSpan.FromMilliseconds(50);
                _timer.Tick += _timer_Tick;
                _timer.Start();
            }

            private async void _timer_Tick(object sender, object e)
            {
                using (var stream = new InMemoryRandomAccessStream())
                {
                    var previewProperties = _mediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview) as VideoEncodingProperties;
                    //将截图写入内存流中
                    await _mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), stream);

                    //利用Zxing识别，成功：停止timer；失败：继续
                    var reader = new BarcodeReader();
                    var bitmapWriteable = new WriteableBitmap((int)previewProperties.Width, (int)previewProperties.Height);
                    bitmapWriteable.SetSource(stream);
                    var result = reader.Decode(bitmapWriteable);

                    if (!string.IsNullOrEmpty(result.Text))
                    {
                        _timer.Stop();
                    }
                }
            }

            private async void _timer_Tick(object sender, object e)
            {
                var previewProperties = _mediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview) as VideoEncodingProperties;

                using (var videoFrame = new VideoFrame(BitmapPixelFormat.Rgba8, (int)previewProperties.Width, (int)previewProperties.Height))
                {
                    using (var currentFrame = await _mediaCapture.GetPreviewFrameAsync(videoFrame))
                    {
                        using (var previewFrame = currentFrame.SoftwareBitmap)
                        {
                            var buffer = new Windows.Storage.Streams.Buffer((uint)(4 * previewFrame.PixelWidth * previewFrame.PixelHeight));
                            previewFrame.CopyToBuffer(buffer);

                            using (var stream = buffer.AsStream().AsRandomAccessStream())
                            {
                                //利用Zxing识别，成功：停止timer；失败：继续
                                var reader = new BarcodeReader();
                                var bitmapWriteable = new WriteableBitmap((int)previewProperties.Width, (int)previewProperties.Height);
                                bitmapWriteable.SetSource(stream);
                                var result = reader.Decode(bitmapWriteable);

                                if (!string.IsNullOrEmpty(result.Text))
                                {
                                    _timer.Stop();
                                }
                            }
                        }
                    }
                }
            }

            using (var stream = buffer.AsStream().AsRandomAccessStream())
            {
                var decoder = await BitmapDecoder.CreateAsync(BitmapDecoder.JpegDecoderId, stream);
                var destStream = new InMemoryRandomAccessStream();

                var encoder = await BitmapEncoder.CreateForTranscodingAsync(destStream, decoder);

                //剪裁
                encoder.BitmapTransform.Bounds = new BitmapBounds() { X = 0, Y = 0, Width = 100, Height = 100 };
                //缩放
                encoder.BitmapTransform.ScaledWidth = 100;
                encoder.BitmapTransform.ScaledHeight = 100;
                //旋转
                encoder.BitmapTransform.Rotation = BitmapRotation.Clockwise90Degrees;

                await encoder.FlushAsync();
                await destStream.FlushAsync();
            }
        }
        */
    }
}
