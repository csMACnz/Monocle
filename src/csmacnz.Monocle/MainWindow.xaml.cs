﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Win32;

namespace csmacnz.Monocle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        //http://www.codeproject.com/Articles/97871/WPF-simple-zoom-and-drag-support-in-a-ScrollViewer
        private Point? _lastDragPoint;
        private Point? _lastCenterPositionOnTarget;
        private Point? _lastMousePositionOnTarget;

        public MainWindow()
        {
            InitializeComponent();
            renderButton.IsEnabled = true;
            resetButton.IsEnabled = false;
            saveButton.IsEnabled = false;
        }

        void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                _lastMousePositionOnTarget = Mouse.GetPosition(Canvas);

                if (e.Delta > 0)
                {
                    Slider.Value += 1;
                }
                if (e.Delta < 0)
                {
                    Slider.Value -= 1;
                }

                e.Handled = true;
            }
        }

        private void OnSliderValueChanged(object sender,
            RoutedPropertyChangedEventArgs<double> e)
        {
            var scaleValue = Math.Pow(2, e.NewValue);
            ScaleTransform.ScaleX = scaleValue;
            ScaleTransform.ScaleY = scaleValue;

            var centerOfViewport = new Point(ScrollViewer.ViewportWidth / 2,
                ScrollViewer.ViewportHeight / 2);
            _lastCenterPositionOnTarget = ScrollViewer.TranslatePoint(centerOfViewport, Canvas);
        }

        void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_lastDragPoint.HasValue)
            {
                Point posNow = e.GetPosition(ScrollViewer);

                double dX = posNow.X - _lastDragPoint.Value.X;
                double dY = posNow.Y - _lastDragPoint.Value.Y;

                _lastDragPoint = posNow;

                ScrollViewer.ScrollToHorizontalOffset(ScrollViewer.HorizontalOffset - dX);
                ScrollViewer.ScrollToVerticalOffset(ScrollViewer.VerticalOffset - dY);
            }
        }

        void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mousePos = e.GetPosition(ScrollViewer);
            if (mousePos.X <= ScrollViewer.ViewportWidth && mousePos.Y <
                ScrollViewer.ViewportHeight) //make sure we still can use the scrollbars
            {
                ScrollViewer.Cursor = Cursors.SizeAll;
                _lastDragPoint = mousePos;
                Mouse.Capture(ScrollViewer);
            }
        }


        void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ScrollViewer.Cursor = Cursors.Arrow;
            ScrollViewer.ReleaseMouseCapture();
            _lastDragPoint = null;
        }

        void OnScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentHeightChange != 0 || e.ExtentWidthChange != 0)
            {
                Point? targetBefore = null;
                Point? targetNow = null;

                if (!_lastMousePositionOnTarget.HasValue)
                {
                    if (_lastCenterPositionOnTarget.HasValue)
                    {
                        var centerOfViewport = new Point(ScrollViewer.ViewportWidth / 2,
                                                         ScrollViewer.ViewportHeight / 2);
                        Point centerOfTargetNow =
                              ScrollViewer.TranslatePoint(centerOfViewport, Canvas);

                        targetBefore = _lastCenterPositionOnTarget;
                        targetNow = centerOfTargetNow;
                    }
                }
                else
                {
                    targetBefore = _lastMousePositionOnTarget;
                    targetNow = Mouse.GetPosition(Canvas);

                    _lastMousePositionOnTarget = null;
                }

                if (targetBefore.HasValue)
                {
                    double dXInTargetPixels = targetNow.Value.X - targetBefore.Value.X;
                    double dYInTargetPixels = targetNow.Value.Y - targetBefore.Value.Y;

                    double multiplicatorX = e.ExtentWidth / Canvas.ActualWidth;
                    double multiplicatorY = e.ExtentHeight / Canvas.ActualHeight;

                    double newOffsetX = ScrollViewer.HorizontalOffset -
                                        dXInTargetPixels * multiplicatorX;
                    double newOffsetY = ScrollViewer.VerticalOffset -
                                        dYInTargetPixels * multiplicatorY;

                    if (double.IsNaN(newOffsetX) || double.IsNaN(newOffsetY))
                    {
                        return;
                    }

                    ScrollViewer.ScrollToHorizontalOffset(newOffsetX);
                    ScrollViewer.ScrollToVerticalOffset(newOffsetY);
                }
            }
        }

        private async void OnRenderClick(object sender, RoutedEventArgs e)
        {
            renderButton.IsEnabled = false;

            await RenderAsync();

            resetButton.IsEnabled = true;
            saveButton.IsEnabled = true;
        }

        private Task RenderAsync()
        {
            //int width = 640, height = 480;
            int width = 1024, height = 768;

            var pixelFormat = PixelFormats.Rgb24;
            var bitsPerPixel = pixelFormat.BitsPerPixel;

            var rawStride = (width * bitsPerPixel + 7) / 8;
            var pixelData = new byte[rawStride * height];

            Action updateScreen = () =>
            {
                var bitmap = BitmapSource.Create(width, height,
                    96, 96, pixelFormat, null, pixelData, rawStride);

                Canvas.Source = bitmap;
            };
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += (s, e) => updateScreen();
            timer.Start();

            return Task.Factory.StartNew(
                () =>
                {
                    var horizontalSectionCount = 16;
                    var verticalSectionCount = 16;
                    var sectionWidth = width / horizontalSectionCount;
                    var sectionHeight = height / verticalSectionCount;
                    List<RenderTask> taskQueue = new List<RenderTask>();

                    foreach (var y in Enumerable.Range(0, verticalSectionCount))
                    {
                        foreach (var x in Enumerable.Range(0, horizontalSectionCount))
                        {
                            taskQueue.Add(
                                new RenderTask
                                {
                                    MinVerticalPixel = y * sectionHeight,
                                    VerticalCount = sectionHeight,
                                    MinHorizontalPixel = x * sectionWidth,
                                    HorizontalCount = sectionWidth
                                });
                        }
                    }
                    taskQueue.AsParallel().Select(task =>
                    {
                        var defaultColor = Colors.Firebrick;

                        foreach (var y in Enumerable.Range(task.MinVerticalPixel, task.VerticalCount))
                        {
                            foreach (var x in Enumerable.Range(task.MinHorizontalPixel, task.HorizontalCount))
                            {
                                //System.Threading.Thread.Sleep(0);
                                System.Threading.Thread.Sleep(1);
                                int xIndex = x * 3;
                                int yIndex = y * rawStride;
                                pixelData[xIndex + yIndex] = defaultColor.R;
                                pixelData[xIndex + yIndex + 1] = defaultColor.G;
                                pixelData[xIndex + yIndex + 2] = defaultColor.B;
                            }
                        }

                        return 0;
                    }).Aggregate((_, __) => 0);

                }).ContinueWith(task =>
                {
                    timer.Stop();
                    updateScreen();
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void OnResetClick(object sender, RoutedEventArgs e)
        {
            Canvas.Source = null;
            resetButton.IsEnabled = false;
            saveButton.IsEnabled = false;
            renderButton.IsEnabled = true;
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                DefaultExt = ".png",
                Filter = "PNG (*.png)|*.png"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                using (var filestream = saveFileDialog.OpenFile())
                {
                    var bitmapSource = Canvas.Source as BitmapSource;
                    if (bitmapSource == null) throw new ArgumentNullException(nameof(bitmapSource));
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                    encoder.Save(filestream);
                }
            }
        }
    }
}
