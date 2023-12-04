using Hedgehogcat.Web.Admin.Entities;
using Hedgehogcat.Web.Admin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Hedgehogcat.Web.Admin.Services;
using System.Drawing;
using Hedgehogcat.Web.Admin.Helpers;
using Newtonsoft.Json;

namespace Hedgehogcat.Web.Admin.Controllers
{
    public class AccountController : Controller
    {
        private IAccountService _accountService;

        private readonly ILogger<AccountController> _logger;

        public AccountController(IAccountService accountService,ILogger<AccountController> logger)
        {
            _accountService = accountService;
            _logger = logger;

        }
        /// <summary>
        /// //返回一个页面：这个页面是用来展示登录的页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        /// <summary>
        /// //返回一个页面：这个页面是用来展示登录的页面
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            _logger.LogInformation("登录:"+JsonConvert.SerializeObject(model.UserName)+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            if (ModelState.IsValid)
            {
                //1.验证验证码
                var checkCode = base.HttpContext.Session.GetString("CheckCode");
                if (checkCode == null || checkCode.ToString().Equals(model.CheckCode, StringComparison.InvariantCultureIgnoreCase) == false)
                {
                    ModelState.AddModelError("CheckCode", "验证码输入错误");
                    return View();
                }
                else
                {
                    #region 验证用户名密码
                    var currentAccount = _accountService.GetAccount(model);
                    if (currentAccount == null)
                    {
                        ModelState.AddModelError("", "用户名或者密码错误");
                        _logger.LogInformation("登录失败:"+JsonConvert.SerializeObject(model.UserName)+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        return View();
                    }
                    else
                    {
                        var currentAccountstr = currentAccount.ToString();
                        base.HttpContext.Session.SetString("CurrentUser12", currentAccount.ToString());

                        HttpContext.Response.Cookies.Append("username", currentAccount.Username);
                        HttpContext.Response.Cookies.Append("password", currentAccount.Id.ToString());
                        _logger.LogInformation("登录成功:"+JsonConvert.SerializeObject(model.UserName)+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                        return RedirectToAction("Index", "Home");
                    }
                    #endregion
                }
            }
            else
            {
                return View();
            }
        }
        /// <summary>
        /// 返回一个注册页面
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        /// <summary>
        /// 注册  Form表单提交
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Register(RegisterAccountViewModel register)
        {
            if (!ModelState.IsValid)
            {
                return View(register);
            }
            if (_accountService.AccountExist(register.UserName))
            {
                ModelState.AddModelError("Name", "用户名已存在");
                return View(register);
            }
            else if (_accountService.Register(register))
            {
                return View("Login", new LoginViewModel()
                {
                    UserName = register.UserName
                });
            }
            else
            {
                return View(register);
            }
        }
        /// <summary>
        /// 声场验证码
        /// </summary>
        public IActionResult VerifyCode()
        {
            int width = 90;
            int height = 35;
            int fontsize = 25;
            var (code, bytes) = VCode.CreateValidateGraphic(4, width, height, fontsize);
            base.HttpContext.Session.SetString("CheckCode", code);
            return File(bytes, "image/jpeg");
        }
    }
}
