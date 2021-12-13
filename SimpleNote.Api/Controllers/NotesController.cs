using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleNote.Api.Data;
using SimpleNote.Api.Dtos.Note;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleNote.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public NotesController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IEnumerable<Note>> GetNotes()
        {
            return await _context.Notes.OrderByDescending(x => x.Time).ToListAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Note>> GetNote(int id)
        {
            Note note = await _context.Notes.FindAsync(id);
            if (note == null)
            {
                return NotFound();
            }
            return note;
        }
        [HttpPost]
        public async Task<ActionResult<Note>> PostNote(PostNoteDto postNoteDto)
        {
            if (ModelState.IsValid)
            {
                Note note = new() { Content = postNoteDto.Content, Title = postNoteDto.Title };
                _context.Add(note);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetNote), new { id = note.Id }, note);
            }
            return BadRequest(ModelState);
        }
        // PUT: api/Notes/3
        [HttpPut("{id}")]
        public async Task<ActionResult<Note>> PutNote(int id, PutNoteDto putNoteDto)
        {
            if (id != putNoteDto.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                Note note = await _context.Notes.FindAsync(id);
                if (note is null) return NotFound();

                note.Title = putNoteDto.Title;
                note.Content = putNoteDto.Content;
                note.Time = DateTime.Now;
                await _context.SaveChangesAsync();
                return note;

            }
            return BadRequest(ModelState);
        }
    }
}
