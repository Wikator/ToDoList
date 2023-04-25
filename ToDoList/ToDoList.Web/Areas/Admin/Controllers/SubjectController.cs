using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoList.DataAccess.Repository.IRepository;
using ToDoList.Models;
using ToDoList.Models.ViewModels;

namespace ToDoList.Web.Areas.Admin.Controllers
{
    [Authorize(Policy = "RequireAdmin")]
    [Area("Admin")]
    public class SubjectController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubjectController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Subject> subjects = await _unitOfWork.Subject.GetAllAsync(order: s => s.Name);
            return View(subjects);
        }

        // GET: SubjectController/Create
        public async Task<IActionResult> Create()
        {
            Subject subject = new()
            {
                SubjectType = SubjectType.Standard
            };

            IEnumerable<Group> groups = await _unitOfWork.Group.GetAllAsync(filter: x => x.GroupType == GroupType.Standard || x.GroupType == GroupType.All, order: x => x.Name);

            SubjectVM subjectVM = new()
            {
                Subject = subject,
                SubjectTimes = groups.Select(g => new SubjectTime()
                {
                    GroupId = g.Id,
                    Group = g,
                    Time = null
                }).ToArray()
            };

            return View(subjectVM);
        }

        // POST: SubjectController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubjectVM subjectVM)
        {
            IEnumerable<Subject> subjectList = await _unitOfWork.Subject.GetAllAsync();

            if (subjectList.Any(s => s.Name == subjectVM.Subject.Name))
            {
                ModelState.AddModelError("Subject.Name", "Subject already exists!");
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.Subject.Add(subjectVM.Subject);
                await _unitOfWork.SaveAsync();

                foreach (SubjectTime subjectTime in subjectVM.SubjectTimes)
                {
					Group? group = await _unitOfWork.Group.GetFirstOrDefaultAsync(x => x.Id == subjectTime.GroupId);

                    if (group is null)
                        continue;

					subjectTime.SubjectId = subjectVM.Subject.Id;
					_unitOfWork.SubjectTime.Add(subjectTime);
                }

                await _unitOfWork.SaveAsync();
                return RedirectToAction(nameof(Index));
            }      

            return View(subjectVM);
        }

        // GET: SubjectController/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            Subject? subject = await _unitOfWork.Subject.GetFirstOrDefaultAsync(s => s.Id == id);

            if (subject is null)
                return RedirectToAction(nameof(Index));

            IEnumerable<Group> groups = subject.SubjectType switch
            {
                SubjectType.English => await _unitOfWork.Group.GetAllAsync(filter: x => x.GroupType == GroupType.English || x.GroupType == GroupType.All, order: x => x.Name),
                SubjectType.Standard => await _unitOfWork.Group.GetAllAsync(filter: x => x.GroupType == GroupType.Standard || x.GroupType == GroupType.All, order: x => x.Name),
                SubjectType.Other => await _unitOfWork.Group.GetAllAsync(order: x => x.Name),
                _ => throw new Exception("Unknown subject type")
            };

            SubjectVM subjectVM = new()
            {
                Subject = subject,
                SubjectTimes = groups.Select(g =>
                {
                    Task<SubjectTime?> subjectTime = _unitOfWork.SubjectTime.GetFirstOrDefaultAsync(filter: x => x.GroupId == g.Id && x.SubjectId == subject.Id);
                    return subjectTime.Result ?? new SubjectTime() { GroupId = g.Id, Group = g, SubjectId = subject.Id, Time = null };
                }).ToArray()
            };

            return View(subjectVM);
        }

        // POST: SubjectController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubjectVM subjectVM)
        {
            IEnumerable<Subject> subjectList = await _unitOfWork.Subject.GetAllAsync(filter: s => s.Name == subjectVM.Subject.Name && s.Id != subjectVM.Subject.Id);

            if (subjectList.Any())
            {
                ModelState.AddModelError("Subject.Name", "Subject already exists!");
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.Subject.Update(subjectVM.Subject);
                await _unitOfWork.SaveAsync();

                foreach (SubjectTime subjectTime in subjectVM.SubjectTimes)
                {
					Group? group = await _unitOfWork.Group.GetFirstOrDefaultAsync(x => x.Id == subjectTime.GroupId);

                    if (group is null)
                        continue;

					if (subjectTime.Id == 0)
					{
						subjectTime.SubjectId = subjectVM.Subject.Id;
						_unitOfWork.SubjectTime.Add(subjectTime);
					}
					else
					{
					    subjectTime.SubjectId = subjectVM.Subject.Id;
					    _unitOfWork.SubjectTime.Update(subjectTime);
					}			
                }

                await _unitOfWork.SaveAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(subjectVM);
        }

        #region API CALLS

        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                Subject? subject = await _unitOfWork.Subject.GetFirstOrDefaultAsync(x => x.Id == id);

                if (subject is null)
                    return Json(new { success = false });

                if (subject.SubjectType != SubjectType.Standard)
					return Json(new { success = false });

				_unitOfWork.Subject.Remove(subject);
                await _unitOfWork.SaveAsync();
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        #endregion
    }
}
