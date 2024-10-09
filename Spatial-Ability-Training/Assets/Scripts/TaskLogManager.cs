using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class TaskLogManager
{
    private Dictionary<int, TaskLog> taskLogs = new Dictionary<int, TaskLog>();
    private int currentTaskId = 0;
    private float taskStartTime;
    private int angle;
    private string cube;

    // Call this method when starting a task
    public void StartTask(string cube, int angle)
    {
        this.angle = angle;
        this.cube = cube;
        taskStartTime = Time.time; // Record the start time
        currentTaskId++;
    }

    // Call this method when finishing a task
    public void FinishTask()
    {
        float timeSpent = Time.time - taskStartTime; // Calculate time spent
        TaskLog log = new TaskLog(cube, angle, timeSpent);
        taskLogs.Add(currentTaskId, log); // Store the log
    }

    // Call this method to save logs to a file
    public void SaveLogsToFile()
    {
        #if UNITY_EDITOR
                var folder = Application.streamingAssetsPath;

                if (!Directory.Exists(folder)) 
                    Directory.CreateDirectory(folder);
        #else
                    var folder = Application.persistentDataPath;
        #endif
        string filePath = Path.Combine(folder, $"TaskLogs-{DateTime.Now.ToString().Replace('.', '-').Replace(' ', '-').Replace(':', '-')}.csv");
                
            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                writer.WriteLine("Cube, Angle, Time Spent (s)"); // Write header
                foreach (var log in taskLogs.Values)
                {
                    writer.WriteLine(log.ToString()); // Write each log
                }
            }

            Debug.Log($"CSV file written to \"{filePath}\"");

        #if UNITY_EDITOR
                AssetDatabase.Refresh();
        #endif
    }
    
}
