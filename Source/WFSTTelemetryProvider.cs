//MIT License
//
//Copyright(c) 2019 PHARTGAMES
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
//
using SimFeedback.log;
using SimFeedback.telemetry;
using System;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Numerics;
using NoiseFilters;
using Sojaner.MemoryScanner;
using System.IO;

namespace WFSTTelemetry
{
    /// <summary>
    /// Wreckfest Telemetry Provider
    /// </summary>
    public sealed class WFSTTelemetryProvider : AbstractTelemetryProvider
    {
        private bool isStopped = true;                                  // flag to control the polling thread
        private Thread t;
        int memoryAddress;
        RegularMemoryScan scan;
        Process wfstProcess = null;
        ScanDialog scanDialog = null;

        /// <summary>
        /// Default constructor.
        /// Every TelemetryProvider needs a default constructor for dynamic loading.
        /// Make sure to call the underlying abstract class in the constructor.
        /// </summary>
        public WFSTTelemetryProvider() : base()
        {
            Author = "PEZZALUCIFER";
            Version = "v1.0";
            BannerImage = @"img\banner_WFST.png"; // Image shown on top of the profiles tab
            IconImage = @"img\WFST.jpg";  // Icon used in the tree view for the profile
            TelemetryUpdateFrequency = 100;     // the update frequency in samples per second
        }

        /// <summary>
        /// Name of this TelemetryProvider.
        /// Used for dynamic loading and linking to the profile configuration.
        /// </summary>
        public override string Name { get { return "wfst"; } }

        public override void Init(ILogger logger)
        {
            base.Init(logger);
            Log("Initializing WFSTTelemetryProvider");
        }

        /// <summary>
        /// A list of all telemetry names of this provider.
        /// </summary>
        /// <returns>List of all telemetry names</returns>
        public override string[] GetValueList()
        {
            return GetValueListByReflection(typeof(WFSTAPI));
        }

        public void ScanButtonClicked(object sender, EventArgs e)
        {
            Process[] processes = Process.GetProcesses();
                
            foreach (Process process in processes)
            {
                if (process.ProcessName.Contains("Wreckfest") && !process.ProcessName.Contains("64"))
                    wfstProcess = process;
            }

            if (wfstProcess == null ) //no processes, better stop
            {
                scanDialog.StatusLabel.Text = "32 bit Wreckfest not running!";
                return;
            }

            scanDialog.StatusLabel.Text = "Please Wait";
            scanDialog.ExecutableText.Text = "Found process " + wfstProcess.ProcessName + ".exe" ;
            scanDialog.progressBar1.Value = 0;
            scanDialog.ScanButton.Enabled = false;

            scan = new RegularMemoryScan(wfstProcess, 0, 2147483647);
            scan.ScanProgressChanged += new RegularMemoryScan.ScanProgressedEventHandler(scan_ScanProgressChanged);
            scan.ScanCompleted += new RegularMemoryScan.ScanCompletedEventHandler(scan_ScanCompleted);
            scan.ScanCanceled += new RegularMemoryScan.ScanCanceledEventHandler(scan_ScanCanceled);

            scan.StartScanForString("carRootNode00");
        }

        /// <summary>
        /// Start the polling thread
        /// </summary>
        public override void Start()
        {
            if (isStopped)
            {
                LogDebug("Starting WFSTTelemetryProvider");

                if(scanDialog != null)
                {
                    scanDialog.Close();
                    scanDialog = null;
                }

                if (scanDialog == null)
                {
                    scanDialog = new ScanDialog();
                    scanDialog.onButtonClicked = ScanButtonClicked;
                    scanDialog.StatusLabel.Text = "Click Initialize!";

                    Thread x = new Thread(new ParameterizedThreadStart(ShowForm));
                    x.Start(scanDialog);
                }
            }
        }

        void TelemetryLost()
        {
            if(scanDialog != null)
            {
                scanDialog.StatusLabel.Text = "Telemetry Lost";
                scanDialog.progressBar1.Value = 0;
            }
        }

        void ShowForm(object newForm)
        {
            ((ScanDialog)newForm).ShowDialog();
        }

