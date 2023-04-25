using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Immutable;
using System.Security.Claims;
using ToDoList.DataAccess.Repository.IRepository;
using ToDoList.Models;

namespace ToDoList.Web.Areas.Customer.Controllers
{
	[Authorize]
	[Area("Customer")]
	public class CommentController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IWebHostEnvironment _hostEnvironment;

		private readonly ImmutableArray<string> allowedExtensions = ImmutableArray.Create(".png", ".jpg", ".txt", ".js", ".py", ".scala", ".cs", ".css", ".html", ".sh", ".rb" );

		public CommentController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
		{
			_unitOfWork = unitOfWork;
			_hostEnvironment = hostEnvironment;
		}

		public async Task<IActionResult> Index(int? collegeId)
		{
			if (collegeId is null)
				return RedirectToAction(nameof(Index), "Home");

			IEnumerable<Comment> comments = await _unitOfWork.Comment.GetAllAsync(u => u.CollegeId == collegeId, includeProperties: "ApplicationUser,College");
			ViewBag.CollegeId = collegeId!;

			return View(comments);
		}


		public async Task<IActionResult> Details(int? commentId, int? collegeId)
		{
			if (collegeId is null)
				return RedirectToAction(nameof(Index), "Home");

			Comment? comment = await _unitOfWork.Comment.GetFirstOrDefaultAsync(u => u.Id == commentId, includeProperties: "ApplicationUser,College");

			if (comment is null)
				return RedirectToAction(nameof(Index), new { collegeId });

			ClaimsIdentity claimsIdentity = (User.Identity as ClaimsIdentity)!;
			Claim claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)!;

			ViewBag.IsOwner = comment.ApplicationUserId == claim.Value;

			return View(comment);
		}


		public IActionResult Create(int? collegeId)
		{
			if (collegeId is null)
				return RedirectToAction(nameof(Index), "Home");

			ClaimsIdentity claimsIdentity = (User.Identity as ClaimsIdentity)!;
			Claim claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)!;

			Comment comment = new()
			{
				ApplicationUserId = claim.Value,
				CollegeId = (int)collegeId
			};
				
			return View(comment);
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(Comment comment, IFormFile? file)
		{
			try
			{
				if (ModelState.IsValid)
				{
					string wwwRootPath = _hostEnvironment.WebRootPath;

					if (file is not null)
					{
						string fileName = Guid.NewGuid().ToString();
						var uploads = Path.Combine(wwwRootPath, @"files");
						var extension = Path.GetExtension(file.FileName);

						if (!allowedExtensions.Contains(extension))
						{
                            ModelState.AddModelError("FileUrl", $"Your file type is not allowed, please upload a file with the following extensions:  {string.Join(", ", allowedExtensions)}");
                            return View(comment);
                        }

						using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
						{
							await file.CopyToAsync(fileStreams);
						}

						comment.FileUrl = @"\files\" + fileName + extension;
					}

					
					_unitOfWork.Comment.Add(comment);
					await _unitOfWork.SaveAsync();
					return RedirectToAction(nameof(Details), new { commentId = comment.Id, collegeId = comment.CollegeId });
				}

				return View(comment);
			}
			catch
			{
				return NotFound("An unexpected error occured");
			}
		}


		public async Task<IActionResult> Edit(int? commentId)
		{
			Comment? comment = await _unitOfWork.Comment.GetFirstOrDefaultAsync(filter: x => x.Id == commentId);

			if (comment is null)
				return RedirectToAction(nameof(Index), "Home");

			return View(comment);
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Comment comment, IFormFile? file)
		{
			try
			{
				if (ModelState.IsValid)
				{
					ClaimsIdentity claimsIdentity = (User.Identity as ClaimsIdentity)!;
					Claim claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)!;

					if (claim.Value != comment.ApplicationUserId)
							return RedirectToAction(nameof(Index), new { collegeId = comment.CollegeId });

					string wwwRootPath = _hostEnvironment.WebRootPath;

					if (file is not null)
					{
						string fileName = Guid.NewGuid().ToString();
						var uploads = Path.Combine(wwwRootPath, @"files");
						var extension = Path.GetExtension(file.FileName);

                        if (!allowedExtensions.Contains(extension))
                        {
                            ModelState.AddModelError("FileUrl", $"Your file type is not allowed, please upload a file with the following extensions:  {string.Join(", ", allowedExtensions)}");
                            return View(comment);
                        }

                        if (comment.FileUrl is not null)
						{
							var oldImagePath = Path.Combine(wwwRootPath, comment.FileUrl.TrimStart('\\'));
							if (System.IO.File.Exists(oldImagePath))
							{
								System.IO.File.Delete(oldImagePath);
							}
						}

						using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
						{
							await file.CopyToAsync(fileStreams);
						}

						comment.FileUrl = @"\files\" + fileName + extension;
					}


					_unitOfWork.Comment.Update(comment);
					await _unitOfWork.SaveAsync();
					return RedirectToAction(nameof(Details), new { commentId = comment.Id, collegeId = comment.CollegeId });
				}
				return View(comment);
			}
			catch
			{
				return NotFound("An unexpected error occured");
			}
		}


		#region API CALLS

		[HttpDelete]
		public async Task<IActionResult> Delete(int? id)
		{
			try
			{
				Comment? comment = await _unitOfWork.Comment.GetFirstOrDefaultAsync(x => x.Id == id);

				if (comment is null)
					return Json(new { success = false });

				ClaimsIdentity claimsIdentity = (User.Identity as ClaimsIdentity)!;
				Claim claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)!;

				if (claim.Value != comment.ApplicationUserId)
						return Json(new { success = false });

				string wwwRootPath = _hostEnvironment.WebRootPath;

				if (comment.FileUrl is not null)
				{
					var oldImagePath = Path.Combine(wwwRootPath, comment.FileUrl.TrimStart('\\'));
					if (System.IO.File.Exists(oldImagePath))
					{
						System.IO.File.Delete(oldImagePath);
					}
				}

				_unitOfWork.Comment.Remove(comment);
				await _unitOfWork.SaveAsync();
				return Json(new { success = true });
			}
			catch
			{
				return Json(new { success = false });
			}
		}


        public IActionResult Download(string fileUrl)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;
            var filePath = Path.Combine(wwwRootPath, fileUrl.TrimStart('\\'));
            var fileStream = new FileStream(filePath, FileMode.Open);
            return File(fileStream, "application/octet-stream", Path.GetFileName(filePath));
        }

        #endregion
    }
}
