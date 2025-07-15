using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TaskPlanner.Models;

namespace TaskPlanner.Models
{
    public class TaskFileStorage
    {
        private readonly string _filePath;
        private readonly object _lock = new object();

        public TaskFileStorage(string filePath)
        {
            _filePath = filePath;
        }

        public List<TaskItem> LoadTasks()
        {
            lock (_lock)
            {
                if (!File.Exists(_filePath))
                    return new List<TaskItem>();
                var json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<List<TaskItem>>(json) ?? new List<TaskItem>();
            }
        }

        public void SaveTasks(List<TaskItem> tasks)
        {
            lock (_lock)
            {
                var json = JsonSerializer.Serialize(tasks);
                File.WriteAllText(_filePath, json);
            }
        }

        public void AddTask(TaskItem task)
        {
            var tasks = LoadTasks();
            tasks.Add(task);
            SaveTasks(tasks);
        }

        public void RemoveTask(Guid id)
        {
            var tasks = LoadTasks();
            var toRemove = tasks.FirstOrDefault(t => t.Id == id);
            if (toRemove != null)
            {
                tasks.Remove(toRemove);
                SaveTasks(tasks);
            }
        }

        public void UpdateTask(TaskItem updatedTask)
        {
            var tasks = LoadTasks();
            var idx = tasks.FindIndex(t => t.Id == updatedTask.Id);
            if (idx >= 0)
            {
                tasks[idx] = updatedTask;
                SaveTasks(tasks);
            }
        }
    }
} 