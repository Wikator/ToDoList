#region

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ToDoList.DataAccess.Repository.IRepository;
using ToDoList.Models;
using ToDoList.Models.ViewModels;
using ToDoList.Web.Extensions;

#endregion

namespace ToDoList.Web.Areas.Customer.Controllers;

[Authorize]
[Area("Customer")]
public class CollegeController : Controller
{
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly IUnitOfWork _unitOfWork;

    public CollegeController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _hostEnvironment = hostEnvironment;
    }

    // GET: CollegeController
    public async Task<IActionResult> Index()
    {
        var userId = User.GetLoggedInUserId();

        var collegeList = await _unitOfWork.College.GetAllAsync(
            filter: x => x.ApplicationUserId == userId, order: x => x.Deadline, includeProperties: "Subject");
        return View(collegeList);
    }

    // GET: CollegeController/Create
    public async Task<IActionResult> Create()
    {
        var userId = User.GetLoggedInUserId();

        var applicationUser = (await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(x => x.Id == userId))!;
        var group = await _unitOfWork.Group.GetFirstOrDefaultAsync(x => x.GroupType == GroupType.None) ??
                    throw new Exception("Group not found");


        CollegeVM collegeVM = new()
        {
            ListItem = new College
            {
                Deadline = DateTime.Now,
                ApplicationUserId = applicationUser.Id
            },
            Time = null,
            Group1Id = applicationUser.Group1Id,
            Group2Id = applicationUser.Group2Id,
            Group3Id = group.Id
        };

        await SetUpCollegeVMAsync(collegeVM);
        return View(collegeVM);
    }

    // POST: CollegeController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CollegeVM collegeVM)
    {
        if (ModelState.IsValid)
        {
            var chosenSubjectType =
                _unitOfWork.Subject.GetFirstOrDefaultAsync(x => x.Id == collegeVM.ListItem.SubjectId).Result!
                    .SubjectType;

            var deadline = chosenSubjectType switch
            {
                SubjectType.Standard => await _unitOfWork.SubjectTime.GetTimeAsync(collegeVM.ListItem.SubjectId,
                    collegeVM.Group1Id),
                SubjectType.English => await _unitOfWork.SubjectTime.GetTimeAsync(collegeVM.ListItem.SubjectId,
                    collegeVM.Group2Id),
                SubjectType.Other => await _unitOfWork.SubjectTime.GetTimeAsync(collegeVM.ListItem.SubjectId,
                    collegeVM.Group3Id),
                _ => throw new Exception("Unknown subject")
            };

            if (deadline is not null)
            {
                collegeVM.ListItem.Deadline =
                    collegeVM.ListItem.Deadline.Add(collegeVM.Time == null
                        ? (TimeSpan)deadline
                        : collegeVM.Time.Value.TimeOfDay);
            }
            else
            {
                if (collegeVM.Time is null)
                {
                    ModelState.AddModelError("ListItem.Deadline",
                        "Your chosen group doesn't have a default time for this subject. Please choose a custom time.");
                    await SetUpCollegeVMAsync(collegeVM);
                    return View(collegeVM);
                }

                collegeVM.ListItem.Deadline = collegeVM.ListItem.Deadline.Add(collegeVM.Time.Value.TimeOfDay);
            }

            if (collegeVM.ListItem.Deadline < DateTime.Now.AddDays(-7))
            {
                ModelState.AddModelError("ListItem.Deadline", "Can't create a task that has a deadline due 1 week ago");
                await SetUpCollegeVMAsync(collegeVM);

                return View(collegeVM);
            }

            SetUpGroup(collegeVM);
            _unitOfWork.College.Add(collegeVM.ListItem);
            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Index));
        }

        await SetUpCollegeVMAsync(collegeVM);
        return View(collegeVM);
    }

    // GET: CollegeController/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        var userId = User.GetLoggedInUserId();

        var applicationUser = (await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(x => x.Id == userId))!;
        var group = await _unitOfWork.Group.GetFirstOrDefaultAsync(x => x.GroupType == GroupType.None) ??
                    throw new Exception("Group not found");
        var college = await _unitOfWork.College.GetFirstOrDefaultAsync(x => x.Id == id, "Group");

        if (college is null || college.ApplicationUserId != applicationUser.Id)
            return RedirectToAction(nameof(Index));

        CollegeVM collegeVM = new()
        {
            ListItem = college
        };

        await SetUpCollegeVMAsync(collegeVM);

        switch (collegeVM.ListItem.Subject.SubjectType)
        {
            case SubjectType.Standard:
                collegeVM.Group1Id = collegeVM.ListItem.GroupId;
                collegeVM.Group2Id = applicationUser.Group2Id;
                collegeVM.Group3Id = group.Id;
                break;
            case SubjectType.English:
                collegeVM.Group1Id = applicationUser.Group1Id;
                collegeVM.Group2Id = collegeVM.ListItem.GroupId;
                collegeVM.Group3Id = group.Id;
                break;
            case SubjectType.Other:
                collegeVM.Group1Id = applicationUser.Group1Id;
                collegeVM.Group2Id = applicationUser.Group2Id;
                collegeVM.Group3Id = collegeVM.ListItem.GroupId;
                break;
            default:
                throw new Exception("Unknown subject");
        }

        return View(collegeVM);
    }

    // POST: CollegeController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CollegeVM collegeVM)
    {
        if (ModelState.IsValid)
        {
            if (collegeVM.ListItem.Deadline < DateTime.Now.AddDays(-7))
            {
                ModelState.AddModelError("ListItem.Deadline", "Can't create a task that has a deadline due 1 week ago");
                await SetUpCollegeVMAsync(collegeVM);

                return View(collegeVM);
            }

            SetUpGroup(collegeVM);
            _unitOfWork.College.Update(collegeVM.ListItem);
            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Index));
        }

        await SetUpCollegeVMAsync(collegeVM);
        return View(collegeVM);
    }

    #region API CALLS

    [HttpDelete]
    public async Task<IActionResult> Delete(int? id)
    {
        try
        {
            var userId = User.GetLoggedInUserId();
            var college =
                await _unitOfWork.College.GetFirstOrDefaultAsync(x => x.Id == id, "CompletedColleges,Comments");

            if (college is null || college.ApplicationUserId != userId)
                return Json(new { success = false });

            _unitOfWork.College.RemoveCollege(college, _hostEnvironment.WebRootPath);
            await _unitOfWork.SaveAsync();
            return Json(new { success = true });
        }
        catch
        {
            return Json(new { success = false });
        }
    }

    #endregion

    private async Task SetUpCollegeVMAsync(CollegeVM collegeVM)
    {
        var groupList = await _unitOfWork.Group.GetAllAsync(x => x.Name);
        var subjects = await _unitOfWork.Subject.GetAllAsync(x => x.Name);

        collegeVM.Groups1 = groupList
            .Where(x => x.GroupType is GroupType.Standard or GroupType.None or GroupType.All)
            .Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });

        collegeVM.Groups2 = groupList
            .Where(x => x.GroupType is GroupType.English or GroupType.None or GroupType.All)
            .Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });

        collegeVM.Groups3 = groupList
            .Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });

        collegeVM.Subjects = subjects
            .Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
    }

    private void SetUpGroup(CollegeVM collegeVM)
    {
        var chosenSubjectType =
            _unitOfWork.Subject.GetFirstOrDefaultAsync(x => x.Id == collegeVM.ListItem.SubjectId).Result!.SubjectType;

        collegeVM.ListItem.GroupId = chosenSubjectType switch
        {
            SubjectType.Standard => collegeVM.Group1Id,
            SubjectType.English => collegeVM.Group2Id,
            SubjectType.Other => collegeVM.Group3Id,
            _ => throw new Exception("Unknown subject")
        };
    }
}