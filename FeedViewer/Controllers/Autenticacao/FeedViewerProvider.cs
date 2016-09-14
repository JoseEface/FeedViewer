using FeedViewer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace FeedViewer.Controllers.Autenticacao
{
    public sealed class FeedViewerProvider : RoleProvider
    {
        public string pApplicationName;

        public override string ApplicationName
        {
            get
            {
                return this.pApplicationName;
            }
            set
            {
                this.pApplicationName = value;
            }
        }

        public override string Name
        {
            get
            {
                return "FeedViewerProvider";
            }
        }

        public override string Description
        {
            get
            {
                return "Provedor de acessos para a aplicacao FeedViewer";
            }
        }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);
            if (config["applicationName"] == null && config["applicationName"].Trim() == "")
                ApplicationName = "FeedViewer";
            else
                ApplicationName = config["applicationName"];

        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {            
            System.Diagnostics.Debug.WriteLine("FeedViewerProvider:: Metodo nao usado AddUsersToRoles");
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            System.Diagnostics.Debug.WriteLine("FeedViewerProvider:: Metodo nao usado CreateRole");
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            System.Diagnostics.Debug.WriteLine("FeedViewerProvider:: Metodo nao usado DeleteRole");
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            string[] listaRoles = new string[1];
            using (feedviewerContext contexto = new feedviewerContext())
            {
                usuario usr = contexto.usuarios.Where(u => u.login.Equals(username)).Single<usuario>();
                listaRoles[0] = (usr.admin ? "admin" : "usuario");
            }
            return listaRoles;           
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            string[] usuarios;
            using (feedviewerContext contexto = new feedviewerContext())
            {
                if (roleName.Equals("usuario"))
                    usuarios = contexto.usuarios.Where(u => (u.admin == false && u.login.Equals(usernameToMatch))).Select(u => u.login).ToArray<string>();
                else if (roleName.Equals("admin"))
                    usuarios = contexto.usuarios.Where(u => (u.admin == true && u.login.Equals(usernameToMatch))).Select(u => u.login).ToArray<string>();
                else
                {
                    contexto.Dispose();
                    throw new Exception("Role inválido");
                }
            }
            return usuarios;
        }

        public override string[] GetUsersInRole(string roleName)
        {
            string[] usuarios;
            using (feedviewerContext contexto = new feedviewerContext())
            {
                if (roleName.Equals("usuario"))
                    usuarios = contexto.usuarios.Where(u => (u.admin == false)).Select(u => u.login).ToArray<string>();
                else if (roleName.Equals("admin"))
                    usuarios = contexto.usuarios.Where(u => (u.admin == true)).Select(u => u.login).ToArray<string>();
                else
                {
                    contexto.Dispose();
                    throw new Exception("Role inválido");
                }
            }
            return usuarios;
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            bool resultado;
            using (feedviewerContext contexto = new feedviewerContext())
            {
                if (roleName.Equals("admin"))
                    resultado = contexto.usuarios.Where(u => (u.login.Equals(username) && u.admin)).Any();
                else if (roleName.Equals("usuario"))
                    resultado = contexto.usuarios.Where(u => (u.login.Equals(username) && !u.admin)).Any();
                else
                    throw new Exception("Role invalido");
            }
            return resultado;
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            System.Diagnostics.Debug.WriteLine("FeedViewerProvider:: Metodo nao usado RemoveUsersFromRole");
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            if (roleName.Equals("admin") || roleName.Equals("usuario"))
                return true;
            else
                return false;
        }

        public override string[] GetAllRoles()
        {
            return new string[2]{"admin","usuario"};
        }

    }

}