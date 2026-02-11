using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentCRUD.Data;
using StudentCRUD.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace StudentCRUD.Controllers
{
    public class StudentsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public StudentsController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            return View(await _context.Students.ToListAsync());
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
        {
            if (ModelState.IsValid)
            {
                // IMAGE SAVE
                if (student.ImageFile != null)
                {
                    string folder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(student.ImageFile.FileName);

                    string filePath = Path.Combine(folder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await student.ImageFile.CopyToAsync(stream);
                    }

                    student.ImagePath = fileName;
                }

                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }


        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // old data nikal lo
                    var studentFromDb = await _context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);

                    // agar nayi image upload hui
                    if (student.ImageFile != null)
                    {
                        string folder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(student.ImageFile.FileName);

                        string filePath = Path.Combine(folder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await student.ImageFile.CopyToAsync(stream);
                        }

                        // purani image delete
                        if (!string.IsNullOrEmpty(studentFromDb.ImagePath))
                        {
                            var oldImage = Path.Combine(folder, studentFromDb.ImagePath);
                            if (System.IO.File.Exists(oldImage))
                                System.IO.File.Delete(oldImage);
                        }

                        student.ImagePath = fileName;
                    }
                    else
                    {
                        // agar new image nahi di to purani hi rakho
                        student.ImagePath = studentFromDb.ImagePath;
                    }

                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }


        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                // image delete from folder
                if (!string.IsNullOrEmpty(student.ImagePath))
                {
                    string folder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    string oldImage = Path.Combine(folder, student.ImagePath);

                    if (System.IO.File.Exists(oldImage))
                        System.IO.File.Delete(oldImage);
                }

                _context.Students.Remove(student);
            }


            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }
    }
}
