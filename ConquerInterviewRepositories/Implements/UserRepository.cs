﻿using ConquerInterviewBO.Models;
using ConquerInterviewDAO;
using ConquerInterviewRepositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerInterviewRepositories.Implements
{
    public class UserRepository: IUserRepository
    {
        private readonly ConquerInterviewDbContext _context;

        public UserRepository(ConquerInterviewDbContext context)
        {
            _context = context;
        }
        public List<User> GetAllUsers() => UserDAO.Instance.GetAllUsers();

        public User GetUserById(int id) => UserDAO.Instance.GetUserById(id);
        public void UpdateUser(User user) => UserDAO.Instance.UpdateUser(user);

        public void SoftDeleteUser(int userId) => UserDAO.Instance.SoftDeleteUser(userId);

    }
}
