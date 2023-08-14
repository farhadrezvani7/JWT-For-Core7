using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Jwt7.Model.context;
using Jwt7.Model.Dto;
using Jwt7.Model.Dtos;
using Jwt7.Model.Entitys;
using Jwt7.Model.Services;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Jwt7.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly DataBaseContext db;

        public HomeController(DataBaseContext db)
        {
            this.db = db;
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> Register(RegisterViewModelDto registerViewModel)
        {
            var userIsExist = await db.Users.Where(u=>u.UserName == registerViewModel.UserName).AnyAsync();
            if (userIsExist)
            {
                return Conflict(".شما قبلا با این نام کاربری ثبت نام کرده اید");
            }
            //هست Entity اسم کلاس  User منظور از
            var user = new User
            {
                //ها، این مقادیر مدل می شوند Dto هاست و سمت راست مقدار ارسال شده از سمت کلاینته که با استفاده از Entity سمت چپ اسم فیلدهای
                UserName = registerViewModel.UserName,
                PasswordHash = GetHash(registerViewModel.PasswordHash),
            };
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return Ok(true);
        }


        [HttpGet("[action]")]
        public async Task<IActionResult> Login([FromQuery]RegisterViewModelDto registerViewModel)
        {
            //برای جستجوی یوزرنیم در دیتابیس استفاده شده است Where از
            var user = await db.Users.Where(u=> u.UserName == registerViewModel.UserName).FirstOrDefaultAsync();
            //در شرط پایین گفتیم که اگه یوزرنیم رو پیدا نکردی یه پیغام خطا ارسال بشه به سمت کلاینت
            if (user == null)
            {
                return NotFound("نام کاربری پیدا نشد.");
            }
            //در شرط پایین گفتیم که اگر کاربر رو پیدا کردی اما پسوردی که ارسال شده با پسورد توی دیتابیس یکی نبود، به کاربر یه پیغام خطا نشون بده
            if(user.PasswordHash != GetHash(registerViewModel.PasswordHash))
                return Conflict("رمز عبور صحیح نیست.");

         
            return Ok(GenerateToken.GetToken(user));
        }

        [Authorize]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetCategory()
        {
            //برای جستجوی یوزرنیم در دیتابیس استفاده شده است Where از
            var categories = await db.Categorys.ToListAsync();
   
            return Ok(categories);
        }
        //در کد زیر مشخص می کنیم که چه کاربری هایی می توانند از این رکوئست استفاده کنند
        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> AddCategory(string categoryName)
        {
            var category = new Category
            {
                CategoryName = categoryName,
            };
            //جهت خواندن اطلاعات داخل توکن
            /*            
             *         string uId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
             *         string fullName = User.FindFirst(ClaimTypes.Name)?.Value;
             *         string role = User.FindFirst(ClaimTypes.Role)?.Value;
             *         string test_dar_pay = User.FindFirstValue("test dar pay");
            */
            string userName = User.FindFirst(ClaimTypes.GivenName)?.Value;
            string createdDate = User.FindFirst("CreatedDate")?.Value;
            db.Categorys.Add(category);
            await db.SaveChangesAsync();

            return Ok();
        }








        //کردن پسوردی هست که از سمت کلاینت ارسال شده است Hash فانکشن زیر برای
        private string GetHash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

    }
}
