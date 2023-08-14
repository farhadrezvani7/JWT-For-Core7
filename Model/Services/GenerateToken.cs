using Microsoft.IdentityModel.Tokens;
using Jwt7.Model.Entitys;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Jwt7.Model.Services
{
    public class GenerateToken
    {
        public static string GetToken(User users)
        {
            //در فانکشن زیر تمامی کلاین هایی که می خواهیم باهاشون بدنه یا پیلود توکن خودمون رو بسازیم رو وارد می کنیم
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.GivenName,users.UserName.ToString()),
                new Claim("CreatedDate",users.CreatedDate.ToString()),
         /*       new Claim(ClaimTypes.Name,users.FullName),
                new Claim(ClaimTypes.Role,users.Role),*/
                // همچنین می توان مقدار دستی هم وارد کرد
                /*new Claim("test dar pay","test value")*/
            };

            //محافظت می کند و طول آن باید 16 کاراکتر یا بیشتر باشد و از علائم هم می شود استفاده کرد JWT کدی که در پایین آمده، همان کلیدی است که از
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("abcdefgh123456789"));
            //مورد استفاده قرار گیرد JWT در خط زیر، همان کد 16 رقمی یا بیشتر را که نوشتیم را تبدیل به یه امضا می کنیم که در فرمت
            var signCredential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            //قابل مشاهده نباشد JWT کد زیر بابت این است که قسمت بدنه یا پیلود توکن مخفی شود تا از طریق سایت
            //این کد حتما باید 16 کاراکتر داشته باشد
            var encryptionKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("1111111111111112"));
            var encryptionCredential = new EncryptingCredentials(encryptionKey, SecurityAlgorithms.Aes128KW,SecurityAlgorithms.Aes128CbcHmacSha256);
            //خودمون رو تنظیم می کنیم JWT در فانکشن زیر ویژگی های کلی توکن
            //برخی از این ویژگی ها در بدنه یا همان پیلود توکن دیده قابل مشاهده هستند
            var descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Audience = "SampleJwtClient",
                Issuer = "SampleJwtServer",
                //تاریخ شروع اعتبار توکن
                IssuedAt = DateTime.Now,
                //تاریخ پایان اعتبار توکن
                Expires = DateTime.Now.AddDays(7),
                //می تونیم تاریخ شروع اعتبار توکن رو الان بزاریم اما در خط زیر مشخص کنیم که از چند وقت دیگه این توکن قابل استفاده باشه
                NotBefore = DateTime.Now,
                //امضایی که قبلا از کلید 16 رقمی خودمون درست کرده بودیم رو به پارامتر پایین پاس میدیم
                SigningCredentials = signCredential,

                //وقتی که بدنه یا پیلود توکن رو مخفی می کنیم، کمی حجم آن بیشتر می شود که با دو خط کد زیر، حجم آن مدیریت می شود
                EncryptingCredentials = encryptionCredential,
                CompressionAlgorithm = CompressionAlgorithms.Deflate,
            };
            //در کد زیر، کل توکن رو حالا می سازیم
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(descriptor);

            return tokenHandler.WriteToken(securityToken);
        }
    }
}
