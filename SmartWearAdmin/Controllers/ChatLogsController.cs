using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Business.Data;
using Business.Models;

namespace SmartWearAdmin.Controllers
{
    public class ChatLogsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChatLogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ChatLogs
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ChatLogs.Include(c => c.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ChatLogs/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chatLog = await _context.ChatLogs
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chatLog == null)
            {
                return NotFound();
            }

            return View(chatLog);
        }

        // GET: ChatLogs/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email");
            return View();
        }

        // POST: ChatLogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,UserQuestion,BotResponse,Id,CreatedOn,ModifiedOn,IsDeleted,DeletedOn")] ChatLog chatLog)
        {
            if (ModelState.IsValid)
            {
                chatLog.Id = Guid.NewGuid();
                _context.Add(chatLog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", chatLog.UserId);
            return View(chatLog);
        }

        // GET: ChatLogs/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chatLog = await _context.ChatLogs.FindAsync(id);
            if (chatLog == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", chatLog.UserId);
            return View(chatLog);
        }

        // POST: ChatLogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("UserId,UserQuestion,BotResponse,Id,CreatedOn,ModifiedOn,IsDeleted,DeletedOn")] ChatLog chatLog)
        {
            if (id != chatLog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chatLog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChatLogExists(chatLog.Id))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", chatLog.UserId);
            return View(chatLog);
        }

        // GET: ChatLogs/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chatLog = await _context.ChatLogs
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chatLog == null)
            {
                return NotFound();
            }

            return View(chatLog);
        }

        // POST: ChatLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var chatLog = await _context.ChatLogs.FindAsync(id);
            if (chatLog != null)
            {
                _context.ChatLogs.Remove(chatLog);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChatLogExists(Guid id)
        {
            return _context.ChatLogs.Any(e => e.Id == id);
        }
    }
}
