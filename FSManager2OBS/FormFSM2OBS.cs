using HOVTP;
using OBSWebsocketDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FSManager2OBS
{
    public partial class formFSM2OBS : Form
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();


        protected OBSWebsocket obs;
        private CancellationTokenSource keepAliveTokenSource;
        private readonly int keepAliveInterval = 500;

        public formFSM2OBS()
        {
            InitializeComponent();

            //Initialize obs-component
            obs = new OBSWebsocket();

            obs.Connected += onOBSConnect;
            obs.Disconnected += onDisconnect;

            //obs.CurrentProgramSceneChanged += onCurrentProgramSceneChanged;
            //obs.CurrentSceneCollectionChanged += onSceneCollectionChanged;
            //obs.CurrentProfileChanged += onCurrentProfileChanged;
            //obs.CurrentSceneTransitionChanged += onCurrentSceneTransitionChanged;
            //obs.CurrentSceneTransitionDurationChanged += onCurrentSceneTransitionDurationChanged;

            //obs.StreamStateChanged += onStreamStateChanged;
            //obs.RecordStateChanged += onRecordStateChanged;

            //obs.VirtualcamStateChanged += onVirtualCamStateChanged;

        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Trace("AvslutaToolStripMenuItem_Click");
            Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Trace("OmToolStripMenuItem_Click");
            using (AboutBox.aboutBox ab = new AboutBox.aboutBox())
            {
                ab.ShowDialog();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox1.Items.Add("Ansluter");
            HOVTPServer hovtp = new HOVTPServer();
            hovtp.OnHovtpOdfMessage += hovtp_OnOdfMessage;
            hovtp.OnHovtpClientConnection += hovtp_OnHovtpClientConnection;
            hovtp.StartListen(tbFSMServer.Text, int.Parse(tbFSMPort.Text), false);
        }

        private static void hovtp_OnHovtpClientConnection(string ip, int port)
        {
            //Console.WriteLine("Ansluter");
            //listBox1.Items.Add("hej");
            //listBox1.Items.Add(ip);
            //listBox1.SelectedIndex = listBox1.Items.Count - 1;
            //Console.WriteLine(String.Format("[HOVTP:{0}] Client connected {1}:{2}", DateTime.Now.ToString("HH:mm:ss.ff"), ip, port));
        }

        private void hovtp_OnOdfMessage(HOVTPMessage msg)
        {
            //if (msg.MsgType!=eHovtpMessage.Clock)
            //{
            //    Logger.Log(Logger.Level.Debug, String.Format("[HOVTP Message from FSM] {0}", msg.MsgType.ToString()));
            //}
            //if (msg.MsgType == eHovtpMessage.Current)
            //{
            //    Logger.Log(Logger.Level.Debug, String.Format("[HOVTP Message from FSM] {0}", ((HOVTP.DtCurrent)msg).Competition.ExtendedInfos[1].Pos));
            string status = string.Empty;
            if (msg.MsgType == eHovtpMessage.Current)
            {
                var m = ((HOVTP.DtCurrent)msg).Competition.ExtendedInfos;
                foreach (var i in m)
                {
                    if (i.Code == "STATUS")
                    {
                        status = i.Pos;
                    }
                    if (i.Code == "ICE")
                    {
                        status = i.Value;
                    }
                }
            }
            tbFSMStatus.Invoke((Action)delegate { tbFSMStatus.Text = status; });

            Console.WriteLine(msg.MsgType.ToString());
            listBox1.Invoke((Action)delegate { listBox1.Items.Add(DateTime.Now.ToShortTimeString() + ":" + msg.MsgType.ToString() + "  " + status); });
            listBox1.Invoke((Action)delegate { listBox1.SelectedIndex = listBox1.Items.Count - 1; });
            //}
        }

        private void onOBSConnect(object sender, EventArgs e)
        {
            BeginInvoke((MethodInvoker)(() =>
            {
                tbOBSServer.Enabled = false;
                tbOBSPort.Enabled = false;
                tbOBSPassword.Enabled = false;
                btnOBSConnect.Text = "Disconnect";


                var versionInfo = obs.GetVersion();
                lbOBSVersion.Text = versionInfo.OBSStudioVersion;
                lbOBSWSVersion.Text = versionInfo.PluginVersion;

                //Delay
                tbOnIceDelay.Value = settings.delayOnIce;
                tbStartedDelay.Value = settings.delayStarted;
                tbFinishedDelay.Value = settings.delayFinished;
                tbScoreDelay.Value = settings.delayScore;
                tbWarmupDelay.Value = settings.delayWarmup;
                tbResurfaceDelay.Value = settings.delayResurface;

                //Load transisions
                //Empty checkboxes
                cbOnIceTransition.Items.Clear();
                cbStartedTransition.Items.Clear();
                cbFinishedTransition.Items.Clear();
                cbScoreTransition.Items.Clear();
                cbWarmupTransition.Items.Clear();
                cbResurfaceTransition.Items.Clear();
                //Add blank value (so we can select nothing in the future)
                cbOnIceTransition.Items.Add(string.Empty);
                cbStartedTransition.Items.Add(string.Empty); ;
                cbFinishedTransition.Items.Add(string.Empty);
                cbScoreTransition.Items.Add(string.Empty);
                cbWarmupTransition.Items.Add(string.Empty);
                cbResurfaceTransition.Items.Add(string.Empty);

                var transitions = obs.GetSceneTransitionList();
                foreach (var transition in transitions.Transitions)
                {
                    cbOnIceTransition.Items.Add(transition.Name);
                    cbStartedTransition.Items.Add(transition.Name);
                    cbFinishedTransition.Items.Add(transition.Name);
                    cbScoreTransition.Items.Add(transition.Name);
                    cbWarmupTransition.Items.Add(transition.Name);
                    cbResurfaceTransition.Items.Add(transition.Name);
                }

                //Set transition from settings
                setComboBox(cbOnIceTransition, settings.transitionOnIce);
                setComboBox(cbStartedTransition, settings.transitionStarted);
                setComboBox(cbFinishedTransition, settings.transitionFinished);
                setComboBox(cbScoreTransition, settings.transitionScore);
                setComboBox(cbWarmupTransition, settings.transitionWarmup);
                setComboBox(cbResurfaceTransition, settings.transitionResurface);


                //Duration
                tbOnIceTransitionTime.Value = settings.durationOnIce;
                tbStartedTransitionTime.Value = settings.durationStarted;
                tbFinishedTransitionTime.Value = settings.durationFinished;
                tbScoreTransitionTime.Value = settings.durationScore;
                tbWarmupTransitionTime.Value = settings.durationWarmup;
                tbResurfaceTransitionTime.Value = settings.durationResurface;


                //Load Scenes
                //Empty checkboxes
                cbOnIceScene.Items.Clear();
                cbStartedScene.Items.Clear();
                cbFinishedScene.Items.Clear();
                cbScoreScene.Items.Clear();
                cbWarmupScene.Items.Clear();
                cbResurfaceScene.Items.Clear();
                //Add blank value (so we can select nothing in the future)
                cbOnIceScene.Items.Add(string.Empty);
                cbStartedScene.Items.Add(string.Empty);
                cbFinishedScene.Items.Add(string.Empty);
                cbScoreScene.Items.Add(string.Empty);
                cbWarmupScene.Items.Add(string.Empty);
                cbResurfaceScene.Items.Add(string.Empty);
                var scenes = obs.ListScenes();
                foreach (var scene in scenes)
                {
                    cbOnIceScene.Items.Add(scene.Name);
                    cbStartedScene.Items.Add(scene.Name);
                    cbFinishedScene.Items.Add(scene.Name);
                    cbScoreScene.Items.Add(scene.Name);
                    cbWarmupScene.Items.Add(scene.Name);
                    cbResurfaceScene.Items.Add(scene.Name);
                }

                //Set scenes from settings
                setComboBox(cbOnIceScene, settings.sceneOnIce);
                setComboBox(cbStartedScene, settings.sceneStarted);
                setComboBox(cbFinishedScene, settings.sceneFinished);
                setComboBox(cbScoreScene, settings.sceneScore);
                setComboBox(cbWarmupScene, settings.sceneWarmup);
                setComboBox(cbResurfaceScene, settings.sceneResurface);



                //    btnListScenes.PerformClick();
                //    btnGetCurrentScene.PerformClick();

                //    btnListSceneCol.PerformClick();
                //    btnGetCurrentSceneCol.PerformClick();

                //    btnListProfiles.PerformClick();
                //    btnGetCurrentProfile.PerformClick();

                //    btnListTransitions.PerformClick();
                //    btnGetCurrentTransition.PerformClick();

                //    btnGetTransitionDuration.PerformClick();
                //    tbFolderPath.Text = obs.GetRecordDirectory().ToString();

                //    var streamStatus = obs.GetStreamStatus();
                //    if (streamStatus.IsActive)
                //    {
                //        onStreamStateChanged(obs, new StreamStateChangedEventArgs(new OutputStateChanged() { IsActive = true, StateStr = nameof(OutputState.OBS_WEBSOCKET_OUTPUT_STARTED) }));
                //    }
                //    else
                //    {
                //        onStreamStateChanged(obs, new StreamStateChangedEventArgs(new OutputStateChanged() { IsActive = false, StateStr = nameof(OutputState.OBS_WEBSOCKET_OUTPUT_STOPPED) }));
                //    }

                //    var recordStatus = obs.GetRecordStatus();
                //    if (recordStatus.IsRecording)
                //    {
                //        onRecordStateChanged(obs, new RecordStateChangedEventArgs(new RecordStateChanged() { IsActive = true, StateStr = nameof(OutputState.OBS_WEBSOCKET_OUTPUT_STARTED) }));
                //    }
                //    else
                //    {
                //        onRecordStateChanged(obs, new RecordStateChangedEventArgs(new RecordStateChanged() { IsActive = false, StateStr = nameof(OutputState.OBS_WEBSOCKET_OUTPUT_STOPPED) }));
                //    }

                //    var camStatus = obs.GetVirtualCamStatus();
                //    if (camStatus.IsActive)
                //    {
                //        onVirtualCamStateChanged(this, new VirtualcamStateChangedEventArgs(new OutputStateChanged() { IsActive = true, StateStr = nameof(OutputState.OBS_WEBSOCKET_OUTPUT_STARTED) }));
                //    }
                //    else
                //    {
                //        onVirtualCamStateChanged(this, new VirtualcamStateChangedEventArgs(new OutputStateChanged() { IsActive = false, StateStr = nameof(OutputState.OBS_WEBSOCKET_OUTPUT_STOPPED) }));
                //    }

                //    keepAliveTokenSource = new CancellationTokenSource();
                //    CancellationToken keepAliveToken = keepAliveTokenSource.Token;
                //    Task statPollKeepAlive = Task.Factory.StartNew(() =>
                //    {
                //        while (true)
                //        {
                //            Thread.Sleep(keepAliveInterval);
                //            if (keepAliveToken.IsCancellationRequested)
                //            {
                //                break;
                //            }

                //            BeginInvoke((MethodInvoker)(() =>
                //            {
                //                switch (tabStats.SelectedIndex)
                //                {
                //                    case 0: // OBS
                //                        var stats = obs.GetStats();
                //                        UpdateOBSStats(stats);
                //                        break;
                //                    case 1: // Stream
                //                        var streamStats = obs.GetStreamStatus();
                //                        UpdateStreamStats(streamStats);
                //                        break;

                //                    case 2: // Recording
                //                        var recStats = obs.GetRecordStatus();
                //                        UpdateRecordingStats(recStats);
                //                        break;
                //                }
                //            }));



                //        }
                //    }, keepAliveToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }));
        }

        private static void setComboBox(ComboBox cb, string sceneName)
        {
            cb.SelectedIndex = cb.FindStringExact(sceneName);
        }

        private void onDisconnect(object sender, OBSWebsocketDotNet.Communication.ObsDisconnectionInfo e)
        {
            BeginInvoke((MethodInvoker)(() =>
            {
                //if (keepAliveTokenSource != null)
                //{
                //    keepAliveTokenSource.Cancel();
                //}
                //gbControls.Enabled = false;

                tbOBSServer.Enabled = true;
                tbOBSPort.Enabled = true;
                tbOBSPassword.Enabled = true;
                btnOBSConnect.Text = "Connect";
                lbOBSVersion.Text = "?.?.?";
                lbOBSWSVersion.Text = "?.?.?";

                if (e.ObsCloseCode == OBSWebsocketDotNet.Communication.ObsCloseCodes.AuthenticationFailed)
                {
                    MessageBox.Show("Authentication failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                else if (e.WebsocketDisconnectionInfo != null)
                {
                    if (e.WebsocketDisconnectionInfo.Exception != null)
                    {
                        MessageBox.Show($"Connection failed: CloseCode: {e.ObsCloseCode} Desc: {e.WebsocketDisconnectionInfo?.CloseStatusDescription} Exception:{e.WebsocketDisconnectionInfo?.Exception?.Message}\nType: {e.WebsocketDisconnectionInfo.Type}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    else
                    {
                        MessageBox.Show($"Connection failed: CloseCode: {e.ObsCloseCode} Desc: {e.WebsocketDisconnectionInfo?.CloseStatusDescription}\nType: {e.WebsocketDisconnectionInfo.Type}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                else
                {
                    MessageBox.Show($"Connection failed: CloseCode: {e.ObsCloseCode}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }));

        }

        private void btnOBSConnect_Click(object sender, EventArgs e)
        {
            if (!obs.IsConnected)
            {
                System.Threading.Tasks.Task.Run(() =>
                {
                    try
                    {
                        obs.ConnectAsync(tbOBSServer.Text + ":" + tbOBSPort.Text, tbOBSPassword.Text);
                    }
                    catch (Exception ex)
                    {
                        BeginInvoke((MethodInvoker)delegate
                        {
                            MessageBox.Show("Connect failed : " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        });
                    }
                });
            }
            else
            {
                obs.Disconnect();
            }
        }

        private async void switchScene(string scene)
        {
            decimal delay = (gbOBS.Controls["tb" + scene + "Delay"] as NumericUpDown).Value;
            string transition = (gbOBS.Controls["cb" + scene + "Transition"] as ComboBox).Text;
            decimal duration = (gbOBS.Controls["tb" + scene + "TransitionTime"] as NumericUpDown).Value;
            string programScene = (gbOBS.Controls["cb" + scene + "Scene"] as ComboBox).Text;

            await Task.Delay((int)delay);
            if (transition != string.Empty)
            {
                obs.SetCurrentSceneTransition(transition);
                obs.SetCurrentSceneTransitionDuration((int)duration);
            }
            if (programScene != string.Empty)
            {
                obs.SetCurrentProgramScene(programScene);
            }
        }

        private void btnOnIce_Click(object sender, EventArgs e)
        {
            switchScene("OnIce");
            //await Task.Delay((int)tbOnIceDelay.Value);
            //obs.SetCurrentSceneTransition(cbOnIceTransition.Text);
            //obs.SetCurrentSceneTransitionDuration((int)tbOnIceTransitionTime.Value);
            //obs.SetCurrentProgramScene(cbOnIceScene.Text);
        }

        private void btnStarted_Click(object sender, EventArgs e)
        {
            //Start replay buffer if not already started
            //if (!obs.GetReplayBufferStatus())
            //{
            //    obs.StartReplayBuffer();
            //}

            switchScene("Started");
            //await Task.Delay((int)tbStartedDelay.Value);
            //obs.SetCurrentSceneTransition(cbStartedTransition.Text);
            //obs.SetCurrentSceneTransitionDuration((int)tbStartedTransitionTime.Value);
            //obs.SetCurrentProgramScene(cbStartedScene.Text);
        }

        private void btnFinished_Click(object sender, EventArgs e)
        {
            if (obs.GetReplayBufferStatus())
            {
                obs.SaveReplayBuffer();
                //obs.StopReplayBuffer();
            }
            switchScene("Finished");

            //await Task.Delay((int)tbFinishedDelay.Value);
            //obs.SetCurrentSceneTransition(cbFinishedTransition.Text);
            //obs.SetCurrentSceneTransitionDuration((int)tbFinishedTransitionTime.Value);
            //obs.SetCurrentProgramScene(cbFinishedScene.Text);
        }

        private void btnScore_Click(object sender, EventArgs e)
        {
            switchScene("Score");

            //await Task.Delay((int)tbScoreDelay.Value);
            //obs.SetCurrentSceneTransition(cbScoreTransition.Text);
            //obs.SetCurrentSceneTransitionDuration((int)tbScoreTransitionTime.Value);
            //obs.SetCurrentProgramScene(cbScoreScene.Text);
        }

        private void btnWarmup_Click(object sender, EventArgs e)
        {
            switchScene("Warmup");
            //await Task.Delay((int)tbWarmupDelay.Value);
            //obs.SetCurrentSceneTransition(cbWarmupTransition.Text);
            //obs.SetCurrentSceneTransitionDuration((int)tbWarmupTransitionTime.Value);
            //obs.SetCurrentProgramScene(cbWarmupScene.Text);
        }

        private void btnResurface_Click(object sender, EventArgs e)
        {
            switchScene("Resurface");
            //await Task.Delay((int)tbResurfaceDelay.Value);
            //obs.SetCurrentSceneTransition(cbResurfaceTransition.Text);
            //obs.SetCurrentSceneTransitionDuration((int)tbResurfaceTransitionTime.Value);
            //obs.SetCurrentProgramScene(cbResurfaceScene.Text);
        }

        private void tbFSMStatus_TextChanged(object sender, EventArgs e)
        {
            switch (tbFSMStatus.Text)
            {
                case "ON_ICE": switchScene("OnIce"); break;
                case "STARTED": switchScene("Started"); break;
                case "FINISHED": switchScene("Finished"); break;
                case "SCORE": switchScene("Score"); break;
                case "WARMUP": switchScene("Warmup");break;
                case "RESURFACE": switchScene("Resurface"); break;
                default:
                    break;
            }
        }
    }
}
