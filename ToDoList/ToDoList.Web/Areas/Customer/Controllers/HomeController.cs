#region

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ToDoList.DataAccess.Repository.IRepository;
using ToDoList.Models;
using ToDoList.Models.ViewModels;
using ToDoList.Utility;

#endregion

namespace ToDoList.Web.Areas.Customer.Controllers;

[Area("Customer")]
public class HomeController : Controller
{
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<IdentityUser> _userManager;

    public HomeController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager,
        IWebHostEnvironment webEnvironment)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _hostEnvironment = webEnvironment;
    }


    public async Task<IActionResult> Index()
    {
        var colleges =
            await _unitOfWork.College.GetAllAsync(x => x.Deadline < DateTime.Now.AddDays(-7),
                "CompletedColleges,Comments");

        foreach (var college in colleges) _unitOfWork.College.RemoveCollege(college, _hostEnvironment.WebRootPath);

        await _unitOfWork.SaveAsync();

        HomeVM homeVM = new();

        IEnumerable<IdentityUser> admins = await _userManager.GetUsersInRoleAsync(SD.RoleUserAdmin);
        IEnumerable<IdentityUser> owners = await _userManager.GetUsersInRoleAsync(SD.RoleUserOwner);

        if (User.Identity is not ClaimsIdentity claimsIdentity) return NotFound();
        {
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim is null)
            {
                var collegeList = await _unitOfWork.College.GetCollegesAsync(true, x => x.Deadline);

                var group = await _unitOfWork.Group.GetFirstOrDefaultAsync(x => x.GroupType == GroupType.None) ??
                            throw new Exception("Group not found");

                homeVM.Group1 = group;
                homeVM.Group2 = group;
                homeVM.AllCollegeList = new HomeVM.CollegeLists
                {
                    CollegePastDeadlineList = Enumerable.Empty<College>(),
                    CollegeList = collegeList.Where(x => x.Deadline > DateTime.Now),
                    CompletedCollegeList = Enumerable.Empty<College>()
                };
            }
            else
            {
                var applicationUser =
                    (await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(x => x.Id == claim.Value,
                        "Group1,Group2"))!;

                var mainCollegeList =
                    await _unitOfWork.College.GetCollegesAsync(false, x => x.Deadline, applicationUser);
                var mainCompletedCollegeList =
                    await _unitOfWork.CompletedCollege.GetAllAsync(filter: x =>
                        x.ApplicationUserId == applicationUser.Id);

                bool AllTasksFilter(College x)
                {
                    return x.ApplicationUserId == applicationUser.Id || x.Group.GroupType switch
                    {
                        GroupType.English => true,
                        GroupType.Standard => true,
                        GroupType.All => true,
                        GroupType.None => false,
                        _ => throw new ArgumentException("GroupType is not valid")
                    };
                }

                var completedCollegeList =
                    mainCollegeList.Where(x => mainCompletedCollegeList.Any(y => y.CollegeId == x.Id));
                var collegePastDeadlineList = mainCollegeList.Where(x =>
                    x.Deadline <= DateTime.Now && completedCollegeList.All(y => y.Id != x.Id));
                var collegeList = mainCollegeList.Where(x =>
                    x.Deadline > DateTime.Now && completedCollegeList.All(y => y.Id != x.Id));

                homeVM.Group1 = applicationUser.Group1;
                homeVM.Group2 = applicationUser.Group2;
                homeVM.ApplicationUser = applicationUser;
                homeVM.AllCollegeList = new HomeVM.CollegeLists
                {
                    CollegePastDeadlineList = collegePastDeadlineList.Where(AllTasksFilter),
                    CollegeList = collegeList.Where(AllTasksFilter),
                    CompletedCollegeList = completedCollegeList.Where(AllTasksFilter)
                };

                bool GroupTasksFilter(College x)
                {
                    return x.ApplicationUserId == applicationUser.Id || x.Group.GroupType switch
                    {
                        GroupType.English => x.GroupId == applicationUser.Group1Id,
                        GroupType.Standard => x.GroupId == applicationUser.Group2Id,
                        GroupType.All => true,
                        GroupType.None => false,
                        _ => throw new ArgumentException("GroupType is not valid")
                    };
                }

                homeVM.GroupCollegeList = new HomeVM.CollegeLists
                {
                    CollegePastDeadlineList = collegePastDeadlineList.Where(GroupTasksFilter),
                    CollegeList = collegeList.Where(GroupTasksFilter),
                    CompletedCollegeList = completedCollegeList.Where(GroupTasksFilter).Reverse()
                };

                if (applicationUser.Group1.Name == "None" || applicationUser.Group2.Name == "None")
                    TempData["Groups"] = "Please select your groups to use all of the features of the website.";
            }

            return View(homeVM);
        }

    }

    [Authorize]
    public async Task<IActionResult> MarkAsCompleted(int? id)
    {
        var claimsIdentity = (User.Identity as ClaimsIdentity)!;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)!;

        var applicationUser = (await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(x => x.Id == claim.Value))!;

        var college = await _unitOfWork.College.GetFirstOrDefaultAsync(x => x.Id == id);

        if (college is null)
            return RedirectToAction(nameof(Index));

        CompletedCollege completedCollege1 = new()
        {
            CollegeId = college.Id,
            ApplicationUserId = applicationUser.Id
        };

        var completedCollege2 = await _unitOfWork.CompletedCollege.GetFirstOrDefaultAsync(x =>
            x.CollegeId == completedCollege1.CollegeId && x.ApplicationUserId == completedCollege1.ApplicationUserId);

        if (completedCollege2 is null)
        {
            _unitOfWork.CompletedCollege.Add(completedCollege1);
        }
        else
        {
            if (completedCollege2.ApplicationUserId != applicationUser.Id)
                return RedirectToAction(nameof(Index));

            _unitOfWork.CompletedCollege.Remove(completedCollege2);
        }

        await _unitOfWork.SaveAsync();

        return RedirectToAction(nameof(Index));
    }

    public IActionResult About()
    {
        return View();
    }
}