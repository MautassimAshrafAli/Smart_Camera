using System;
using System.Collections.Generic;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.IO;
using System.Windows;
using System.ComponentModel;
using System.Timers;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Runtime.CompilerServices;
using Emgu.CV.Face;
using Emgu.CV.Util;
using Microsoft.Win32;
using System.Windows.Media;
using System.Windows.Input;
using System.Drawing.Imaging;
using DirectX.Capture;
using System.Threading.Tasks;
using System.Windows.Interop;
using Tesseract;

namespace FaceDetectionAndRecognition
{

    public partial class WFFaceRecognition : Window, INotifyPropertyChanged
    {
        #region Properties
        string s = System.Environment.GetEnvironmentVariable("USERPROFILE");
        public Capture cap_d;
        public Filters filters_;
        public event PropertyChangedEventHandler PropertyChanged;
        private VideoCapture videoCapture;
        private VideoCapture videoCapture2;
        private VideoCapture videoCapture_text;
        private CascadeClassifier haarCascade;
        private Image<Bgr, Byte> bgrFrame = null;
        private Image<Gray, Byte> grayFrame = null;
        private Image<Hsv, Byte> hsvFrame = null;
        private Image<Lab, Byte> labFrame = null;
        private Image<Gray, Byte> detectedFace = null;
        private Image<Bgr, Byte> bgrFrame_text = null;
        private List<FaceData> faceList = new List<FaceData>();
        private VectorOfMat imageList = new VectorOfMat();
        private List<string> nameList = new List<string>();
        private VectorOfInt labelList = new VectorOfInt();

        private EigenFaceRecognizer recognizer;
        private Timer captureTimer;
        private Timer captureTimer_text;
        private DispatcherTimer lTimer_v;
        #region FaceName
        private string faceName;
        string camera_color_opetions = "bgr";

