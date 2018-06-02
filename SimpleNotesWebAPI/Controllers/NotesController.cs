using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using SimpleNotesWebAPI.Models;

namespace SimpleNotesWebAPI.Controllers
{
    public class NotesController : ApiController
    {
        private SimpleNotesEntities db = new SimpleNotesEntities();

        // GET: api/Notes
        [HttpGet]
        [Route("api/notes")]
        public IEnumerable<SimpleNotesModel> GetSimpleNotes()
        {
            try
            {
                var notes = new List<SimpleNotesModel>();

                foreach(SimpleNotes s in db.SimpleNotes)
                {
                    SimpleNotesModel note = new SimpleNotesModel()
                    {
                        Note_ID = s.Note_ID,
                        Title = s.Title,
                        Note = s.Note
                    };
                    notes.Add(note);
                }
                return notes;
            }
            catch (Exception)
            {
                return null;
            }

        }

        // GET: api/Notes/5
        [HttpGet]
        [Route("api/notes/{id}")]
        [ResponseType(typeof(SimpleNotesModel))]
        public async Task<IHttpActionResult> GetSimpleNotes(int id)
        {
            try
            {
                var sn = await db.SimpleNotes.FindAsync(id);
                return Ok(new SimpleNotesModel()
                {
                    Note_ID = sn.Note_ID,
                    Title = sn.Title,
                    Note = sn.Note
                });
            }
            catch
            {
                return NotFound();
            }
        }

        // PUT: api/Notes/5
        [HttpPut]
        [Route("api/notes/{id}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSimpleNotes(int id, SimpleNotes simpleNotes)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != simpleNotes.Note_ID)
            {
                return BadRequest();
            }

            db.Entry(simpleNotes).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SimpleNotesExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Notes
        [HttpPost]
        [Route("api/notes", Name = "POSTRoute")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PostSimpleNotes(SimpleNotes simpleNotes)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.SimpleNotes.Add(simpleNotes);
            await db.SaveChangesAsync();

            return CreatedAtRoute("POSTRoute", new { id = simpleNotes.Note_ID }, simpleNotes);
        }

        // DELETE: api/Notes/5
        [HttpDelete]
        [Route("api/notes/{id}")]
        [ResponseType(typeof(SimpleNotes))]
        public async Task<IHttpActionResult> DeleteSimpleNotes(int id)
        {
            SimpleNotes simpleNotes = await db.SimpleNotes.FindAsync(id);
            if (simpleNotes == null)
            {
                return NotFound();
            }

            db.SimpleNotes.Remove(simpleNotes);
            await db.SaveChangesAsync();

            return Ok(simpleNotes);
        }

        // DELETE: api/deleteallnotes
        [HttpDelete]
        [Route("api/deleteallnotes")]
        [ResponseType(typeof(SimpleNotes))]
        public async Task<IHttpActionResult> DeleteAllSimpleNotes()
        {
            using (var db = new SimpleNotesEntities())
            {
                int noOfRowDeleted = db.Database.ExecuteSqlCommand("delete from SimpleNotes");
                await db.SaveChangesAsync();
            }
            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SimpleNotesExists(int id)
        {
            return db.SimpleNotes.Count(e => e.Note_ID == id) > 0;
        }
    }
}