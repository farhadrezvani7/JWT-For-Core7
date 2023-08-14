using System;

namespace Jwt7.Model.Dto
{
    public class RegisterViewModelDto
    {
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
    }
}
