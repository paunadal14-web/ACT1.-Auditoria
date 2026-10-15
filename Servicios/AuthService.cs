
using System;
using System.Security.Cryptography;
using AppInsegura.Datos;
using AppInsegura.Modelos;

namespace AppInsegura.Servicios
{
    public class AuthService
    {
        private readonly BaseDatosUsuarios baseDatos;

        public AuthService(BaseDatosUsuarios baseDatos)
        {
            this.baseDatos = baseDatos;
        }

        public Usuario Registrar(string nombre, string contrasena, string rol = "jugador")
        {
            // Comprobamos que los campos no esten vacios
            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(contrasena))
            {
                throw new Exception("El nombre y la contraseña son obligatorios.");
            }

            var nuevo = new Usuario
            {
                Nombre = nombre,
                ContrasenaHash = CalcularHash(contrasena),
                Rol = rol,
                TokenSesion = ""
            };

            baseDatos.Agregar(nuevo);
            return nuevo;
        }

        public Usuario? IniciarSesion(string nombre, string contrasena)
        {
            Usuario? usuario = baseDatos.BuscarExacto(nombre);

            if (usuario == null)
            {
                return null;
            }

            // Comprobamos la contraseña introducida
            if (!ComprobarContrasena(contrasena, usuario.ContrasenaHash))
            {
                return null;
            }

            usuario.TokenSesion = GenerarTokenSesion();

            // No mostramos el token ni lo guardamos en archivos
            Console.WriteLine($"[LOG] Login correcto: {usuario.Nombre}");

            return usuario;
        }

        private string CalcularHash(string contrasena)
        {
            // Creamos una sal aleatoria para cada contraseña
            byte[] sal = RandomNumberGenerator.GetBytes(16);

            // Usamos PBKDF2 en lugar de MD5
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                contrasena,
                sal,
                100000,
                HashAlgorithmName.SHA256,
                32
            );

            // Guardamos la sal y el hash juntos
            return Convert.ToBase64String(sal) + ":" +
                   Convert.ToBase64String(hash);
        }

        private bool ComprobarContrasena(string contrasena, string contrasenaGuardada)
        {
            // Separamos la sal y el hash guardados
            string[] partes = contrasenaGuardada.Split(':');

            if (partes.Length != 2)
            {
                return false;
            }

            byte[] sal = Convert.FromBase64String(partes[0]);
            byte[] hashGuardado = Convert.FromBase64String(partes[1]);

            // Calculamos el hash con la misma sal
            byte[] hashIntento = Rfc2898DeriveBytes.Pbkdf2(
                contrasena,
                sal,
                100000,
                HashAlgorithmName.SHA256,
                32
            );

            // Comprobamos si las contraseñas coinciden
            return CryptographicOperations.FixedTimeEquals(
                hashGuardado,
                hashIntento
            );
        }

        private string GenerarTokenSesion()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}
