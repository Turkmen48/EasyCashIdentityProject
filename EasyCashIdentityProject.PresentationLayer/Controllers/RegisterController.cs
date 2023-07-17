using EasyCashIdentityProject.DtoLayer.Dtos.AppUserDtos;
using EasyCashIdentityProject.EntityLayer.Concrete;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace EasyCashIdentityProject.PresentationLayer.Controllers
{
    public class RegisterController : Controller
    {
        private readonly UserManager<AppUser> _usermanager;

        public RegisterController(UserManager<AppUser> usermanager)
        {
            _usermanager = usermanager;
        }


        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(AppUserRegisterDto appUserRegisterDto)
        {
            if (ModelState.IsValid)
            {
                Random random = new Random();
                int code = random.Next(100000, 1000000);

				AppUser appUser = new AppUser()
                {
                    UserName=appUserRegisterDto.Username,
                    Name= appUserRegisterDto.Name,
                    Email=appUserRegisterDto.Email,
                    Surname=appUserRegisterDto.Surname,
                    City="Kocaeli",
                    District="B",
                    ImageUrl="a",
                    ConfirmCode=code

                    
                };
                var result= await _usermanager.CreateAsync(appUser,appUserRegisterDto.Password);
                if(result.Succeeded)
                {
                    
                    MailboxAddress mailboxAddressFrom = new MailboxAddress("Easy Cash Admin", "enesenesoglu48@gmail.com");
                    MailboxAddress mailboxAddressTo = new MailboxAddress(
                        "User", appUser.Email);
                    SmtpClient smtpClient = new SmtpClient();
                    smtpClient.Connect("smtp.gmail.com", 587, false);
                    smtpClient.Authenticate("liathecat48@gmail.com", "zxtadlwchdrcbioc");
                    
                    var bodyBuilder = new BodyBuilder()
                    {
                        TextBody=$"Kayıt için onay kodunuz {code}",  
                     
                    };
                    MimeMessage mimeMessage = new MimeMessage();
					mimeMessage.From.Add(mailboxAddressFrom);
					mimeMessage.To.Add(mailboxAddressTo);
                    mimeMessage.Subject = "Easy Cash Onay Kodu";
                    mimeMessage.Body=bodyBuilder.ToMessageBody();
                    smtpClient.Send(mimeMessage);
                    smtpClient.Disconnect(true);
                    TempData["Mail"] = appUserRegisterDto.Email;
					return RedirectToAction("Index","ConfirmMail");
                }
                else
                {
                    foreach(var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                    
                }
            }
           return View();
			
        }
    }
}
