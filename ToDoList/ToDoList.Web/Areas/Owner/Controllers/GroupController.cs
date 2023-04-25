using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ToDoList.DataAccess.Repository.IRepository;
using ToDoList.Models;

namespace ToDoList.Web.Areas.Admin.Controllers
{
    [Authorize(Policy = "RequireOwner")]
    [Area("Owner")]
    public class GroupController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public GroupController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Group> groups = await _unitOfWork.Group.GetAllAsync(order: g => g.Name, filter: g => g.GroupType == GroupType.Standard || g.GroupType == GroupType.English);
			return View(groups);
        }

        // GET: SubjectController/Create
        public IActionResult Create()
        {
            Group group = new()
            {
                GroupType = GroupType.Standard
            };

            ViewBag.Groups = GetGroupTypes();
			return View(group);
        }

        // POST: SubjectController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Group group)
        {
            IEnumerable<Group> groupList = await _unitOfWork.Group.GetAllAsync();

            if (groupList.Any(s => s.Name == group.Name))
            {
                ModelState.AddModelError("Name", "Group already exists!");
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.Group.Add(group);
                await _unitOfWork.SaveAsync();
                return RedirectToAction(nameof(Index));
            }

			ViewBag.Groups = GetGroupTypes();
			return View(group);
        }

        // GET: SubjectController/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            Group? group = await _unitOfWork.Group.GetFirstOrDefaultAsync(s => s.Id == id);

            if (group is null || group.GroupType == GroupType.None || group.GroupType == GroupType.All)
                return RedirectToAction(nameof(Index));

			ViewBag.Groups = GetGroupTypes();
			return View(group);
		}

        // POST: SubjectController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Group group)
        {
            IEnumerable<Group> groupList = await _unitOfWork.Group.GetAllAsync(filter: s => s.Name == group.Name && s.Id != group.Id);

            if (groupList.Any())
            {
                ModelState.AddModelError("Name", "Group already exists!");
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.Group.Update(group);
                await _unitOfWork.SaveAsync();
                return RedirectToAction(nameof(Index));
            }

			ViewBag.Groups = GetGroupTypes();
			return View(group);
        }

        #region API CALLS

        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                Group? group = await _unitOfWork.Group.GetFirstOrDefaultAsync(filter: s => s.Id == id, includeProperties: "ApplicationUsers1,ApplicationUsers2,Colleges");

                if (group is null)
                    return Json(new { success = false });

                Group noneGroup = await _unitOfWork.Group.GetFirstOrDefaultAsync(x => x.GroupType == GroupType.None) ?? throw new Exception("Group not found");

                switch (group.GroupType)
                {
                    case GroupType.English:
						foreach (ApplicationUser applicationUser in group.ApplicationUsers2)
						{
							applicationUser.Group2Id = noneGroup.Id;
							_unitOfWork.ApplicationUser.Update(applicationUser);
						}
                        break;
				    case GroupType.Standard:
						foreach (ApplicationUser applicationUser in group.ApplicationUsers1)
						{
							applicationUser.Group1Id = noneGroup.Id;
							_unitOfWork.ApplicationUser.Update(applicationUser);
						}
                        break;
                    default:
						return Json(new { success = false });
				}

                foreach (College college in group.Colleges)
                {
                    college.GroupId = noneGroup.Id;
                    _unitOfWork.College.Update(college);
                }

                await _unitOfWork.SaveAsync();

                _unitOfWork.Group.Remove(group);
                await _unitOfWork.SaveAsync();
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        #endregion

		private static IEnumerable<SelectListItem> GetGroupTypes()
        {
            GroupType[] groupTypeList = new GroupType[] { GroupType.Standard, GroupType.English };
			IEnumerable<SelectListItem> groupItemList1 = groupTypeList.Select(i => new SelectListItem
			{
				Text = i.ToString(),
				Value = i.ToString()
			});
			return groupItemList1;
		}
	}
}