        /// <summary>
        /// Stop the polling thread
        /// </summary>
        public override void Stop()
        {
            LogDebug("Stopping WFSTTelemetryProvider");
            isStopped = true;

            if (scanDialog != null)
            {
                scanDialog.Close();
                scanDialog = null;
            }

            if (t != null) t.Join();
        }

        static float transformLerp = 0.25f;
        /// <summary>
        /// The thread funktion to poll the telemetry data and send TelemetryUpdated events.
        /// </summary>
        private void Run()
        {
            isStopped = false;

            WFSTAPI lastTelemetryData = new WFSTAPI();
            lastTelemetryData.Reset();
            Matrix4x4 lastTransform = Matrix4x4.Identity;
            bool lastFrameValid = false;
            Vector3 lastVelocity = Vector3.Zero;
            float lastYaw = 0.0f;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            KalmanFilter accXFilter = new KalmanFilter(1, 1, 0.02f, 1, 0.1f, 0.0f);
            KalmanFilter accYFilter = new KalmanFilter(1, 1, 0.02f, 1, 0.1f, 0.0f);
            KalmanFilter accZFilter = new KalmanFilter(1, 1, 0.02f, 1, 0.1f, 0.0f);
  /*          
                        NoiseFilter accXSmooth = new NoiseFilter(6, 0.5f);
                        NoiseFilter accYSmooth = new NoiseFilter(6, 0.5f);
                        NoiseFilter accZSmooth = new NoiseFilter(6, 0.5f);
    */        
            
            NestedSmooth accXSmooth = new NestedSmooth( 3, 6, 0.5f );
            NestedSmooth accYSmooth = new NestedSmooth( 3, 6, 0.5f );
            NestedSmooth accZSmooth = new NestedSmooth( 3, 6, 0.5f );
            
            KalmanFilter velXFilter = new KalmanFilter(1, 1, 0.02f, 1, 0.02f, 0.0f);
            KalmanFilter velZFilter = new KalmanFilter(1, 1, 0.02f, 1, 0.02f, 0.0f);

            NoiseFilter velXSmooth = new NoiseFilter(6, 0.5f);
            NoiseFilter velZSmooth = new NoiseFilter(6, 0.5f);

            KalmanFilter yawRateFilter = new KalmanFilter(1, 1, 0.02f, 1, 0.02f, 0.0f);
            NoiseFilter yawRateSmooth = new NoiseFilter(6, 0.5f);

            NoiseFilter pitchFilter = new NoiseFilter(3);
            NoiseFilter rollFilter = new NoiseFilter(3);
            NoiseFilter yawFilter = new NoiseFilter(3);

            KalmanFilter posXFilter = new KalmanFilter( 1, 1, 0.02f, 1, 0.1f, 0.0f );
            KalmanFilter posYFilter = new KalmanFilter( 1, 1, 0.02f, 1, 0.1f, 0.0f );
            KalmanFilter posZFilter = new KalmanFilter( 1, 1, 0.02f, 1, 0.1f, 0.0f );

            NestedSmooth posXSmooth = new NestedSmooth( 12, 6, 0.5f );
            NestedSmooth posYSmooth = new NestedSmooth( 12, 6, 0.5f );
            NestedSmooth posZSmooth = new NestedSmooth( 12, 6, 0.5f );



            NoiseFilter slipAngleSmooth = new NoiseFilter(6, 0.25f);

            ProcessMemoryReader reader = new ProcessMemoryReader();

            reader.ReadProcess = wfstProcess;
            uint readSize = 4 * 4 * 4;
            byte[] readBuffer = new byte[readSize];
            reader.OpenProcess();

            while (!isStopped)
            {
                try
                {
                    float dt = (float)sw.ElapsedMilliseconds / 1000.0f;


                    int byteReadSize;
                    reader.ReadProcessMemory((IntPtr)memoryAddress, readSize, out byteReadSize, readBuffer);


                    if (byteReadSize == 0)
                    {
                        continue;
                    }

                    float[] floats = new float[4 * 4];

                    Buffer.BlockCopy(readBuffer, 0, floats, 0, readBuffer.Length);

                    Matrix4x4 transform = new Matrix4x4(floats[0], floats[1], floats[2], floats[3]
                                                        , floats[4], floats[5], floats[6], floats[7]
                                                        , floats[8], floats[9], floats[10], floats[11]
                                                        , floats[12], floats[13], floats[14], floats[15]);



                    Vector3 rht = new Vector3(transform.M11, transform.M12, transform.M13);
                    Vector3 up = new Vector3(transform.M21, transform.M22, transform.M23);
                    Vector3 fwd = new Vector3(transform.M31, transform.M32, transform.M33);

                    float rhtMag = rht.Length();
                    float upMag = up.Length();
                    float fwdMag = fwd.Length();

                    //reading garbage
                    if (rhtMag < 0.9f || upMag < 0.9f || fwdMag < 0.9f)
                    {
                        IsConnected = false;
                        IsRunning = false;
                        TelemetryLost();
                        break;
                    }
                    /*
                    rht = Vector3.Normalize( rht );
                    up = Vector3.Normalize( up );
                    fwd = Vector3.Normalize( fwd );

                    transform.M11 = rht.X;
                    transform.M12 = rht.Y;
                    transform.M13 = rht.Z;

                    transform.M21 = up.X;
                    transform.M22 = up.Y;
                    transform.M23 = up.Z;

                    transform.M31 = fwd.X;
                    transform.M32 = fwd.Y;
                    transform.M33 = fwd.Z;
                    */
                    if ( !lastFrameValid)
                    {
                        lastTransform = transform;
                        lastFrameValid = true;
                        lastVelocity = Vector3.Zero;
                        lastYaw = 0.0f;
                        continue;
                    }

                    WFSTAPI telemetryData = new WFSTAPI();

                    if (dt <= 0)
                        dt = 1.0f;

                    //smooth translation
                    //                    transform.Translation = Vector3.Lerp( lastTransform.Translation, transform.Translation, transformLerp );


                    //                    transform.Translation = new Vector3( posXFilter.Filter( transform.Translation.X ), posYFilter.Filter( transform.Translation.Y ), posZFilter.Filter( transform.Translation.Z ) );
                    //transform.Translation = new Vector3( posXSmooth.Filter( transform.Translation.X ), posYSmooth.Filter( transform.Translation.Y ), posZSmooth.Filter( transform.Translation.Z ) );


                    //                    Debug.WriteLine( "Last World Z = " + lastTransform.Translation.Z );
                    //                    Debug.WriteLine( "Curr World Z = " + transform.Translation.Z );

                    Vector3 worldVelocity = ( transform.Translation - lastTransform.Translation ) / dt;
                    lastTransform = transform;

//                    Debug.WriteLine( "deltaTime = " + dt );
//                    Debug.WriteLine( "World Vel Z = " + worldVelocity.Z);
//                    Debug.WriteLine( "-------------------------------------------------------------------------" );


                    Matrix4x4 rotation = new Matrix4x4();
                    rotation = transform;
                    rotation.M41 = 0.0f;
                    rotation.M42 = 0.0f;
                    rotation.M43 = 0.0f;

                    Matrix4x4 rotInv = new Matrix4x4();
                    Matrix4x4.Invert(rotation, out rotInv);
                                       
                    Vector3 localVelocity = Vector3.Transform(worldVelocity, rotInv);

                    telemetryData.velX = worldVelocity.X;
                    telemetryData.velZ = worldVelocity.Z;

                    Vector3 localAcceleration = localVelocity - lastVelocity;
                    lastVelocity = localVelocity;


                    telemetryData.accX = localAcceleration.X * 10.0f;
                    telemetryData.accY = localAcceleration.Y * 100.0f;
                    telemetryData.accZ = localAcceleration.Z * 10.0f;


                    double pitch = Math.Asin(-fwd.Y);
                    double yaw = Math.Atan2(fwd.X, fwd.Z);

                    double planeRightX = Math.Sin(yaw);
                    double planeRightZ = -Math.Cos(yaw);

                    // Roll is the rightward lean of our up vector, computed here using a dot product.
                    double roll = Math.Asin(up.Z * planeRightX + up.X * planeRightZ);
                    // If we're twisted upside-down, return a roll in the range +-(pi/2, pi)
                    if (up.Y < 0)
                        roll = Math.Sign(roll) * Math.PI - roll;

                    telemetryData.pitchPos = (float)pitch;
                    telemetryData.yawPos = (float)yaw;
                    telemetryData.rollPos = (float)roll;

                    telemetryData.yawRate = CalculateAngularChange(lastYaw, (float)yaw) * (180.0f / (float)Math.PI);
                    lastYaw = (float)yaw;

                    // otherwise we are connected
                    IsConnected = true;

                    if(IsConnected)
                    { 
                        IsRunning = true;


                        WFSTAPI telemetryToSend = new WFSTAPI();
                        telemetryToSend.Reset();

                        telemetryToSend.CopyFields(telemetryData);
                        /*
                                                //accXSmooth.Filter(accXFilter.Filter(telemetryData.accX));
                                                //telemetryToSend.accY = accYSmooth.Filter(accYFilter.Filter(telemetryData.accY));
                                                //telemetryToSend.accZ = accZSmooth.Filter(accZFilter.Filter(telemetryData.accZ));
                        */
                        telemetryToSend.accX = accXSmooth.Filter( telemetryData.accX );
                        telemetryToSend.accY = accYSmooth.Filter( telemetryData.accY );
                        telemetryToSend.accZ = accZSmooth.Filter( telemetryData.accZ );


                        telemetryToSend.pitchPos = pitchFilter.Filter(telemetryData.pitchPos);
                        telemetryToSend.rollPos = rollFilter.Filter(telemetryData.rollPos);
                        telemetryToSend.yawPos = yawFilter.Filter(telemetryData.yawPos);

                        telemetryToSend.velX = velXSmooth.Filter(velXFilter.Filter(telemetryData.velX));
                        telemetryToSend.velZ = velZSmooth.Filter(velZFilter.Filter(telemetryData.velZ));

                        telemetryToSend.yawRate = yawRateSmooth.Filter(yawRateFilter.Filter(telemetryData.yawRate));

                        telemetryToSend.yawAcc = slipAngleSmooth.Filter(telemetryToSend.CalculateSlipAngle());

                        sw.Restart();

                        TelemetryEventArgs args = new TelemetryEventArgs(
                            new WFSTTelemetryInfo(telemetryToSend, lastTelemetryData));
                        RaiseEvent(OnTelemetryUpdate, args);

                        lastTelemetryData = telemetryToSend;
                        Thread.Sleep(1000/30);
                    }
                    else if (sw.ElapsedMilliseconds > 500)
                    {
                        IsRunning = false;
                    }
                }
                catch (Exception e)
                {
                    LogError("WFSTTelemetryProvider Exception while processing data", e);
                    IsConnected = false;
                    IsRunning = false;
                    Thread.Sleep(1000);
                }

            }

            IsConnected = false;
            IsRunning = false;
            reader.CloseHandle();

        }

