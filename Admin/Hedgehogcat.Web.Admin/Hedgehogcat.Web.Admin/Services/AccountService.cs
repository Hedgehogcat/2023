namespace Hedgehogcat.Web.Admin.Services;

using BCrypt.Net;
using Microsoft.Extensions.Options;
using Hedgehogcat.Web.Admin.Entities;
using Hedgehogcat.Web.Admin.Helpers;
using Hedgehogcat.Web.Admin.Models;

public interface IAccountService
{
    IEnumerable<Account> GetAll();
    Account GetById(int id);
    /// <summary>
    /// 查询当前用户信息
    /// </summary>
    /// <param name="loginViewModel"></param>
    /// <returns></returns>
    Account? GetAccount(LoginViewModel loginViewModel);
    /// <summary>
    ///  判断用户是否存在
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool AccountExist(string name);
    /// <summary>
    /// 注册
    /// </summary>
    /// <param name="currentUser"></param>
    /// <returns></returns>
    bool Register(RegisterAccountViewModel registerUser);
}

public class AccountService : IAccountService
{
    private DataContext _context;
    private readonly AppSettings _appSettings;

    public AccountService(
        DataContext context,
        IOptions<AppSettings> appSettings)
    {
        _context = context;
        _appSettings = appSettings.Value;
    }

    public IEnumerable<Account> GetAll()
    {
        return _context.Accounts;
    }

    public Account GetById(int id)
    {
        var account = _context.Accounts.Find(id);
        if (account == null) throw new KeyNotFoundException("Account not found");
        return account;
    }

    public Account? GetAccount(LoginViewModel loginViewModel)
    {
        var account = _context.Set<Account>()
            .Where(u => u.Username == loginViewModel.UserName && u.Password == loginViewModel.Password)
            .Select(u => new Account
            {
                Id = u.Id,
                Username = u.Username

            }).FirstOrDefault();
        return account;
    }

    public bool AccountExist(string name)
    {
        return _context.Set<Account>().Where(c => c.Username == name).FirstOrDefault() != null;
    }

    public bool Register(RegisterAccountViewModel registerUser)
    {
        Account account = new Account()
        {
            Username = registerUser.UserName,
            Password = registerUser.Password,
        };
        _context.Set<Account>().Add(account);
        _context.SaveChanges();
        return true;
    }
}