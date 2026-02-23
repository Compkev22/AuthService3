using AuthService.Application.Services;
using AuthService.Domain.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Persistence.Repositories;

// La clase implementa IRoleRepository para cumplir con el contrato de métodos definidos.
public class RoleRepository : IRoleRepository
{
    private readonly ApplicationDbContext context;

    // Constructor: Recibe el contexto de la base de datos mediante Inyección de Dependencias.
    public RoleRepository(ApplicationDbContext context)
    {
        this.context = context;
    }

    // Busca un Rol por su nombre (ej. "Admin") e incluye sus relaciones con usuarios.
    public async Task<Role?> GetByNameAsync(string roleName)
    {
        return await context.Roles
            .Include(r => r.UserRoles) // Carga la lista de usuarios asociados a este rol (Eager Loading).
            .FirstOrDefaultAsync(r => r.Name == roleName); // Devuelve el primero que coincida o null.
    }

    // Cuenta cuántos usuarios tienen asignado un rol específico.
    public async Task<int> CountUsersInRoleAsync(string roleName)
    {
        return await context.UserRoles
            .Where(ur => ur.Role.Name == roleName) // Filtra en la tabla intermedia por el nombre del rol.
            .CountAsync(); // Ejecuta un "SELECT COUNT(*)" en la base de datos.
    }

    // Obtiene la lista completa de objetos Usuario que pertenecen a un rol.
    public async Task<IReadOnlyList<User>> GetUsersInRoleAsync(string roleName)
    {
        return await context.UserRoles
            .Where(ur => ur.Role.Name == roleName)
            .Select(ur => ur.User) // De la tabla intermedia, solo nos interesan los datos del Usuario.
            // .Include: Carga datos de otras tablas relacionadas para que no lleguen nulos.
            .Include(u => u.UserProfile) 
            .Include(u => u.UserEmail)
            .Include(u => u.UserPasswordReset)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role) // Carga los roles de esos usuarios para ver qué otros permisos tienen.
            .ToListAsync() // Convierte el resultado en una lista de forma asíncrona.
            .ContinueWith(t => (IReadOnlyList<User>)t.Result); // Convierte el tipo List a una interfaz de solo lectura.
    }

    // Obtiene solo los nombres (strings) de los roles que tiene un usuario específico.
    public async Task<IReadOnlyList<string>> GetUserRoleNamesAsync(string userId)
    {
        return await context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role.Name) // Proyección: Solo trae el texto del nombre, no todo el objeto.
            .ToListAsync()
            .ContinueWith(t => (IReadOnlyList<string>)t.Result);
    }
}