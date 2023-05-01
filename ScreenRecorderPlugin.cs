using BepInEx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System;
using System.IO;
using System.Threading;

[BepInPlugin("com.example.vam.screenrecorder", "VAM Screen Recorder", "1.0.0")]
public class ScreenRecorderPlugin : BaseUnityPlugin {

    private bool recording = false;
    private string outputFolder = "";
    private string ffmpegPath = "";
    private int frameRate = 30;
    private int duration = 10;

    private void Awake() {
        // Register the key combination for opening the GUI
        Keybind.Add("com.example.vam.screenrecorder.toggle", KeyCode.R, ModifierKey.Control);
    }

    private void Update() {
        // Check if the key combination was pressed
        if (Keybind.Get("com.example.vam.screenrecorder.toggle").IsPressed()) {
            // Toggle the recording flag
            recording = !recording;
            if (recording) {
                // Start recording
                StartRecording();
            } else {
                // Stop recording
                StopRecording();
            }
        }
    }

    private void OnGUI() {
        if (recording) {
            GUI.Label(new Rect(10, 10, 200, 20), "Recording...");
        } else {
            GUI.Label(new Rect(10, 10, 200, 20), "Not recording");
        }
        if (GUI.Button(new Rect(10, 30, 200, 20), "Record")) {
            StartRecording();
        }
        if (GUI.Button(new Rect(10, 50, 200, 20), "Record on next click")) {
            recording = true;
        }
        GUI.Label(new Rect(10, 70, 100, 20), "Filename:");
        outputFolder = GUI.TextField(new Rect(70, 70, 200, 20), outputFolder);
        if (GUI.Button(new Rect(10, 90, 200, 20), "Choose output folder")) {
            outputFolder = StandaloneFileBrowser.OpenFolderPanel("Select output folder", "", false)[0];
        }
        GUI.Label(new Rect(10, 110, 100, 20), "Frame rate:");
        frameRate = int.Parse(GUI.TextField(new Rect(70, 110, 50, 20), frameRate.ToString()));
        GUI.Label(new Rect(10, 130, 100, 20), "Duration:");
        duration = int.Parse(GUI.TextField(new Rect(70, 130, 50, 20), duration.ToString()));
    }

   private void StartRecording()
    {
    // Generate a unique filename
    string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
    string filename = Path.Combine(outputFolder, $"vam_recording_{timestamp}.mp4");

    // Get the ffmpeg executable path
    if (ffmpegPath == "")
    {
        ffmpegPath = Path.Combine(Path.GetDirectoryName(AssemblyLocation), "FFMPEG_EXE", "ffmpeg.exe");
    }

    // Get the screen resolution
    int width = Screen.width;
    int height = Screen.height;

    // Start the ffmpeg process to record the screen
    ProcessStartInfo startInfo = new ProcessStartInfo(ffmpegPath);
    startInfo.Arguments = $"-f gdigrab -framerate {frameRate} -offset_x 0 -offset_y 0 -video_size {width}x{height} -i desktop -c:v libx264 -preset ultrafast -crf 23 \"{filename}\"";
    Process process = new Process();
    process.StartInfo = startInfo;
    process.Start();
}


private void StopRecording() {
    // Stop the ffmpeg process
    foreach (Process process in Process.GetProcessesByName("ffmpeg")) {
        process.Kill();
    }
}
}