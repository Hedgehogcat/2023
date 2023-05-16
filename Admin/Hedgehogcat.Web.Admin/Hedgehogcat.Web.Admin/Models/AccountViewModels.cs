using System.ComponentModel.DataAnnotations;

namespace Hedgehogcat.Web.Admin.Models
{
    public class AccountViewModels
    {

    }
    public class LoginViewModel
    {
        [Required(ErrorMessage ="用户名必填!")]
        [Display(Name = "用户名")]
        public string UserName { get; set; }= string.Empty;

        [Required(ErrorMessage ="密码必填!")]
        [DataType(DataType.Password)]
        [StringLength(10, ErrorMessage = "密码不能超过64个字符")]
        [Display(Name = "密码")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "记住密码")]
        public bool RememberMe { get; set; }

        [Display(Name = "验证码")]
        [Required(ErrorMessage = "验证码必填!")]
        public string CheckCode { get; set; } = string.Empty;

        //public string ConfirmVCode{ get; set; }
    }

    public class RegisterAccountViewModel
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required(ErrorMessage = "UserName不能为空")]
        [StringLength(20, ErrorMessage = "UserName不能超过20个字符")]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "Password不能为空")]
        [StringLength(64, ErrorMessage = "Password不能超过64个字符")]
        public string Password { get; set; } = string.Empty;

    }
}