        public string FaceName
        {
            get { return faceName; }
            set
            {
                faceName = value.ToUpper();
                lblFaceName.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { lblFaceName.Text = faceName; }));
            }
        }
        #endregion
        #region CameraCaptureImage
        private Bitmap cameraCapture;
        public Bitmap CameraCapture
        {
            get { return cameraCapture; }
            set
            {
                cameraCapture = value;
                imgCamera.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { imgCamera.Source = BitmapToImageSource(cameraCapture); }));
            }
        }
        private Bitmap cameraCapture_text_vid;
        public Bitmap CameraCapture_t_v
        {
            get { return cameraCapture_text_vid; }
            set
            {
                cameraCapture_text_vid = value;
                imgCamera_text_vid.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { imgCamera_text_vid.Source = BitmapToImageSource(cameraCapture_text_vid); }));
            }
        }
        #endregion

        #endregion

        #region CameraCaptureFaceImage
        private Bitmap cameraCaptureFace;
        public Bitmap CameraCaptureFace
        {
            get { return cameraCaptureFace; }
            set
            {
                cameraCaptureFace = value;
                imgDetectFace.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { imgDetectFace.Source = BitmapToImageSource(cameraCaptureFace); }));
            }
        }
        #endregion

        public WFFaceRecognition()
        {
            InitializeComponent();

            captureTimer = new Timer()
            {
                Interval = Config.TimerResponseValue - 300
            };
            captureTimer.Elapsed += CaptureTimer_Elapsed;

            captureTimer_text = new Timer()
            {
                Interval = Config.TimerResponseValue - 300

            };
            captureTimer_text.Elapsed += CaptureTimer_text_Elapsed;


            lTimer_v = new DispatcherTimer(DispatcherPriority.SystemIdle);
            lTimer_v.Tick += new EventHandler(OnUpdateTimerTick);
            lTimer_v.Interval = TimeSpan.FromMilliseconds(1000);

        }

        #region video record time
        int time_v_s = 0;
        int time_v_m = 0;
        int time_v_h = 0;
        string time_v;
        private void OnUpdateTimerTick(object sender, EventArgs e)
        {
            time_v_s += 1;

            vpo.time = time_v_h.ToString() + ":" + time_v_m.ToString() + ":" + time_v_s.ToString();


            if (time_v_s > 59)
            {

                time_v_s = 0;
                time_v_m += 1;
                time_v_h = 0;

                time_v = time_v_h.ToString() + ":" + time_v_m.ToString() + ":" + time_v_s.ToString();
                vpo.time = time_v;

            }
            if (time_v_m > 59)
            {

                time_v_s = 0;
                time_v_m = 0;
                time_v_h += 1;

                time_v = time_v_h.ToString() + ":" + time_v_m.ToString() + ":" + time_v_s.ToString();
                vpo.time = time_v;


            }

            if (time_v_h > 12)
            {


                time_v_s = 0;
                time_v_m = 0;
                time_v_h = 1;

                time_v = time_v_h.ToString() + ":" + time_v_m.ToString() + ":" + time_v_s.ToString();
                vpo.time = time_v;


            }

        }

        #endregion

        private void vid_cap()
        {
            Task.Factory.StartNew(() =>
            {
                videoCapture = new VideoCapture(0);
                videoCapture.SetCaptureProperty(CapProp.Fps, 20);
                videoCapture.SetCaptureProperty(CapProp.FrameHeight, 450);
                videoCapture.SetCaptureProperty(CapProp.FrameWidth, 360);

            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            captureTimer.Interval = 70;
            GetFacesList();
            vid_cap();
            captureTimer.Start();

        }

        private void CaptureTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                ProcessFrame();
            });
        }
        private void CaptureTimer_text_Elapsed(object sender, ElapsedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                ProcessFrame_text();
            });
        }
      
        bool face_vid = false;
        int t = 0;

        #region Process face
        public void GetFacesList()
        {
            //haar cascade classifier
            if (!File.Exists(Config.HaarCascadePath))
            {
                string text = "Cannot find Haar cascade data file:\n\n";
                text += Config.HaarCascadePath;
                MessageBoxResult result = MessageBox.Show(text, "Error",
                       MessageBoxButton.OK, MessageBoxImage.Error);
            }

            haarCascade = new CascadeClassifier(Config.HaarCascadePath);
            faceList.Clear();
            string line;
            FaceData faceInstance = null;

            // Create empty directory / file for face data if it doesn't exist
            if (!Directory.Exists(Config.FacePhotosPath))
            {
                Directory.CreateDirectory(Config.FacePhotosPath);
            }

            if (!File.Exists(Config.FaceListTextFile))
            {
                string text = "Cannot find face data file:\n\n";
                text += Config.FaceListTextFile + "\n\n";
                text += "If this is your first time running the app, an empty file will be created for you.";
                MessageBoxResult result = MessageBox.Show(text, "Warning",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                switch (result)
                {
                    case MessageBoxResult.OK:
                        String dirName = Path.GetDirectoryName(Config.FaceListTextFile);
                        Directory.CreateDirectory(dirName);
                        File.Create(Config.FaceListTextFile).Close();
                        break;
                }
            }

            StreamReader reader = new StreamReader(Config.FaceListTextFile);
            int i = 0;
            while ((line = reader.ReadLine()) != null)
            {
                string[] lineParts = line.Split(':');
                faceInstance = new FaceData();
                faceInstance.FaceImage = new Image<Gray, byte>(Config.FacePhotosPath + lineParts[0] + Config.ImageFileExtension);
                faceInstance.PersonName = lineParts[1];
                faceList.Add(faceInstance);
            }
            foreach (var face in faceList)
            {
                imageList.Push(face.FaceImage.Mat);
                nameList.Add(face.PersonName);
                labelList.Push(new[] { i++ });
            }
            reader.Close();

            // Train recogniser
            if (imageList.Size > 0)
            {
                recognizer = new EigenFaceRecognizer(imageList.Size);
                recognizer.Train(imageList, labelList);
            }

        }
        bool m = false;
        private void ProcessFrame()
        {

            Task.Factory.StartNew(() =>
            {

                switch (camera_color_opetions)
                {
                    case "bgr":

                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            try
                            {
                                using (bgrFrame = videoCapture.QueryFrame().ToImage<Bgr, Byte>())
                                {
                                    if (bgrFrame != null)
                                    {
                                        try
                                        {//for emgu cv bug

                                            if (m == false)
                                            {
                                                Image<Gray, byte> grayframe = bgrFrame.Convert<Gray, byte>();

                                                Rectangle[] faces = haarCascade.DetectMultiScale(grayframe, 1.2, 10, new System.Drawing.Size(50, 50), new System.Drawing.Size(200, 200));

                                                //detect face
                                                FaceName = "No face detected";
                                                foreach (var face in faces)
                                                {
                                                    bgrFrame.Draw(face, new Bgr(255, 255, 0), 2);
                                                    detectedFace = bgrFrame.Copy(face).Convert<Gray, byte>();
                                                    FaceRecognition();
                                                    break;
                                                }

                                            }
                                            //CameraCapture = frame;
                                            CameraCapture = bgrFrame.ToBitmap();

                                            System.Threading.Thread.Sleep(1);



                                        }
                                        catch (Exception)
                                        { }
                                    }
                                }
                                videoCapture.Pause();
                            }
                            catch (Exception)
                            { }
                        }));

                        break;
                    case "gray":


                        this.Dispatcher.Invoke((Action)(() =>
                        {

                            try
                            {



                                using (grayFrame = videoCapture.QueryFrame().ToImage<Gray, Byte>())
                                {

                                    if (grayFrame != null)
                                    {
                                        try
                                        {

                                            if (m == false)
                                            {
                                                Image<Gray, byte> grayframe = grayFrame.Convert<Gray, byte>();

                                                Rectangle[] faces = haarCascade.DetectMultiScale(grayframe, 1.2, 10, new System.Drawing.Size(50, 50), new System.Drawing.Size(200, 200));

                                            //detect face
                                            FaceName = "No face detected";
                                                foreach (var face in faces)
                                                {
                                                // grayFrame.Draw(face, new (255, 255, 0), 2);
                                                detectedFace = grayFrame.Copy(face).Convert<Gray, byte>();
                                                    FaceRecognition();
                                                    break;
                                                }

                                            }
                                            CameraCapture = grayFrame.ToBitmap();
                                            System.Threading.Thread.Sleep(1);


                                        }
                                        catch (Exception)
                                        { }
                                    }


                                }

                                videoCapture.Pause();
                            }
                            catch (Exception)
                            { }
                        }));



                        break;
                    case "hsv":


                        this.Dispatcher.Invoke((Action)(() =>
                        {

                            try
                            {


                                using (hsvFrame = videoCapture.QueryFrame().ToImage<Hsv, Byte>())
                                {


                                    if (hsvFrame != null)
                                    {
                                        try
                                        {

                                            if (m == false)
                                            {
                                                Image<Gray, byte> grayframe = hsvFrame.Convert<Gray, byte>();

                                                Rectangle[] faces = haarCascade.DetectMultiScale(grayframe, 1.2, 10, new System.Drawing.Size(50, 50), new System.Drawing.Size(200, 200));

                                                //detect face
                                                FaceName = "No face detected";
                                                foreach (var face in faces)
                                                {
                                                    // grayFrame.Draw(face, new (255, 255, 0), 2);
                                                    detectedFace = hsvFrame.Copy(face).Convert<Gray, byte>();
                                                    FaceRecognition();
                                                    break;
                                                }

                                            }
                                            CameraCapture = hsvFrame.ToBitmap();
                                            System.Threading.Thread.Sleep(1);


                                        }
                                        catch (Exception)
                                        { }
                                    }

                                }

                                videoCapture.Pause();
                            }
                            catch (Exception)
                            { }

                        }));



                        break;
                    case "lab":


                        this.Dispatcher.Invoke((Action)(() =>
                        {

                            try
                            {



                                using (labFrame = videoCapture.QueryFrame().ToImage<Lab, Byte>())
                                {
                                    if (labFrame != null)
                                    {
                                        try
                                        {

                                            if (m == false)
                                            {
                                                Image<Gray, byte> grayframe = labFrame.Convert<Gray, byte>();

                                                Rectangle[] faces = haarCascade.DetectMultiScale(grayframe, 1.2, 10, new System.Drawing.Size(50, 50), new System.Drawing.Size(200, 200));

                                                //detect face
                                                FaceName = "No face detected";
                                                foreach (var face in faces)
                                                {
                                                    // grayFrame.Draw(face, new (255, 255, 0), 2);
                                                    detectedFace = labFrame.Copy(face).Convert<Gray, byte>();
                                                    FaceRecognition();
                                                    break;
                                                }

                                            }
                                            CameraCapture = labFrame.ToBitmap();
                                            System.Threading.Thread.Sleep(1);


                                        }
                                        catch (Exception)
                                        { }
                                    }

                                }

                                videoCapture.Pause();
                            }
                            catch (Exception)
                            { }
                        }));

                        break;

                    default:

                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            try
                            {
                                using (bgrFrame = videoCapture.QueryFrame().ToImage<Bgr, Byte>())
                                {
                                    if (bgrFrame != null)
                                    {
                                        try
                                        {//for emgu cv bug

                                            if (m == false)
                                            {
                                                Image<Gray, byte> grayframe = bgrFrame.Convert<Gray, byte>();

                                                Rectangle[] faces = haarCascade.DetectMultiScale(grayframe, 1.2, 10, new System.Drawing.Size(50, 50), new System.Drawing.Size(200, 200));

                                                //detect face
                                                FaceName = "No face detected";
                                                foreach (var face in faces)
                                                {
                                                    bgrFrame.Draw(face, new Bgr(255, 255, 0), 2);
                                                    detectedFace = bgrFrame.Copy(face).Convert<Gray, byte>();
                                                    FaceRecognition();
                                                    break;
                                                }

                                            }
                                            //CameraCapture = frame;
                                            CameraCapture = bgrFrame.ToBitmap();

                                            System.Threading.Thread.Sleep(1);



                                        }
                                        catch (Exception)
                                        { }
                                    }
                                }
                                videoCapture.Pause();
                            }
                            catch (Exception)
                            { }
                        }));
                        break;
                }

            });
        }

        private void ProcessFrame_text()
        {
            Task.Factory.StartNew(() =>
            {

                bgrFrame_text = videoCapture_text.QueryFrame().ToImage<Bgr, Byte>();
                Image<Gray, byte> grayframe = bgrFrame_text.Convert<Gray, byte>();
                CameraCapture_t_v = bgrFrame_text.ToBitmap();

            });

        }

        private void FaceRecognition()
        {
            if (imageList.Size != 0)
            {

                //Eigen Face Algorithm
                FaceRecognizer.PredictionResult result = recognizer.Predict(detectedFace.Resize(100, 100, Inter.Cubic));
                FaceName = nameList[result.Label];
                CameraCaptureFace = detectedFace.ToBitmap();

                System.Threading.Thread.Sleep(1);

                ////Save detected face
                //detectedFace = detectedFace.Resize(100, 100, Inter.Cubic);
                //detectedFace.Save(Config.FacePhotosPath + "face" + (faceList.Count + 1) + Config.ImageFileExtension);
                //StreamWriter writer = new StreamWriter(Config.FaceListTextFile, true);
                //writer.WriteLine(String.Format("face{0}:{1}", (faceList.Count + 1), m.ToString()));
                //writer.Close();
                //GetFacesList();
            }
            else
            {
                FaceName = "Please Add Face";
            }
        }
        /// <summary>
        /// Convert bitmap to bitmap image for image control
        /// </summary>
        /// <param name="bitmap">Bitmap image</param>
        /// <returns>Image Source</returns>
        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }


        #endregion

        #region stop video
        private void vstop_MouseUp(object sender, MouseButtonEventArgs e)
        {
            t = 1;

            video_take.Isactive = Visibility.Hidden;
            camera_take.Isactive = Visibility.Visible;
            face_take.Isactive = Visibility.Hidden;
            //camera_object_i.Visibility = Visibility.Hidden;
          
            cap_d.Stop();
            cap_d.Dispose();

            pic_h.Visibility = Visibility.Hidden;

            vpo.Visibility = Visibility.Hidden;
            lblFaceName.Visibility = Visibility.Visible;
            face_take.Visibility = Visibility.Visible;

            imgCamera.Visibility = Visibility.Visible;


            lTimer_v.Stop();
            time_v_s = 0;
            time_v_m = 0;
            time_v_h = 0;
            vpo.time = "0:0:0";

            video_start = false;

            GetFacesList();

            vid_cap();

            captureTimer.Start();
        }

        #endregion

        #region Detect Text in video
        bool iscam_text = false;
        private void camera_text_MouseUp(object sender, MouseButtonEventArgs e)
        {

            if (face_vid_o_c == false)
            {

                if (cam_text_live_check == false)
                {

                    if (iscam_text == false)
                    {

                        OpenFileDialog ofd = new OpenFileDialog();

                        if (ofd.ShowDialog() == true)
                        {


                            cam_text.icon = char.ConvertFromUtf32(0xE8BB);

                            captureTimer.Stop();
                            videoCapture.Stop();
                            videoCapture.Dispose();

                            start_text = true;

                            lblFaceName.Visibility = Visibility.Hidden;

                            imgCamera.Visibility = Visibility.Hidden;

                            videoCapture2 = new VideoCapture(ofd.FileName);
                            Mat m = new Mat();
                            videoCapture2.Read(m);
                            CameraCapture_t_v = m.ToBitmap();


                            WindowState = System.Windows.WindowState.Maximized;

                            imgCamera_text_vid.Visibility = Visibility.Visible;
                            i2.Visibility = Visibility.Visible;


                            if (isplay_text == true)
                            {


                                cam_text_o.Visibility = Visibility.Visible;

                            }

                            iscam_text = true;


                        }



                    }
                    else
                    {



                        cam_text.icon = char.ConvertFromUtf32(0xF0E3);
                        cam_text_o.Visibility = Visibility.Hidden;

                        start_text = false;
                        imgCamera_text_vid.Source = null;
                        videoCapture2.Stop();

                        imgCamera_text_vid.Visibility = Visibility.Hidden;
                        i2.Visibility = Visibility.Hidden;

                        imgCamera.Visibility = Visibility.Visible;


                        GetFacesList();

                        vid_cap();

                        captureTimer.Start();

                        camera_color_opetions = "bgr";

                        isplay_text = true;
                        iscam_text = false;

                    }


                }
                else {MessageBox.Show("you are in video text recognition"); }



            }
            else { MessageBox.Show("you are in video face detection mode");}
        }
        private void DetectText_live( Image<Bgr, byte> img)
        {

            Image<Gray, byte> sobel = img.Convert<Gray, byte>().Sobel(1, 0, 3).AbsDiff(new Gray(0.0)).Convert<Gray, byte>().ThresholdBinary(new Gray(50), new Gray(255));
            Mat SE = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new System.Drawing.Size(15, 1), new System.Drawing.Point(-1, -1));
            sobel = sobel.MorphologyEx(MorphOp.Dilate, SE, new System.Drawing.Point(-1, -1), 1, BorderType.Reflect, new MCvScalar(255));
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat m = new Mat();

            CvInvoke.FindContours(sobel, contours, m, RetrType.External, ChainApproxMethod.ChainApproxSimple);

            List<Rectangle> list = new List<Rectangle>();

            for (int i = 0; i < contours.Size; i++)
            {
                Rectangle brect = CvInvoke.BoundingRectangle(contours[i]);

                double ar = brect.Width / brect.Height;
                if (ar > 2 && brect.Width > 25 && brect.Height > 8 && brect.Height < 100)
                {
                    list.Add(brect);

                }
            }

            Image<Bgr, byte> imgout = img.CopyBlank();
            foreach (var r in list)
            {
                CvInvoke.Rectangle(img, r, new MCvScalar(0, 0, 255), 2);
                CvInvoke.Rectangle(imgout, r, new MCvScalar(0, 255, 255), -1);
            }
            imgout._And(img);

            
            CameraCapture_t_v = img.ToBitmap();

            ip2.Image = imgout.ToBitmap();

        }

        bool start_text = true;
        private async void camtextAsync(VideoCapture vid_cap, Bitmap img_)
        {

            if (vid_cap == null)
            {
                return;
            }

            try
            {

                while (start_text)
                {

                    Mat frame = new Mat();
                    vid_cap.Read(frame);

                    if (!frame.IsEmpty)
                    {
                        img_ = frame.ToBitmap();
                        DetectText_live(frame.ToImage<Bgr, byte>());
                        double fps = vid_cap.GetCaptureProperty(CapProp.Fps);
                        await Task.Delay(1000 / Convert.ToInt32(fps));

                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (Exception)
            { }

        }
        bool isplay_text = true;
        private void cam_text_o_MouseUp(object sender, MouseButtonEventArgs e)
        {

            camtextAsync(videoCapture2, CameraCapture_t_v);

            Task.Delay(5);

            if (isplay_text == true)
            {

                cam_text_o.Visibility = Visibility.Hidden;

                isplay_text = false;

            }
        }
        private void if_cantext_ison()
        {

            cam_text_live.icon = char.ConvertFromUtf32(0xEDFB);

            start_text = false;
            imgCamera_text_vid.Source = null;
            captureTimer_text.Stop();
            videoCapture_text.Stop();

            i2.Visibility = Visibility.Hidden;
            imgCamera_text_vid.Visibility = Visibility.Hidden;
            imgCamera.Visibility = Visibility.Visible;
            cam_text_live_check = false;


            vid_cap();

            captureTimer.Start();

            camera_color_opetions = "bgr";

            cameratext_live_c = 0;

        }
        #endregion

        #region mobile contorl

        //mobile control op
        private void windows__KeyUp(object sender, KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if ((Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.T) || (Keyboard.IsKeyDown(Key.RightCtrl) && e.Key == Key.T))
            {

                if (m == true)
                {

                    img.Source = imgCamera.Source;

                    String filePath = @"C:\Users\" + Path.GetFileName(s) + @"\Pictures\pic" + DateTime.Now.Year.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Second.ToString() + ".png";

                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create((BitmapSource)imgCamera.Source));
                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                        encoder.Save(stream);

                }

                m = false;

            }
            if ((Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.V) || (Keyboard.IsKeyDown(Key.RightCtrl) && e.Key == Key.V))
            {

                cap_d.Filename = @"C:\Users\" + Path.GetFileName(s) + @"\Videos\" + DateTime.Now.ToString("hh") + "_" + DateTime.Now.Second.ToString() + ".mp4";
                cap_d.Cue();
                cap_d.Start();

            }

            if ((Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.N) || (Keyboard.IsKeyDown(Key.RightCtrl) && e.Key == Key.N))
            {
                gray_o.Isactiv = Visibility.Hidden;
                hsv_o.Isactiv = Visibility.Hidden;
                lab_o.Isactiv = Visibility.Hidden;
                bgr_o.Isactiv = Visibility.Hidden;

                color_o = false;

            }


            if ((Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.L) || (Keyboard.IsKeyDown(Key.RightCtrl) && e.Key == Key.L))
            {

                gray_o.Isactiv = Visibility.Hidden;
                hsv_o.Isactiv = Visibility.Hidden;
                lab_o.Isactiv = Visibility.Hidden;
                bgr_o.Isactiv = Visibility.Hidden;

                color_o = false;

            }
            if ((Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.F) || (Keyboard.IsKeyDown(Key.RightCtrl) && e.Key == Key.F))
            {
                gray_o.Isactiv = Visibility.Hidden;
                hsv_o.Isactiv = Visibility.Hidden;
                lab_o.Isactiv = Visibility.Hidden;
                bgr_o.Isactiv = Visibility.Hidden;

                color_o = false;

            }
            if ((Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.R) || (Keyboard.IsKeyDown(Key.RightCtrl) && e.Key == Key.R))
            {
                gray_o.Isactiv = Visibility.Hidden;
                hsv_o.Isactiv = Visibility.Hidden;
                lab_o.Isactiv = Visibility.Hidden;
                bgr_o.Isactiv = Visibility.Hidden;

                color_o = false;

            }
            if ((Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.X) || (Keyboard.IsKeyDown(Key.RightCtrl) && e.Key == Key.X))
            {
                camera_text_live_MouseUp(null, null);

            }
            
        }

        private void windows__KeyDown(object sender, KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if ((Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.T) || (Keyboard.IsKeyDown(Key.RightCtrl) && e.Key == Key.T))
            {
                pic_MouseUp(null, null);
            }

            if ((Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.V) || (Keyboard.IsKeyDown(Key.RightCtrl) && e.Key == Key.V))
            {
                video_MouseUp(null, null);
            }

            if ((Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.S) || (Keyboard.IsKeyDown(Key.RightCtrl) && e.Key == Key.S))
            {
                vstop_MouseUp(null, null);
            }

            if ((Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.N) || (Keyboard.IsKeyDown(Key.RightCtrl) && e.Key == Key.N))
            {
                lab_o_MouseUp(null, null);
            }

            if ((Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.L) || (Keyboard.IsKeyDown(Key.RightCtrl) && e.Key == Key.L))
            {
                gray_o_MouseUp(null, null);
            }
            if ((Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.F) || (Keyboard.IsKeyDown(Key.RightCtrl) && e.Key == Key.F))
            {
                hsv_o_MouseUp(null, null);
            }
            if ((Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.R) || (Keyboard.IsKeyDown(Key.RightCtrl) && e.Key == Key.R))
            {
                bgr_o_MouseUp(null, null);
            }
            if ((Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.X) || (Keyboard.IsKeyDown(Key.RightCtrl) && e.Key == Key.X))
            {
                camera_text_live_MouseUp(null, null);
            }
            if ((Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.D) || (Keyboard.IsKeyDown(Key.RightCtrl) && e.Key == Key.D))
            {
                DetectFace_MouseUp(null, null);
            }
        }

        #endregion

        #region camear_op
        public void camp_pic()
        {

            t = 1;

            if ((video_start == true) || (face_vid == true))
            {
                if (video_start == true)
                {
                    cap_d.Stop();
                    cap_d.Dispose();
                }

                if (face_vid == true)
                {

                    face_vid_o.icon = char.ConvertFromUtf32(0xF7EE);

                    captureTimer.Stop();
                    videoCapture.Dispose();

                    GetFacesList();

                    vid_cap();

                    captureTimer.Start();

                    camera_color_opetions = "bgr";

                    this.Title = "Near Camera";

                }

            }
            else
            {
                m = true;

                video_take.Isactive = Visibility.Hidden;
                vpo.Visibility = Visibility.Hidden;
                face_take.Isactive = Visibility.Hidden;
                camera_take.Isactive = Visibility.Visible;
                lblFaceName.Visibility = Visibility.Visible;
                face_take.Visibility = Visibility.Visible;

                imgCamera.Visibility = Visibility.Visible;



                captureTimer.Start();
                videoCapture.Start();

            }
        }

        //DetectFace
        private void DetectFace_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            if (detectedFace == null)
            {
                MessageBox.Show("No face detected.");
                return;
            }
            //Save detected face
            detectedFace = detectedFace.Resize(100, 100, Inter.Cubic);
            detectedFace.Save(Config.FacePhotosPath + "face" + (faceList.Count + 1) + Config.ImageFileExtension);
            StreamWriter writer = new StreamWriter(Config.FaceListTextFile, true);
            string personName = Microsoft.VisualBasic.Interaction.InputBox("Your Name");
            writer.WriteLine(String.Format("face{0}:{1}", (faceList.Count + 1), personName));
            writer.Close();
            GetFacesList();
            MessageBox.Show("Successful.");


        }

        //take picture
        private void pic_MouseUp(object sender, MouseButtonEventArgs e)
        {

            camp_pic();

            Task.Delay(1);

            if (m == true)
            {

                img.Source = imgCamera.Source;

                String filePath = @"C:\Users\" + Path.GetFileName(s) + @"\Pictures\pic" + DateTime.Now.Year.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Second.ToString() + ".png";

                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)imgCamera.Source));
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    encoder.Save(stream);

            }

            m = false;


        }

        //record video
        bool video_start = false;
        private void video_MouseUp(object sender, MouseButtonEventArgs e)
        {

            video_start = true;

            t = 2;

             video_take.Isactive = Visibility.Visible;
             camera_take.Isactive = Visibility.Hidden;
             face_take.Isactive = Visibility.Hidden;


            vpo.Visibility = Visibility.Visible;

            lblFaceName.Visibility = Visibility.Hidden;
            face_take.Visibility = Visibility.Hidden;

            imgCamera.Visibility = Visibility.Hidden;
            captureTimer.Stop();
            videoCapture.Dispose();

            pic_h.Visibility = Visibility.Visible;

            PictureBoxvideo.BringToFront();

            filters_ = new Filters();
            cap_d = new Capture(filters_.VideoInputDevices[0], filters_.AudioInputDevices[0]);
            cap_d.PreviewWindow = PictureBoxvideo;

            lTimer_v.Start();

            Task.Delay(1);

            cap_d.Filename = @"C:\Users\" + Path.GetFileName(s) + @"\Videos\" + DateTime.Now.ToString("hh") + "_" + DateTime.Now.Second.ToString() + ".mp4";
            cap_d.Cue();
            cap_d.Start();

        }


        #endregion

        #region camear mode

        bool face_vid_o_c = false;

        private void open_vid_face_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (face_vid_o_c == false)
            {
                face_vid_o_c = true;

                face_vid_o.icon = char.ConvertFromUtf32(0xE8BB);

                OpenFileDialog openDialog = new OpenFileDialog();
                if (openDialog.ShowDialog().Value == true)
                {
                    face_vid = true;

                    captureTimer.Stop();
                    videoCapture.Dispose();

                    if (iscam_text == true)
                    {
                        videoCapture2.Dispose();
                        imgCamera_text_vid.Visibility = Visibility.Hidden;
                        i2.Visibility = Visibility.Hidden;

                        imgCamera.Visibility = Visibility.Visible;

                        iscam_text = false;

                    }

                    videoCapture = new VideoCapture(openDialog.FileName);
                    captureTimer.Start();
                    this.Title = openDialog.FileName;
                    return;


                }


            }

            if (face_vid_o_c == true)
            {


                face_vid_o.icon = char.ConvertFromUtf32(0xF7EE);

                captureTimer.Stop();
                videoCapture.Dispose();

                GetFacesList();

                vid_cap();

                captureTimer.Start();

                camera_color_opetions = "bgr";

                this.Title = "Camera";

                face_vid_o_c = false;

            }

        }

        bool color_o = false;

        private void camera_color_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (face_vid_o_c == false)
            {
                if (iscam_text == true)
                {

                    MessageBox.Show("you are in Text video Detection");

                }
                else
                {

                    if (color_o == false)
                    {
                        gray_o.Isactiv = Visibility.Visible;
                        hsv_o.Isactiv = Visibility.Visible;
                        lab_o.Isactiv = Visibility.Visible;
                        bgr_o.Isactiv = Visibility.Visible;

                        color_o = true;
                    }
                    else
                    {

                        gray_o.Isactiv = Visibility.Hidden;
                        hsv_o.Isactiv = Visibility.Hidden;
                        lab_o.Isactiv = Visibility.Hidden;
                        bgr_o.Isactiv = Visibility.Hidden;

                        color_o = false;

                    }
                }


            }
            else
            {

                MessageBox.Show("you are in video face detection mode");

            }
        }
       
        private void cam_text_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("15$ for full code --> Contact me on whatsApp to complete the payment process +201124932549");
        }

        bool camtext_ison = false;
        int cameratext_live_c = 0;
        bool cam_text_live_check = false;
        private void camera_text_live_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("15$ for full code --> Contact me on whatsApp to complete the payment process +201124932549");

        }

        private void camera_text_d__MouseUp(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("15$ for full code --> Contact me on whatsApp to complete the payment process +201124932549");

        }

        #endregion

        #region camera color mode

        private void gray_o_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (camtext_ison == true)
            { if_cantext_ison(); }

            captureTimer.Stop();
            videoCapture.Dispose();

            GetFacesList();

            camera_color_opetions = "gray";

            vid_cap();

            captureTimer.Start();

            Task.Delay(5);

            gray_o.Isactiv = Visibility.Hidden;
            hsv_o.Isactiv = Visibility.Hidden;
            lab_o.Isactiv = Visibility.Hidden;
            bgr_o.Isactiv = Visibility.Hidden;

            color_o = false;

        }

        private void hsv_o_MouseUp(object sender, MouseButtonEventArgs e)
        {

            if (camtext_ison == true)
            { if_cantext_ison(); }

            captureTimer.Stop();
            videoCapture.Dispose();



            GetFacesList();

            camera_color_opetions = "hsv";

            vid_cap();

            captureTimer.Start();

            Task.Delay(5);

            gray_o.Isactiv = Visibility.Hidden;
            hsv_o.Isactiv = Visibility.Hidden;
            lab_o.Isactiv = Visibility.Hidden;
            bgr_o.Isactiv = Visibility.Hidden;

            color_o = false;

        }

        private void lab_o_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (camtext_ison == true)
            { if_cantext_ison(); }

            captureTimer.Stop();
            videoCapture.Dispose();



            GetFacesList();

            camera_color_opetions = "lab";

            vid_cap();

            captureTimer.Start();

            Task.Delay(5);

            gray_o.Isactiv = Visibility.Hidden;
            hsv_o.Isactiv = Visibility.Hidden;
            lab_o.Isactiv = Visibility.Hidden;
            bgr_o.Isactiv = Visibility.Hidden;

            color_o = false;
        }

        private void bgr_o_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (camtext_ison == true)
            { if_cantext_ison(); }

            captureTimer.Stop();
            videoCapture.Dispose();

            GetFacesList();

            camera_color_opetions = "bgr";

            vid_cap();

            captureTimer.Start();

            Task.Delay(5);

            gray_o.Isactiv = Visibility.Hidden;
            hsv_o.Isactiv = Visibility.Hidden;
            lab_o.Isactiv = Visibility.Hidden;
            bgr_o.Isactiv = Visibility.Hidden;

            color_o = false;
        }

        #endregion

    }

}
