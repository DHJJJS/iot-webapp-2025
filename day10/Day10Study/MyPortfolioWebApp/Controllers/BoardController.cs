using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyPortfolioWebApp.Models;

namespace MyPortfolioWebApp.Controllers
{
    public class BoardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BoardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Board - 페이징과 검색 기능 포함
        public async Task<IActionResult> Index(int page = 1, string search = "")
        {
            // 뷰쪽에 보내고 싶은 데이터
            ViewData["Title"] = "게시판";

            var countList = 10; // 한페이지에 기본 게시글 갯수 10개

            // 페이지 값 검증 - 음수나 0이면 1로 설정
            if (page < 1) page = 1;

            // 검색 조건이 있으면 적용, 없으면 전체 조회
            var query = _context.Board.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                // Title, Contents에서 검색
                query = query.Where(b => b.Title.Contains(search) ||
                                        b.Contents.Contains(search));
            }

            // 전체 개수 계산
            var totalCount = await query.CountAsync();

            // 데이터가 없으면 빈 리스트 반환
            if (totalCount == 0)
            {
                ViewBag.StartPage = 1;
                ViewBag.EndPage = 1;
                ViewBag.Page = 1;
                ViewBag.TotalPage = 1;
                ViewBag.Search = search;
                return View(new List<Board>());
            }

            // 페이지 계산
            var totalPage = totalCount / countList; // 한페이지당 개수로 나누면 전체페이지 수
            // HACK : 게시판페이지 중요로직. 남는 데이터도 한페이지를 차지해야 함
            if (totalCount % countList > 0) totalPage++;  // 남은 게시글이 있으면 페이지수 증가            

            // 페이지 범위 검증 - 최대 페이지를 넘으면 마지막 페이지로 설정
            if (page > totalPage) page = totalPage;

            // 마지막 페이지 구하기
            var countPage = 10; // 페이지를 표시할 최대페이지개수, 10개
            var startPage = ((page - 1) / countPage) * countPage + 1;
            var endPage = startPage + countPage - 1;
            // HACK : 나타낼 페이수가 10이 안되면 페이지수 조정.
            if (totalPage < endPage) endPage = totalPage;

            // 페이징 처리된 데이터 가져오기
            var boards = await query
                .OrderByDescending(b => b.PostDate) // PostDate로 정렬
                .Skip((page - 1) * countList)       // 페이지 시작점 (이제 안전함)
                .Take(countList)                    // 페이지 크기만큼 가져오기
                .ToListAsync();

            // View로 넘기는 데이터, 페이징 숫자컨트롤 사용
            ViewBag.StartPage = startPage;
            ViewBag.EndPage = endPage;
            ViewBag.Page = page;
            ViewBag.TotalPage = totalPage;
            ViewBag.Search = search; // 검색어

            return View(boards); // boards 변수 사용
        }

        // GET: Board/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var board = await _context.Board
                .FirstOrDefaultAsync(m => m.Id == id);
            if (board == null)
            {
                return NotFound();
            }

            // 조회수 증가 로직
            board.ReadCount++;
            _context.Board.Update(board);
            await _context.SaveChangesAsync();

            return View(board);
        }

        // GET: Board/Create
        public IActionResult Create()
        {
            var board = new Board
            {
                Writer = "관리자",
                Email = "admin@portfolio.com",
                PostDate = DateTime.Now,
                ReadCount = 0,
            };
            return View(board);
        }

        // POST: Board/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Contents")] Board board)
        {
            if (ModelState.IsValid)
            {
                board.Writer = "관리자"; // 작성자는 자동으로 관리자
                board.Email = "admin@portfolio.com"; // 이메일 자동 설정
                board.PostDate = DateTime.Now; // 게시일자는 현재
                board.ReadCount = 0;

                // INSERT INTO...
                _context.Add(board);
                // COMMIT
                await _context.SaveChangesAsync();

                TempData["success"] = "게시글 저장 성공!";
                return RedirectToAction(nameof(Index));
            }
            return View(board);
        }

        // GET: Board/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var board = await _context.Board.FindAsync(id);
            if (board == null)
            {
                return NotFound();
            }
            return View(board);
        }

        // POST: Board/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Contents")] Board board)
        {
            if (id != board.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 기존 게시글 찾아서 수정
                    var existingBoard = await _context.Board.FindAsync(id);
                    if (existingBoard == null)
                    {
                        return NotFound();
                    }

                    existingBoard.Title = board.Title;
                    existingBoard.Contents = board.Contents;

                    // UPDATE
                    await _context.SaveChangesAsync();
                    TempData["success"] = "게시글 수정 성공!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoardExists(board.Id))
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
            return View(board);
        }

        // GET: Board/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var board = await _context.Board
                .FirstOrDefaultAsync(m => m.Id == id);
            if (board == null)
            {
                return NotFound();
            }

            return View(board);
        }

        // POST: Board/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var board = await _context.Board.FindAsync(id);
            if (board != null)
            {
                _context.Board.Remove(board);
            }

            await _context.SaveChangesAsync();
            TempData["success"] = "게시글 삭제 성공!";
            return RedirectToAction(nameof(Index));
        }

        private bool BoardExists(int id)
        {
            return _context.Board.Any(e => e.Id == id);
        }
    }
}