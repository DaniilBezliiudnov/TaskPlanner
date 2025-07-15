using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaskPlanner.Models;
using Microsoft.Extensions.Localization;

namespace TaskPlanner.Pages;

public class IndexModel : PageModel
{
    private string _storagePath;
    private TaskFileStorage _storage;
    public IStringLocalizer<IndexModel> Localizer { get; }

    public IndexModel(IStringLocalizer<IndexModel> localizer)
    {
        Localizer = localizer;
    }

    public List<TaskItem> Tasks { get; set; }

    [BindProperty]
    public TaskInputModel NewTask { get; set; }

    public class TaskInputModel
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        public DateTime DueDate { get; set; }
    }

    public void OnGet()
    {
        var username = HttpContext.Session.GetString("Username");
        if (string.IsNullOrEmpty(username))
        {
            Response.Redirect("/Login");
            return;
        }
        _storagePath = Path.Combine(Directory.GetCurrentDirectory(), $"tasks-{username}.json");
        _storage = new TaskFileStorage(_storagePath);
        Tasks = _storage.LoadTasks();
    }

    public IActionResult OnPostAdd()
    {
        var username = HttpContext.Session.GetString("Username");
        if (string.IsNullOrEmpty(username))
            return RedirectToPage("Login");
        _storagePath = Path.Combine(Directory.GetCurrentDirectory(), $"tasks-{username}.json");
        _storage = new TaskFileStorage(_storagePath);
        if (!ModelState.IsValid)
        {
            Tasks = _storage.LoadTasks();
            return Page();
        }
        var task = new TaskItem
        {
            Name = NewTask.Name,
            Description = NewTask.Description,
            DueDate = NewTask.DueDate,
            IsCompleted = false
        };
        _storage.AddTask(task);
        return RedirectToPage();
    }

    public IActionResult OnPostRemove(Guid id)
    {
        var username = HttpContext.Session.GetString("Username");
        if (string.IsNullOrEmpty(username))
            return RedirectToPage("Login");
        _storagePath = Path.Combine(Directory.GetCurrentDirectory(), $"tasks-{username}.json");
        _storage = new TaskFileStorage(_storagePath);
        _storage.RemoveTask(id);
        return RedirectToPage();
    }

    public IActionResult OnPostComplete(Guid id)
    {
        var username = HttpContext.Session.GetString("Username");
        if (string.IsNullOrEmpty(username))
            return RedirectToPage("Login");
        _storagePath = Path.Combine(Directory.GetCurrentDirectory(), $"tasks-{username}.json");
        _storage = new TaskFileStorage(_storagePath);
        var tasks = _storage.LoadTasks();
        var task = tasks.Find(t => t.Id == id);
        if (task != null)
        {
            task.IsCompleted = true;
            _storage.UpdateTask(task);
        }
        return RedirectToPage();
    }
}
