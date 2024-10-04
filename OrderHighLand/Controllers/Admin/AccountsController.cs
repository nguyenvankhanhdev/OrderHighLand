using Microsoft.AspNetCore.Mvc;
using OrderHighLand.Models.VM;
using OrderHighLand.Service;

namespace OrderHighLand.Controllers.Admin
{
	public class AccountsController : Controller
	{
		private readonly AccountService _accountService;
		private readonly RoleService _roleService;
		public AccountsController(AccountService accountService, RoleService roleService)
        {
			_accountService = accountService;
			_roleService = roleService;
		}
        public async Task<IActionResult> Index()
		{
			var accounts = await _accountService.getAllAccountAsync();
			var role = await _roleService.getAllRoleAsync();
			AccountRoleVM accountRoleVM = new AccountRoleVM
			{
				accounts = accounts,
				roles = role
			};
			return View(accounts);
		}

	}
}
