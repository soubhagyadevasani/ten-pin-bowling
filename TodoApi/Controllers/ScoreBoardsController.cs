﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScoreBoardsController : ControllerBase
    {
        private readonly ScoreBoardContext _context;

        public ScoreBoardsController(ScoreBoardContext context)
        {
            _context = context;
        }

        // GET: api/ScoreBoards
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ScoreBoard>>> GetScoreBoardList()
        {
            return await _context.ScoreBoardList.ToListAsync();
        }

        // GET: api/ScoreBoards/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ScoreBoard>> GetScoreBoard(long id)
        {
            var scoreBoard = await _context.ScoreBoardList.FindAsync(id);

            if (scoreBoard == null)
            {
                return NotFound();
            }

            return scoreBoard;
        }

        // PUT: api/ScoreBoards/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutScoreBoard(long id, ScoreBoard scoreBoard)
        {
            if (id != scoreBoard.PlayerId)
            {
                return BadRequest();
            }

            _context.Entry(scoreBoard).State = EntityState.Modified;

            try
            {
                BowlingGame game = new BowlingGame();
                Frames frames = JsonConvert.DeserializeObject<Frames>(scoreBoard.FramesData);

                scoreBoard.TotalScore = 0;

                for(int frameIndex = 0; frameIndex < frames.frames.Count; frameIndex++)
                {
                    Frame eachframe = new Frame();
                    eachframe.Roll_1 = frames.frames[frameIndex].Roll_1;
                    eachframe.Roll_2 = frames.frames[frameIndex].Roll_2;
                    if (game.isStrike(eachframe.Roll_1, eachframe.Roll_2))
                    {
                        scoreBoard.TotalScore += 10 + frames.frames[frameIndex + 1].Roll_1 + frames.frames[frameIndex + 1].Roll_2;
                    }
                    else if (game.isSpare(eachframe.Roll_1, eachframe.Roll_2))
                    {
                        scoreBoard.TotalScore += 10 + frames.frames[frameIndex + 1].Roll_1;
                    }
                    else
                    {
                        scoreBoard.TotalScore +=  frames.frames[frameIndex].Roll_1 + frames.frames[frameIndex].Roll_2;
                    }

                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScoreBoardExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ScoreBoards
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<ScoreBoard>> PostScoreBoard(ScoreBoard scoreBoard)
        {
            _context.ScoreBoardList.Add(scoreBoard);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetScoreBoard", new { id = scoreBoard.PlayerId }, scoreBoard);
        }

        // DELETE: api/ScoreBoards/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ScoreBoard>> DeleteScoreBoard(long id)
        {
            var scoreBoard = await _context.ScoreBoardList.FindAsync(id);
            if (scoreBoard == null)
            {
                return NotFound();
            }

            _context.ScoreBoardList.Remove(scoreBoard);
            await _context.SaveChangesAsync();

            return scoreBoard;
        }

        private bool ScoreBoardExists(long id)
        {
            return _context.ScoreBoardList.Any(e => e.PlayerId == id);
        }
    }
}
