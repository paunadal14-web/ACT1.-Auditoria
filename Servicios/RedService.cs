
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AppInsegura.Servicios
{
    public class RedService
    {
        // Usamos HTTPS para enviar los datos de forma segura
        private const string UrlServidor = "https://api.miapp-insegura.local/puntuaciones";

        public void EnviarPuntuacion(string nombreUsuario, int puntuacion)
        {
            try
            {
                EnviarPuntuacionAsync(nombreUsuario, puntuacion).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine("No se ha podido conectar con el servidor (es normal si no tienes conexión real):");
                Console.WriteLine(ex.Message);
            }
        }

        private async Task EnviarPuntuacionAsync(string nombreUsuario, int puntuacion)
        {
            // Leemos la clave desde una variable de entorno
            string? apiKey = Environment.GetEnvironmentVariable("API_KEY");

            // Comprobamos que exista una clave configurada
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                Console.WriteLine("No se ha configurado la clave de API.");
                return;
            }

            using var cliente = new HttpClient();

            // Preparamos la direccion con los datos de la puntuacion
            string url = $"{UrlServidor}?usuario={Uri.EscapeDataString(nombreUsuario)}&puntos={puntuacion}";

            // Enviamos la clave en la cabecera, no en la URL
            cliente.DefaultRequestHeaders.Add("X-API-Key", apiKey);

            Console.WriteLine("Enviando puntuacion al servidor...");

            HttpResponseMessage respuesta = await cliente.GetAsync(url);

            Console.WriteLine($"Respuesta del servidor: {(int)respuesta.StatusCode}");
        }
    }
}