        float CalculateAngularChange(float sourceA, float targetA)
        {
            sourceA *= (180.0f / (float)Math.PI);
            targetA *= (180.0f / (float)Math.PI);

            float a = targetA - sourceA;
            a = (a + 180) % 360 - 180;

            return a * ((float)Math.PI / 180.0f);
        }
       

        void scan_ScanCanceled(object sender, ScanCanceledEventArgs e)
        {
            scanDialog.ScanButton.Enabled = true;
        }

        delegate void Completed(object sender, ScanCompletedEventArgs e);
        void scan_ScanCompleted(object sender, ScanCompletedEventArgs e)
        {
            scanDialog.ScanButton.Enabled = true;

            if (e.MemoryAddresses == null || e.MemoryAddresses.Length == 0)
            {
                scanDialog.StatusLabel.Text = "Failed!";
                return;
            }

            memoryAddress = e.MemoryAddresses[0] - ((4 * 4 * 4) + 4); //offset backwards from found address to start of matrix
            scanDialog.StatusLabel.Text = "Success!";

            t = new Thread(Run);
            t.Start();
        }


        delegate void Progress(object sender, ScanProgressChangedEventArgs e);
        void scan_ScanProgressChanged(object sender, ScanProgressChangedEventArgs e)
        {
            scanDialog.progressBar1.Value = e.Progress;
        }


    }


}
