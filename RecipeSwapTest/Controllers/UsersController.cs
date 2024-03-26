using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using RecipeSwapTest.Data;
using RecipeSwapTest.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using RecipeSwapTest.ViewModels;


namespace RecipeSwapTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly RecipeSwapTestContext _context;

        public UsersController(RecipeSwapTestContext context)
        {
            _context = context;
        }



        //////////////////LOGIN/////////////////////


        // POST: api/Users/Login
        [HttpPost]
        [Route("Login")]
        public IActionResult Login([FromBody] LoginViewModel user)
        {
            // Cerca l'utente nel database
            User dbUser = _context.Users.FirstOrDefault(u => u.Username == user.Username);
            if (dbUser == null)
            {
                return BadRequest(new { message = "Username o password non validi." });
            }

            // Verifica la password
            if (!VerifyPassword(user.PswHash, dbUser.PswHash))
            {
                return BadRequest(new { message = "Username o password non validi." });
            }
            if (!dbUser.VerifiedEmail)
            {
                return BadRequest(new { message = "Email non confermata." });
            }

            // Genera il token JWT
            string tokenString = GenerateJwtToken(dbUser);

            // Restituisci il token
            return Ok(new
            {
                //todo aggiungere i campi che si vogliono restituire
                Id = dbUser.UserId,
                Username = dbUser.Username,
                Email = dbUser.Email,
                Token = tokenString
            });
        }

        // POST: api/Users/Register
        [HttpPost]
        [Route("Register")]
        public IActionResult Register([FromBody] RegisterViewModel user)
        {
            // Verifica che l'email sia valida
            if (!IsEmailValid(user.Email))
            {
                return BadRequest(new { message = "Email non valida." });
            }
            if (_context.Users.Any(u => u.Username == user.Username))
            {
                return BadRequest(new { message = "Username già in uso." });
            }
            // Verifica che la password sia valida
            if (!IsPasswordValid(user.PswHash))
            {
                return BadRequest(new { message = "Password non valida." });
            }
            if (user.PswHash != user.ConfirmPsw)
            {
                return BadRequest(new { message = "Le password non corrispondono." });
            }

            // Verifica che l'utente non esista già
            if (_context.Users.Any(u => u.Email == user.Email))
            {
                return BadRequest(new { message = "Utente già registrato." });
            }

            // Genera l'hash della password
            user.PswHash = HashPassword(user.PswHash);

            // Genera il token per la conferma dell'email
            string tokenEmail = GenerateEmailConfirmationToken(user.Email);
            // Aggiungi l'utente al database
            _context.Users.Add(new User
            {
                Email = user.Email,
                TokenConfirmEmail = tokenEmail,
                Bio = user.Bio,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfilePicture = user.ProfilePicture,
                PswHash = user.PswHash,
                Username = user.Username,
                RegistrationDate = DateOnly.FromDateTime(DateTime.Now),
                VerifiedEmail = false,
                Role = "user"
            });
            // Invia l'email di conferma
            InviaEmailConferma(user.Email, tokenEmail);;
            
            _context.SaveChanges();

            return Ok(new { message = "Utente registrato con successo." });
        }

        // POST: api/Users/Logout
        [HttpPost]
        [Route("Logout")]
        public IActionResult Logout()
        {
            return Ok(new { message = "Sei stato disconesso" });
        }

        //generate jwt token
        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("unaChiaveSegretaMoltoMoltoLungaPerSoddisfareIRequisiti");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserId.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        [HttpGet]
        [Route("ConfirmEmail")]
        public IActionResult ConfermaEmail(string email, string token)
        {
            // Trova l'utente nel database
            User user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user != null && user.TokenConfirmEmail == token)
            {
                // Imposta il campo EmailVerificata a true
                user.VerifiedEmail = true;
                _context.SaveChanges();

                return Ok("Email confermata con successo!");
            }

            return BadRequest(new { message = "Token o indirizzo email non validi." });
        }


        private bool IsPasswordValid(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return false;
            }
            if (password.Length < 8)
            {
                return false;
            }
            else
            {
                return true;
            }
            //Regex.IsMatch(password, @"^(?=.[a-z])(?=.[A-Z])(?=.\d)(?=.[^\da-zA-Z]).{8,}$");
        }

        private bool IsEmailValid(string email) =>
            Regex.IsMatch(email, @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$");

        private string HashPassword(string password)
        {
            // Genera un sale casuale
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            // Crea l'hash della password usando PBKDF2
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            // Combina sale e hash
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            // Converte in stringa base64
            string savedPasswordHash = Convert.ToBase64String(hashBytes);
            return savedPasswordHash;
        }

        [HttpGet]
        [Route("confirmEmail")]
        private void InviaEmailConferma(string destinatario, string token)
        {
            // Configura le impostazioni per l'invio dell'email
            string mittente = "napolitest14@gmail.com";
            string oggetto = "Conferma email";
            string corpo = $"Clicca sul link seguente per confermare la tua email: https://localhost:7026/api/Users/ConfirmEmail?email={destinatario}&token={token}";

            // Crea un oggetto MailMessage
            MailMessage message = new MailMessage(mittente, destinatario, oggetto, corpo);

            // Configura il client SMTP per l'invio dell'email
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.UseDefaultCredentials = false;
            //TODO: inserire le credenziali del mittente
            //cambiare mail e password per l'account definitivo
            smtpClient.Credentials = new NetworkCredential("napolitest14@gmail.com", "kegv pdde knob lihp");
            smtpClient.EnableSsl = true;

            // Invia l'email
            smtpClient.Send(message);
        }

        private string GenerateEmailConfirmationToken(string email)
        {
            // Genera un token univoco utilizzando l'email e un timestamp
            string timestamp = DateTime.Now.Ticks.ToString();
            string token = email + timestamp;

            // Calcola l'hash del token utilizzando SHA256
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
                string hashedToken = Convert.ToBase64String(hashBytes);
                hashedToken = hashedToken.Replace("/", "").Replace("+", "").Replace("=", "");
                return hashedToken;
            }
        }

        private bool VerifyPassword(string enteredPassword, string savedPasswordHash)
        {
            // Converte l'hash salvato in un array di byte
            byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);

            // Estrae il sale
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            // Crea l'hash della password fornita utilizzando lo stesso sale
            var pbkdf2 = new Rfc2898DeriveBytes(enteredPassword, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            // Confronta l'hash della password fornita con l'hash salvato
            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                {
                    return false;
                }
            }

            return true;
        }


    }
}
