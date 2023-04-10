using System.Data.SqlClient;
using System.Security.Claims;
using INFOTOOLSSV.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;




namespace INFOTOOLSSV.Controllers
{
	public class CuentaController : Controller
	{
		private readonly Contexto _contexto;

		public CuentaController(Contexto contexto) { 
			this._contexto = contexto;	
		}


		public ActionResult Login()
		{

		
            ClaimsPrincipal c = HttpContext.User;
			if (c.Identity != null) { 
				if (c.Identity.IsAuthenticated)
				{
					return RedirectToAction("Index", "Home");
				}
			}


			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(Usario u)
		{


			try {
				using (SqlConnection con = new(_contexto.Conexion)) {

					using (SqlCommand cmd = new("sp_validar_login", con)) {

						cmd.CommandType = System.Data.CommandType.StoredProcedure;
						cmd.Parameters.Add("@UserName", System.Data.SqlDbType.VarChar).Value = u.UserName;
						cmd.Parameters.Add("@Clave", System.Data.SqlDbType.VarChar).Value = u.clave;
						con.Open();
						var dr =  cmd.ExecuteReader();

						while(dr.Read())
						{
							if (dr["UserName"] != null && u.UserName != null)
							{
								List<Claim> c = new List<Claim>() {
									new Claim (ClaimTypes.NameIdentifier, u.UserName),
									new Claim ("Nombre", u.UserName ),                                  


                                };

								ClaimsIdentity ci = new(c, CookieAuthenticationDefaults.AuthenticationScheme);
								AuthenticationProperties P = new();

								P.AllowRefresh = true;
								P.IsPersistent = u.MantenerActivo;

								if (!u.MantenerActivo)
								{
									P.ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(1);
								}
								else
								{
									P.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1);
								}

                                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(ci), P);
								
                                
                                
                                return RedirectToAction("Index", "Home");
							}
							else 
							{
								ViewBag.Error = "Credenciales incorrectas o cuenta no registrada";
							
							}

						}
					}
				
				}

				return View();
			
			
			} catch(System.Exception e) 
			
			{
				ViewBag.Error = e.Message;
				return View();
			}







		}


		
	}
}
