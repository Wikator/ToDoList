using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoList.DataAccess.Repository.IRepository;
using ToDoList.Models;
using ToDoList.Models.ViewModels;
using ToDoList.Utility;

namespace ToDoList.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _hostEnvironment;

        public HomeController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, IWebHostEnvironment webEnvironment)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _hostEnvironment = webEnvironment;
        }


        public async Task<IActionResult> Index()
        {
            IEnumerable<College> colleges = await _unitOfWork.College.GetAllAsync(filter: x => x.Deadline < DateTime.Now.AddDays(-7), includeProperties: "CompletedColleges,Comments");

            foreach (College college in colleges)
            {
                _unitOfWork.College.RemoveCollege(college, _hostEnvironment.WebRootPath);
            }

            await _unitOfWork.SaveAsync();

            HomeVM homeVM = new();

            IEnumerable<IdentityUser> admins = await _userManager.GetUsersInRoleAsync(SD.Role_User_Admin);
            IEnumerable<IdentityUser> owners = await _userManager.GetUsersInRoleAsync(SD.Role_User_Owner);

            if (User.Identity is ClaimsIdentity claimsIdentity)
            {
                Claim? claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                if (claim is null)
                {
                    IEnumerable<College> collegeList = await _unitOfWork.College.GetCollegesAsync(true, x => x.Deadline);

                    Group group = await _unitOfWork.Group.GetFirstOrDefaultAsync(x => x.GroupType == GroupType.None) ?? throw new Exception("Group not found");

                    homeVM.Group1 = group;
                    homeVM.Group2 = group;
                    homeVM.AllCollegeList = new()
                    {
                        CollegePastDeadlineList = Enumerable.Empty<College>(),
                        CollegeList = collegeList.Where(x => x.Deadline > DateTime.Now),
                        CompletedCollegeList = Enumerable.Empty<College>()
                    };
                }
                else
                {
                    ApplicationUser applicationUser = (await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(x => x.Id == claim.Value, includeProperties: "Group1,Group2"))!;

                    IEnumerable<College> mainCollegeList = await _unitOfWork.College.GetCollegesAsync(false, x => x.Deadline, applicationUser);
                    IEnumerable<CompletedCollege> mainCompletedCollegeList = await _unitOfWork.CompletedCollege.GetAllAsync(filter: x => x.ApplicationUserId == applicationUser.Id);

					bool allTasksFilter(College x) => x.ApplicationUserId == applicationUser.Id || x.Group.GroupType switch
					{
						GroupType.English => true,
						GroupType.Standard => true,
						GroupType.All => true,
						GroupType.None => false,
						_ => throw new ArgumentException("GroupType is not valid")
					};

					IEnumerable<College> completedCollegeList = mainCollegeList.Where(x => mainCompletedCollegeList.Any(y => y.CollegeId == x.Id));
                    IEnumerable<College> collegePastDeadlineList = mainCollegeList.Where(x => x.Deadline <= DateTime.Now && !completedCollegeList.Any(y => y.Id == x.Id));
                    IEnumerable<College> collegeList = mainCollegeList.Where(x => x.Deadline > DateTime.Now && !completedCollegeList.Any(y => y.Id == x.Id));

                    homeVM.Group1 = applicationUser.Group1;
                    homeVM.Group2 = applicationUser.Group2;
                    homeVM.ApplicationUser = applicationUser;
                    homeVM.AllCollegeList = new()
                    {
                        CollegePastDeadlineList = collegePastDeadlineList.Where(allTasksFilter),
                        CollegeList = collegeList.Where(allTasksFilter),
                        CompletedCollegeList = completedCollegeList.Where(allTasksFilter)
					};

					bool groupTasksFilter(College x) => x.ApplicationUserId == applicationUser.Id || x.Group.GroupType switch
					{
						GroupType.English => x.GroupId == applicationUser.Group1Id,
						GroupType.Standard => x.GroupId == applicationUser.Group2Id,
                        GroupType.All => true,
                        GroupType.None => false,
                        _ => throw new ArgumentException("GroupType is not valid")
					};

					homeVM.GroupCollegeList = new()
                    {
                        CollegePastDeadlineList = collegePastDeadlineList.Where(groupTasksFilter),
                        CollegeList = collegeList.Where(groupTasksFilter),
                        CompletedCollegeList = completedCollegeList.Where(groupTasksFilter).Reverse()
                    };

                    if (applicationUser.Group1.Name == "None" || applicationUser.Group2.Name == "None")
                    {
                        TempData["Groups"] = "Please select your groups to use all of the features of the website.";
                    }
                }

                return View(homeVM);
            }

            return NotFound();
        }

        [Authorize]
        public async Task<IActionResult> MarkAsCompleted(int? id)
        {
			ClaimsIdentity claimsIdentity = (User.Identity as ClaimsIdentity)!;
			Claim claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)!;

			ApplicationUser applicationUser = (await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(x => x.Id == claim.Value))!;

            College? college = await _unitOfWork.College.GetFirstOrDefaultAsync(x => x.Id == id);

            if (college is null)
				return RedirectToAction(nameof(Index));

			CompletedCollege completedCollege1 = new()
            {
                CollegeId = college.Id,
                ApplicationUserId = applicationUser.Id
            };

            CompletedCollege? completedCollege2 = await _unitOfWork.CompletedCollege.GetFirstOrDefaultAsync(x => x.CollegeId == completedCollege1.CollegeId && x.ApplicationUserId == completedCollege1.ApplicationUserId);

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
}
