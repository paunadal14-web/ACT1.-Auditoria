
using System;
using System.Collections.Generic;
using System.Linq;
using AppInsegura.Modelos;

namespace AppInsegura.Datos
{
    // Simula una tabla de base de datos
    public class BaseDatosUsuarios
    {
        private readonly List<Usuario> usuarios = new List<Usuario>();

        public void Agregar(Usuario usuario)
        {
            // Comprobamos que el nombre de usuario no exista ya
            if (usuarios.Any(u => u.Nombre == usuario.Nombre))
            {
                throw new Exception("Este usuario ya existe.");
            }

            usuarios.Add(usuario);
        }

        public List<Usuario> ListarTodos()
        {
            return usuarios;
        }

        public Usuario? BuscarExacto(string nombre)
        {
            return usuarios.FirstOrDefault(u => u.Nombre == nombre);
        }

        public Usuario? BuscarPorNombre(string nombreBuscado)
        {
            // Buscamos directamente en la lista para evitar la inyeccion SQL
            return BuscarExacto(nombreBuscado);
        }
    }
}
