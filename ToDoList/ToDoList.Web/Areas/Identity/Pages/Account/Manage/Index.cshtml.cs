// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

#region

using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ToDoList.DataAccess.Repository.IRepository;
using ToDoList.Models;
using ToDoList.Utility;

#endregion

namespace ToDoList.Web.Areas.Identity.Pages.Account.Manage;

public class IndexModel : PageModel
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<IdentityUser> _userManager;

    public IndexModel(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    [TempData]
    public string StatusMessage { get; set; }

    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    [BindProperty]
    public InputModel Input { get; set; }

    private async Task LoadAsync(IdentityUser user)
    {
        var userName = await _userManager.GetUserNameAsync(user);
        var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

        var claimsIdentity = User.Identity as ClaimsIdentity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        var applicationUser =
            await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(u => u.Id == claim.Value, "Group1,Group2");


        Username = userName;

        Input = new InputModel
        {
            PhoneNumber = phoneNumber,
            ApplicationUser = applicationUser,
            Groups1 = await GetGroupsAsync(SubjectType.Standard),
            Groups2 = await GetGroupsAsync(SubjectType.English)
        };


        if (applicationUser.Group1.Name == "None" || applicationUser.Group2.Name == "None")
            StatusMessage = "Please select your groups to use all of the features of the website.";
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

        await LoadAsync(user);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

        if (!ModelState.IsValid)
        {
            await LoadAsync(user);
            return Page();
        }

        var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

        var claimsIdentity = User.Identity as ClaimsIdentity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        var applicationUser = await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(u => u.Id == claim.Value);
        applicationUser.Nickname = Input.ApplicationUser.Nickname;
        applicationUser.Group1Id = Input.ApplicationUser.Group1Id;
        applicationUser.Group2Id = Input.ApplicationUser.Group2Id;

        _unitOfWork.ApplicationUser.Update(applicationUser);
        if (User.IsInRole(SD.RoleUserCustomer))
        {
            var collegeList =
                await _unitOfWork.College.GetAllAsync(s => s.ApplicationUserId == applicationUser.Id, "Subject");
            foreach (var college in collegeList)
            {
                college.GroupId = college.Subject.SubjectType switch
                {
                    SubjectType.Standard => college.GroupId = Input.ApplicationUser.Group1Id,
                    SubjectType.English => college.GroupId = Input.ApplicationUser.Group2Id,
                    _ => college.GroupId
                };

                _unitOfWork.College.Update(college);
            }
        }

        await _unitOfWork.SaveAsync();

        if (Input.PhoneNumber != phoneNumber)
        {
            var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
            if (!setPhoneResult.Succeeded)
            {
                StatusMessage = "Unexpected error when trying to set phone number.";
                return RedirectToPage();
            }
        }

        await _signInManager.RefreshSignInAsync(user);
        StatusMessage = "Your profile has been updated";
        return RedirectToPage();
    }

    private async Task<IEnumerable<SelectListItem>> GetGroupsAsync(SubjectType subjectType)
    {
        switch (subjectType)
        {
            case SubjectType.Standard:
                var groupList1 = await _unitOfWork.Group.GetAllAsync(
                    filter: x => x.GroupType == GroupType.Standard || x.GroupType == GroupType.None,
                    order: x => x.Name);
                var groupItemList1 = groupList1.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
                return groupItemList1;
            case SubjectType.English:
                var groupList2 = await _unitOfWork.Group.GetAllAsync(
                    filter: x => x.GroupType == GroupType.English || x.GroupType == GroupType.None, order: x => x.Name);
                var groupItemList2 = groupList2.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
                return groupItemList2;
            case SubjectType.Other:
                var groupList3 = await _unitOfWork.Group.GetAllAsync(x => x.Name);
                var groupItemList3 = groupList3.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
                return groupItemList3;
            default:
                throw new Exception("Unknown subject type");
        }
    }

    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class InputModel
    {
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        [ValidateNever] public IEnumerable<SelectListItem> Groups1 { get; set; }

        [ValidateNever] public IEnumerable<SelectListItem> Groups2 { get; set; }
    }
}